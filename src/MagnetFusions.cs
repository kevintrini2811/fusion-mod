using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Native equipment transitions and MagnetItem serialization remain native.
// These variants own the source attribute clock and acquisition range.
internal static class MagnetFusions
{
    internal sealed class AbilityFrame
    {
        internal readonly Plant Plant;
        internal readonly Board Board;
        internal readonly uint Id;
        internal bool InnerCompleted;
        internal (float X, float Y, int Row)? Shock;
        internal (float X, float Y)? Grenade;
        internal AbilityFrame(Plant plant) { Plant = plant; Board = plant.mBoard; Id = plant.DataID; }
    }
    [ThreadStatic] internal static AbilityFrame? CurrentAbility;

    private static void RequestShock(Plant plant, Board board, uint id, float x, float y, int row)
    {
        // Damage must run after the inner UpdateMagnetShroom has returned
        // through Zombie Library's armor-borrow finalizer. Keep only this
        // synchronous outer ability invocation; never carry work to a tick.
        if (CurrentAbility is { } frame && frame.Plant.Pointer == plant.Pointer
            && frame.Id == id && frame.Board.Pointer == board.Pointer)
            frame.Shock ??= (x, y, row);
    }
    internal static void FinishAbility(AbilityFrame frame)
    {
        var pending = frame.Shock;
        var grenade = frame.Grenade;
        frame.Shock = null; // Commit once before damage callbacks can reenter.
        frame.Grenade = null;
        if (frame.InnerCompleted && pending is { } shock && Same(frame.Plant, frame.Board, frame.Id))
            ZombieLibrary.NativeArmorMagnet.RunAfterRestore(() =>
            {
                if (Same(frame.Plant, frame.Board, frame.Id))
                    Shock(frame.Plant, frame.Board, frame.Id, shock.X, shock.Y, shock.Row);
            });
        if (frame.InnerCompleted && grenade is { } impact && Same(frame.Plant, frame.Board, frame.Id))
            ZombieLibrary.NativeArmorMagnet.RunAfterRestore(() =>
            {
                var plant = frame.Plant;
                if (Same(plant, frame.Board, frame.Id))
                    CherryBombPlants.Explode(new(frame.Board, FusionPlant.GrenadierMagnet, frame.Id,
                        plant.mPlantCol, plant.mRow, impact.X, impact.Y, plant.mRenderOrder),
                        sourceRadius: 2 * Unit, damage: 300);
            });
    }
    internal const int BlowEvent = 39, BlowDuration = 127, Lifetime = 1000;
    internal const float Unit = 80f / 1.35f;
    internal static bool Owns(Plant plant) => FusionPlant.IsMagnetFusion(FusionPlant.From(plant));
    internal static bool Blover(Plant plant) => FusionPlant.From(plant) == FusionPlant.MagnetBlover;
    internal static bool Grenadier(Plant plant) => FusionPlant.From(plant) == FusionPlant.GrenadierMagnet;
    internal static int Light(Plant plant) => Math.Clamp(PumpkinFusions.LightLevel(plant), 0, 3);
    internal static int StoredLight(Plant plant) => Math.Clamp(plant.mTargetY / 4, 0, 3);
    internal static int Interval(int light) => light switch { 0 => 1500, 1 => 1100, 2 => 750, _ => 500 };
    internal static float Radius(int light) => (light switch { 0 => 2, 1 => 5, 2 => 6, _ => 7 }) * Unit;
    internal static int Rows(int light) => light switch { 0 => 2, 1 => 3, 2 => 4, _ => 5 };

