using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

namespace FusionMod;

// PuffChomper1459: one acquisition detaches the plant and starts a continuous
// original jump loop. No native Puff spores or Chomper digestion participate.
internal static class NibbleShroom
{
    internal const int LoopUnits = 680, StartUnits = 300, LandUnits = 600;
    internal const float Unit = 80f / 1.35f;
    internal static bool Owns(Plant plant) => FusionPlant.From(plant) == FusionPlant.NibbleShroom;
    internal static bool Activated(Plant plant) => Owns(plant) && plant.mShootingCounter == 1;
    internal static int Phase(Plant plant) => plant.mStartRow * 6 + plant.mTargetX;
    internal static bool Airborne(Plant plant) => Activated(plant) && Phase(plant) is >= StartUnits and < LandUnits;
    internal static bool Retained(Plant plant) => Activated(plant) && !plant.mDead && !plant.mSquished;
    internal static bool DamageImmune(Plant plant) => Activated(plant) && Live(plant);
    internal static (string Clip, float Time) Animation(Plant plant) => Activated(plant)
        ? ("jump", Phase(plant) / 600f)
        : ("idle", (float)(FusionTime.ElapsedSeconds(plant.mBoard) % 2));

    internal static bool Live(Plant plant) => Owns(plant) && ScaredContent.Playing(plant.mBoard)
        && !plant.mBoard.mPaused && !plant.mDead && plant.mPlantHealth > 0 && !plant.mSquished
        && plant.mOnBungeeState == PlantOnBungeeState.NotOnBungee
        && (plant.mIsOnBoard || Activated(plant));

    internal static void Initialize(Plant plant)
    {
        if (!Owns(plant)) return;
        plant.mPlantHealth = plant.mPlantMaxHealth = 300;
        plant.mStartRow = plant.mTargetX = plant.mShootingCounter = 0;
        plant.mLaunchCounter = 0;
    }

    internal static bool Target(Plant plant, Zombie zombie, float length)
    {
        if (zombie.mRow != plant.mRow || zombie.mZombieType is ZombieType.Gravestone or ZombieType.Target
            || zombie.mZombieType == ZombieType.Boss
            || !PuffJumpers.GroundTarget(plant.mBoard, zombie)) return false;
        // Original start=Shadow+(0.5,0), direction=right, height=2. Native
        // collision rectangles replace Unity colliders; range is not a centre test.
        float x = plant.mX + plant.mWidth / 2f;
        var body = zombie.GetZombieRect();
        return body.xMax >= x + .5f * Unit && body.xMin <= x + (.5f + length) * Unit;
    }

    internal static bool Swallow(Zombie zombie) => zombie.mZombieType is not (ZombieType.Gargantuar or ZombieType.RedeyeGargantuar or ZombieType.Boss)
        && (zombie.mZombieType == ZombieType.Imp
            || (long)Math.Max(0, zombie.mBodyHealth) + Math.Max(0, zombie.mHelmHealth)
                < ((long)Math.Max(0, zombie.mBodyMaxHealth) + Math.Max(0, zombie.mHelmMaxHealth)) / 3);

    internal static void Tick(Plant plant)
    {
        if (!Live(plant) || plant.mIsAsleep || !FusionTime.NativeStep(plant, typeof(NibbleShroom))) return;
        var board = plant.mBoard;
        uint id = plant.DataID;
        bool current() => plant.DataID == id && plant.mBoard?.Pointer == board.Pointer && Live(plant);
        if (!Activated(plant))
        {
            if (!FusionTime.IsSourcePulse(board)) return;
            bool found = false;
            for (int i = 0; i < board.m_zombies.Count && !found; ++i)
                found = board.m_zombies[i] is { } zombie && Target(plant, zombie, 1.5f);
            if (!found) return;
            plant.mShootingCounter = 1;
            plant.mStartRow = plant.mTargetX = 0;
            // Retain the native pool generation for Update and Serialize. Native
            // ground queries/our small-slot owner release this occupancy flag.
            plant.mIsOnBoard = false;
            plant.UpdateReanim();
            return;
        }
        int before = Phase(plant);
        if (before is < 0 or >= LoopUnits) return; // Do not reinterpret a corrupt saved action.
        double seconds = FusionTime.StepSeconds(board);
        int units = (int)Math.Round(seconds * 600);
        if (units <= 0 || Math.Abs(units / 600d - seconds) > 1e-8)
            throw new InvalidOperationException("Nibble-shroom requires an exact native 1/600-second interval.");
        int after = before + units;
        int motion = Math.Max(0, Math.Min(after, LandUnits) - Math.Max(before, StartUnits));
        bool land = before < LandUnits && after >= LandUnits;
        int phase = after % LoopUnits;
        // Timer fields retain canonical centiseconds; only mTargetX is the
        // explicit serialized sub-centisecond residue. Commit before callbacks.
        plant.mStartRow = phase / 6;
        plant.mTargetX = phase % 6;
        if (motion > 0)
        {
            float previousX = plant.mX;
            plant.mX += motion * Unit / 600f;
            // Use the native row/roof ground projector at the moving X. Keep
            // the original tile coordinates for the source BoxType branch.
            int col = board.PixelToGridX(plant.mX + plant.mWidth / 2f, plant.mY + plant.mHeight);
            if (col >= 9 || plant.mX + plant.mWidth / 2f > board.GridToPixelX(8, plant.mRow) + 80) { plant.Die(); return; }
            plant.mY += board.GetPosYBasedOnRow(plant.mX, plant.mRow)
                - board.GetPosYBasedOnRow(previousX, plant.mRow);
            plant.UpdateReanim();
            if (!current()) return;
        }
        if (land) Land(plant, id, board);
    }

    internal static void Land(Plant plant, uint id, Board board)
    {
        bool current() => plant.DataID == id && plant.mBoard?.Pointer == board.Pointer && Live(plant);
        if (!current()) return;
        if (board.IsPoolSquare(plant.mPlantCol, plant.mRow))
        {
            float x = plant.mX + plant.mWidth / 2f, y = plant.mY + plant.mHeight;
            int order = plant.mRenderOrder;
            plant.Die();
            if (!ScaredContent.Playing(board)) return;
            board.mApp.AddTodParticle(x, y, order, ParticleEffect.PlantingPool);
            board.mApp.SoundSystem.PlayFoley(FoleyType.ZombieSplash);
            return;
        }
        var targets = new List<(Zombie Zombie, uint Id)>();
        for (int i = 0; i < board.m_zombies.Count; ++i)
            if (board.m_zombies[i] is { } zombie && Target(plant, zombie, 1f)) targets.Add((zombie, zombie.DataID));
        board.mApp.SoundSystem.PlayFoley(FoleyType.Chomp);
        foreach (var (zombie, targetId) in targets)
        {
            if (!current()) return;
            if (zombie.DataID != targetId || !Target(plant, zombie, 1f)) continue;
            if (Swallow(zombie))
            {
                ScaredContent.InstantKill(zombie);
                if (!current()) return;
                if (zombie.DataID == targetId && zombie.mBoard?.Pointer == board.Pointer && !zombie.mDead) zombie.DieWithLoot();
            }
            else zombie.TakeDamage(200, DamageFlags.BypassesShield);
        }
    }
}
