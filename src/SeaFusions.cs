using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// Native saved fields own timers and growth. Sea-shroom owns aquatic terrain,
// coffee, carrying, health and serialization. Imported clips never emit shots.
internal static class SeaFusions
{
    internal static bool Low(FusionPlant identity) => identity == FusionPlant.MinnowShroom
        || identity == FusionPlant.UrchinShroom || identity == FusionPlant.SeaStar;
    internal const float Unit = 80f / 1.35f;
    internal static bool Active(Plant plant) => ChomperFusions.Active(plant) && !plant.mIsAsleep;
    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsSeaFusion(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mLaunchRate = 150;
        plant.mLaunchCounter = plant.mStartRow = plant.mDoSpecialCountdown = plant.mTargetY = 0;
        plant.mShootingCounter = identity == FusionPlant.MinnowShroom ? 2 : 0;
        plant.mTargetX = identity == FusionPlant.MinnowShroom ? 1500
            : identity == FusionPlant.UrchinShroom ? 12000 : identity == FusionPlant.SeaStar ? 6000 : 0;
    }

    internal static void Fertilize(Plant plant)
    {
        // Source caps AttributeCountdown, including mature reproduction and
        // the current Shark range buff. It preserves shorter timers.
        if (FusionPlant.IsSeaFusion(FusionPlant.From(plant)) && plant.mTargetX > 50) plant.mTargetX = 50;
    }