    internal static void Initialize(Plant plant)
    {
        if (!Owns(plant)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mDoSpecialCountdown = plant.mStateCountdown = plant.mShootingCounter = 0;
        plant.mState = PlantState.Ready;
        plant.mLaunchCounter = Blover(plant) ? 50 : 0;
        // Blover age, captured physical item kind/age, action age respectively.
        plant.mTargetX = plant.mTargetY = plant.mLaunchRate = plant.mStartRow = 0;
        if (FusionPlant.From(plant) == FusionPlant.AnchorShroom) plant.mIsAsleep = false;
    }

    internal static void Fertilize(Plant plant)
    { if (Owns(plant) && plant.mLaunchCounter > 50) plant.mLaunchCounter = 50; }

    internal static bool Same(Plant plant, Board board, uint id) => plant.DataID == id
        && plant.mBoard?.Pointer == board.Pointer && Owns(plant) && plant.mPlantHealth > 0 && ChomperFusions.Active(plant);
    internal static bool InRange(Plant plant, float x, float y, int row, float radius, int rows) =>
        Math.Abs(row - plant.mRow) <= rows
        && MathF.Pow(x - (plant.mX + plant.mWidth / 2f), 2)
            + MathF.Pow(y - (plant.mY + plant.mHeight / 2f), 2) <= radius * radius;

    private static bool Equipment(Zombie zombie) =>
        zombie.mHelmHealth > 0 && zombie.mHelmType is HelmType.Pail or HelmType.Football
        || zombie.mShieldHealth > 0 && zombie.mShieldType is ShieldType.Door or ShieldType.Ladder
        || zombie.mHasObject && (zombie.mZombieType == ZombieType.Pogo
            || zombie.mZombieType == ZombieType.JackInTheBox
            || zombie.mZombieType == ZombieType.Digger && zombie.mZombiePhase == ZombiePhase.DiggerTunneling);

    internal static void Tick(Plant plant)
    {
        if (!Owns(plant) || !ChomperFusions.Active(plant) || plant.mIsAsleep
            || !FusionTime.SourceStep(plant, typeof(MagnetFusions))) return;
        var board = plant.mBoard;
        uint id = plant.DataID;
        bool blover = Blover(plant);
        bool grenadier = Grenadier(plant);
        bool anchor = FusionPlant.From(plant) == FusionPlant.AnchorShroom;
        if (grenadier && TickGrenadier(plant, board, id)) return;
        if (blover)
        {
            ++plant.mTargetX;
            if (plant.mTargetX == BlowEvent) BloverFusions.Blow(plant);
            if (!Same(plant, board, id)) return;
            if (plant.mTargetX >= BlowEvent + Lifetime) { plant.Die(); return; }
        }
        MoveItem(plant);
        if (plant.mStartRow > 0 && ++plant.mStartRow > 69) plant.mStartRow = 0;
        if (plant.mLaunchCounter > 0)
        {
            if (--plant.mLaunchCounter > 0) return;
            if (anchor && plant.mTargetX < 3 && plant.mMagnetItems[0].ItemType != MagnetItemType.None)
            {
                if (++plant.mTargetX == 3) board.mApp.SoundSystem.PlayFoley(FoleyType.PlantGrow);
            }
        }
        // The previous item is digested before acquisition. Light changes do
        // not rewrite an already running interval.
        ClearItem(plant);
        int light = blover || anchor || grenadier ? 0 : Light(plant);
        float radius = blover ? 40 * Unit : anchor || grenadier ? 4 * Unit : Radius(light);
        int rows = blover || anchor || grenadier ? 2 : Rows(light);
        int interval = blover ? 100 : anchor ? (plant.mTargetX >= 3 ? 1000 : 1500) : Interval(light);
        plant.mTargetY = light * 4;
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || !BloverFusions.Live(board, zombie) || zombie.mMindControlled
                || !Equipment(zombie)) continue;
            var rect = zombie.GetZombieRect();
            if (!InRange(plant, rect.center.x, rect.center.y, zombie.mRow, radius, rows)) continue;
            float x = rect.center.x, y = rect.center.y;
            int row = zombie.mRow;
            // Source TryAttrackZombie removes equipment before the Lumos
            // shock. The native call bundles that removal with presentation.
            plant.MagnetShroomAttactItem(zombie);
            if (!Same(plant, board, id)) return;
            plant.mStateCountdown = 0;
            if (plant.mMagnetItems[0].ItemType == MagnetItemType.None) return;
            Commit(plant, interval);
            if (!blover && light >= 2) RequestShock(plant, board, id, x, y, row);
            // The captured object is independent of its former zombie. Never
            // read the zombie after damage or equipment callbacks.
            return;
        }
        for (int i = 0; i < board.m_gridItems.Count; ++i)
        {
            var item = board.m_gridItems[i];
            if (item == null || item.mDead || item.mGridItemType != GridItemType.Ladder
                || !InRange(plant, item.mX + 40, item.mY + 40, item.mGridY, radius, rows)) continue;
            float x = item.mX + 40, y = item.mY + 40;
            int row = item.mGridY;
            uint itemId = item.DataID;
            Capture(plant, MagnetItemType.LadderPlaced, x, y);
            item.GridItemDie();
            if (!Same(plant, board, id)) return;
            if (item.DataID != itemId || !item.mDead) { ClearItem(plant); return; }
            Commit(plant, interval);
            if (!blover && light >= 2) RequestShock(plant, board, id, x, y, row);
            return;
        }
        if (BucketDrops.TakeForMagnet(plant, radius, rows, out int dropRow))
        {
            var item = plant.mMagnetItems[0];
            float x = item.PosX, y = item.PosY;
            int row = dropRow;
            Commit(plant, interval);
            if (!blover && light >= 2) RequestShock(plant, board, id, x, y, row);
        }
    }

    internal static void Capture(Plant plant, MagnetItemType kind, float x, float y)
    {
        var item = plant.mMagnetItems[0];
        item.ItemType = kind; item.PosX = x; item.PosY = y;
        item.DestOffsetX = plant.mWidth / 2f; item.DestOffsetY = 10;
    }
    private static void Commit(Plant plant, int interval)
    {
        plant.mLaunchCounter = interval;
        plant.mState = PlantState.MagnetshroomSucking;
        plant.mStartRow = 1;
        if (Grenadier(plant))
        {
            plant.mTargetX = 0;
            plant.mShootingCounter = 100;
        }
    }

    internal static bool CanReceive(Plant plant) => Grenadier(plant) && ChomperFusions.Active(plant)
        && plant.mLaunchCounter == 0 && plant.mMagnetItems[0].ItemType == MagnetItemType.None;

    internal static bool TryReceive(Plant plant, MagnetItemType type, float x, float y)
    {
        if (!CanReceive(plant)) return false;
        Capture(plant, type, x, y);
        Commit(plant, 1500);
        return true;
    }

    // Negative action phases are outside the ordinary 1..69 acquisition
    // animation. Native drawing and saves own the item's world position.
    private const int Outbound = -1, Returning = -2;
    internal static bool CanTakeItem(Plant plant) => !Grenadier(plant) || plant.mStartRow >= 0;

    private static bool TickGrenadier(Plant plant, Board board, uint id)
    {
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        var item = plant.mMagnetItems[0];
        if (item.ItemType == MagnetItemType.None) return plant.mLaunchCounter > 0;

        MoveItem(plant);
        if (plant.mStartRow > 0 && ++plant.mStartRow > 69) plant.mStartRow = 0;
        if (plant.mShootingCounter > 0) --plant.mShootingCounter;
        if (plant.mShootingCounter == 0)
        {
            plant.mShootingCounter = 300;
            if (plant.mStartRow != Outbound && !BucketPlants.Holding(plant)) AimGrenade(plant, board, item);
        }

        if (plant.mStartRow == Outbound)
        {
            float x = item.DestOffsetX, y = item.DestOffsetY;
            if (!MoveToward(item, x, y, 40 * Unit / 100, Unit)) return true;
            item.DestOffsetX = plant.mWidth / 2f;
            item.DestOffsetY = 10;
            plant.mStartRow = Returning;
            if (++plant.mTargetX == 5)
            {
                ClearItem(plant);
                plant.mLaunchCounter = Math.Min(plant.mLaunchCounter, 50);
            }
            if (CurrentAbility is { } frame && frame.Id == id && frame.Plant.Pointer == plant.Pointer
                && frame.Board.Pointer == board.Pointer) frame.Grenade ??= (x, y);
        }
        else if (plant.mStartRow == Returning && MoveToward(item,
            plant.mX + item.DestOffsetX, plant.mY + item.DestOffsetY, 32 * Unit / 100, 0))
            plant.mStartRow = 0;
        return true;
    }

    private static void AimGrenade(Plant plant, Board board, MagnetItem item)
    {
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        float nearest = float.PositiveInfinity;
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mRow != plant.mRow || !BloverFusions.Live(board, zombie)
                || !ShockEligible(zombie)) continue;
            var center = zombie.GetZombieRect().center;
            float dx = center.x - x, dy = center.y - y;
            float distance = dx * dx + dy * dy;
            if (distance >= nearest) continue;
            nearest = distance;
            item.DestOffsetX = center.x;
            item.DestOffsetY = center.y;
            plant.mStartRow = Outbound;
        }
    }

    private static bool MoveToward(MagnetItem item, float x, float y, float speed, float tolerance)
    {
        float dx = x - item.PosX, dy = y - item.PosY;
        float distance = MathF.Sqrt(dx * dx + dy * dy);
        if (distance <= tolerance) return true;
        float step = Math.Min(distance, speed);
        item.PosX += dx * step / distance;
        item.PosY += dy * step / distance;
        return distance <= tolerance + step;
    }
    private static void MoveItem(Plant plant)
    {
        var item = plant.mMagnetItems[0];
        if (item.ItemType == MagnetItemType.None || plant.mStartRow is < 1 or > 30) return;
        // Linear 0.3s source flight, reconstructed from current position and
        // saved remaining time. Arrival is exact; no unsaved origin is needed.
        int remaining = 31 - plant.mStartRow;
        item.PosX += (plant.mX + item.DestOffsetX - item.PosX) / remaining;
        item.PosY += (plant.mY + item.DestOffsetY - item.PosY) / remaining;
    }
    internal static void ClearItem(Plant plant)
    {
        if (BucketPlants.Holding(plant)) BucketPlants.Clear();
        plant.mMagnetItems[0].ItemType = MagnetItemType.None;
        plant.mTargetY = StoredLight(plant) * 4;
        plant.mLaunchRate = 0;
        plant.mState = PlantState.Ready;
        if (Grenadier(plant)) plant.mTargetX = plant.mStartRow = plant.mShootingCounter = 0;
    }
    internal static bool ShockEligible(Zombie zombie)
    {
        // Pinned Plant.AttackLandZombie rejects source statuses3,7,9,10,12,
        //16,20,21,22,23,24,44. Source Miner_rising13 IS eligible. Native
        // Ground also rejects rising/landing phases and admits boss heads.
        // Source SuperPogo big-jump20 has no native counterpart; ordinary
        // Pogo bounces remain source Polo_jump14, including high bounces.
        // Map the source categories explicitly; keep native damage ownership.
        if (zombie.mMindControlled || zombie.mZombieType is ZombieType.Boss or ZombieType.Bungee
            || zombie.mZombieType == ZombieType.Snorkel && zombie.mInPool && !zombie.mIsEating) return false;
        return zombie.mZombiePhase is not (ZombiePhase.PolevaulterInVault or ZombiePhase.DolphinInJump
            or ZombiePhase.BalloonFlying or ZombiePhase.BalloonPopping or ZombiePhase.DiggerTunneling
            or ZombiePhase.DiggerTunnelingPauseWithoutAxe or ZombiePhase.ImpGettingThrown);
    }

    private static void Shock(Plant plant, Board board, uint id, float x, float y, int row)
    {
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mRow != row || !BloverFusions.Live(board, zombie)
                || !ShockEligible(zombie)) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax), dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= Unit * Unit) targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, zombieId) in targets)
        {
            if (!Same(plant, board, id)) return;
            if (zombie.DataID == zombieId && BloverFusions.Live(board, zombie) && ShockEligible(zombie))
                // Source DamageType.Shieldless bypasses second armor, not helmets.
                // FusionDamageScale still converts at the final native boundary.
                zombie.TakeDamage(200, DamageFlags.BypassesShield);
        }
        if (Same(plant, board, id)) ImportedPeaVisuals.ShowMagnetLight(plant, x, y);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class MagnetFusionAbilityPatch
{
    internal sealed record Invocation(MagnetFusions.AbilityFrame? Parent, MagnetFusions.AbilityFrame? Frame);
    internal static void Prefix(Plant __instance, bool __runOriginal, out Invocation __state)
    {
        var frame = __runOriginal && MagnetFusions.Owns(__instance)
            ? new MagnetFusions.AbilityFrame(__instance) : null;
        __state = new(MagnetFusions.CurrentAbility, frame);
        // Even an unrelated nested ability must not write its parent's frame.
        MagnetFusions.CurrentAbility = frame;
    }
    internal static void Postfix(Invocation? __state)
    {
        if (__state?.Frame is { } frame) MagnetFusions.FinishAbility(frame);
    }
    internal static void Finalizer(Invocation? __state)
    {
        if (__state == null) return; // Prefix skipped: this call owns no frame.
        // A native or callback exception skips the postfix. Discard pending
        // damage and restore the exact enclosing invocation in either case.
        if (__state.Frame is { } frame) { frame.Shock = null; frame.Grenade = null; }
        MagnetFusions.CurrentAbility = __state.Parent;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class MagnetFusionInitializePatch
{ private static void Postfix(Plant __instance) => MagnetFusions.Initialize(__instance); }

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class GrenadierNativeShootingPatch
{ private static bool Prefix(Plant __instance) => !MagnetFusions.Grenadier(__instance); }

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateMagnetShroom))]
internal static class MagnetFusionUpdatePatch
{
    // Zombie Library exposes extra native armor in its earlier prefix and
    // restores that borrow in its finalizer, including when original is skipped.
    [HarmonyPriority(Priority.Last)]
    internal static bool Prefix(Plant __instance, bool __runOriginal)
    {
        if (!MagnetFusions.Owns(__instance)) return true;
        if (MagnetFusions.CurrentAbility is { } frame && frame.Plant.Pointer == __instance.Pointer
            && frame.Id == __instance.DataID) frame.InnerCompleted = false;
        if (__runOriginal) MagnetFusions.Tick(__instance);
        return false;
    }
    internal static void Postfix(Plant __instance)
    {
        // A thrown inner body never reaches this postfix, including when a
        // later Harmony finalizer suppresses the exception.
        if (MagnetFusions.CurrentAbility is { } frame && frame.Plant.Pointer == __instance.Pointer
            && frame.Id == __instance.DataID && frame.Board.Pointer == __instance.mBoard?.Pointer)
            frame.InnerCompleted = true;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class MagnetFusionDeathPatch
{
    private static void Prefix(Plant __instance, bool __runOriginal, out BucketDrops.MagnetCapture? __state) =>
        __state = __runOriginal && !__instance.mDead && MagnetFusions.Owns(__instance) && !MagnetFusions.Blover(__instance)
            // CherryMagnet.DieEventMustExecute destroys the held object.
            && !MagnetFusions.Grenadier(__instance)
            ? BucketDrops.CaptureMagnet(__instance) : null;
    private static void Postfix(Plant __instance, bool __runOriginal, BucketDrops.MagnetCapture? __state)
    {
        if (__runOriginal && __state != null && __instance.mDead && __instance.DataID == __state.Id
            && __instance.mBoard?.Pointer == __state.Board.Pointer) BucketDrops.ReleaseFromMagnet(__instance, __state);
    }
}
