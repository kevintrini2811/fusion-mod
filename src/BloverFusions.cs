using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Blover's native support/terrain identity remains intact. This family owns
// its saved ability clock and suppresses the native one-shot disappearance.
internal static class BloverFusions
{
    internal const int BlowEvent = 49, CycloneTicks = 120;
    internal const float Unit = 80f / 1.35f;
    internal static bool Playing(Board board) => !board.mPaused && board.mApp != null
        && board.mApp.GameScene == GameScenes.Playing
        && board.mApp.GameMode != GameMode.ChallengeZenGarden
        && board.Pointer != CherryBombPlants.DisposingBoard;
    internal static bool Live(Board board, Zombie zombie) => zombie.mBoard?.Pointer == board.Pointer
        && !zombie.mDead && !zombie.IsDeadOrDying()
        && zombie.mZombieType is not (ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target);

    internal static void Initialize(Plant plant)
    {
        if (!FusionPlant.IsWindBlover(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mDoSpecialCountdown = 0;
        plant.mLaunchCounter = plant.mStartRow = plant.mTargetX = plant.mTargetY = 0;
    }

    internal static void Tick(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsWindBlover(identity) || !ChomperFusions.Active(plant)) return;
        if (identity == FusionPlant.Cyclover) { Cyclone(plant); return; }
        ++plant.mTargetX;
        if (plant.mStartRow == 0 && plant.mTargetX >= BlowEvent)
        {
            if (identity == FusionPlant.Glowver && !Glowver.Grant(plant.mBoard)) return;
            plant.mStartRow = 1;
            Blow(plant);
            if (identity == FusionPlant.Disguster) Poison(plant);
            else if (identity == FusionPlant.MindBlover) Drag(plant, true);
        }
        if (plant.mStartRow == 0) return;
        if (identity == FusionPlant.BreezeBlover)
        {
            // DoomBlover.KeepEffect starts with zero remaining time, then
            // tests strictly below zero. Ten pulses, including the last one.
            PolarProjectiles.Convert(plant.mBoard);
            if (plant.mTargetX == BlowEvent || --plant.mLaunchCounter >= 0) return;
            plant.mLaunchCounter = 100;
            Cold(plant);
            if (++plant.mTargetY >= 10) plant.Die();
        }
        else if (identity == FusionPlant.MindBlover)
        {
            if (plant.mTargetX >= BlowEvent + 500) { plant.Die(); return; }
            if (plant.mTargetX > BlowEvent) Drag(plant, false);
        }
        else if (plant.mTargetX >= BlowEvent + 200) plant.Die();
    }

    internal static void Blow(Plant plant)
    {
        var board = plant.mBoard;
        board.mFogBlownCountDown = Math.Max(board.mFogBlownCountDown, 3000);
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && Live(board, zombie)
                && !zombie.mMindControlled && zombie.mZombieType == ZombieType.Balloon && zombie.IsFlying())
                zombie.mBlowingAway = true;
        plant.mBoard.mApp.SoundSystem.PlayFoley(Il2CppReloaded.Services.FoleyType.Floop);
    }

