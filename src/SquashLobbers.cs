using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Native lobbers own placement and saves. These fields are unused by their
// native AI: TargetX is Spring charge, TargetY its attribute countdown.
internal static class SquashLobbers
{
    internal const int Lettuce = -540100, Little = -540110, Corn = -540200,
        Butter = -540201, Gourd = -540300, GourdLanded = -540310, Spring = -540400;
    internal static bool Owns(FusionPlant identity) => identity == FusionPlant.LettucePult
        || identity == FusionPlant.ButternutPult || identity == FusionPlant.GourdPult
        || identity == FusionPlant.SpringMelon;
    internal static int Family(Projectile shot) => shot.mProjectileType == ProjectileType.Cabbage
        && shot.mCobTargetX <= Lettuce && shot.mCobTargetX >= Lettuce - 2 ? 1
        : shot.mProjectileType == ProjectileType.Cabbage && shot.mCobTargetX <= Little && shot.mCobTargetX >= Little - 2 ? 2
        : shot.mProjectileType == ProjectileType.Kernel && shot.mCobTargetX == Corn ? 3
        : shot.mProjectileType == ProjectileType.Butter && shot.mCobTargetX == Butter ? 4
        : shot.mProjectileType == ProjectileType.Melon && shot.mCobTargetX == Gourd ? 5
        : shot.mProjectileType == ProjectileType.Melon && shot.mCobTargetX == GourdLanded ? 6
        : shot.mProjectileType == ProjectileType.Melon && shot.mCobTargetX == Spring ? 7
        : shot.mProjectileType == ProjectileType.Kernel && shot.mCobTargetX == Corn - 10 ? 8
        : shot.mProjectileType == ProjectileType.Butter && shot.mCobTargetX == Butter - 10 ? 9 : 0;
    internal static int Generation(Projectile shot) => Family(shot) == 2
        ? Little - (int)shot.mCobTargetX : Family(shot) == 1 ? Lettuce - (int)shot.mCobTargetX : 0;
    internal static int ChargedDamage(int charge) => 120 + Math.Clamp(charge, 0, 3000) * 60 / 100;
    internal static void Initialize(Plant plant)
    {
        if (!Owns(FusionPlant.From(plant))) return;
        plant.mTargetX = 0;
        plant.mTargetY = FusionPlant.From(plant) == FusionPlant.SpringMelon ? 100 : 0;
    }
    internal static void Fertilize(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.SpringMelon && plant.mTargetY > 50)
            plant.mTargetY = 50;
    }
    internal static Zombie? Target(Plant plant)
    {
        var target = PuffLobbers.Target(plant);
        if (target == null || FusionPlant.From(plant) != FusionPlant.SpringMelon) return target;
        var rect = target.GetZombieRect();
        // The shipped CheckRange uses seven source units, not four tiles.
        return (rect.xMin + rect.xMax) / 2 < plant.mX + plant.mWidth / 2f + 7 * ScaredyFusions.SourceUnit
            ? target : null;
    }
    internal static void Tick(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        bool spring = identity == FusionPlant.SpringMelon;
        if (spring)
        {
            plant.mTargetX = Math.Min(3000, plant.mTargetX + 1);
            if (plant.mTargetY > 0) --plant.mTargetY;
        }
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mStartRow == 0)
        {
            if (plant.mLaunchCounter > 0 || spring && (plant.mTargetX <= 150 || plant.mTargetY > 0)
                || Target(plant) == null) return;
            plant.mLaunchCounter = 285 + plant.mBoard.Rand(31);
            plant.mStartRow = LobberFusions.ShootTicks;
            plant.mShootingCounter = identity == FusionPlant.ButternutPult && plant.mBoard.Rand(4) == 0 ? 1 : 0;
        }
        if (LobberFusions.ShootTicks - plant.mStartRow == (spring ? 42 : 34)) Fire(plant);
        if (--plant.mStartRow == 0) plant.mShootingCounter = 0;
    }
    internal static void Fire(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        bool spring = identity == FusionPlant.SpringMelon, melon = spring || identity == FusionPlant.GourdPult;
        var shot = plant.mBoard.AddProjectile(plant.mX + plant.mWidth / 2f,
            plant.mY + plant.mHeight / 2f - 10, plant.mRenderOrder + 1, plant.mRow,
            melon ? ProjectileType.Melon : identity == FusionPlant.ButternutPult
                ? plant.mShootingCounter == 1 ? ProjectileType.Butter : ProjectileType.Kernel : ProjectileType.Cabbage);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = spring ? Spring : melon ? Gourd : identity == FusionPlant.LettucePult ? Lettuce
            : plant.mShootingCounter == 1 ? Butter : Corn;
        int damage = spring ? ChargedDamage(plant.mTargetX) : melon ? 120 : 60;
        shot.mCobTargetRow = melon && PotBoosts.HasSupport(plant, FusionPlant.MelonPot) ? -damage : damage;
        LobberProjectiles.Launch(shot, Target(plant), PuffLobbers.FlightTicks);
        if (spring) { plant.mTargetX = 0; plant.mTargetY = 100; }
        ImportedPeaVisuals.EnsureSquashLobProjectile(shot);
    }
    private static bool Eligible(Board board, Zombie zombie) => zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
        && SquashFusions.Eligible(board, zombie, DamageRangeFlags.Ground);
    internal static void Impact(Projectile shot, Zombie? direct)
    {
        int family = Family(shot);
        if (shot.mDead || family == 0 || family is 2 or 6 or 8 or 9) return;
        if (family == 7)
        {
            LobberImpact.Apply(shot, direct, Math.Abs(shot.mCobTargetRow), true, false, 0, shot.mCobTargetRow < 0);
            return;
        }
        // These source bullets do not damage on contact in flight. A collider
        // slows horizontal motion; the landing owns the entire crush event.
        if (direct != null && Eligible(shot.mBoard, direct)
            && (uint)shot.mTargetZombieID != direct.DataID)
        {
            if (family != 1 || shot.mVelZ >= 0) shot.mVelX *= .5f;
            shot.mTargetZombieID = (ZombieID)direct.DataID;
        }
    }
    internal static void Motion(Projectile shot)
    {
        var board = shot.mBoard;
        if (shot.mDead || board.mPaused || board.mApp.GameScene != GameScenes.Playing) return;
        int family = Family(shot);
        if (family is 2 or 6 or 8 or 9)
        {
            int previous = shot.mHitTorchwoodGridX;
            int age = shot.mHitTorchwoodGridX += family == 2 ? (int)(uint)shot.mTargetZombieID : 1000;
            if (family == 6 && age >= 100000)
            {
                // The original creates a normal melon and calls HitLand:
                // every target receives splash, with no direct-hit recipient.
                LobberImpact.Apply(shot, null, Math.Abs(shot.mCobTargetRow), true, false, 0, shot.mCobTargetRow < 0);
            }
            else if (family == 2)
            {
                if (previous < 48334 && age >= 48334 || previous < 146667 && age >= 146667) Child(shot);
                if (age >= 213334) shot.Die();
            }
            else if (age >= 100000) shot.Die();
            return;
        }
        shot.mVelZ += shot.mAccZ;
        shot.mPosX += shot.mVelX;
        shot.mPosY += shot.mVelY;
        shot.mPosZ += shot.mVelZ;
        if (shot.mVelZ >= 0 && shot.mPosZ >= 0) Land(shot);
    }
    internal static void Land(Projectile shot)
    {
        var board = shot.mBoard;
        int family = Family(shot), generation = Generation(shot), damage = Math.Abs(shot.mCobTargetRow);
        uint id = shot.DataID;
        float x = shot.mPosX, y = shot.mPosY;
        shot.mPosZ = 0;
        shot.mVelX = shot.mVelY = shot.mVelZ = shot.mAccZ = 0;
        shot.mHitTorchwoodGridX = 0;
        shot.mTargetZombieID = family == 1 ? (ZombieID)(uint)(950 + board.Rand(101)) : 0;
        // Commit the phase before damage callbacks can re-enter this object.
        shot.mCobTargetX = family == 1 ? Little - generation : family == 3 ? Corn - 10 : family == 4 ? Butter - 10 : GourdLanded;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mRow != shot.mRow || !Eligible(board, zombie)) continue;
            var rect = zombie.GetZombieRect();
            float dx = x - Math.Clamp(x, rect.xMin, rect.xMax), dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= ScaredyFusions.SourceUnit * ScaredyFusions.SourceUnit)
                targets.Add((zombie, zombie.DataID));
        }
        int col = board.PixelToGridX(x, y);
        bool water = col >= 0 && col < 9 && board.IsPoolSquare(col, shot.mRow);
        if (water && family == 1) { shot.Die(); return; }
        int buttered = 0;
        foreach (var (zombie, targetId) in targets)
        {
            if (shot.DataID != id || shot.mDead || shot.mBoard?.Pointer != board.Pointer) return;
            if (zombie.DataID != targetId || !Eligible(board, zombie)) continue;
            zombie.TakeDamage(damage, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody);
            // Shipped AttackZombies increments after comparison: four targets,
            // despite the English almanac's description of three.
            if (family == 4 && buttered++ < 4 && zombie.DataID == targetId && Eligible(board, zombie))
            {
                int previous = zombie.mButteredCounter;
                zombie.ApplyButter();
                if (zombie.mButteredCounter > 0) zombie.mButteredCounter = Math.Max(previous, 400);
            }
        }
        if (shot.DataID != id || shot.mDead || shot.mBoard?.Pointer != board.Pointer) return;
        if (water || family == 1 && generation == 2) shot.Die();
        else ImportedPeaVisuals.EnsureSquashLobProjectile(shot);
    }
    private static void Child(Projectile parent)
    {
        int generation = Generation(parent);
        if (generation >= 2) return;
        var shot = parent.mBoard.AddProjectile(parent.mPosX, parent.mPosY,
            parent.mRenderOrder, parent.mRow, ProjectileType.Cabbage);
        if (shot == null || shot.mDead) return;
        shot.mCobTargetX = Lettuce - generation - 1;
        shot.mCobTargetRow = Math.Abs(parent.mCobTargetRow) / 2;
        LobberProjectiles.LaunchAt(shot, parent.mPosX, parent.mPosY, 140);
        shot.mVelX = (2000 + parent.mBoard.Rand(601) - 300) * ScaredyFusions.SourceUnit / 100000;
        shot.mVelZ = -(7000 + parent.mBoard.Rand(601) - 300) * ScaredyFusions.SourceUnit / 100000;
        ImportedPeaVisuals.EnsureSquashLobProjectile(shot);
    }
}

[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateLobMotion))]
internal static class SquashLobberMotionPatch
{
    internal static bool Prefix(Projectile __instance)
    {
        int family = SquashLobbers.Family(__instance);
        if (family == 0 || family == 7) return true;
        SquashLobbers.Motion(__instance); return false;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class SquashLobberImpactPatch
{
    internal static bool Prefix(Projectile __instance, Zombie theZombie)
    {
        if (SquashLobbers.Family(__instance) == 0) return true;
        SquashLobbers.Impact(__instance, theZombie); return false;
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class SquashLobberVisualPatch
{
    internal static void Postfix(Projectile __instance)
    { if (!__instance.mDead && SquashLobbers.Family(__instance) != 0) ImportedPeaVisuals.EnsureSquashLobProjectile(__instance); }
}
