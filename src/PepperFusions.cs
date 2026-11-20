using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppSource.Controllers;

namespace FusionMod;

internal static class PepperFusions
{
    private const int PopperFuse = 100;
    private const int CherryDamage = 300;
    private const int DoomDamage = 1800;
    private const float Unit = 80f / 1.35f;
    private const float RollStep = 10f * Unit / 100f;
    private const float Knockback = .8f * Unit;
    private const float RightEdge = 900;

    internal static bool Owns(FusionPlant plant) => plant == FusionPlant.PepperPopper || plant == FusionPlant.MadPepper;
    internal static bool Rolling(Plant plant) => plant.mStartRow == 1 && !plant.mDead
        && FusionPlant.From(plant) == FusionPlant.PepperPopper;
    internal static bool PopperImmune(Plant plant) => FusionPlant.From(plant) == FusionPlant.PepperPopper
        && !plant.mDead && !plant.mSquished && ScaredContent.Playing(plant.mBoard);

    internal static void Initialize(Plant plant, FusionPlant identity)
    {
        plant.mDoSpecialCountdown = plant.mStartRow = plant.mTargetY = 0;
        plant.mLaunchCounter = identity == FusionPlant.PepperPopper ? PopperFuse : 0;
        plant.mIsAsleep = false;
    }

    internal static void Fertilize(Plant plant)
    {
        if (FusionPlant.From(plant) == FusionPlant.PepperPopper && plant.mLaunchCounter > 50)
            plant.mLaunchCounter = 50;
    }

    internal static void TickPopper(Plant plant)
    {
        var board = plant.mBoard;
        bool rolling = Rolling(plant);
        if (board == null || board.mPaused || !ScaredContent.Playing(board) || plant.mDead || plant.mSquished
            || plant.mOnBungeeState != PlantOnBungeeState.NotOnBungee
            || !rolling && (!plant.mIsOnBoard || plant.NotOnGround())
            || !FusionTime.SourceStep(plant, typeof(PepperFusions))) return;

        if (plant.mLaunchCounter > 0 && --plant.mLaunchCounter == 0) StartRolling(plant);

        float previousX = plant.mX;
        if (Rolling(plant))
        {
            plant.mX += RollStep;
            plant.mY = board.GetPosYBasedOnRow(plant.mX, plant.mRow);
            plant.UpdateReanim();
            if (plant.mX >= RightEdge) { plant.Die(); return; }
        }
        // Source trigger contacts are sampled on Unity fixed steps, even during
        // the fuse. Preserve that rate on 100/120 Hz Replanted boards.
        if ((plant.mTargetY ^= 1) == 1) Contact(plant, board, previousX);
    }

    internal static void StartRolling(Plant plant)
    {
        if (!ChomperFusions.Active(plant)) return;
        plant.mLaunchCounter = 0;
        plant.mStartRow = 1;
        plant.mIsOnBoard = false;
    }

