using System.Reflection;
using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native Pumpkin owns the accessory layer, blocking, terrain and saved HP.
internal static class PumpkinFusions
{
    internal static bool IsSpiky(Plant plant) => FusionPlant.From(plant) == FusionPlant.SpikyPumpkin;
    internal static bool CanFeed(Plant plant, FusionPlant incoming) => incoming == (FusionPlant)SeedType.Seashroom
        && FusionPlant.From(plant) == FusionPlant.SeaShell && ChomperFusions.Active(plant);
    internal static void Feed(Plant plant) => ReactivePots.Heal(plant, plant.mPlantMaxHealth / 3);

    // Query live light owners instead of saving a duplicate light grid. Source
    // AdjustLightLevel adds providers, including stacked supports and shells.
    internal static int LightLevel(Plant target)
    {
        int level = Glowver.Level(target.mBoard);
        for (int i = 0; i < target.mBoard.m_plants.Count; ++i)
        {
            var plant = target.mBoard.m_plants[i];
            if (plant == null || !ChomperFusions.Active(plant) || plant.mIsAsleep) continue;
            var identity = FusionPlant.From(plant);
            int radius = identity == FusionPlant.LumosPot ? 0
                : identity == FusionPlant.BeaconShroom ? plant.mShootingCounter == 0 ? 1 : 2
                : identity == FusionPlant.LumosUmbrella || identity == FusionPlant.JackOPlantern || identity == FusionPlant.IllumiNut || identity == (FusionPlant)SeedType.Plantern
                    || identity == FusionPlant.Triplantern || identity == FusionPlant.AuroraPlantern || identity == FusionPlant.BlacklightPlantern ? 1 : -1;
            if (radius >= 0 && Math.Abs(plant.mPlantCol - target.mPlantCol) <= radius
                && Math.Abs(plant.mRow - target.mRow) <= radius) level += identity == FusionPlant.Triplantern ? 3 : 1;
        }
        return level;
    }

    internal static void SummonBlover(Plant plant)
    {
        var board = plant.mBoard;
        // Source scans from its own tile toward the right edge. Native terrain
        // and support checks own whether the empty interior accepts a Blover.
        for (int col = plant.mPlantCol; col < 9; ++col)
        {
            if (board.GetPlantsOnLawn(col, plant.mRow).NormalPlant != null
                || FusionService.CheckNativePlacement(board, col, plant.mRow, SeedType.Blover) != PlantingReason.Ok) continue;
            var previous = PlayerPlantingScope.Current;
            try
            {
                PlayerPlantingScope.Current = null;
                var blover = board.AddPlant(col, plant.mRow, SeedType.Blover, SeedType.None);
                if (blover != null && !blover.mDead) return;
            }
            finally { PlayerPlantingScope.Current = previous; }
            return; // Allocation failure consumes neither another tile nor a plant.
        }
    }

    private static bool ProtectedWeed(Plant plant) => plant.mSeedType is SeedType.Spikeweed or SeedType.Spikerock
        || FusionPlant.From(plant) == FusionPlant.SpikeNut;
    internal static bool AbsorbCrush(Plant plant)
    {
        if (!ChomperFusions.Active(plant)) return false;
        var shell = IsSpiky(plant) ? plant : ProtectedWeed(plant)
            ? plant.mBoard.GetPlantsOnLawn(plant.mPlantCol, plant.mRow).PumpkinPlant : null;
        if (shell == null || !IsSpiky(shell) || !ChomperFusions.Active(shell)) return false;
        // CaltropPumpkin.Crashed charges once per source OnUpdate, even when
        // one tire/hammer encounters both the shell and the enclosed weed.
        int tick = plant.mBoard.mMainCounter;
        if (shell.mTargetX != tick)
        {
            shell.mTargetX = tick;
            shell.mPlantHealth -= PotDefense.Damage(shell, 500);
            if (shell.mPlantHealth <= 0) shell.Die();
            var crush = WeedFusions.CurrentCrush;
            if (crush != null && crush.Board.Pointer == plant.mBoard.Pointer
                && crush.Zombie.DataID == crush.ZombieId && !crush.Zombie.mDead && !crush.Zombie.IsDeadOrDying()
                && crush.Zombie.mZombieType is ZombieType.Zamboni or ZombieType.Catapult)
                crush.Zombie.TakeDamage(1800, DamageFlags.BypassesShield | DamageFlags.Spike);
        }
        return true;
    }

