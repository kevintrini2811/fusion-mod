using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Flowerpot owns placement, health and serialization. Recovery and pulse clocks
// use its otherwise unused native fields; recipient effects own their lifetime.
internal static class OffensivePots
{
    private const int Buff = -28001, Flight = -28002, ScaredyBuff = -28003;
    internal const int Full = 100000;
    internal static bool Owns(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount is Buff or Flight or ScaredyBuff;

    private static GridItem? Find(Board board, int marker, uint id)
    {
        for (int i = 0; i < board.m_gridItems.Count; ++i)
            if (board.m_gridItems[i] is { mDead: false } item && Owns(item)
                && item.mSunCount == marker && unchecked((uint)item.mGridX) == id) return item;
        return null;
    }

    private static GridItem? Record(Board board, int marker, uint id)
    {
        var previous = Find(board, marker, id);
        if (previous != null) return previous;
        var item = board.m_gridItems.DataArrayAlloc();
        if (item == null) return null;
        try
        {
            item.Init(board.mApp, GridItemType.ZenTool);
            item.mGridItemState = GridItemState.Normal;
            item.mSunCount = marker;
            item.mGridX = unchecked((int)id);
            item.mGridY = item.mGridItemCounter = 0;
            item.mX = item.mY = item.mPosX = item.mPosY = -1000;
            return item;
        }
        catch { item.GridItemDie(); throw; }
    }

    private static IEnumerable<Plant> Cell(Plant pot)
    {
        for (int i = 0; i < pot.mBoard.m_plants.Count; ++i)
            if (pot.mBoard.m_plants[i] is { } plant && plant.Pointer != pot.Pointer
                && plant.mPlantCol == pot.mPlantCol && plant.mRow == pot.mRow
                && ChomperFusions.Active(plant)) yield return plant;
    }

    internal static void Tick(Plant pot)
    {
        var identity = FusionPlant.From(pot);
        if ((identity != FusionPlant.BloverPot && identity != FusionPlant.ScaredyPot)
            || !ChomperFusions.Active(pot) || pot.NotOnGround()) return;
        if (!FusionTime.SourceStep(pot, typeof(OffensivePots))) return;
        if (identity == FusionPlant.ScaredyPot)
        {
            bool afraid = false;
            for (int i = 0; i < pot.mBoard.m_zombies.Count; ++i)
                if (pot.mBoard.m_zombies[i] is { } zombie && ScaredyFusions.EnemyNear(pot, zombie, 1.5f))
                { afraid = true; break; }
            // 30% per second at 100 simulation ticks/second, initially zero.
            pot.mTargetY = afraid ? 0 : Math.Min(Full, Math.Max(0, pot.mTargetY) + 300);
            if (pot.mLaunchCounter > 0 && --pot.mLaunchCounter > 0) return;
            pot.mLaunchCounter = 100;
            UpdateScaredyRecipients(pot);
            return;
        }
        if (pot.mLaunchCounter > 0 && --pot.mLaunchCounter > 0) return;
        pot.mLaunchCounter = 100;
        var air = Cell(pot).Where(AirPlants.Owns).ToArray();
        if (air.Length == 0) return;
        // Reserve recipients before consuming the first flyer. Allocation failure
        // leaves every flyer available for the following pulse.
        var recipients = Cell(pot).Append(pot).Where(p => !AirPlants.Owns(p)).ToArray();
        var effects = new List<GridItem>();
        foreach (var plant in recipients)
        {
            var effect = Record(pot.mBoard, Buff, plant.DataID);
            if (effect == null) return;
            effects.Add(effect);
        }
        foreach (var flyer in air) PlayerPlantingScope.Consume(flyer);
        for (int i = 0; i < recipients.Length; ++i)
        {
            effects[i].mGridItemCounter = 500;
            ReactivePots.Heal(recipients[i], 150);
        }
    }

    private static IntPtr valueBoard;
    private static int valueTick;
    private static Dictionary<FusionPlant, int>? values;

    private static bool SupportedRecipient(Plant plant) => !AirPlants.Owns(plant)
        && plant.mSeedType is not (SeedType.Flowerpot or SeedType.Pumpkinshell or SeedType.Lilypad or SeedType.InstantCoffee);

    internal static bool HasRecipient(Plant pot)
    {
        for (int i = 0; i < pot.mBoard.m_gridItems.Count; ++i)
            if (pot.mBoard.m_gridItems[i] is { mDead: false } item && Owns(item)
                && item.mSunCount == ScaredyBuff && unchecked((uint)item.mGridItemCounter) == pot.DataID) return true;
        return false;
    }

    private static void UpdateScaredyRecipients(Plant pot)
    {
        var recipients = Cell(pot).Where(SupportedRecipient).ToArray();
        var effects = new List<GridItem>();
        foreach (var recipient in recipients)
        {
            var effect = Record(pot.mBoard, ScaredyBuff, recipient.DataID);
            if (effect == null) return;
            effects.Add(effect);
        }
        // Remove only this pot's previous assignments, preserving a new owner
        // when another pot has already acquired a moved recipient.
        for (int i = 0; i < pot.mBoard.m_gridItems.Count; ++i)
            if (pot.mBoard.m_gridItems[i] is { mDead: false } item && Owns(item)
                && item.mSunCount == ScaredyBuff && unchecked((uint)item.mGridItemCounter) == pot.DataID
                && !effects.Any(effect => effect.Pointer == item.Pointer)) item.GridItemDie();
        if (recipients.Length == 0) return;
        if (values == null || valueBoard != pot.mBoard.Pointer || valueTick != pot.mBoard.mMainCounter)
        {
            values = PlantValue.Calculate(pot.mBoard);
            valueBoard = pot.mBoard.Pointer; valueTick = pot.mBoard.mMainCounter;
        }
        for (int i = 0; i < recipients.Length; ++i)
        {
            int cost = values.GetValueOrDefault(FusionPlant.From(recipients[i]));
            int bonus = (int)Math.Clamp((long)cost * 40, 0, 20000);
            if (recipients[i].mSeedType == SeedType.Scaredyshroom) bonus = 2 * (bonus + 10000);
            effects[i].mGridY = (int)((long)bonus * Math.Clamp(pot.mTargetY, 0, Full) / Full);
            effects[i].mGridItemCounter = unchecked((int)pot.DataID);
        }
    }

    internal static int Bonus(Plant plant)
    {
        if (!ChomperFusions.Active(plant)) return 0;
        int bonus = Find(plant.mBoard, Buff, plant.DataID) is { mGridItemCounter: > 0 } ? 20000 : 0;
        if (Find(plant.mBoard, ScaredyBuff, plant.DataID) is not { } effect) return bonus;
        for (int i = 0; i < plant.mBoard.m_plants.Count; ++i)
            if (plant.mBoard.m_plants[i] is { mDead: false } pot
                && pot.DataID == unchecked((uint)effect.mGridItemCounter)
                && FusionPlant.From(pot) == FusionPlant.ScaredyPot)
                return bonus + Math.Clamp(effect.mGridY, 0, 60000);
        return bonus;
    }

    internal static int Scale(int damage, int factor) => damage <= 0 ? damage
        : (int)Math.Min(int.MaxValue, Math.Round((long)damage * factor / (double)Full));

    internal sealed record Attack(Board Board, int Factor, Projectile? Shot = null, Zombie? Direct = null, bool Splash = false);
    [ThreadStatic] internal static Attack? Current;
    [ThreadStatic] internal static Damage? Hitting;
    internal sealed record Damage(Zombie Zombie, uint Id, Attack Attack);

    internal static Attack? PlantAttack(Plant plant) => plant.mBoard == null ? null
        : new(plant.mBoard, Full + Bonus(plant));

    internal static Attack? ProjectileAttack(Projectile shot) => Find(shot.mBoard, Flight, shot.DataID) is { } effect
        ? new(shot.mBoard, Math.Max(Full, effect.mGridY), shot) : null;

    internal static int HitDamage(Damage hit, int damage)
    {
        var attack = hit.Attack;
        if (damage <= 0 || attack.Factor == Full) return damage;
        var shot = attack.Shot;
        // Torchwood's extra 20 and iron peas' armor removal are not the
        // attacker's base attack. Preserve these additions after scaling.
        if (shot != null && IronProjectiles.Contains(shot))
        {
            int shield = IronProjectiles.IsRed(shot) ? 0 : Math.Max(0, hit.Zombie.mShieldHealth);
            return (int)Math.Min(int.MaxValue, (long)Scale(Math.Max(0, damage - shield), attack.Factor) + shield);
        }
        if (shot?.mProjectileType is ProjectileType.Fireball or ProjectileType.PeashooterFireball)
        {
            bool secondary = attack.Splash && attack.Direct?.Pointer != hit.Zombie.Pointer;
            long full = secondary ? damage * 3L : damage;
            long scaled = (long)Math.Round(Math.Max(0, full - 20) * attack.Factor / (double)Full) + 20;
            return (int)Math.Min(int.MaxValue, secondary ? Math.Max(1, scaled / 3) : scaled);
        }
        return Scale(damage, attack.Factor);
    }

    internal static void Capture(Projectile shot)
    {
        if (shot.mDead || Current is not { } attack || attack.Factor <= Full
            || attack.Board.Pointer != shot.mBoard?.Pointer) return;
        // Nested Fire/AddProjectile hooks never compound the multiplier or
        // consume fields reserved by the projectile family.
        var effect = Record(shot.mBoard, Flight, shot.DataID);
        if (effect != null) effect.mGridY = attack.Factor;
    }

    internal static void TickEffect(GridItem item)
    {
        if (item.mBoard.mPaused || item.mBoard.mApp.GameScene != GameScenes.Playing) return;
        bool live = false;
        if (item.mSunCount is Buff or ScaredyBuff)
        {
            for (int i = 0; i < item.mBoard.m_plants.Count; ++i)
                if (item.mBoard.m_plants[i] is { mDead: false } plant
                    && plant.DataID == unchecked((uint)item.mGridX)) { live = true; break; }
            if (item.mSunCount == Buff)
            {
                if (item.mGridItemCounter > 0
                    && FusionTime.SourceStep(item.mBoard, typeof(OffensivePots), item.Pointer, item.DataID)) --item.mGridItemCounter;
                if (item.mGridItemCounter == 0) live = false;
            }
            else
            {
                bool owner = false;
                for (int i = 0; i < item.mBoard.m_plants.Count; ++i)
                    if (item.mBoard.m_plants[i] is { mDead: false } pot
                        && pot.DataID == unchecked((uint)item.mGridItemCounter)
                        && FusionPlant.From(pot) == FusionPlant.ScaredyPot) { owner = true; break; }
                live &= owner;
            }
        }
        else
            for (int i = 0; i < item.mBoard.m_projectiles.Count; ++i)
                if (item.mBoard.m_projectiles[i] is { mDead: false } shot
                    && shot.DataID == unchecked((uint)item.mGridX)) { live = true; break; }
        if (!live) item.GridItemDie();
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class OffensivePotTickPatch
{ private static void Postfix(Plant __instance) => OffensivePots.Tick(__instance); }

[HarmonyPatch]
internal static class OffensivePotPlantScope
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] { "Update", "UpdateAbilities", "Fire", "DoSpecial", "Die" }
        .Select(name => AccessTools.Method(typeof(Plant), name));
    [HarmonyPriority(int.MaxValue)]
    private static void Prefix(Plant __instance, out OffensivePots.Attack? __state)
    { __state = OffensivePots.Current; OffensivePots.Current = OffensivePots.PlantAttack(__instance); }
    private static void Finalizer(OffensivePots.Attack? __state) => OffensivePots.Current = __state;
}

[HarmonyPatch]
internal static class OffensivePotProjectileScope
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] { "DoImpact", "DoSplashDamage" }
        .Select(name => AccessTools.Method(typeof(Projectile), name));
    [HarmonyPriority(int.MaxValue)]
    private static void Prefix(Projectile __instance, Zombie theZombie, MethodBase __originalMethod, out OffensivePots.Attack? __state)
    {
        __state = OffensivePots.Current;
        var attack = OffensivePots.ProjectileAttack(__instance);
        OffensivePots.Current = attack == null ? null : attack with
        { Direct = theZombie, Splash = __originalMethod.Name == nameof(Projectile.DoSplashDamage) };
    }
    private static void Finalizer(OffensivePots.Attack? __state) => OffensivePots.Current = __state;
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class OffensivePotProjectileUpdateScope
{
    [HarmonyPriority(int.MaxValue)]
    private static void Prefix(Projectile __instance, out OffensivePots.Attack? __state)
    { __state = OffensivePots.Current; OffensivePots.Current = OffensivePots.ProjectileAttack(__instance); }
    private static void Finalizer(OffensivePots.Attack? __state) => OffensivePots.Current = __state;
}