    private static void Contact(Plant plant, Board board, float previousX)
    {
        uint actorId = plant.DataID;
        int row = plant.mRow, order = plant.mRenderOrder;
        var contact = plant.GetPlantRect();
        if (Rolling(plant)) contact.xMin -= plant.mX - previousX + RollStep;
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && Touches(board, row, zombie, contact))
                targets.Add((zombie, zombie.DataID));
        foreach (var (zombie, id) in targets)
        {
            if (plant.DataID != actorId || plant.mBoard?.Pointer != board.Pointer || plant.mDead
                || !ScaredContent.Playing(board)) return;
            if (zombie.DataID != id || !Touches(board, row, zombie, contact)) continue;
            var rect = zombie.GetZombieRect();
            const float radius = 1.5f * Unit;
            CherryBombPlants.Explode(new(board, FusionPlant.PepperPopper, actorId,
                plant.mPlantCol, row, rect.center.x, rect.center.y, order), sourceRadius: radius, damage: CherryDamage);
            if (zombie.DataID != id || !PuffJumpers.GroundTarget(board, zombie)) continue;
            zombie.StopEating();
            // Source clamps a contacted zombie 2.2 world units before the exit.
            float shift = Math.Clamp(RightEdge - 2.2f * Unit - zombie.mPosX, 0, Knockback);
            BloverFusions.Move(zombie, shift);
        }
    }

    private static bool Touches(Board board, int row, Zombie zombie, UnityEngine.Rect contact) =>
        zombie.mRow == row && zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target)
        && PuffJumpers.GroundTarget(board, zombie) && contact.Overlaps(zombie.GetZombieRect());

    internal static bool TryActivate(Board board, int x, int y, int player, bool controller = false)
    {
        if (!ScaredContent.Playing(board) || board.mPaused || player is < 0 or > 1) return false;
        var cursor = board.CursorObjects[player];
        if (cursor == null || cursor.CursorType != CursorType.Normal
            && !(controller && cursor.CursorType == CursorType.PlantFromBank)) return false;
        int col = board.PixelToGridX(x, y), row = board.PixelToGridY(x, y);
        if (col < 0 || col >= 9 || row < 0 || row >= board.GetNumRows()) return false;
        var plant = board.GetPlantsOnLawn(col, row).NormalPlant;
        if (plant == null || FusionPlant.From(plant) != FusionPlant.MadPepper || !ChomperFusions.Active(plant)) return false;
        if (cursor.CursorType == CursorType.PlantFromBank && BucketController.HasSeedAction(board, cursor, col, row)) return false;
        Detonate(plant);
        return true;
    }

    internal static void Detonate(Plant plant)
    {
        if (plant.mStartRow != 0 || !ChomperFusions.Active(plant)) return;
        var blast = IceInstantFusions.Capture(plant);
        var board = blast.Board;
        plant.mStartRow = 1;
        plant.Die();
        if (plant.DataID != blast.Id || !plant.mDead || plant.mBoard?.Pointer != board.Pointer)
        {
            if (plant.DataID == blast.Id) plant.mStartRow = 0;
            return;
        }
        if (!ScaredContent.Playing(board)) return;
        DoomEffects.Damage(board, blast.Row, blast.X, blast.Y, blast.Order, fire: true, damage: DoomDamage);
        if (!ScaredContent.Playing(board)) return;
        DoomEffects.Damage(board, blast.Row, blast.X, blast.Y, blast.Order, damage: DoomDamage);
        if (!ScaredContent.Playing(board)) return;
        DoomEffects.LeaveCrater(board, blast.Col, blast.Row, () => ScaredContent.Playing(board));

        // Other populated cells each emit one blast. Snapshot a cell before
        // killing anything because death effects can recycle native slots.
        for (int col = 0; col < 9 && ScaredContent.Playing(board); ++col)
        {
            if (col == blast.Col) continue;
            var victims = new List<(Plant Plant, uint Id)>();
            for (int i = 0; i < board.m_plants.Count; ++i)
                if (board.m_plants[i] is { mDead: false, mIsOnBoard: true } victim
                    && victim.mPlantCol == col && victim.mRow == blast.Row
                    && victim.mOnBungeeState == PlantOnBungeeState.NotOnBungee)
                    victims.Add((victim, victim.DataID));
            if (victims.Count == 0) continue;
            foreach (var (victim, id) in victims)
            {
                if (!ScaredContent.Playing(board)) return;
                if (victim.mDead || victim.DataID != id || victim.mBoard?.Pointer != board.Pointer
                    || !victim.mIsOnBoard || victim.mPlantCol != col || victim.mRow != blast.Row) continue;
                float x = victim.mX, y = victim.mY;
                var seed = victim.mSeedType;
                victim.Die();
                if (victim.DataID == id && victim.mDead && ScaredContent.Playing(board)
                    && board.Rand(4) == 0 && seed is > SeedType.None and < SeedType.NumSeedTypes and not SeedType.Imitater)
                    DropSeed(board, x, y, seed);
            }
            if (!ScaredContent.Playing(board)) return;
            float cellX = board.GridToPixelX(col, blast.Row) + 40;
            float cellY = board.GridToPixelY(col, blast.Row) + 40;
            DoomEffects.Damage(board, blast.Row, cellX, cellY, blast.Order, damage: DoomDamage);
        }
    }

    private static void DropSeed(Board board, float x, float y, SeedType seed)
    {
        var coin = board.AddCoin(x, y, CoinType.UsableSeedPacket, CoinMotion.FromPlant);
        if (coin == null || coin.mDead) return;
        coin.mUsableSeedType = seed;
        if (coin.mController != null) coin.mController.SeedType = seed;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
internal static class PepperFusionInitializePatch
{
    private static void Postfix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        if (PepperFusions.Owns(identity)) PepperFusions.Initialize(__instance, identity);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateAbilities))]
internal static class PepperFusionAbilityPatch
{
    private static bool Prefix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        if (identity == FusionPlant.PepperPopper) PepperFusions.TickPopper(__instance);
        return !PepperFusions.Owns(identity);
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateShooting))]
internal static class PepperFusionShootingPatch
{
    private static bool Prefix(Plant __instance) => !PepperFusions.Owns(FusionPlant.From(__instance));
}

[HarmonyPatch(typeof(Plant), nameof(Plant.DoSpecial))]
internal static class PepperFusionSpecialPatch
{
    private static bool Prefix(Plant __instance)
    {
        var identity = FusionPlant.From(__instance);
        if (identity == FusionPlant.MadPepper) PepperFusions.Detonate(__instance);
        else if (identity == FusionPlant.PepperPopper) PepperFusions.StartRolling(__instance);
        else return true;
        return false;
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.Squish))]
internal static class PepperFusionCrushPatch
{
    private static bool Prefix(Plant __instance)
    {
        if (PepperFusions.PopperImmune(__instance)) return false;
        if (FusionPlant.From(__instance) != FusionPlant.MadPepper || !ChomperFusions.Active(__instance)) return true;
        PepperFusions.Detonate(__instance);
        return false;
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.MouseDown))]
internal static class MadPepperClickPatch
{
    private static bool Prefix(Board __instance, int x, int y, int theClickCount, int playerIndex) =>
        theClickCount < 0 || !PepperFusions.TryActivate(__instance, x, y, playerIndex);
}

[HarmonyPatch(typeof(GamepadCursorController), nameof(GamepadCursorController._onCursorConfirmed))]
internal static class MadPepperControllerPatch
{
    private static bool Prefix(GamepadCursorController __instance, bool __runOriginal)
    {
        if (!__runOriginal || !BucketController.CanUse(__instance)) return true;
        var board = __instance.Board;
        if (board.mCursorObject.CursorType is not (CursorType.Normal or CursorType.PlantFromBank)) return true;
        __instance._updateMousePosition();
        int x = board.mApp.WidgetManager.LastMouseX[0] - board.mX;
        int y = board.mApp.WidgetManager.LastMouseY[0] - board.mY;
        return !PepperFusions.TryActivate(board, x, y, 0, controller: true);
    }
}