    internal static bool CanFeed(Plant plant, FusionPlant incoming) => incoming == (FusionPlant)SeedType.Seashroom
        && (FusionPlant.From(plant) == FusionPlant.SharkShroom && Active(plant)
            || SeaSupportFusions.CanFeed(plant, incoming) || PumpkinFusions.CanFeed(plant, incoming));
    internal static Plant? FeedTarget(Board board, int col, int row)
    {
        var lawn = board.GetPlantsOnLawn(col, row);
        return lawn.PumpkinPlant is { } shell && PumpkinFusions.CanFeed(shell, SeedType.Seashroom)
            ? shell : lawn.NormalPlant;
    }
    internal static void Feed(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.SeaShell) { PumpkinFusions.Feed(plant); return; }
        if (FusionPlant.IsSeaSupport(FusionPlant.From(plant))) { SeaSupportFusions.Feed(plant); return; }
        plant.mTargetX = 1000;
        plant.mTargetY = -100;
        // Source status 25 is PlantStatus.Raised, not a shooting delay.
        // Eat replaces the current animation; preserve its attack cooldown.
        plant.mStartRow = 0;
    }

    internal static int Duration(Plant plant) => FusionPlant.From(plant) == FusionPlant.SharkShroom ? 100
        : FusionPlant.From(plant) == FusionPlant.SeaStar ? (plant.mDoSpecialCountdown & 1) != 0 ? 84 : 67
        : FusionPlant.From(plant) == FusionPlant.UrchinShroom && (plant.mDoSpecialCountdown & 1) != 0 ? 110 : 50;

    private static bool Eligible(Board board, Zombie zombie, bool air) => zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
        && zombie.IsFlying() == air && SquashFusions.Eligible(board, zombie, air ? DamageRangeFlags.Flying : DamageRangeFlags.Ground);

    internal static Zombie? Target(Plant plant, bool air)
    {
        Zombie? target = null;
        float distance = float.MaxValue, origin = plant.mX + plant.mWidth / 2f;
        var identity = FusionPlant.From(plant);
        float range = identity == FusionPlant.MinnowShroom ? 4.5f * Unit
            : identity == FusionPlant.SharkShroom ? (plant.mTargetX > 0 ? 14 : 7) * Unit : float.MaxValue;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || zombie.mRow != plant.mRow || !Eligible(plant.mBoard, zombie, air)) continue;
            var rect = zombie.GetZombieRect();
            float d = (rect.xMin + rect.xMax) / 2 - origin;
            if (d >= 0 && d < range && d < distance) { target = zombie; distance = d; }
        }
        return target;
    }

    internal static void Tick(Plant plant)
    {
        if (!Active(plant)) return;
        var identity = FusionPlant.From(plant);
        if (plant.mTargetY > 0) --plant.mTargetY;
        else if (plant.mTargetY < 0) ++plant.mTargetY;
        if (plant.mTargetX > 0 && --plant.mTargetX == 0)
        {
            if (identity == FusionPlant.MinnowShroom)
            {
                plant.mTargetX = 1500; // Commit before allocation and callbacks.
                if (plant.mShootingCounter == 2) plant.mShootingCounter = 3;
                else Reproduce(plant);
                if (!Active(plant)) return;
            }
            else if (identity != FusionPlant.SharkShroom && plant.mShootingCounter == 0)
            { plant.mShootingCounter = 1; plant.mTargetY = 100; }
        }
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mTargetY != 0) return;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0) return;
            bool air = identity == FusionPlant.UrchinShroom && Target(plant, true) != null;
            bool target = identity == FusionPlant.SeaStar ? StarFusions.HasStarTarget(plant)
                : air || Target(plant, false) != null;
            if (!target) return;
            plant.mDoSpecialCountdown = (identity != FusionPlant.MinnowShroom && plant.mShootingCounter > 0 ? 1 : 0)
                | (air ? 2 : 0);
            plant.mStartRow = Duration(plant);
            plant.mLaunchCounter = 143 + plant.mBoard.Rand(15);
        }
        int elapsed = Duration(plant) - plant.mStartRow + 1;
        --plant.mStartRow;
        bool grown = (plant.mDoSpecialCountdown & 1) != 0;
        bool fire = identity == FusionPlant.SeaStar ? elapsed == 42 || grown && elapsed == 67
            : identity == FusionPlant.SharkShroom ? elapsed == 35
            : elapsed == 31 || identity == FusionPlant.UrchinShroom && grown && elapsed is 50 or 70 or 90;
        if (fire) Fire(plant);
    }

    internal static void Fire(Plant plant)
    {
        if (!Active(plant)) return;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.SeaStar)
        {
            foreach (float angle in StarProjectiles.Angles)
                StarProjectiles.Spawn(plant.mBoard, plant.mX + plant.mWidth / 2f,
                    plant.mY + plant.mHeight / 2f, plant.mRenderOrder, plant.mRow, 6,
                    angle, angle == 180 ? 4.8f : 4, 20);
            return;
        }
        if (identity == FusionPlant.SharkShroom) { Smoke(plant); return; }
        int count = identity == FusionPlant.MinnowShroom ? plant.mShootingCounter : 1;
        bool air = (plant.mDoSpecialCountdown & 2) != 0;
        for (int i = 0; i < count; ++i)
            SeaProjectiles.Spawn(plant, identity == FusionPlant.MinnowShroom ? 1 : air ? 3 : 2,
                (i - (count - 1) / 2f) * 8);
    }

    private static void Smoke(Plant plant)
    {
        var board = plant.mBoard;
        var targets = new List<(Zombie Zombie, uint Id)>();
        float origin = plant.mX + plant.mWidth / 2f;
        float range = (plant.mTargetX > 0 ? 14 : 7) * Unit;
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mRow != plant.mRow || !Eligible(board, zombie, false)) continue;
            var rect = zombie.GetZombieRect();
            float x = (rect.xMin + rect.xMax) / 2;
            if (x >= origin && x <= origin + range) targets.Add((zombie, zombie.DataID));
        }
        uint owner = plant.DataID;
        foreach (var (zombie, id) in targets)
        {
            if (plant.DataID != owner || !Active(plant)) return;
            if (zombie.DataID == id && Eligible(board, zombie, false)) zombie.TakeDamage(20, DamageFlags.BypassesShield);
            // Extra knockback requires Kraken's three-second marker. Kraken
            // is unavailable; unrelated native knockback must not qualify.
        }
        ImportedPeaVisuals.FumeSmoke(plant, FusionPlant.SharkShroom);
        board.mApp.SoundSystem.PlayFoley(FoleyType.Fume);
    }

    internal static void Reproduce(Plant plant)
    {
        var board = plant.mBoard;
        int col = plant.mPlantCol + 1;
        var forward = col < 9 ? FeedTarget(board, col, plant.mRow) : null;
        if (forward != null && CanFeed(forward, SeedType.Seashroom)) { Feed(forward); return; }
        var plans = FusionService.ResolveAll(board, SeedType.Seashroom, col, plant.mRow);
        if (plans.Count > 0 && plans.All(plan => FusionService.CheckFusionPlacement(board, plan.Target, plan.Result) == PlantingReason.Ok))
        {
            var context = PlayerPlantingScope.Current;
            try { PlayerPlantingScope.Current = null; FusionService.CompleteBatch(board, SeedType.Seashroom, plans); return; }
            finally { PlayerPlantingScope.Current = context; }
        }
        var coin = board.AddCoin(plant.mX, plant.mY, CoinType.UsableSeedPacket, CoinMotion.FromPlant);
        if (coin == null) return;
        coin.mUsableSeedType = SeedType.Seashroom;
        if (coin.mController != null) coin.mController.SeedType = SeedType.Seashroom;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class SeaInitializePatch
{ private static void Postfix(Plant __instance) => SeaFusions.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class SeaAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsSeaFusion(FusionPlant.From(__instance))) return true;
        SeaFusions.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class SeaShootingPatch
{ private static bool Prefix(Plant __instance) => !FusionPlant.IsSeaFusion(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class SeaNativeFirePatch
{ private static bool Prefix(Plant __instance) => !FusionPlant.IsSeaFusion(FusionPlant.From(__instance)); }

// Keep native payment/card/cooldown handling, returning the living recipient
// from AddPlant instead of allocating or initializing a replacement.
[HarmonyPatch(typeof(Board), nameof(Board.CanPlantAt))]
internal static class SeaFeedPlacementPatch
{
    private static void Postfix(Board __instance, int theGridX, int theGridY, SeedType theType, ref PlantingReason __result)
    {
        if (FusionService.CheckingNativePlacement
            || theGridX < 0 || theGridX >= 9 || theGridY < 0 || theGridY >= __instance.GetNumRows()) return;
        var plant = SeaFusions.FeedTarget(__instance, theGridX, theGridY);
        if (__result != PlantingReason.NotHere && !(plant != null
            && FusionPlant.From(plant) == FusionPlant.SeaShell && __result == PlantingReason.OnlyInPool)) return;
        if (plant != null && SeaFusions.CanFeed(plant, theType)) __result = PlantingReason.Ok;
    }
}
[HarmonyPatch(typeof(Board), nameof(Board.AddPlant))]
internal static class SeaFeedCreationPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Board __instance, int theGridX, int theGridY, SeedType theSeedType,
        SeedType theImitaterType, ref Plant __result)
    {
        if (FusionService.CreatingBatch || PlayerPlantingScope.ActiveBoard?.Pointer != __instance.Pointer
            || theImitaterType != SeedType.None || theGridX < 0 || theGridX >= 9
            || theGridY < 0 || theGridY >= __instance.GetNumRows()) return true;
        var plant = SeaFusions.FeedTarget(__instance, theGridX, theGridY);
        if (plant == null || !SeaFusions.CanFeed(plant, theSeedType)) return true;
        SeaFusions.Feed(plant); __result = plant; return false;
    }
}

// Reuse the native low category during collision lookup only. Preserve native
// overlap iteration and pumpkin priority. Basketballs still hit these plants.
[HarmonyPatch(typeof(Projectile), nameof(Projectile.FindCollisionTargetPlant))]
internal static class SeaLowCollisionScope
{
    internal sealed record Saved(Plant Plant, uint Id, IntPtr Board, SeedType Type);
    internal static void Prefix(Projectile __instance, out List<Saved>? __state)
    {
        __state = null;
        if (__instance.mProjectileType != ProjectileType.ZombiePea || __instance.mBoard == null) return;
        __state = new();
        for (int i = 0; i < __instance.mBoard.m_plants.Count; ++i)
        {
            var plant = __instance.mBoard.m_plants[i];
            if (plant == null || plant.mDead || !(SeaFusions.Low(FusionPlant.From(plant)) || FusionPlant.From(plant) == FusionPlant.SeaMine || FusionPlant.From(plant) == FusionPlant.KelpMine || SeaSupportFusions.Low(plant) || FusionPlant.From(plant) == FusionPlant.PuffKin || FusionPlant.From(plant) == FusionPlant.PumpkinMine || PumpkinFusions.IsSpiky(plant))) continue;
            __state.Add(new(plant, plant.DataID, plant.mBoard.Pointer, plant.mSeedType));
            plant.mSeedType = SeedType.Potatomine;
        }
    }
    internal static void Finalizer(List<Saved>? __state)
    {
        if (__state == null) return;
        foreach (var saved in __state)
            if (saved.Plant.DataID == saved.Id && saved.Plant.mBoard?.Pointer == saved.Board)
                saved.Plant.mSeedType = saved.Type;
    }
}