    internal const int KernelGrowthTicks = 4950; // .01 to 1 at .02 source units/second.
    internal static bool Armored(Plant shell) => shell.mPlantHealth * 3L > shell.mPlantMaxHealth;
    internal static bool Protects(Plant shell) => shell.mPlantHealth * 3L > shell.mPlantMaxHealth * 2L;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsPumpkinFusion(identity)) return;
        plant.mPlantMaxHealth = identity == FusionPlant.NutShell ? 8000
            : identity == FusionPlant.ScarelessPumpkin ? 2000 : 4000;
        plant.mPlantHealth = identity == FusionPlant.PuffKin ? 1000 : plant.mPlantMaxHealth;
        plant.mLaunchCounter = identity == FusionPlant.PuffKin ? 300 : identity == FusionPlant.NutShell ? KernelGrowthTicks : 0;
        plant.mStartRow = 0;
        plant.mTargetX = identity == FusionPlant.NutShell ? 3 : identity == FusionPlant.SpikyPumpkin ? -1 : 0;
        if (identity == FusionPlant.WindyPumpkin) plant.mLaunchCounter = 3000;
        if (identity == FusionPlant.JackOPlantern) plant.mLaunchCounter = 1500;
    }

    internal static void Fertilize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if ((identity == FusionPlant.PuffKin || identity == FusionPlant.WindyPumpkin
            || identity == FusionPlant.LumosUmbrella || identity == FusionPlant.JackOPlantern || identity == FusionPlant.PumpkinPod
            || identity == FusionPlant.CryoPumpkin || identity == FusionPlant.Chompkin) && plant.mLaunchCounter > 50)
            plant.mLaunchCounter = 50;
    }

    internal static bool Target(Plant plant, Zombie zombie) =>
        zombie.mRow >= plant.mRow && zombie.mRow <= plant.mRow + 3
        && zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
        && SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)
        && (int)MathF.Floor((zombie.GetZombieRect().center.x
            - plant.mBoard.GridToPixelX(0, zombie.mRow)) / 80) == plant.mPlantCol;

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant)) return;
        if (!FusionTime.SourceStep(plant, typeof(PumpkinFusions))) return;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.WindyPumpkin || identity == FusionPlant.JackOPlantern)
        {
            if (plant.mLaunchCounter > 0 && --plant.mLaunchCounter > 0) return;
            plant.mLaunchCounter = identity == FusionPlant.WindyPumpkin ? 3000 : 1500;
            if (identity == FusionPlant.WindyPumpkin) SummonBlover(plant);
            else ReactivePots.Heal(plant, Math.Max(0, LightLevel(plant) - 1) * 150);
            return;
        }
        if (identity == FusionPlant.NutShell)
        {
            if (plant.mLaunchCounter > 0 && --plant.mLaunchCounter > 0) return;
            GrowNut(plant);
            return;
        }
        if (identity == FusionPlant.PuffKin)
        {
            if (plant.mLaunchCounter > 0 && --plant.mLaunchCounter > 0) return;
            plant.mLaunchCounter = 300;
            ReactivePots.Heal(plant, 50);
        }
        if (identity != FusionPlant.FumingPumpkin) return;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0) return;
            bool target = false;
            for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
                if (plant.mBoard.m_zombies[i] is { } zombie && Target(plant, zombie)) { target = true; break; }
            if (!target) return;
            plant.mStartRow = 75;
            plant.mLaunchCounter = 150;
        }
        int elapsed = 75 - plant.mStartRow;
        if (elapsed == 33) Smoke(plant);
        if (elapsed is 43 or 53 or 63)
        {
            var targets = new List<(Zombie Zombie, uint Id)>();
            for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
                if (plant.mBoard.m_zombies[i] is { } zombie && Target(plant, zombie)) targets.Add((zombie, zombie.DataID));
            foreach (var (zombie, id) in targets)
            {
                if (!ChomperFusions.Active(plant)) break;
                if (zombie.DataID == id && Target(plant, zombie)) zombie.TakeDamage(20, DamageFlags.BypassesShield);
            }
        }
        --plant.mStartRow;
    }

    // The original kernel grows over 50 seconds in Update, independently of
    // Plant's attribute countdown. Fertilizer heals but does not rush this clock.
    internal static void GrowNut(Plant plant)
    {
        int remaining = 1000;
        for (int i = 0; i < plant.mBoard.m_plants.Count && remaining > 0; ++i)
        {
            var other = plant.mBoard.m_plants[i];
            if (other == null || other.Pointer == plant.Pointer || other.mSeedType != SeedType.Pumpkinshell
                || !ChomperFusions.Active(other) || Math.Abs(other.mRow - plant.mRow) > 1
                || Math.Abs(other.mPlantCol - plant.mPlantCol) > 1 || other.mPlantHealth >= other.mPlantMaxHealth) continue;
            int heal = Math.Min(remaining, other.mPlantMaxHealth - other.mPlantHealth);
            ReactivePots.Heal(other, heal);
            remaining -= heal;
        }
        if (remaining == 1000 && plant.mTargetX > 0)
        {
            var coin = plant.mBoard.AddCoin(plant.mX, plant.mY, CoinType.UsableSeedPacket, CoinMotion.FromPlant);
            // Keep the ripe kernel on allocation failure; never spend a card.
            if (coin == null) return;
            coin.mUsableSeedType = SeedType.Wallnut;
            if (coin.mController != null) coin.mController.SeedType = SeedType.Wallnut;
            --plant.mTargetX;
        }
        plant.mLaunchCounter = KernelGrowthTicks;
    }

    internal static void Cherry(Board board, float x, float y, int row, float radius)
    {
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var shell = board.m_plants[i];
            if (shell == null || FusionPlant.From(shell) != FusionPlant.CherryPumpkin
                || !ChomperFusions.Active(shell) || Math.Abs(shell.mRow - row) > 1) continue;
            float dx = shell.mX + shell.mWidth / 2f - x, dy = shell.mY + shell.mHeight / 2f - y;
            if (dx * dx + dy * dy <= radius * radius) ReactivePots.Heal(shell, 200);
        }
    }

    internal static bool AbsorbCherry(Projectile shot)
    {
        if (shot.mDead || shot.mBoard.mPaused || !CherryShooter.IsExplosive(shot)) return false;
        // Source Cherry Pumpkin blocks the explosive cherry itself, stopping
        // its splash. Sweep the next native step so a fast boosted shot cannot skip it.
        float x = shot.mPosX, end = x + FumeProjectiles.Speed(shot);
        Plant? first = null;
        for (int i = 0; i < shot.mBoard.m_plants.Count; ++i)
        {
            var shell = shot.mBoard.m_plants[i];
            if (shell == null || shell.mRow != shot.mRow || FusionPlant.From(shell) != FusionPlant.CherryPumpkin
                || !ChomperFusions.Active(shell)) continue;
            float center = shell.mX + shell.mWidth / 2f;
            if (center + 20 < x || center - 20 > end || first != null && shell.mX >= first.mX) continue;
            first = shell;
        }
        if (first == null) return false;
        shot.Die();
        ReactivePots.Heal(first, 200);
        return true;
    }

    private static void Smoke(Plant plant)
    {
        ImportedPeaVisuals.FumeSmoke(plant, FusionPlant.FumeSpreader, degrees: -90);
        plant.mBoard.mApp.SoundSystem.PlayFoley(Il2CppReloaded.Services.FoleyType.Fume);
    }

    internal sealed class Hit
    {
        internal readonly Plant Plant, Shell;
        internal readonly uint Id, ShellId;
        internal readonly int Health;
        internal readonly bool Armor;
        internal bool Eating;
        internal bool Applied;
        internal Hit(Plant plant, Plant shell)
        {
            Plant = plant; Shell = shell; Id = plant.DataID; ShellId = shell.DataID;
            Health = plant.mPlantHealth; Armor = Armored(shell);
        }
        internal void Apply()
        {
            if (Applied || Plant.DataID != Id || Shell.DataID != ShellId
                || Plant.mBoard?.Pointer != Shell.mBoard?.Pointer || Plant.mPlantHealth >= Health) return;
            Applied = true;
            int damage = Health - Plant.mPlantHealth;
            if (Plant.Pointer == Shell.Pointer)
                Plant.mPlantHealth = Health - (Armor ? damage / 2 : damage);
            else
            {
                // Eligibility belongs to the pre-hit snapshot. Lethal damage
                // still redirects, even when the shell cannot survive it.
                Plant.mPlantHealth = Health;
                Shell.mPlantHealth -= PotDefense.Damage(Shell, Armor ? damage / 2 : damage);
                if (Current != null)
                    foreach (var shellHit in Current.Where(hit => hit.Plant.Pointer == Shell.Pointer)) shellHit.Applied = true;
                if (Shell.mPlantHealth <= 0 && !Shell.mDead) Shell.Die();
            }
        }
    }
    [ThreadStatic] internal static List<Hit>? Current;
    internal static Hit? Capture(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mPlantHealth <= 0) return null;
        if (FusionPlant.From(plant) == FusionPlant.PuffKin) return new(plant, plant);
        var shell = plant.mBoard.GetPlantsOnLawn(plant.mPlantCol, plant.mRow).PumpkinPlant;
        return shell != null && shell.Pointer != plant.Pointer && ChomperFusions.Active(shell)
            && FusionPlant.From(shell) == FusionPlant.PuffKin && Protects(shell) ? new(plant, shell) : null;
    }
    internal static void Apply(List<Hit>? hits)
    { if (hits != null) foreach (var hit in hits) hit.Apply(); }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class PumpkinInitializePatch
{
    private static void Postfix(Plant __instance) => PumpkinFusions.Initialize(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class PumpkinAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsPumpkinFusion(FusionPlant.From(__instance))) return true;
        PumpkinFusions.Tick(__instance);
        return false;
    }
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class PumpkinBitePatch
{
    private static void Prefix(Plant thePlant,
        out (List<PumpkinFusions.Hit>? Previous, Plant? Plant, uint Id, int Health) __state)
    {
        __state = (PumpkinFusions.Current, thePlant, thePlant?.DataID ?? 0, thePlant?.mPlantHealth ?? 0);
        PumpkinFusions.Current = thePlant != null && PumpkinFusions.Capture(thePlant) is { } hit ? new() { hit } : null;
        if (PumpkinFusions.Current != null) PumpkinFusions.Current[0].Eating = true;
    }
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(Zombie __instance,
        (List<PumpkinFusions.Hit>? Previous, Plant? Plant, uint Id, int Health) __state)
    {
        PumpkinFusions.Apply(PumpkinFusions.Current);
        var plant = __state.Plant;
        if (plant == null || plant.mBoard == null || plant.DataID != __state.Id || plant.mBoard.Pointer != __instance.mBoard?.Pointer
            || (FusionPlant.From(plant) != FusionPlant.ThornyPumpkin && FusionPlant.From(plant) != FusionPlant.StinkyPumpkin)
            || plant.mPlantHealth >= __state.Health
            || __instance.mDead || __instance.IsDeadOrDying() || __instance.mMindControlled) return;
        if (FusionPlant.From(plant) == FusionPlant.StinkyPumpkin)
        {
            PoisonShooters.PoisonBite(plant.mBoard, __instance);
            __instance.StopEating();
            __instance.mYuckyFace = true;
            __instance.mYuckyFaceCounter = 0;
            __instance.UpdateAnimSpeed();
        }
        else __instance.TakeDamage((int)Math.Min(int.MaxValue,
            (long)__state.Health - plant.mPlantHealth), DamageFlags.BypassesShield);
    }
    private static void Finalizer((List<PumpkinFusions.Hit>? Previous, Plant? Plant, uint Id, int Health) __state) =>
        PumpkinFusions.Current = __state.Previous;
}
[HarmonyPatch]
internal static class PumpkinProjectileDefensePatch
{
    private static IEnumerable<MethodBase> TargetMethods() => new[] {
        AccessTools.Method(typeof(Projectile), nameof(Projectile.CheckForCollision)),
        AccessTools.Method(typeof(Projectile), nameof(Projectile.UpdateLobMotion)) };
    private static void Prefix(Projectile __instance, out List<PumpkinFusions.Hit>? __state)
    {
        __state = PumpkinFusions.Current;
        PumpkinFusions.Current = null;
        if (__instance.mDead || __instance.mProjectileType is not (ProjectileType.ZombiePea or ProjectileType.Basketball)) return;
        var hits = new List<PumpkinFusions.Hit>();
        for (int i = 0; i < __instance.mBoard.m_plants.Count; ++i)
            if (__instance.mBoard.m_plants[i] is { } plant && PumpkinFusions.Capture(plant) is { } hit) hits.Add(hit);
        PumpkinFusions.Current = hits;
    }
    [HarmonyPriority(Priority.Last)]
    private static void Postfix() => PumpkinFusions.Apply(PumpkinFusions.Current);
    private static void Finalizer(List<PumpkinFusions.Hit>? __state) => PumpkinFusions.Current = __state;
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class PumpkinLethalDefensePatch
{
    [HarmonyPriority(Priority.Last)]
    private static bool Prefix(Plant __instance, bool __runOriginal)
    {
        if (!__runOriginal || __instance.mPlantHealth > 0) return true;
        var hit = PumpkinFusions.Current?.FirstOrDefault(hit => hit.Plant.Pointer == __instance.Pointer && !hit.Applied);
        if (hit == null) return true;
        hit.Apply();
        if (__instance.mPlantHealth <= 0) return true;
        if (hit.Eating) --__instance.mBoard.mPlantsEaten;
        return false;
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateNormalMotion))]
internal static class CherryPumpkinAbsorptionPatch
{
    [HarmonyPriority(Priority.First)]
    private static bool Prefix(Projectile __instance) => !PumpkinFusions.AbsorbCherry(__instance);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.CanTargetPlant))]
