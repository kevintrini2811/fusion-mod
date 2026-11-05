using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Effects follow their recipient, not the pot. Native GridItems serialize the
// recipient DataID, duration and unused speed credits; no profile state is used.
internal static class ReactivePots
{
    private const int EffectMarker = -26000;
    internal static bool IsEffect(GridItem item) => item.mGridItemType == GridItemType.ZenTool
        && item.mGridItemState == GridItemState.Normal && item.mSunCount == EffectMarker
        && item.mSeedType is SeedType.Cherrybomb or SeedType.Jalapeno or SeedType.Hypnoshroom;

    private static IEnumerable<GridItem> Effects(Board board, uint id, SeedType kind)
    {
        for (int i = 0; i < board.m_gridItems.Count; ++i)
        {
            var item = board.m_gridItems[i];
            if (item != null && !item.mDead && IsEffect(item)
                && item.mGridX == unchecked((int)id) && item.mSeedType == kind) yield return item;
        }
    }

    private static GridItem? AddEffect(Board board, uint id, SeedType kind, int ticks)
    {
        var item = board.m_gridItems.DataArrayAlloc();
        if (item == null) return null;
        try
        {
            item.Init(board.mApp, GridItemType.ZenTool);
            item.mGridItemState = GridItemState.Normal;
            item.mSunCount = EffectMarker;
            item.mSeedType = kind;
            item.mGridX = unchecked((int)id);
            item.mGridY = 0;
            item.mGridItemCounter = ticks;
            item.mX = item.mY = item.mPosX = item.mPosY = -1000;
            return item;
        }
        catch { item.GridItemDie(); throw; }
    }

    private static Plant? PlantOwner(GridItem item)
    {
        var plants = item.mBoard.m_plants;
        for (int i = 0; i < plants.Count; ++i)
            if (plants[i] is { } plant && plant.DataID == unchecked((uint)item.mGridX) && !plant.mDead)
                return plant;
        return null;
    }

    private static Zombie? ZombieOwner(GridItem item)
    {
        var zombies = item.mBoard.m_zombies;
        for (int i = 0; i < zombies.Count; ++i)
            if (zombies[i] is { } zombie && zombie.DataID == unchecked((uint)item.mGridX)
                && !zombie.mDead && !zombie.IsDeadOrDying()) return zombie;
        return null;
    }

    internal static void TickEffect(GridItem item)
    {
        var board = item.mBoard;
        if (!Playing(board)) return;
        bool exists = item.mSeedType == SeedType.Hypnoshroom
            ? ZombieOwner(item) != null : PlantOwner(item) != null;
        if (!exists || item.mGridItemCounter > 0 && --item.mGridItemCounter == 0) item.GridItemDie();
    }

    private static bool Playing(Board board) => !board.mPaused && board.mApp != null
        && board.mApp.GameScene == GameScenes.Playing
        && board.mApp.GameMode != GameMode.ChallengeZenGarden;

