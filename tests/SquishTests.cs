using System.Reflection;
using FusionMod;
using Il2CppReloaded.Gameplay;
using Native = Il2CppReloaded.Constants.Gameplay;

internal static class Program
{
    private static int checks;
    private static void Check(bool condition, string name)
    { ++checks; if (!condition) throw new Exception(name); }
    private static Plant Fresh(int hz = 100, int slot = 0)
    {
        Native.TIME_FOR_UPDATE = 1d / hz;
        var b = new Board(); FusionTime.Initialize(b);
        var p = new Plant { mBoard = b, mPlantCol = 3, mRow = 2 };
        var offset = ShroomSlots.Offset(slot);
        p.mX = b.GridToPixelX(3, 2) + offset.X;
        p.mY = b.GridToPixelY(3, 2) + offset.Y;
        b.m_plants.Add(p); Squish.Initialize(p);
        WeedFusions.CurrentCrush = null; PumpkinFusions.Absorbing = null;
        PotDefense.Numerator = PotDefense.Denominator = 1;
        return p;
    }
    private static void Tick(Plant p, int ticks = 1)
    {
        for (int i = 0; i < ticks; ++i)
        {
            ++p.mBoard.mMainCounter; FusionTime.BeginTick(p.mBoard); Squish.Tick(p);
            var state = State(p); Squish.Tick(p);
            Check(State(p) == state, "duplicate native dispatch must not move or advance action");
        }
    }
    private static string State(Plant p) => string.Join(",", p.mStartRow, p.mStateCountdown,
        p.mDoSpecialCountdown, p.mShootingCounter, p.mTargetX, p.mTargetY, p.mX, p.mY, p.mLaunchCounter);
    private static Zombie Enemy(Plant p, float offset = 55)
    {
        var z = new Zombie { mBoard = p.mBoard, mRow = p.mRow, mBodyHealth = 10000,
            mPosX = p.mX + offset, mPosY = p.mY + p.mHeight - 60 };
        p.mBoard.m_zombies.Add(z); return z;
    }
    private static void Until(Plant p, Func<bool> condition, int max = 400)
    { for (int i = 0; i < max && !condition(); ++i) Tick(p); Check(condition(), "bounded action reaches requested state"); }
    private static void PutLanding(Plant p, float x, float y)
    {
        Tick(p); p.mShootingCounter |= 8; p.mIsOnBoard = false;
        p.mX = x; p.mY = y; p.mStartRow = Squish.Fall;
    }
    private static void DamageAndMotion()
    {
        foreach (int hz in new[] { 100, 120 })
        {
            var p = Fresh(hz); var z = Enemy(p);
            Until(p, () => p.mStartRow == Squish.Windup);
            Check(!Squish.Airborne(p) && p.mIsOnBoard, "windup remains a vulnerable native occupant");
            Check(Squish.Animation(p).Clip == "jump", "original jump clip begins before flight");
            Until(p, () => Squish.Airborne(p));
            Check(!p.mIsOnBoard && Squish.Retained(p) && !Squish.ShadowVisible(p), "flight retains saved generation and disables its second shadow");
            Check(z.Received == 0, "no damage at acquisition or takeoff");
            Until(p, () => z.Received > 0);
            Check(z.Received == 300 && !p.mDead && p.mPlantHealth == 300, "one landing deals300 and preserves plant");
            Check(!Squish.Airborne(p) && Squish.ShadowVisible(p), "landed away plant restores collider shadow visibility");
            Check((z.LastFlags & (DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody))
                == (DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody), "landing uses squash flags");
            Until(p, () => z.Received >= 600);
            Check(z.Received == 600, "a second real jump creates one further hit");
        }
        var idle = Fresh(); Tick(idle, 130);
        Check(idle.mStartRow == Squish.Idle && !Squish.Retained(idle), "empty home retries repeatedly without becoming stuck");
        var left = Fresh(); var target = Enemy(left, -45);
        Until(left, () => target.Received == 300);
        Check(left.mX < left.mBoard.GridToPixelX(3, 2), "a nearby left zombie can be squashed");
        var far = Fresh(); Enemy(far, 300); Tick(far, 100);
        Check(!Squish.Retained(far), "a remote zombie cannot start a jump");
        foreach (string excluded in new[] { "ally", "dead", "row", "offground", "grave", "submerged", "boss" })
        {
            var p = Fresh(); var z = Enemy(p);
            if (excluded == "ally") z.mMindControlled = true;
            if (excluded == "dead") z.mDead = true;
            if (excluded == "row") ++z.mRow;
            if (excluded == "offground") z.Category = DamageRangeFlags.Flying;
            if (excluded == "grave") z.mZombieType = ZombieType.Gravestone;
            if (excluded == "submerged") z.Category = DamageRangeFlags.Submerged;
            if (excluded == "boss") z.mZombieType = ZombieType.Boss;
            Tick(p, 90); Check(!Squish.Retained(p), "excluded native target does not activate: " + excluded);
        }
        var pogoPlant = Fresh(); var pogo = Enemy(pogoPlant);
        pogo.mZombieType = ZombieType.Pogo; pogo.mZombiePhase = ZombiePhase.PogoBouncing; pogo.Height = 20;
        Until(pogoPlant, () => pogo.Received == 300);
        Check(pogo.Height == 20 && pogo.LastFlags == (DamageFlags.HitsShieldAndBody | DamageFlags.DoesntLeaveBody),
            "ordinary Pogo acquires and receives native landing damage without height mutation");
        foreach (string excluded in new[] { "high", "dead", "dying", "foreign", "ally" })
        {
            var p = Fresh(); var z = Enemy(p); z.mZombieType = ZombieType.Pogo;
            z.mZombiePhase = ZombiePhase.PogoBouncing; z.Height = 20;
            if (excluded == "high") { z.mZombiePhase = ZombiePhase.PogoHighBounce1; z.Height = 0; }
            if (excluded == "dead") z.mDead = true;
            if (excluded == "dying") z.Dying = true;
            if (excluded == "foreign") z.mBoard = new Board();
            if (excluded == "ally") z.mMindControlled = true;
            Tick(p, 90); Check(!Squish.Retained(p), "Pogo source/lifetime guard prevents acquisition: " + excluded);
            PutLanding(p, p.mX, p.mY); Squish.Land(p, p.DataID, p.mBoard, 3, p.mY);
            Check(z.Received == 0, "Pogo source/lifetime guard prevents landing hit: " + excluded);
        }
        var landing = Fresh(); PutLanding(landing, landing.mX, landing.mY);
        var ground = Enemy(landing, 20); var snorkel = Enemy(landing, 20); snorkel.Category = DamageRangeFlags.Submerged;
        var boss = Enemy(landing, 20); boss.mZombieType = ZombieType.Boss;
        Squish.Land(landing, landing.DataID, landing.mBoard, 3, landing.mY);
        Check(ground.Received == 300 && snorkel.Received == 0 && boss.Received == 0,
            "landing shares the original ground/dodge exclusion with acquisition");
    }
    private static void HomeAndInactivity()
    {
        for (int slot = 0; slot < 3; ++slot)
        {
            var p = Fresh(slot: slot); var z = Enemy(p);
            float homeX = p.mX, homeY = p.mY;
            Until(p, () => z.Received > 0); z.mDead = true;
            Until(p, () => p.mIsOnBoard && Math.Abs(p.mX - homeX) < .01f && Math.Abs(p.mY - homeY) < .01f);
            Check(!p.mDead, "return preserves its own original subfield");
        }
        var frozen = Fresh(); Enemy(frozen); Tick(frozen, 20);
        string before = State(frozen);
        frozen.mBoard.mPaused = true; Tick(frozen, 20); Check(State(frozen) == before, "pause preserves phase and movement");
        frozen.mBoard.mPaused = false; frozen.mIsAsleep = true; Tick(frozen, 20);
        Check(State(frozen) == before, "sleep preserves the action");
        frozen.mIsAsleep = false; frozen.mOnBungeeState = PlantOnBungeeState.OnBungee; Tick(frozen, 20);
        Check(State(frozen) == before, "carrying preserves the action");
        frozen.mOnBungeeState = PlantOnBungeeState.NotOnBungee; frozen.mBoard.DisposeBoard(); Tick(frozen, 10);
        Check(State(frozen) == before, "disposed board cannot resume action");
        var invalid = Fresh(); invalid.mStartRow = 99; string bad = State(invalid); Tick(invalid, 2);
        Check(State(invalid) != bad && invalid.mStartRow == 99, "slot initialization does not reinterpret corrupt stage");
    }
    private static void WaterAndReentrancy()
    {
        foreach (string support in new[] { "none", "dead", "pot", "lily" })
        {
            var p = Fresh(); var b = p.mBoard;
            float x = b.GridToPixelX(4, 2), y = b.GridToPixelY(4, 2) - 16;
            PutLanding(p, x, y); b.Water.Add((4, 2));
            if (support != "none") b.Supports[(4, 2)] = new Plant { mBoard = b,
                mSeedType = support == "pot" ? SeedType.Wallnut : SeedType.Lilypad, mDead = support == "dead" };
            Squish.Land(p, p.DataID, b, 4, y);
            Check(p.mDead == (support != "lily"), "actual landing cell requires a live Lily Pad: " + support);
            Check(b.mApp.Particles == (support != "lily" ? 1 : 0), "drowning effect occurs only after confirmed death");
        }
        var veto = Fresh(); PutLanding(veto, 400, 264); veto.mBoard.Water.Add((4, 2)); veto.Veto = true;
        Squish.Land(veto, veto.DataID, veto.mBoard, 4, 264);
        Check(!veto.mDead && veto.mBoard.mApp.Particles == 0, "death veto cannot create a fake splash");
        var dead = Fresh(); PutLanding(dead, dead.mX, dead.mY);
        var a = Enemy(dead, 20); var b2 = Enemy(dead, 25); a.OnDamage = () => dead.mBoard.DisposeBoard();
        Squish.Land(dead, dead.DataID, dead.mBoard, 3, dead.mY);
        Check(a.Received == 300 && b2.Received == 0, "disposal during first hit stops subsequent targets");
        var reused = Fresh(); PutLanding(reused, reused.mX, reused.mY);
        var first = Enemy(reused, 20); var second = Enemy(reused, 25); first.OnDamage = () => ++second.DataID;
        Squish.Land(reused, reused.DataID, reused.mBoard, 3, reused.mY);
        Check(first.Received == 300 && second.Received == 0, "target recycling cannot redirect a captured hit");
    }
    private static void CrushCases()
    {
        var p = Fresh(); p.mPlantHealth = 10000;
        Check(Squish.Crush(p) && p.mPlantHealth == 10000, "null caller crushing is handled without invented damage");
        var z = Enemy(p); z.mZombieType = ZombieType.Zamboni;
        WeedFusions.CurrentCrush = new(z, p.mBoard, z.DataID);
        Check(Squish.Crush(p) && p.mPlantHealth == 9500, "actual vehicle contact costs500");
        Squish.Crush(p); Check(p.mPlantHealth == 9500, "same-tick vehicle contact cannot repeat");
        Tick(p); Squish.Crush(p); Check(p.mPlantHealth == 9500, "vehicle cooldown spans two source centiseconds");
        Tick(p); Squish.Crush(p); Check(p.mPlantHealth == 9000, "vehicle can hit again after0.02simulatedseconds");
        z.mZombieType = ZombieType.Catapult; Tick(p, 2); Squish.Crush(p);
        Check(p.mPlantHealth == 8500, "Catapult uses its independently pinned500damage");
        z.mZombieType = ZombieType.Gargantuar; Squish.Crush(p);
        Check(p.mPlantHealth == 6700, "Gargantuar uses1800 rather than vehicle500");
        z.mZombieType = ZombieType.Boss; Squish.Crush(p); Check(p.mPlantHealth == 6700, "unknown caller never gains guessed damage");
        z.mZombieType = ZombieType.Gargantuar; ++z.DataID; Squish.Crush(p);
        Check(p.mPlantHealth == 6700, "recycled crush caller is rejected");
        WeedFusions.CurrentCrush = new(z, p.mBoard, z.DataID); p.mShootingCounter |= 8; p.mStartRow = Squish.Travel;
        Squish.Crush(p); Check(p.mPlantHealth == 6700, "disabled airborne collider cannot take crush damage");
        p.mStartRow = Squish.Ground; PumpkinFusions.Absorbing = actor => { ++actor.DataID; return false; };
        Squish.Crush(p); Check(p.mPlantHealth == 6700, "support callback recycling stops stale health write");
        var fractional = Fresh(120); Tick(fractional); fractional.mPlantHealth = 10000;
        var vehicle = Enemy(fractional); vehicle.mZombieType = ZombieType.Zamboni;
        WeedFusions.CurrentCrush = new(vehicle, fractional.mBoard, vehicle.DataID);
        Squish.Crush(fractional); Tick(fractional, 2); Squish.Crush(fractional);
        Check(fractional.mPlantHealth == 9500, "120Hz phase cannot shorten the0.02second contact interval");
        Tick(fractional); Squish.Crush(fractional);
        Check(fractional.mPlantHealth == 9000, "120Hz contact resumes at the first native step after0.02seconds");
    }
    private static void SaveAndRate()
    {
        var p = Fresh(120, 2); var z = Enemy(p);
        Until(p, () => p.mStartRow == Squish.Travel); Tick(p, 3);
        FusionTime.Capture(p.mBoard);
        var loaded = new Board { mMainCounter = p.mBoard.mMainCounter };
        foreach (var old in p.mBoard.m_gridItems)
        {
            var item = new GridItem();
            foreach (var f in typeof(GridItem).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) f.SetValue(item, f.GetValue(old));
            item.mBoard = loaded; loaded.m_gridItems.Add(item);
        }
        var restored = new Plant();
        foreach (var f in typeof(Plant).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) f.SetValue(restored, f.GetValue(p));
        restored.mBoard = loaded; loaded.m_plants.Add(restored);
        var rz = new Zombie { mBoard = loaded, mRow = z.mRow, mPosX = z.mPosX, mPosY = z.mPosY, mBodyHealth = z.mBodyHealth };
        loaded.m_zombies.Add(rz);
        FusionTime.BeginLoad(); FusionTime.EndLoad(loaded, true);
        Check(State(restored) == State(p), "all saved motion, home and clock residue fields reconstruct exactly");
        Native.TIME_FOR_UPDATE = .01;
        for (int i = 0; i < 150; ++i) { Tick(p); Tick(restored); Check(State(restored) == State(p), "rate handoff after load preserves actual motion phase"); }
        Check(z.Received == rz.Received, "reconstructed action emits the same landing count");
    }
    private static void PlainPuffGroundBoundary()
    {
        Check(typeof(PuffJumpers).Assembly.GetType("FusionMod.PuffJumperGroundPatch") == null,
            "global native ground query has no product patch");
        var plain=Fresh(); plain.Identity=FusionPlant.Other;
        foreach(int stage in Enumerable.Range(0,6))
        foreach(int flags in new[]{0,1,8,0x3ff})
        {
            plain.mStartRow=stage; plain.mShootingCounter=flags;
            Check(!Squish.Owns(plain)&&!Squish.Retained(plain)&&!Squish.Airborne(plain),
                "plain Puff never inherits Squish flight from shared fields");
            Check(!plain.NotOnGround(), "ordinary native Puff ground result stays native");
        }
    }
    public static void Main()
    {
        PlainPuffGroundBoundary(); DamageAndMotion(); HomeAndInactivity(); WaterAndReentrancy(); CrushCases(); SaveAndRate();
        Console.WriteLine($"PASS {checks} Squish assertions using the actual leaf, clock and clock-record owners.");
    }
}