internal static class SpikyPumpkinTargetPatch
{
    private static bool Prefix(Plant thePlant, ZombieAttackType theAttackType, ref bool __result)
    {
        if (thePlant == null || !PumpkinFusions.IsSpiky(thePlant)
            || theAttackType == ZombieAttackType.DriveOver) return true;
        __result = false;
        return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class SpikyPumpkinCrushPatch
{
    private static bool Prefix(Plant __instance, bool __runOriginal)
    {
        if (!__runOriginal || !PumpkinFusions.AbsorbCrush(__instance)) return true;
        // SquishAllInSquare counts every plant immediately before Squish.
        // Undo only a surviving recipient in this proven native caller.
        if (!__instance.mDead && WeedFusions.CurrentCrush?.Board.Pointer == __instance.mBoard.Pointer)
            --__instance.mBoard.mPlantsEaten;
        return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.SpikeRockTakeDamage))]
internal static class SpikyPumpkinRockPatch
{
    private static bool Prefix(Plant __instance) => !PumpkinFusions.AbsorbCrush(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class SpikyPumpkinTirePatch
{
    private static bool Prefix(Plant __instance)
    {
        var attack = WeedFusions.CurrentAttack;
        // Only tire-contact deaths inside the same plant's spike attack.
        // Shovel, ingredient consumption and unrelated death remain native.
        return attack == null || attack.PlantId != __instance.DataID
            || attack.Board.Pointer != __instance.mBoard?.Pointer
            || PlayerPlantingScope.ConsumingIngredient == __instance.Pointer
            || !PumpkinFusions.AbsorbCrush(__instance);
    }
}
