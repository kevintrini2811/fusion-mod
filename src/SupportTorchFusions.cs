using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Saved native counters and projectile tags own state across pause and loading.
// The source plants are passive: native animation events never create shots.
internal static class SupportTorchFusions
{
    internal const int Scorch = -622001, Garlic = PoisonShooters.GarlicTorchMarker, Stench = PoisonShooters.ToxicTorchMarker;
    internal static bool Owns(FusionPlant identity) => identity == FusionPlant.TideShroom
        || identity == FusionPlant.BlacklightPlantern || identity == FusionPlant.Scorchwood || identity == FusionPlant.Stenchwood;
    internal static bool Tagged(Projectile shot) => shot.mCobTargetX is Scorch or Garlic or Stench || ChargeTorchFusions.IsLowFire(shot);
    internal static int Damage(Projectile shot) => Tagged(shot) ? shot.mCobTargetRow : 20;
    internal static void Initialize(Plant plant)
    {
        if (!Owns(FusionPlant.From(plant))) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mDoSpecialCountdown = plant.mLaunchCounter = plant.mTargetX = plant.mStartRow = 0;
        plant.mTargetY = 300;
    }
    internal static void Tick(Plant plant)
    {
        if (!BeamShooters.Active(plant) || plant.NotOnGround()) return;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.BlacklightPlantern)
        { if (plant.mTargetY > 0) --plant.mTargetY; if (plant.mTargetY == 0) plant.mTargetY = 300; return; }
        if (identity != FusionPlant.TideShroom) return;
        if (plant.mTargetX < 100) ++plant.mTargetX;
        if (plant.mStartRow == 0)
        { plant.mStartRow = 1; BloverFusions.Blow(plant); }
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || zombie.mRow != plant.mRow || !zombie.mInPool || zombie.mMindControlled
                || !BloverFusions.Live(plant.mBoard, zombie) || zombie.mPosX >= 900) continue;
            // Compensate native forward walking, then provide a slow current.
            BloverFusions.Move(zombie, Math.Min(900 - zombie.mPosX, Math.Abs(zombie.mVelX) + .1f));
        }
    }
    internal static void Convert(Projectile shot)
    {
        var board = shot.mBoard;
        if (shot.mDead || board == null || board.mPaused || shot.mMotionType is not (ProjectileMotion.Straight or ProjectileMotion.Threepeater)
            || shot.mProjectileType is not (ProjectileType.Pea or ProjectileType.PeashooterPea or ProjectileType.Snowpea or ProjectileType.Fireball or ProjectileType.PeashooterFireball)
            || shot.mPosZ != 0) return;
        if (TorchPlantIndex.For(board).Length == 0) return;
        // Preserve custom projectile ownership. Only the original plain, yellow
        // flame, potato and garlic families are accepted by the original stumps.
        bool potato = PotatoProjectiles.Contains(shot);
        bool yellow = shot.mProjectileType is ProjectileType.Fireball or ProjectileType.PeashooterFireball;
        bool garlic = PoisonShooters.Contains(shot) || shot.mCobTargetX == Garlic;
        if (!Tagged(shot) && !garlic && (HypnoShooters.Contains(shot) || BlazeShooters.Contains(shot)
            || shot.mCobTargetX < 0 && !potato)) return;
        foreach (var plant in Stumps(shot))
        {
            if (shot.mHitTorchwoodGridX == plant.mPlantCol) continue;
            var identity = FusionPlant.From(plant);
            if (identity == FusionPlant.Scorchwood)
            {
                if (garlic || shot.mCobTargetX is Scorch or Stench) continue;
                if (shot.mProjectileType == ProjectileType.Snowpea)
                { shot.ConvertToPea(plant.mPlantCol); continue; }
                int damage = potato ? 30 : yellow ? 40 : 20;
                ImportedPeaVisuals.ForgetProjectile(shot);
                // Native flight must keep the low potato above the ground.
                if (potato) shot.mShadowY = shot.mPosY + PuffShooters.FlightHeight;
                shot.mProjectileType = ProjectileType.Fireball;
                shot.mCobTargetX = Scorch; shot.mCobTargetRow = damage + 30;
                shot.mHitTorchwoodGridX = plant.mPlantCol;
                ImportedPeaVisuals.EnsureTorchProjectile(shot);
            }
            else if (identity == FusionPlant.Stenchwood)
            {
                if (potato || yellow || shot.mProjectileType == ProjectileType.Snowpea || shot.mCobTargetX == Stench) continue;
                int damage = garlic ? (Tagged(shot) ? Damage(shot) : 30) * 2 : 30;
                shot.mCobTargetX = garlic ? Stench : Garlic;
                shot.mCobTargetRow = damage;
                shot.mHitTorchwoodGridX = plant.mPlantCol;
                garlic = true;
                ImportedPeaVisuals.EnsureTorchProjectile(shot);
            }
        }
    }
    private static IEnumerable<Plant> Stumps(Projectile shot)
    {
        var list = new List<Plant>();
        var board = shot.mBoard;
        foreach (var entry in TorchPlantIndex.For(board))
        {
            var plant = entry.Plant;
            if (!entry.IsCurrent(board) || plant.mRow != shot.mRow) continue;
            var identity = FusionPlant.From(plant);
            if (identity != FusionPlant.Scorchwood && identity != FusionPlant.Stenchwood) continue;
            if (!BeamShooters.Active(plant) || plant.NotOnGround()) continue;
            float x = plant.mX + plant.mWidth / 2f;
            if (Math.Min(shot.mPosX, shot.mPosX + shot.mVelX) <= x + 16
                && Math.Max(shot.mPosX, shot.mPosX + shot.mVelX) >= x - 16) list.Add(plant);
        }
        return shot.mVelX >= 0 ? list.OrderBy(p => p.mPlantCol) : list.OrderByDescending(p => p.mPlantCol);
    }
    internal sealed record Hit(Board Board, Zombie Zombie, uint Id, int Marker, int Damage, int Row, float X, float Y, int Order);
    [ThreadStatic] internal static Hit? CurrentHit;
    internal static Hit? Capture(Projectile shot, Zombie? zombie) => Tagged(shot) && zombie != null
        ? new(shot.mBoard, zombie, zombie.DataID, (int)shot.mCobTargetX, Damage(shot), zombie.mRow,
            zombie.GetZombieRect().center.x, zombie.GetZombieRect().center.y, zombie.mRenderOrder) : null;
    internal static void Finish(Hit? hit)
    {
        if (hit == null || hit.Marker != Stench || hit.Zombie.DataID != hit.Id) return;
        if (BloverEffects.Reserve(hit.Zombie, false))
        {
            PoisonShooters.AddLevel(hit.Board, hit.Zombie);
            PoisonShooters.ApplyPoison(hit.Board, hit.Zombie, 500);
        }
        PoisonShooters.TorchCloud(hit.Board, hit.Row, hit.X, hit.Y, hit.Order);
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < hit.Board.m_zombies.Count; ++i)
        {
            var zombie = hit.Board.m_zombies[i];
            if (zombie == null || zombie.DataID == hit.Id || !BloverFusions.Live(hit.Board, zombie)
                || zombie.mMindControlled || zombie.mRow != hit.Row) continue;
            if (Math.Abs(zombie.GetZombieRect().center.x - hit.X) <= 80) targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
            if (zombie.DataID == id && BloverFusions.Live(hit.Board, zombie)) zombie.TakeDamage(hit.Damage, 0);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class SupportTorchInitializePatch
{ private static void Postfix(Plant __instance) => SupportTorchFusions.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class SupportTorchAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (!SupportTorchFusions.Owns(FusionPlant.From(__instance))) return true;
        SupportTorchFusions.Tick(__instance); return false;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Fire))]
