using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// PuffSquash1370 owns a repeatable jump, not the native one-shot Squash AI.
// mStartRow is the stage. The two clocks store whole source centiseconds with
// explicit 1/600-second residues; target fields preserve exact float bit patterns.
internal static class Squish
{
    internal const float Unit = 80f / 1.35f;
    internal const int Ground = 0, Windup = 1, Travel = 2, Hover = 3, Fall = 4, Idle = 5;
    private const int Back = 4, Activated = 8;
    internal static bool Owns(Plant plant) => FusionPlant.From(plant) == FusionPlant.Squish;
    internal static bool Retained(Plant plant) => Owns(plant) && (plant.mShootingCounter & Activated) != 0
        && !plant.mDead && !plant.mSquished;
    internal static bool Airborne(Plant plant) => Retained(plant) && plant.mStartRow is Travel or Hover or Fall;
    // The leaf's serialized shadow points to Shadow_1, not Plant's Shadow.
    internal static bool ShadowVisible(Plant plant) => !Airborne(plant);
    internal static bool Live(Plant plant) => Owns(plant) && ScaredContent.Playing(plant.mBoard)
        && plant.mBoard.Pointer != CherryBombPlants.DisposingBoard && !plant.mBoard.mPaused
        && !plant.mDead && !plant.mSquished && plant.mPlantHealth > 0
        && plant.mOnBungeeState == PlantOnBungeeState.NotOnBungee
        && (plant.mIsOnBoard || Retained(plant));
    internal static int Phase(Plant plant) => plant.mStateCountdown * 6 + plant.mDoSpecialCountdown;
    private static int ClipPhase(Plant plant) => plant.mLaunchRate * 6 + ((plant.mShootingCounter >> 4) & 7);
    private static int CrushCooldown(Plant plant) => plant.mLaunchCounter * 6 + ((plant.mShootingCounter >> 7) & 7);
    private static int Slot(Plant plant) => (plant.mShootingCounter & 3) - 1;
    private static float DestinationX(Plant plant) => BitConverter.Int32BitsToSingle(plant.mTargetX);
    private static float DestinationY(Plant plant) => BitConverter.Int32BitsToSingle(plant.mTargetY);
    private static void SetPhase(Plant plant, int units)
    { plant.mStateCountdown = units / 6; plant.mDoSpecialCountdown = units % 6; }
    private static void SetClipPhase(Plant plant, int units)
    {
        plant.mLaunchRate = units / 6;
        plant.mShootingCounter = (plant.mShootingCounter & ~(7 << 4)) | ((units % 6) << 4);
    }
    private static void SetCrushCooldown(Plant plant, int units)
    {
        plant.mLaunchCounter = units / 6;
        plant.mShootingCounter = (plant.mShootingCounter & ~(7 << 7)) | ((units % 6) << 7);
    }
    private static void Stage(Plant plant, int stage)
    { plant.mStartRow = stage; SetPhase(plant, 0); }
    private static (float X, float Y) Home(Plant plant)
    {
        var offset = ShroomSlots.Offset(Slot(plant));
        return (plant.mBoard.GridToPixelX(plant.mPlantCol, plant.mRow) + offset.X,
            plant.mBoard.GridToPixelY(plant.mPlantCol, plant.mRow) + offset.Y);
    }
    internal static (string Clip, float Time) Animation(Plant plant) => plant.mStartRow switch
    {
        Windup or Travel or Hover => ("jump", Math.Min(ClipPhase(plant) / 600f, 2f / 3f - .00001f)),
        Fall => ("jumppop", Math.Min(Phase(plant) / 600f * 1.8f, 7f / 12f - .00001f)),
        _ => ("idle", (float)(FusionTime.ElapsedSeconds(plant.mBoard) % 2))
    };