    private static IEnumerable<Plant> Cell(Board board, int col, int row)
    {
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var plant = board.m_plants[i];
            if (plant != null && plant.mPlantCol == col && plant.mRow == row
                && ChomperFusions.Active(plant) && !plant.NotOnGround()) yield return plant;
        }
    }

    internal static void Heal(Plant plant, int amount)
    {
        plant.mPlantHealth = (int)Math.Min(plant.mPlantMaxHealth, (long)plant.mPlantHealth + amount);
        plant.UpdateReanim();
        plant.UpdateReanimColor();
    }

    internal static bool CherryShooter(FusionPlant identity) => FusionPlant.IsCherryShooter(identity)
        || identity == FusionPlant.CherrySpreader || identity == FusionPlant.BlastyShroom;

    internal static bool FireShooter(FusionPlant identity) => FusionPlant.IsBlazeShooter(identity)
        || identity == FusionPlant.ScorchedThreepeater || identity == FusionPlant.FlameShroom
        || identity == FusionPlant.FlameGloomShroom || identity == FusionPlant.PhoenixThreepeater
        || identity == FusionPlant.FlameCattail;

    internal static void Cherry(Board board, float x, float y, int row, float radius)
    {
        if (!Playing(board)) return;
        PumpkinFusions.Cherry(board, x, y, row, radius);
        var pots = new List<Plant>();
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var pot = board.m_plants[i];
            if (pot == null || FusionPlant.From(pot) != FusionPlant.CherryPot
                || !ChomperFusions.Active(pot) || pot.NotOnGround() || Math.Abs(pot.mRow - row) > 1) continue;
            float dx = pot.mX + pot.mWidth / 2f - x, dy = pot.mY + pot.mHeight / 2f - y;
            if (dx * dx + dy * dy <= radius * radius) pots.Add(pot);
        }
        foreach (var pot in pots)
            foreach (var plant in Cell(board, pot.mPlantCol, pot.mRow).ToArray())
            {
                Heal(plant, 20);
                if (!CherryShooter(FusionPlant.From(plant))) continue;
                var effect = Effects(board, plant.DataID, SeedType.Cherrybomb).FirstOrDefault();
                // CherryFastShoot replaces its existing effect: refresh, never stack.
                if (effect != null) effect.mGridItemCounter = 500;
                else AddEffect(board, plant.DataID, SeedType.Cherrybomb, 500);
            }
    }

    internal static void Fire(Board board, int row)
    {
        if (!Playing(board)) return;
        foreach (var pot in Enumerable.Range(0, 9).SelectMany(col => Cell(board, col, row)).ToArray())
        {
            if (FusionPlant.From(pot) != FusionPlant.HotPot || pot.mTargetY > 0) continue;
            pot.mTargetY = 20;
            foreach (var plant in Cell(board, pot.mPlantCol, row).ToArray())
            {
                if (!FireShooter(FusionPlant.From(plant))) continue;
                Heal(plant, 20);
                // Independent five-second +.5 modifiers, as JalaPot.AddSpeed.
                AddEffect(board, plant.DataID, SeedType.Jalapeno, 500);
            }
        }
    }

    internal static void Speed(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!CherryShooter(identity) && !FireShooter(identity)) return;
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        var effects = Effects(plant.mBoard, plant.DataID, SeedType.Cherrybomb)
            .Concat(Effects(plant.mBoard, plant.DataID, SeedType.Jalapeno)).ToArray();
        // GridY counts saved half-ticks. Keep credits while an animation is
        // running: reducing the next launch below its duration cancels a pea.
        foreach (var effect in effects)
            if (effect.mGridY < int.MaxValue) ++effect.mGridY;
        bool nativeAnimation = FusionPlant.IsBlazeShooter(identity) || FusionPlant.IsCherryShooter(identity);
        if (plant.mLaunchCounter <= 1 || nativeAnimation && plant.mShootingCounter > 0) return;
        long extra = effects.Sum(effect => (long)Math.Max(0, effect.mGridY) / 2);
        int next = (int)Math.Max(1, (long)plant.mLaunchCounter - extra);
        // Leave native UpdateShooter one tick to cross each exact event.
        if ((plant.mSeedType is SeedType.Repeater or SeedType.Splitpea) && plant.mLaunchCounter >= 26)
            next = Math.Max(26, next);
        int spent = plant.mLaunchCounter - next;
        foreach (var effect in effects)
        {
            int take = Math.Min(spent, Math.Max(0, effect.mGridY) / 2);
            effect.mGridY -= take * 2;
            spent -= take;
        }
        plant.mLaunchCounter = next;
    }

    internal static void Tick(Plant pot)
    {
        if (!ChomperFusions.Active(pot)) return;
        var identity = FusionPlant.From(pot);
        if (identity == FusionPlant.HotPot && pot.mTargetY > 0) --pot.mTargetY;
        if (identity != FusionPlant.HypnoPot || pot.NotOnGround()) return;
        if (pot.mLaunchCounter > 0 && --pot.mLaunchCounter > 0) return;
        pot.mLaunchCounter = 4;
        float center = pot.mX + pot.mWidth / 2f;
        for (int i = 0; i < pot.mBoard.m_zombies.Count; ++i)
        {
            var zombie = pot.mBoard.m_zombies[i];
            // Source OverlapCircle radius 1, same row and friendly team.
            if (zombie == null || zombie.mDead || zombie.IsDeadOrDying() || !zombie.mMindControlled
                || zombie.mRow != pot.mRow || Math.Abs(center - Math.Clamp(center, zombie.GetZombieRect().xMin, zombie.GetZombieRect().xMax)) > 80f / 1.35f) continue;
            if (!Effects(pot.mBoard, zombie.DataID, SeedType.Hypnoshroom).Any())
                AddEffect(pot.mBoard, zombie.DataID, SeedType.Hypnoshroom, -1);
            zombie.mBodyHealth = Math.Min(zombie.mBodyMaxHealth, zombie.mBodyHealth + 1);
        }
    }

    internal static int Reduce(Zombie zombie, int damage)
    {
        if (damage <= 0 || !zombie.mMindControlled || zombie.mBoard == null) return damage;
        var effect = Effects(zombie.mBoard, zombie.DataID, SeedType.Hypnoshroom).FirstOrDefault();
        if (effect == null) return damage;
        long value = damage * 2L + effect.mGridY;
        effect.mGridY = (int)(value % 3);
        return (int)(value / 3);
    }

    internal static bool Bite(Plant plant, Zombie zombie)
    {
        if (FusionPlant.From(plant) != FusionPlant.HypnoPot) return false;
        if (!ChomperFusions.Active(plant) || PotFusions.Protected(plant) || zombie.mMindControlled || zombie.mDead
            || zombie.IsDeadOrDying() || zombie.mBoard?.Pointer != plant.mBoard.Pointer
            || plant.mBoard.mPaused) return true;
        var plain = plant.mBoard.AddPlant(plant.mPlantCol, plant.mRow, SeedType.Flowerpot, SeedType.None);
        if (plain == null || plain.mDead || !plain.mIsOnBoard) return true;
        PlayerPlantingScope.Consume(plant);
        zombie.StartMindControlled();
        return true;
    }

    internal static Plant? DoomPot(Board board, int col, int row) =>
        Cell(board, col, row).FirstOrDefault(p => FusionPlant.From(p) == FusionPlant.DoomPot);

    internal static bool PreventCrater(Board board, int col, int row)
    {
        var pot = DoomPot(board, col, row);
        if (pot == null) return false;
        // SetDoom reduces BOTH maximum and current health by 75, then deals
        // 20% of the reduced maximum. The pot can die while still blocking.
        pot.mPlantMaxHealth -= 75;
        pot.mPlantHealth -= 75 + (int)Math.Round(pot.mPlantMaxHealth * .2f);
        if (pot.mPlantHealth <= 0 || pot.mPlantMaxHealth <= 0)
        {
            var old = PotFusions.CostingHealth;
            PotFusions.CostingHealth = pot.Pointer;
            try { pot.Die(); }
            finally { PotFusions.CostingHealth = old; }
        }
        else { pot.UpdateReanim(); pot.UpdateReanimColor(); }
        return true;
    }

    internal static void Crater(GridItem crater)
    {
        var board = crater.mBoard;
        if (!Playing(board) || crater.mDead || crater.mGridItemType != GridItemType.Crater) return;
        var pots = new List<Plant>();
        for (int i = 0; i < board.m_plants.Count; ++i)
            if (board.m_plants[i] is { } pot && FusionPlant.From(pot) == FusionPlant.DoomPot
                && ChomperFusions.Active(pot) && !pot.NotOnGround()) pots.Add(pot);
        if (pots.Count == 0) return;
        // Board.UpdateGridItems already removes one tick. One additional tick
        // halves the native 180-second crater life while any Doom Pot exists.
        if (crater.mGridItemCounter > 0 && --crater.mGridItemCounter == 0)
        { crater.GridItemDie(); return; }
        if (++crater.mTransparentCounter < 100) return;
        crater.mTransparentCounter = 0;
        var target = pots.Where(p => p.mRow == crater.mGridY).OrderBy(p => p.mPlantHealth).FirstOrDefault();
        if (target != null) Heal(target, (int)Math.Round(target.mPlantMaxHealth * .02f));
    }
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.Update))]
internal static class ReactivePotEffectTickPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(GridItem __instance)
    {
        if (!ReactivePots.IsEffect(__instance)) return true;
        ReactivePots.TickEffect(__instance);
        return false;
    }
    private static void Postfix(GridItem __instance) => ReactivePots.Crater(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class ReactivePotAbilityPatch
{
    [HarmonyPriority(Priority.First + 100)]
    private static void Prefix(Plant __instance) => ReactivePots.Speed(__instance);
    private static void Postfix(Plant __instance) => ReactivePots.Tick(__instance);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class HypnoPotDamagePatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Prefix(Zombie __instance, ref int theDamage) => theDamage = ReactivePots.Reduce(__instance, theDamage);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class HypnoPotBitePatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Zombie __instance, Plant thePlant, bool __runOriginal) =>
        !__runOriginal || !ReactivePots.Bite(thePlant, __instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class DoomPotSpecialPatch
{
    [HarmonyPriority(Priority.Last)]
    private static bool Prefix(Plant __instance, bool __runOriginal)
    {
        if (!__runOriginal || __instance.mSeedType != SeedType.Doomshroom
            || !ChomperFusions.Active(__instance) || __instance.mIsAsleep
            || FusionPlant.From(__instance) == FusionPlant.SilverDoomShroom
            || ReactivePots.DoomPot(__instance.mBoard, __instance.mPlantCol, __instance.mRow) == null) return true;
        // Native DoSpecial destroys the whole stack before AddACrater. Own
        // this one supported branch before that irreversible destruction.
        var board = __instance.mBoard;
        int col = __instance.mPlantCol, row = __instance.mRow;
        board.KillAllZombiesInRadius(row, __instance.mX + __instance.mWidth / 2f,
            __instance.mY + __instance.mHeight / 2f, 250, 3, true,
            DamageRangeFlags.Ground | DamageRangeFlags.Flying | DamageRangeFlags.Submerged
            | DamageRangeFlags.Dog | DamageRangeFlags.OffGround | DamageRangeFlags.Dying | DamageRangeFlags.Underground);
        ReactivePots.PreventCrater(board, col, row);
        __instance.Die();
        board.mApp.AddTodParticle(__instance.mX, __instance.mY, 400000,
            ParticleEffect.Doom);
        board.mApp.SoundSystem.PlayFoley(Il2CppReloaded.Services.FoleyType.Explosion);
        board.ShakeBoard(3, -4);
        return false;
    }
}

[HarmonyPatch(typeof(GridItem), nameof(GridItem.DrawGridItem))]
internal static class ReactivePotEffectDrawPatch
{
    private static bool Prefix(GridItem __instance) => !ReactivePots.IsEffect(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class HotPotJalapenoSpecialPatch
{
    private static void Prefix(Plant __instance, bool __runOriginal, out (Board Board, int Row)? __state) =>
        __state = __runOriginal && __instance.mSeedType == SeedType.Jalapeno
            && FusionPlant.From(__instance) == (FusionPlant)SeedType.Jalapeno && ChomperFusions.Active(__instance)
            ? (__instance.mBoard, __instance.mRow) : null;
    private static void Postfix((Board Board, int Row)? __state)
    {
        // Native DoSpecial embeds the row burn. BurnRow hooks alone miss it.
        // Hot Pot's saved debounce prevents duplicates when BurnRow is called.
        if (__state is { } burn) ReactivePots.Fire(burn.Board, burn.Row);
    }
}