internal static class SupportTorchFirePatch
{ private static bool Prefix(Plant __instance) => !SupportTorchFusions.Owns(FusionPlant.From(__instance)); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class SupportTorchCrossingPatch
{ [HarmonyPriority(Priority.First + 800)] private static void Prefix(Projectile __instance) => SupportTorchFusions.Convert(__instance); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class SupportTorchVisualPatch
{ private static void Postfix(Projectile __instance) { if (!__instance.mDead) ImportedPeaVisuals.EnsureTorchProjectile(__instance); } }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.ConvertToFireball))]
internal static class SupportTorchNativeConversionPatch
{ private static bool Prefix(Projectile __instance) => !SupportTorchFusions.Tagged(__instance); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.DoImpact))]
internal static class SupportTorchImpactPatch
{
    private static void Prefix(Projectile __instance, Zombie theZombie, out SupportTorchFusions.Hit? __state)
    { __state = SupportTorchFusions.CurrentHit; SupportTorchFusions.CurrentHit = SupportTorchFusions.Capture(__instance, theZombie); }
    private static void Postfix()
    {
        var hit = SupportTorchFusions.CurrentHit;
        SupportTorchFusions.CurrentHit = null;
        SupportTorchFusions.Finish(hit);
    }
    private static void Finalizer(SupportTorchFusions.Hit? __state) => SupportTorchFusions.CurrentHit = __state;
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class SupportTorchDamagePatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Prefix(Zombie __instance, ref int theDamage)
    {
        var hit = SupportTorchFusions.CurrentHit;
        if (hit != null && __instance.DataID == hit.Id && __instance.mBoard?.Pointer == hit.Board.Pointer)
            theDamage = hit.Damage;
    }
}
[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class ScorchwoodDeathPatch
{
    private static void Prefix(Plant __instance, bool __runOriginal, out CherryBombPlants.Death? __state) =>
        __state = __runOriginal && FusionPlant.From(__instance) == FusionPlant.Scorchwood && CherryBombPlants.CanTriggerDeath(__instance)
            ? IceInstantFusions.Capture(__instance) : null;
    private static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        if (__state != null && __instance.mDead && __instance.DataID == __state.Id)
            DoomEffects.Damage(__state.Board, __state.Row, __state.X, __state.Y, __state.Order, fire: true);
    }
}
