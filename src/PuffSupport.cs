using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// These source prefabs have no shooter. Puff-shroom supplies only its native
// small-plant lifecycle; saved counters own production, growth and the fuse.
internal static class PuffSupport
{
    internal const int BellFuse = 100, GrowTicks = 50, MatureTicks = 12000;
    internal const int BlockTicks = 50, BlockHitTick = 17, ReproduceTicks = 3000;
    internal const float UmbrellaContact = 4.7069025f * .2f * .5f * ScaredyFusions.SourceUnit;
    internal const float BoomContactHalfWidth = 1.0584879f * .15f * .5f * ScaredyFusions.SourceUnit;
    internal const float BlastRadius = ScaredyFusions.SourceUnit;

    internal static void Initialize(Plant plant)
    {
        var identity = FusionPlant.From(plant);
        if (!FusionPlant.IsPuffSupport(identity)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = identity == FusionPlant.NutShroom ? 2000 : 300;
        plant.mLaunchCounter = plant.mStartRow = plant.mShootingCounter = plant.mTargetY = 0;
        plant.mTargetX = identity == FusionPlant.PennyShroom
            ? MatureTicks : identity == FusionPlant.UmbrellaCap ? ReproduceTicks : 0;
    }

    internal static bool GroundEnemy(Board board, Zombie zombie, int row) => zombie.mRow == row
        && zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
        && SquashFusions.Eligible(board, zombie, MineFusions.MineTargets);

    internal static bool Near(Plant plant, Zombie zombie, float radius)
    {
        if (!GroundEnemy(plant.mBoard, zombie, plant.mRow)) return false;
        var rect = zombie.GetZombieRect();
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        float dx = x - Math.Clamp(x, rect.xMin, rect.xMax);
        float dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
        return dx * dx + dy * dy <= radius * radius;
    }

    internal static void Tick(Plant plant)
    {
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep) return;
        var identity = FusionPlant.From(plant);
        if (identity == FusionPlant.CherryPop) { PuffContactFusions.Contact(plant); return; }
        if (identity == FusionPlant.PennyShroom) { ProduceTick(plant); return; }
        if (identity == FusionPlant.UmbrellaCap) { UmbrellaTick(plant); return; }
        if (identity == FusionPlant.NutShroom || identity == FusionPlant.GnomeShroom) return;
        if (identity == FusionPlant.BellPepper && plant.mStartRow > 0)
        {
            if (--plant.mStartRow == 0) Ignite(plant);
            return;
        }
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || !Near(plant, zombie,
                identity == FusionPlant.BellPepper ? BlastRadius : BoomContactHalfWidth)) continue;
            if (identity == FusionPlant.BellPepper)
            {
                // A tile owns one active pepper fuse. Later peppers acquire a
                // surviving target after the current explosion has completed.
                if (ShroomSlots.Plants(plant.mBoard, plant.mPlantCol, plant.mRow).Any(other =>
                    other.Pointer != plant.Pointer && FusionPlant.From(other) == FusionPlant.BellPepper && other.mStartRow > 0)) return;
                plant.mStartRow = BellFuse;
            }
            else if (identity == FusionPlant.BoomShroom) plant.Die(); // Death hook owns the blast, exactly once.
            return;
        }
    }

    private static void ProduceTick(Plant plant)
    {
        if (plant.mTargetY > 0) --plant.mTargetY;
        if (plant.mTargetX > 0 && --plant.mTargetX == 0) plant.mTargetY = GrowTicks;
        if (plant.mLaunchCounter > 0) --plant.mLaunchCounter;
        if (plant.mShootingCounter > 0 && --plant.mShootingCounter == 310)
        {
            // Source CoinMoney 34/35 pays 100/300. Native gold is worth 50;
            // ordinary saved pickups retain their value across save/load.
            int count = plant.mTargetX == 0 ? 6 : 2;
            for (int i = 0; i < count; ++i)
                LevelMoney.Mark(plant.mBoard.AddCoin(plant.mX + (i - (count - 1) / 2f) * 6,
                    plant.mY, CoinType.Gold, CoinMotion.Coin));
        }
        if (plant.mLaunchCounter != 0 || plant.mShootingCounter != 0) return;
        plant.mLaunchCounter = 2375 + plant.mBoard.Rand(251);
        // Producer starts brightening now and invokes ProduceSun after .5 s.
        plant.mShootingCounter = 360;
    }

    // ZombieInitialize writes only native scales 1, .8 and .5. Their .7
    // products are distinct saved identities; the native renderer and save
    // path already preserve mScaleZombie. Never apply shrink twice on load.
    internal static bool IsSmall(Zombie zombie) => Math.Abs(zombie.mScaleZombie - .7f) < .00001f
        || Math.Abs(zombie.mScaleZombie - .56f) < .00001f
        || Math.Abs(zombie.mScaleZombie - .35f) < .00001f;

    internal static void Shrink(Zombie zombie)
    {
        if (IsSmall(zombie) || zombie.mZombieType == ZombieType.Boss) return;
        zombie.mScaleZombie *= .7f;
        // Lawnf.SetZombieHealth scales current AND maximum body/armor HP,
        // truncating each separately. No damage callbacks, drops or healing.
        zombie.mBodyHealth = (int)(zombie.mBodyHealth * .7f);
        zombie.mBodyMaxHealth = (int)(zombie.mBodyMaxHealth * .7f);
        zombie.mHelmHealth = (int)(zombie.mHelmHealth * .7f);
        zombie.mHelmMaxHealth = (int)(zombie.mHelmMaxHealth * .7f);
        zombie.mShieldHealth = (int)(zombie.mShieldHealth * .7f);
        zombie.mShieldMaxHealth = (int)(zombie.mShieldMaxHealth * .7f);
        // Replanted stores Balloon's first armor separately from helmets.
        // Source SetZombieHealth scales that armor with all other health pools.
        zombie.mFlyingHealth = (int)(zombie.mFlyingHealth * .7f);
        zombie.mFlyingMaxHealth = (int)(zombie.mFlyingMaxHealth * .7f);
    }

    internal static bool CanRepel(Plant plant, Zombie zombie) => GroundEnemy(plant.mBoard, zombie, plant.mRow)
        && zombie.mZombieType is not (ZombieType.Boss or ZombieType.Zamboni or ZombieType.Catapult or ZombieType.Bungee);

    internal static bool BiteGnome(Plant plant, Zombie zombie)
    {
        if (FusionPlant.From(plant) != FusionPlant.GnomeShroom) return false;
        if (!ChomperFusions.Active(plant) || plant.mIsAsleep || !CanRepel(plant, zombie)) return false;
        var board = plant.mBoard;
        uint zombieId = zombie.DataID;
        zombie.StartMindControlled();
        plant.Die();
        if (zombie.DataID == zombieId && zombie.mBoard?.Pointer == board.Pointer
            && !zombie.mDead && !zombie.IsDeadOrDying()) Shrink(zombie);
        return true;
    }

    private static void UmbrellaTick(Plant plant)
    {
        if (plant.mTargetX > 0 && --plant.mTargetX == 0)
        {
            // SmallUmbrella.AttributeEvent always rearms, even on a full tile.
            ShroomSlots.TrySpawn(plant.mBoard, FusionPlant.UmbrellaCap, plant.mPlantCol, plant.mRow);
            plant.mTargetX = ReproduceTicks;
        }
        if (plant.mStartRow > 0)
        {
            --plant.mStartRow;
            if (BlockTicks - plant.mStartRow == BlockHitTick) RepelSmallUmbrella(plant);
            return;
        }
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie != null && CanRepel(plant, zombie) && Near(plant, zombie, UmbrellaContact))
            { plant.mStartRow = BlockTicks; return; }
        }
    }

    private static void RepelSmallUmbrella(Plant plant)
    {
        var board = plant.mBoard;
        board.mApp.SoundSystem.PlayFoley(FoleyType.Umbrella);
        uint plantId = plant.DataID;
        bool large = false;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie != null && CanRepel(plant, zombie) && Near(plant, zombie, ScaredyFusions.SourceUnit))
                targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
        {
            if (plant.DataID != plantId || plant.mBoard?.Pointer != board.Pointer || !ChomperFusions.Active(plant)) return;
            if (zombie.DataID != id || !CanRepel(plant, zombie)) continue;
            large |= !IsSmall(zombie);
            // Reuse the existing umbrella displacement adapter; source distance
            // is two units. This pulse causes no damage or butter.
            zombie.mPosX = Math.Min(850, zombie.mPosX + (PotBoosts.HasSupport(plant, FusionPlant.UmbrellaPot) ? 3 : 2) * ScaredyFusions.SourceUnit);
            zombie.mX = (int)zombie.mPosX;
        }
        if (large) plant.Die();
        else
        {
            // AnimBlock charges 50 even if all targets left before the event.
            plant.mPlantHealth -= 50;
            if (plant.mPlantHealth <= 0) plant.Die();
        }
    }

    internal static float Brightness(Plant plant) => plant.mShootingCounter <= 0 ? 1
        : plant.mShootingCounter > 60 ? 1 + (360 - plant.mShootingCounter) * .01f
        : 1 + plant.mShootingCounter * .05f;

    internal static void Ignite(Plant plant)
    {
        if (!CherryBombPlants.CanTriggerDeath(plant) || plant.NotOnGround()) return;
        var board = plant.mBoard;
        uint id = plant.DataID;
        int row = plant.mRow, order = plant.mRenderOrder;
        float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight / 2f;
        plant.mStartRow = 0;
        plant.Die();
        if (!plant.mDead || plant.DataID != id || plant.mBoard?.Pointer != board.Pointer) return;
        DoomEffects.Damage(board, row, x, y, order, fire: true, damage: 600);
    }

    internal static void Explode(CherryBombPlants.Death death)
    {
        // DoomPuff.Explode overlaps a one-unit circle, then calls virtual slot
        // 29 (Zombie.Charred) once per same-row enemy. It does not invoke
        // SetDoom or SetSmallDoom, multiply damage by count or leave a crater.
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < death.Board.m_zombies.Count; ++i)
        {
            var zombie = death.Board.m_zombies[i];
            if (zombie == null || !GroundEnemy(death.Board, zombie, death.Row)) continue;
            var rect = zombie.GetZombieRect();
            float dx = death.X - Math.Clamp(death.X, rect.xMin, rect.xMax);
            float dy = death.Y - Math.Clamp(death.Y, rect.yMin, rect.yMax);
            if (dx * dx + dy * dy <= BlastRadius * BlastRadius) targets.Add((zombie, zombie.DataID));
        }
        foreach (var (zombie, id) in targets)
        {
            if (zombie.DataID != id || !GroundEnemy(death.Board, zombie, death.Row)) continue;
            DoomEffects.AshDamage(zombie, 1800);

        }
        death.Board.mApp.AddTodParticle(death.X, death.Y, death.Order, ParticleEffect.Doom);
        death.Board.mApp.SoundSystem.PlayFoley(FoleyType.Explosion);
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.EatPlant))]
internal static class GnomeShroomBitePatch
{
    internal static bool Prefix(Zombie __instance, Plant thePlant, bool __runOriginal) =>
        !__runOriginal || thePlant == null || ScaredContent.Eyes(__instance)
            || !PuffSupport.BiteGnome(thePlant, __instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class PuffSupportInitializePatch
{
    internal static void Postfix(Plant __instance) => PuffSupport.Initialize(__instance);
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
internal static class BoomShroomDeathPatch
{
    [HarmonyPriority(Priority.Last)]
    internal static void Prefix(Plant __instance, bool __runOriginal, out CherryBombPlants.Death? __state)
    {
        __state = __runOriginal && FusionPlant.From(__instance) == FusionPlant.BoomShroom
            && CherryBombPlants.CanTriggerDeath(__instance) && !__instance.NotOnGround()
            ? new(__instance.mBoard, FusionPlant.BoomShroom, __instance.DataID,
                __instance.mPlantCol, __instance.mRow, __instance.mX + __instance.mWidth / 2f,
                __instance.mY + __instance.mHeight / 2f, __instance.mRenderOrder) : null;
    }
    internal static void Postfix(Plant __instance, CherryBombPlants.Death? __state)
    {
        if (__state != null && __instance.mDead && __instance.DataID == __state.Id
            && __instance.mBoard?.Pointer == __state.Board.Pointer) PuffSupport.Explode(__state);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class BellPepperCrushPatch
{
    internal static bool Prefix(Plant __instance)
    {
        if (FusionPlant.From(__instance) != FusionPlant.BellPepper
            || !CherryBombPlants.CanTriggerDeath(__instance) || __instance.NotOnGround()) return true;
        PuffSupport.Ignite(__instance);
        return false;
    }
}