[HarmonyPatch]
internal static class OffensivePotShotPatch
{
    private static IEnumerable<MethodBase> TargetMethods() => typeof(Board).GetMethods()
        .Where(method => method.Name == nameof(Board.AddProjectile));
    private static void Postfix(Projectile? __result)
    { if (__result != null) OffensivePots.Capture(__result); }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.ProjectileInitialize))]
internal static class OffensivePotInitializedShotPatch
{ private static void Postfix(Projectile __instance) => OffensivePots.Capture(__instance); }

// Retaliation callbacks recurse. Isolate the attack before other prefixes and
// restore it even when a prefix throws.
[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class OffensivePotDamageScope
{
    [HarmonyPriority(int.MaxValue)]
    private static void Prefix(Zombie __instance, out (OffensivePots.Attack?, OffensivePots.Damage?) __state)
    {
        __state = (OffensivePots.Current, OffensivePots.Hitting);
        var attack = OffensivePots.Current;
        OffensivePots.Hitting = attack != null && attack.Board.Pointer == __instance.mBoard?.Pointer
            ? new(__instance, __instance.DataID, attack) : null;
        OffensivePots.Current = null;
    }
    private static void Finalizer((OffensivePots.Attack?, OffensivePots.Damage?) __state)
    { OffensivePots.Current = __state.Item1; OffensivePots.Hitting = __state.Item2; }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class OffensivePotDamagePatch
{
    [HarmonyPriority(Priority.Last + 2)]
    private static void Prefix(Zombie __instance, ref int theDamage)
    {
        if (OffensivePots.Hitting is { } hit && hit.Zombie.Pointer == __instance.Pointer && hit.Id == __instance.DataID)
            theDamage = OffensivePots.HitDamage(hit, theDamage);
    }
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class OffensivePotSavedTickPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(GridItem __instance)
    { if (!OffensivePots.Owns(__instance)) return true; OffensivePots.TickEffect(__instance); return false; }
}
[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class OffensivePotSavedDrawPatch
{ private static bool Prefix(GridItem __instance) => !OffensivePots.Owns(__instance); }
