using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace FusionMod;

// Umbrella state is serialized by the native plant. mTargetX stores energy;
// mTargetY stores recovery delay; mStartRow stores fractional recovery credits.
internal static class UmbrellaVariants
{
    internal static bool Lumos(Plant plant) => FusionPlant.From(plant) == FusionPlant.LumosUmbrella
        && ChomperFusions.Active(plant) && !plant.NotOnGround();
    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.UmbrellaRind) { plant.mPlantHealth = plant.mPlantMaxHealth = 2000; plant.mTargetX = 0; }
        if (identity == FusionPlant.LumosUmbrella) plant.mTargetX = 2000;
        if (identity == FusionPlant.LumosUmbrella || identity == FusionPlant.UmbrellaRind)
            plant.mTargetY = plant.mStartRow = 0;
    }
    internal static void Tick(Plant plant)
    {
        if (!Lumos(plant)) return;
        if (plant.mTargetY > 0) { --plant.mTargetY; return; }
        plant.mStartRow += 25 * PumpkinFusions.LightLevel(plant);
        if (plant.mStartRow >= 100)
        { plant.mTargetX = Math.Min(2000, plant.mTargetX + plant.mStartRow / 100); plant.mStartRow %= 100; }
    }
    internal static void Fog(Board board)
    {
        if (!board.StageHasFog() || board.mPaused) return;
        for (int i = 0; i < board.m_plants.Count; ++i)
            if (board.m_plants[i] is { } plant && Lumos(plant)) FadeFog(board, plant);
    }
    internal static bool Enemy(Projectile shot) => shot.mProjectileType is ProjectileType.ZombiePea or ProjectileType.Basketball;
    internal static bool Intercept(Projectile shot)
    {
        if (shot.mDead || shot.mBoard.mPaused || !Enemy(shot)) return false;
        var board = shot.mBoard;
        for (int i = 0; i < board.m_plants.Count; ++i)
        {
            var plant = board.m_plants[i];
            if (plant == null || !ChomperFusions.Active(plant) || plant.NotOnGround()) continue;
            var identity = FusionPlant.From(plant);
            if (identity != FusionPlant.LumosUmbrella && identity != FusionPlant.UmbrellaRind) continue;
            float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
            if (identity == FusionPlant.LumosUmbrella)
            {
                // Basketballs remain owned by the native umbrella defense.
                if (shot.mProjectileType == ProjectileType.Basketball || plant.mTargetX <= 0) continue;
                float dx = shot.mPosX - x, dy = shot.mPosY - y;
                if (dx * dx + dy * dy > 2.25f * UmbrellaShell.SourceUnit * UmbrellaShell.SourceUnit) continue;
                LumosEnergy.Spend(plant, 20);
                plant.mShootingCounter = UmbrellaShell.AnimationTicks;
                shot.Die(); return true;
            }
            if (shot.mProjectileType == ProjectileType.Basketball)
            {
                // Native lob defense owns the basketball interception.
                continue;
            }
            if (shot.mRow != plant.mRow || Math.Abs(shot.mPosX - x) > .9f * UmbrellaShell.SourceUnit) continue;
            int damage = 20;
            var returned = board.AddProjectile(shot.mPosX, shot.mPosY, shot.mRenderOrder, shot.mRow, ProjectileType.Pea);
            if (returned == null || returned.mDead) return false;
            returned.mCobTargetX = Reflected;
            returned.mCobTargetRow = damage;
            returned.mMotionType = ProjectileMotion.Star;
            returned.mVelX = Math.Abs(shot.mVelX);
            returned.mVelY = -shot.mVelY;
            returned.mShadowY = returned.mPosY + PuffShooters.FlightHeight;
            plant.mTargetX = (int)Math.Min(int.MaxValue, (long)plant.mTargetX + damage);
            plant.mShootingCounter = UmbrellaShell.AnimationTicks;
            ImportedPeaVisuals.EnsureUmbrellaProjectile(returned);
            shot.Die(); return true;
        }
        return false;
    }
    internal const int Reflected = -615201;
    internal static bool ReflectedPea(Projectile shot) => shot.mCobTargetX == Reflected && shot.mProjectileType == ProjectileType.Pea;

    internal const int ReflectedLob = -615202;
    internal static bool ReflectedMelon(Projectile shot) => shot.mCobTargetX == ReflectedLob && shot.mProjectileType == ProjectileType.Melon;
    internal static void Basketball(Plant umbrella, Projectile incoming)
    {
        var board = umbrella.mBoard;
        var returned = board.AddProjectile(incoming.mPosX, incoming.mPosY, umbrella.mRenderOrder + 1, incoming.mRow, ProjectileType.Melon);
        if (returned == null || returned.mDead) return;
        returned.mCobTargetX = ReflectedLob;
        LobberFlight.Launch(returned, Target(umbrella), 80);
        umbrella.mTargetX = (int)Math.Min(int.MaxValue, (long)umbrella.mTargetX + 80);
        ImportedPeaVisuals.EnsureUmbrellaProjectile(returned);
    }
    private static void FadeFog(Board board, Plant plant)
    {
        var fog = board.mGridCelFog;
        if (fog == null) return;
        var array = fog.Cast<Il2CppSystem.Array>();
        PotFusions.FadeFog(fog.Pointer, array.GetLength(0), array.GetLength(1), plant.mPlantCol, plant.mRow, 1);
    }
    internal static Zombie? Target(Plant plant)
    {
        Zombie? target = null; float distance = float.MaxValue;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || zombie.mRow != plant.mRow || !SquashFusions.Eligible(plant.mBoard, zombie, DamageRangeFlags.Ground)) continue;
            float next = Math.Abs(zombie.mPosX - plant.mX - plant.mWidth / 2f);
            if (next < distance) { target = zombie; distance = next; }
        }
        return target;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class UmbrellaVariantsInitializePatch
{ [HarmonyPriority(Priority.Last)] private static void Postfix(Plant __instance) => UmbrellaVariants.Initialize(__instance); }
[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class UmbrellaVariantsTickPatch
{ private static void Postfix(Plant __instance) => UmbrellaVariants.Tick(__instance); }
[HarmonyPatch(typeof(Board), nameof(Board.UpdateFog))]
internal static class UmbrellaVariantsFogPatch
{ private static void Postfix(Board __instance) => UmbrellaVariants.Fog(__instance); }
[HarmonyPatch(typeof(Projectile), nameof(Projectile.CheckForCollision))]
internal static class UmbrellaInterceptPatch
{ [HarmonyPriority(Priority.First + 500)] private static bool Prefix(Projectile __instance) => !UmbrellaVariants.Intercept(__instance); }

[HarmonyPatch(typeof(Projectile), nameof(Projectile.UpdateLobMotion))]
internal static class UmbrellaBasketballScope
{
    [ThreadStatic] internal static Projectile? Incoming;
    private static void Prefix(Projectile __instance, out Projectile? __state)
    { __state = Incoming; Incoming = !__instance.mDead && __instance.mProjectileType == ProjectileType.Basketball ? __instance : null; }
    private static void Finalizer(Projectile? __state) => Incoming = __state;
}
[HarmonyPatch(typeof(Board), nameof(Board.FindUmbrellaPlant))]
internal static class UmbrellaBasketballReturnPatch
{
    private static void Postfix(Plant? __result)
    {
        var shot = UmbrellaBasketballScope.Incoming;
        if (shot == null || __result == null || __result.mBoard?.Pointer != shot.mBoard?.Pointer
            || FusionPlant.From(__result) != FusionPlant.UmbrellaRind) return;
        UmbrellaBasketballScope.Incoming = null;
        UmbrellaVariants.Basketball(__result, shot);
    }
}
[HarmonyPatch(typeof(Projectile), nameof(Projectile.Update))]
internal static class UmbrellaReturnedVisualPatch
{ private static void Postfix(Projectile __instance) { if (!__instance.mDead) ImportedPeaVisuals.EnsureUmbrellaProjectile(__instance); } }