    internal static void Initialize(Plant plant)
    {
        if (!Owns(plant)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mStartRow = plant.mStateCountdown = plant.mDoSpecialCountdown = 0;
        plant.mShootingCounter = plant.mLaunchCounter = plant.mLaunchRate = 0;
        plant.mTargetX = plant.mTargetY = 0;
    }

    private static bool Target(Plant plant, Zombie zombie) => zombie.mRow == plant.mRow
        && zombie.mZombieType is not (ZombieType.Gravestone or ZombieType.Target or ZombieType.Boss)
        && PuffJumpers.GroundTarget(plant.mBoard, zombie);
    private static float Distance(float x, float y, Zombie zombie)
    {
        var rect = zombie.GetZombieRect();
        float dx = x - Math.Clamp(x, rect.xMin, rect.xMax), dy = y - Math.Clamp(y, rect.yMin, rect.yMax);
        return dx * dx + dy * dy;
    }
    private static Zombie? FindTarget(Plant plant)
    {
        float centre = plant.mBoard.GridToPixelX(plant.mPlantCol, plant.mRow) + plant.mWidth / 2f;
        float ground = plant.mY + plant.mHeight, radius = 1.5f * Unit;
        float nearest = float.MaxValue;
        Zombie? result = null;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || !Target(plant, zombie) || Distance(centre, ground, zombie) > radius * radius) continue;
            float dx = zombie.mPosX + zombie.mWidth / 2f - plant.mX - plant.mWidth / 2f;
            float dy = zombie.mPosY + zombie.mHeight - ground;
            float distance = dx * dx + dy * dy;
            if (distance < nearest) { nearest = distance; result = zombie; }
        }
        return result;
    }
    private static void BeginJump(Plant plant, Zombie? zombie)
    {
        float x, y;
        if (zombie == null)
        {
            (x, y) = Home(plant);
            plant.mShootingCounter |= Back;
            y -= Unit;
        }
        else
        {
            plant.mShootingCounter &= ~Back;
            // Native mVelX is displacement per simulation step, directed left
            // for an eligible hostile ground zombie. Frozen/eating actors do not move.
            float prediction = zombie.mIsEating || zombie.mIceTrapCounter > 0 || zombie.mButteredCounter > 0
                ? 0 : zombie.mVelX * (zombie.IsMovingAtChilledSpeed() ? .5f : 1)
                    * (float)(.5 / FusionTime.StepSeconds(plant.mBoard));
            x = zombie.mPosX + zombie.mWidth / 2f - prediction - plant.mWidth / 2f;
            y = zombie.mPosY + zombie.mHeight - Unit - plant.mHeight;
        }
        plant.mTargetX = BitConverter.SingleToInt32Bits(x);
        plant.mTargetY = BitConverter.SingleToInt32Bits(y);
        SetClipPhase(plant, 0);
        Stage(plant, Windup);
    }

    internal static void Tick(Plant plant)
    {
        if (!Live(plant) || plant.mIsAsleep || !FusionTime.NativeStep(plant, typeof(Squish))) return;
        if (Slot(plant) < 0)
            plant.mShootingCounter = (plant.mShootingCounter & ~3) | (ShroomSlots.Slot(plant) + 1) | Back;
        if (plant.mStartRow is < Ground or > Idle || plant.mDoSpecialCountdown is < 0 or > 5
            || ((plant.mShootingCounter >> 4) & 7) > 5 || ((plant.mShootingCounter >> 7) & 7) > 5
            || plant.mStateCountdown < 0 || plant.mStateCountdown > 100000 || plant.mLaunchRate < 0
            || plant.mLaunchRate > 100000 || plant.mLaunchCounter is < 0 or > 2) return;
        double seconds = FusionTime.StepSeconds(plant.mBoard);
        int units = (int)Math.Round(seconds * 600);
        if (units <= 0 || Math.Abs(units / 600d - seconds) > 1e-8)
            throw new InvalidOperationException("Squish requires an exact native 1/600-second interval.");
        if (CrushCooldown(plant) > 0) SetCrushCooldown(plant, Math.Max(0, CrushCooldown(plant) - units));
        var board = plant.mBoard;
        uint id = plant.DataID;
        bool current() => plant.DataID == id && plant.mBoard?.Pointer == board.Pointer && Live(plant);
        if (plant.mStartRow is Ground or Idle)
        {
            TryRestoreHome(plant);
            SetPhase(plant, Phase(plant) + units);
            if (Phase(plant) < (plant.mStartRow == Idle ? 300 : 180)) return;
            var zombie = FindTarget(plant);
            if (zombie == null && (plant.mShootingCounter & Back) != 0)
            { Stage(plant, Idle); return; }
            BeginJump(plant, zombie);
            return;
        }
        SetPhase(plant, Phase(plant) + units);
        if (plant.mStartRow is Windup or Travel or Hover) SetClipPhase(plant, ClipPhase(plant) + units);
        if (plant.mStartRow == Windup)
        {
            if (Phase(plant) < 200) return;
            plant.mShootingCounter |= Activated;
            plant.mIsOnBoard = false;
            Stage(plant, Travel);
            plant.UpdateReanim();
            return;
        }
        if (!float.IsFinite(DestinationX(plant)) || !float.IsFinite(DestinationY(plant))) return;
        if (plant.mStartRow == Travel)
        {
            float dx = DestinationX(plant) - plant.mX, dy = DestinationY(plant) - plant.mY;
            float length = MathF.Sqrt(dx * dx + dy * dy), step = 13f * Unit * (float)seconds;
            if (length <= step)
            { plant.mX = DestinationX(plant); plant.mY = DestinationY(plant); Stage(plant, Hover); }
            else { plant.mX += dx / length * step; plant.mY += dy / length * step; }
            plant.UpdateReanim();
            return;
        }
        if (plant.mStartRow == Hover)
        {
            if (Phase(plant) >= 120) Stage(plant, Fall);
            return;
        }
        int col = board.PixelToGridX(plant.mX + plant.mWidth / 2f, plant.mY + plant.mHeight);
        if (col < 0 || col >= 9) { plant.Die(); return; }
        var offset = ShroomSlots.Offset(Slot(plant));
        float groundY = board.GetPosYBasedOnRow(plant.mX, plant.mRow) + offset.Y;
        if ((plant.mShootingCounter & Back) != 0 && Math.Abs(plant.mX - Home(plant).X) < .01f)
            groundY = Home(plant).Y;
        plant.mY = Math.Min(groundY, plant.mY + 32f * Unit * (float)seconds);
        plant.UpdateReanim();
        if (!current() || Phase(plant) < 56) return;
        Land(plant, id, board, col, groundY);
    }

    private static void TryRestoreHome(Plant plant)
    {
        if (plant.mIsOnBoard || (plant.mShootingCounter & Back) == 0) return;
        var home = Home(plant);
        if (Math.Abs(plant.mX - home.X) > .01f || Math.Abs(plant.mY - home.Y) > .01f) return;
        var occupants = ShroomSlots.Plants(plant.mBoard, plant.mPlantCol, plant.mRow);
        if (occupants.Any(other => other.DataID != plant.DataID
            && (!ShroomSlots.IsSmall(FusionPlant.From(other)) || ShroomSlots.Slot(other) == Slot(plant)))) return;
        if (ShroomSlots.CanAdd(plant.mBoard, FusionPlant.From(plant), plant.mPlantCol, plant.mRow, plant)) plant.mIsOnBoard = true;
    }

    internal static void Land(Plant plant, uint id, Board board, int col, float groundY)
    {
        bool current() => plant.DataID == id && plant.mBoard?.Pointer == board.Pointer && Live(plant);
        if (!current()) return;
        plant.mY = groundY;
        // Commit the hit before callbacks. A grounded retained actor is vulnerable.
        Stage(plant, Ground);
        if (board.IsPoolSquare(col, plant.mRow))
        {
            var support = board.GetPlantsOnLawn(col, plant.mRow).UnderPlant;
            if (support == null || support.mDead || !support.mIsOnBoard || support.mSeedType != SeedType.Lilypad)
            {
                float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight;
                int order = plant.mRenderOrder;
                plant.Die();
                if (plant.DataID != id || plant.mBoard?.Pointer != board.Pointer || !plant.mDead
                    || board.Pointer == CherryBombPlants.DisposingBoard || !ScaredContent.Playing(board)) return;
                board.mApp.AddTodParticle(x, y, order, ParticleEffect.PlantingPool);
                board.mApp.SoundSystem.PlayFoley(FoleyType.ZombieSplash);
                return;
            }
        }
        var targets = new List<(Zombie Zombie, uint Id)>();
        float cx = plant.mX + plant.mWidth / 2f, cy = plant.mY + plant.mHeight, radius = .5f * Unit;
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && Target(plant, zombie) && Distance(cx, cy, zombie) <= radius * radius)
                targets.Add((zombie, zombie.DataID));
        board.mApp.SoundSystem.PlayFoley(FoleyType.Thump);
        foreach (var (zombie, targetId) in targets)
        {
            if (!current()) return;
            if (zombie.DataID == targetId && Target(plant, zombie))
                zombie.TakeDamage(300, DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody);
        }
        if (current()) TryRestoreHome(plant);
    }

    internal static bool Crush(Plant plant)
    {
        if (!Live(plant)) return false;
        // Source null-caller crushing is a no-op, as is contact with a disabled collider.
        if (Airborne(plant)) return true;
        var hit = WeedFusions.CurrentCrush;
        if (hit == null || hit.Zombie.DataID != hit.ZombieId || hit.Board.Pointer != plant.mBoard.Pointer
            || hit.Zombie.mBoard?.Pointer != hit.Board.Pointer || hit.Zombie.mDead || hit.Zombie.IsDeadOrDying()) return true;
        bool vehicle = hit.Zombie.mZombieType is ZombieType.Zamboni or ZombieType.Catapult;
        int damage = vehicle ? 500 : hit.Zombie.mZombieType == ZombieType.Gargantuar ? 1800 : 0;
        if (damage == 0 || vehicle && CrushCooldown(plant) > 0) return true;
        uint id = plant.DataID;
        var board = plant.mBoard;
        if (vehicle) SetCrushCooldown(plant, 12);
        if (PumpkinFusions.AbsorbCrush(plant)) return true;
        if (plant.DataID != id || plant.mBoard?.Pointer != board.Pointer || !Live(plant)) return true;
        plant.mPlantHealth -= PotDefense.Damage(plant, damage);
        if (plant.mPlantHealth <= 0) plant.Die();
        return true;
    }
}