    internal static void Poison(Plant plant)
    {
        var board = plant.mBoard;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && Live(board, zombie) && !zombie.mMindControlled)
                targets.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !Live(board, zombie) || zombie.mMindControlled) continue;
            // GarlicBlover sets the level to at least five, then invokes
            // AddPoisonLevel. SetPoison adds the sixth level and five seconds.
            if (!BloverEffects.Reserve(zombie, false)) continue;
            while (PoisonShooters.Level(zombie) < 5) PoisonShooters.AddLevel(board, zombie);
            PoisonShooters.AddLevel(board, zombie);
            if (zombie.DataID != id || !Live(board, zombie)) continue;
            // Balloon removal is independent of poisoning; flying balloons
            // do not enter the ground-only native garlic row-change state.
            if (!zombie.IsFlying() && CanRedirect(zombie)) SupportFusions.Redirect(zombie);
        }
    }

    internal static bool CanRedirect(Zombie zombie) => zombie.mZombieType is not
        (ZombieType.Boss or ZombieType.Gravestone or ZombieType.Target or ZombieType.Zamboni
         or ZombieType.Catapult or ZombieType.Bobsled or ZombieType.Bungee)
        && !zombie.mYuckyFace && zombie.EffectedByDamage(DamageRangeFlags.Ground | DamageRangeFlags.Dog
            | (zombie.mMindControlled ? DamageRangeFlags.OnlyMindcontrolled : 0));

    internal static void Cold(Plant plant)
    {
        var board = plant.mBoard;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && Live(board, zombie) && !zombie.mMindControlled
                && zombie.mZombieType != ZombieType.Bungee
                && zombie.EffectedByDamage(DamageRangeFlags.Ground | DamageRangeFlags.Flying
                    | DamageRangeFlags.Submerged | DamageRangeFlags.OffGround | DamageRangeFlags.Dog))
                targets.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !Live(board, zombie) || zombie.mMindControlled) continue;
            zombie.TakeDamage(1, DamageFlags.BypassesShield);
            if (zombie.DataID != id || !Live(board, zombie)) continue;
            zombie.ApplyChill(false);
            if (BloverEffects.Reserve(zombie, true)) SnowPea.Hit(zombie, 20);
            board.mApp.AddTodParticle(zombie.mPosX, zombie.mPosY, zombie.mRenderOrder, ParticleEffect.IceSparkle);
        }
    }

    internal static float DragFactor(Zombie zombie) => zombie.IsFlying() ? .2f
        : zombie.mZombieType is ZombieType.Zamboni or ZombieType.Catapult or ZombieType.Bobsled ? .3f
        : zombie.mZombieType is ZombieType.Gargantuar or ZombieType.RedeyeGargantuar ? .4f : 1;

    internal static void Drag(Plant plant, bool first)
    {
        var board = plant.mBoard;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && Live(board, zombie) && zombie.mMindControlled)
                targets.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !Live(board, zombie) || !zombie.mMindControlled
                || board.PixelToGridX((int)zombie.mPosX, (int)zombie.mPosY) <= 0) continue;
            if (first)
            {
                int damage = (int)Math.Min(int.MaxValue, ((long)Math.Max(0, zombie.mBodyMaxHealth)
                    + Math.Max(0, zombie.mHelmMaxHealth)) / 10);
                zombie.TakeDamage(damage, DamageFlags.BypassesShield);
            }
            if (zombie.DataID != id || !Live(board, zombie) || !zombie.mMindControlled) continue;
            Move(zombie, -2 * Unit * .01f * DragFactor(zombie));
        }
    }

    internal static List<Zombie> CycloneTargets(Plant plant)
    {
        var targets = new List<Zombie>();
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
            if (plant.mBoard.m_zombies[i] is { } zombie && Live(plant.mBoard, zombie)
                && !zombie.mMindControlled && zombie.mRow == plant.mRow
                && zombie.mZombieType != ZombieType.Bungee
                && zombie.EffectedByDamage(DamageRangeFlags.Ground | DamageRangeFlags.Flying
                    | DamageRangeFlags.Submerged | DamageRangeFlags.OffGround | DamageRangeFlags.Dog)
                && plant.mBoard.PixelToGridX((int)zombie.mPosX, (int)zombie.mPosY) >= plant.mPlantCol)
                targets.Add(zombie);
        return targets.OrderBy(z => z.mPosX).Take(3).ToList();
    }

    internal static void Cyclone(Plant plant)
    {
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mTargetY == 0)
        {
            if (plant.mLaunchCounter > 0 || CycloneTargets(plant).Count == 0) return;
            plant.mTargetY = CycloneTicks;
            plant.mLaunchCounter = 285 + plant.mBoard.Rand(31);
        }
        int elapsed = CycloneTicks - plant.mTargetY--;
        // The original blow clip's animation events own this active window.
        if (elapsed == 50 && plant.mStartRow == 0) { plant.mStartRow = 1; Blow(plant); }
        if (elapsed == 50) ImportedPeaVisuals.FumeSmoke(plant, FusionPlant.Cyclover);
        if (elapsed < 50 || elapsed >= 100) return;
        foreach (var zombie in CycloneTargets(plant))
        {
            float speed = zombie.IsFlying() ? 2 : 1;
            if (zombie.mAltitude > 0 && !zombie.IsFlying()) speed *= 3;
            if (zombie.mZombieType is ZombieType.Zamboni or ZombieType.Catapult or ZombieType.Bobsled
                or ZombieType.Gargantuar or ZombieType.RedeyeGargantuar) speed *= .33f;
            Move(zombie, speed * Unit * .01f);
        }

    }

    internal static void Move(Zombie zombie, float amount)
    {
        // Board-space adaptation of Zombie.KnockBack and Transform.position.
        // Do not clamp at the right edge: that would pull entering foes left.
        zombie.mPosX += amount;
        zombie.mX = (int)zombie.mPosX;
    }


}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class WindBloverInitializePatch
{
    private static void Postfix(Plant __instance) => BloverFusions.Initialize(__instance);
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class WindBloverAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!FusionPlant.IsWindBlover(FusionPlant.From(__instance))) return true;
        BloverFusions.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class WindBloverShootingPatch
{
    private static bool Prefix(Plant __instance) => !FusionPlant.IsWindBlover(FusionPlant.From(__instance));
}
[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class WindBloverSpecialPatch
{
    private static bool Prefix(Plant __instance) => !FusionPlant.IsWindBlover(FusionPlant.From(__instance));
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class DisgusterBitePatch
{
    private static void Prefix(Zombie __instance, Plant thePlant)
    {
        if (thePlant != null && FusionPlant.From(thePlant) == FusionPlant.Disguster
            && ChomperFusions.Active(thePlant) && BloverFusions.Live(thePlant.mBoard, __instance)
            && !__instance.mMindControlled && BloverFusions.CanRedirect(__instance)) SupportFusions.Redirect(__instance);
    }
}
