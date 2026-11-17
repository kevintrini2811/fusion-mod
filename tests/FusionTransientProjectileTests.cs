using System.Buffers.Binary;
using System.Reflection;
using FusionMod;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using Il2CppOOI.Platforms.Steam;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using R = FusionMod.FusionNativeTimeRecords;

static class TransientTests
{
    static int checks;
    static void Check(bool value, string text) { if (!value) throw new Exception(text); ++checks; }
    static void Refused<T>(Action action, string text) where T : Exception
    { try { action(); } catch (T) { ++checks; return; } throw new Exception(text); }
    static object? Hook(Type type, string method, params object?[] args) => type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, args);
    static void Map()
    {
        var board = new Board(); board.AddProjectile(1);
        Check(R.Get(board, R.Kind.Projectile, 1, 100) == null && board.m_gridItems.Allocations == 0, "disabled backend cannot allocate native projectile records");
        FusionProjectileRecords.Initialize();
        for (uint id = 1; id <= 6000; ++id)
        {
            if (id != 1) board.AddProjectile(id);
            var entry = R.Get(board, R.Kind.Projectile, id, 100)!;
            entry.Flags = 17; entry.A = id;
        }
        Check(R.Entries(board, R.Kind.Projectile).Length == 6000, "6000 live records exceed native128 pool without loss");
        Check(board.m_gridItems.Count == 0 && board.m_gridItems.Allocations == 0, "projectile provenance never touches native pool");
        var old = R.Get(board, R.Kind.Projectile, 1)!;
        board.m_projectiles.Values[0].mDead = true; R.Remove(old);
        Check(R.Get(board, R.Kind.Projectile, 1) == null, "dead entry reclaimed");
        var replacement = board.AddProjectile(1); FusionProjectileRecords.Birth(board, replacement);
        var fresh = R.Get(board, R.Kind.Projectile, 1, 120)!; fresh.A = 42;
        R.Remove(old);
        Check(ReferenceEquals(R.Get(board, R.Kind.Projectile, 1), fresh) && fresh.A == 42, "stale removal cannot remove reused full ID");
        Refused<InvalidOperationException>(() => old.A = 11, "dead lifetime cannot write into reused slot");
        FusionProjectileRecords.Birth(board, replacement);
        Refused<InvalidOperationException>(() => fresh.Flags = 7, "birth invalidates even identical full ID/pointer");
        fresh = R.Get(board, R.Kind.Projectile, 1, 100)!;
        board.m_projectiles.Values.Remove(replacement); board.AddProjectile(1);
        Check(!fresh.Valid, "array replacement authenticates native pointer");
        long before = FusionProjectileRecords.Generation(board); FusionProjectileRecords.Forget(board);
        Check(FusionProjectileRecords.Generation(board) != before, "new level starts new map generation");
        Refused<InvalidOperationException>(() => fresh.B = 1, "previous generation handle cannot mutate");
        var live = R.Get(board, R.Kind.Projectile, 1, 100)!;
        Refused<InvalidOperationException>(() => Task.Run(() => live.C = 1).GetAwaiter().GetResult(), "cross-thread mutation refused");
        FusionProjectileRecords.BeginLoad(); FusionProjectileRecords.EndLoad(board, false);
        Check(ReferenceEquals(R.Get(board, R.Kind.Projectile, 1), live), "failed load preserves an unchanged existing map and entry");
        FusionProjectileRecords.BeginLoad(); FusionProjectileRecords.BeginLoad();
        Check(R.Get(board, R.Kind.Projectile, 1, 100) == null, "nested load cannot attribute provenance");
        FusionProjectileRecords.EndLoad(board);
        Check(FusionProjectileRecords.Loading, "inner load cannot release outer scope");
        FusionProjectileRecords.EndLoad(board);
        Check(!FusionProjectileRecords.Loading && R.Get(board, R.Kind.Projectile, 1) == null, "load clears map even with native board pointer reuse");
        Refused<InvalidOperationException>(() => live.D = 1, "pre-load lifetime handle cannot write");
        FusionProjectileRecords.DisposeBoard(board);
        Check(!FusionProjectileRecords.HasSession(board), "dispose releases complete board session");
    }
    static GridItem Item(Board board, int marker)
    {
        var item = board.m_gridItems.DataArrayAlloc()!;
        item.Init(board.mApp, GridItemType.ZenTool); item.mSunCount = marker; return item;
    }
    static void Load()
    {
        var board = new Board(); var own = board.AddProjectile(32); own.mCobTargetX = -41021;
        own.mProjectileType = ProjectileType.Melon; own.mMotionType = ProjectileMotion.Lobbed;
        own.mCobTargetRow = 600; own.mVelX = 3; own.mVelY = 4; own.mVelZ = 5; own.mAccZ = 6; own.mPosZ = -80;
        own.mProjectileAge = 567; own.mTargetZombieID = 99; own.mHitTorchwoodGridX = 4;
        var controller = own.mController!; var pointer = own.Pointer;
        var foreign = board.AddProjectile(33); foreign.mCobTargetX = -999999; foreign.mVelY = 12; foreign.mMotionType = ProjectileMotion.Homing;
        var foreignController = foreign.mController;
        var carrier = Item(board, -521092); var status = Item(board, -521091); var clock = Item(board, -521094); var crater = Item(board, -521093);
        var nonce = Item(board, -521095); nonce.mGridItemCounter = 1; nonce.mX = nonce.mY = -1000; nonce.mPosX = 3;
        var malformed = Item(board, -521095); malformed.mGridItemCounter = 1; malformed.mX = malformed.mY = -1000; malformed.mPosX = float.NaN;
        var service = new SerializationService { App = board.mApp };
        ProjectileCaches.Seed(own); ProjectileCaches.Seed(foreign);
        Hook(typeof(FusionProjectileLoadPatch), "Prefix", false);
        Hook(typeof(FusionProjectileLoadPatch), "Postfix", service, true);
        Hook(typeof(FusionProjectileLoadPatch), "Finalizer", service, true, true, null);
        Check(!ProjectileCaches.Any(own) && !ProjectileCaches.Any(foreign), "all seven projectile cache owners invalidated before classification");
        Check(own.Pointer == pointer && own.DataID == 32 && board.m_projectiles.Count == 2, "load keeps native slot and full ID");
        Check(controller.Deaths == 1 && own.mController != controller && !own.mDead, "only old controller retired; projectile lives");
        Check(own.Initializations == 1 && own.VisualRemovals == 1, "one native initialize and own visual cleanup");
        Check(own.mMotionType == ProjectileMotion.Straight && own.mVelX == 0 && own.mVelY == 0 && own.mVelZ == 0 && own.mAccZ == 0 && own.mPosZ == 0, "native initialization replaces custom trajectory");
        Check(own.mCobTargetX == 0 && own.mCobTargetRow == 0 && own.mProjectileAge == 0 && own.mTargetZombieID == 0, "Fusion markers and target/age reset");
        Check(own.mHitTorchwoodGridX == -1 && own.mPortalX == -1 && own.mWidth == 40 && own.mHeight == 40 && own.mShadowY == 267, "native torchwood/portal/size/lane defaults");
        Check(foreign.mCobTargetX == -999999 && foreign.mVelY == 12 && foreign.mMotionType == ProjectileMotion.Homing && foreign.mController == foreignController, "foreign shot byte fields and controller unchanged");
        Check(carrier.mDead && nonce.mDead && !malformed.mDead && !status.mDead && !clock.mDead && !crater.mDead, "only exact obsolete private carriers removed");
        FusionProjectileLoad.Normalize(board);
        Check(own.Initializations == 1, "already native projectile cannot normalize twice");
        var failed = board.AddProjectile(34); failed.mCobTargetX = -41001;
        Hook(typeof(FusionProjectileLoadPatch), "Prefix", false);
        ProjectileCaches.Seed(failed);
        Hook(typeof(FusionProjectileLoadPatch), "Postfix", service, false);
        Hook(typeof(FusionProjectileLoadPatch), "Finalizer", service, false, true, null);
        Check(failed.Initializations == 0 && !FusionProjectileRecords.Loading && ProjectileCaches.Any(failed), "false native load preserves existing projectile behavior/views and releases scope");
        Hook(typeof(FusionProjectileLoadPatch), "Prefix", false);
        Hook(typeof(FusionProjectileLoadPatch), "Prefix", false);
        Hook(typeof(FusionProjectileLoadPatch), "Postfix", service, true);
        Hook(typeof(FusionProjectileLoadPatch), "Finalizer", service, true, true, null);
        Check(failed.Initializations == 0, "nested successful load does not normalize early");
        Hook(typeof(FusionProjectileLoadPatch), "Postfix", service, true);
        Hook(typeof(FusionProjectileLoadPatch), "Finalizer", service, true, true, null);
        Check(failed.Initializations == 1, "outer load normalizes once");
        var old = board.AddProjectile(55); ProjectileCaches.Seed(old);
        Hook(typeof(FusionProjectileLoadPatch), "Prefix", false);
        var reused = new Projectile { Pointer = old.Pointer, DataID = old.DataID, mBoard = board, mCobTargetX = -999999 };
        board.m_projectiles.Values.Remove(old); board.m_projectiles.Values.Add(reused);
        Hook(typeof(FusionProjectileLoadPatch), "Postfix", service, true);
        Hook(typeof(FusionProjectileLoadPatch), "Finalizer", service, true, true, null);
        Check(reused.Initializations == 0 && reused.mCobTargetX == -999999 && !ProjectileCaches.Any(reused), "same native pointer/fullID/board cannot inherit any old projectile behavior after load");
        var prior = R.Get(board, R.Kind.Projectile, reused.DataID, 100)!; ProjectileCaches.Seed(reused);
        Hook(typeof(FusionProjectileLoadPatch), "Prefix", false);
        Hook(typeof(FusionProjectileLoadPatch), "Finalizer", service, false, true, new IOException("native partial load"));
        Check(!FusionProjectileRecords.Loading && !prior.Valid && !ProjectileCaches.Any(reused), "exceptional partial native load invalidates all prior projectile lifetimes and balances scope");
    }
    static void Save()
    {
        var board = new Board(); var status = Item(board, -521091); var clock = Item(board, -521094); var foreign = Item(board, 27);
        var service = new SerializationService { App = board.mApp };
        var prepared = new List<uint>(); R.InitializeController = item => prepared.Add(item.DataID);
        try
        {
            Hook(typeof(FusionTimeSavePatch), "Prefix", new SerializationService());
            Check(prepared.Count == 0, "service without board leaves native early-return path untouched");
            Hook(typeof(FusionTimeSavePatch), "Prefix", service);
            Check(prepared.SequenceEqual(new[] { status.DataID, clock.DataID }) && !prepared.Contains(foreign.DataID), "native save readies only own existing status/clock carriers");
            FusionTime.CaptureResult = false; prepared.Clear();
            try { Hook(typeof(FusionTimeSavePatch), "Prefix", service); throw new Exception("clock capture failure was ignored"); }
            catch (TargetInvocationException error) { Check(error.InnerException is IOException, "clock capture failure rejects native serialization"); }
            Check(prepared.Count == 0, "failed clock capture does not touch any controller");
        }
        finally { R.InitializeController = null; FusionTime.CaptureResult = true; }
    }
    static byte[] Header(int version)
    {
        var bytes = new byte[40]; BinaryPrimitives.WriteInt32LittleEndian(bytes, SerializationService.SaveHeader.MAGIC_NUMBER);
        BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(4), version); BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(16), bytes.Length);
        return bytes;
    }
    static void Legacy()
    {
        string root = Path.Combine(Path.GetTempPath(), "fusion-transient-test-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
        try
        {
            var service = new SerializationService(); var platform = (PlatformService)service.m_platformService;
            ((SteamPlatform)platform.Platform).Root = root;
            string target = Path.Combine(root, service.FileName); byte[] original = { 1, 2, 4, 6, 8, 10 };
            File.WriteAllBytes(target, original);
            var native = new Il2CppStructArray<byte>(Header(14)); service.m_saveBuffer.Add(native);
            FusionLegacySave.Prepare(service);
            var nativeState = new object?[] { service, false, false };
            Check((bool)Hook(typeof(FusionLegacySaveLoadPatch), "Prefix", nativeState)! && !(bool)nativeState[1]!, "native14 load installs no legacy scope");
            Hook(typeof(FusionLegacySaveLoadPatch), "Finalizer", service, false, new IOException("ordinary native exception"));
            Check(!FusionLegacySave.IsBlocked(platform, service.FileName), "ordinary native exception does not acquire legacy write/delete guard");
            Check(ReferenceEquals(service.m_saveBuffer[0], native) && Directory.GetDirectories(root).Length == 0, "native14 does not require or create external files");
            var marked = new Il2CppStructArray<byte>(Header(FusionLegacySave.LegacyVersion)); service.m_saveBuffer[0] = marked;
            service.Category = -1;
            Refused<InvalidDataException>(() => FusionLegacySave.Prepare(service), "ambiguous legacy category refuses before native load");
            service.Category = 0;
            string companions = Path.Combine(root, "FusionProvenance"); Directory.CreateDirectory(companions); string companion = Path.Combine(companions, "preserve.bin"); File.WriteAllBytes(companion, original);
            FusionLegacySave.Prepare(service);
            Check(BinaryPrimitives.ReadInt32LittleEndian(service.m_saveBuffer[0].ToArray().AsSpan(4)) == 14, "legacy header changes to native14 in buffer");
            Check(BinaryPrimitives.ReadInt32LittleEndian(marked.ToArray().AsSpan(4)) == FusionLegacySave.LegacyVersion, "legacy shared input wrapper is not mutated");
            Check(File.ReadAllBytes(target).SequenceEqual(original), "legacy original file never overwritten on read");
            Check(File.ReadAllBytes(companion).SequenceEqual(original), "old companion preserved without dependency");
            string backup = Directory.GetFiles(Path.Combine(root, "FusionLegacyBackups"), "*.ig.dat", SearchOption.AllDirectories).Single();
            Check(File.ReadAllBytes(backup).SequenceEqual(original), "legacy backup byte exact");
            service.m_saveBuffer[0] = marked; FusionLegacySave.Prepare(service);
            Check(Directory.GetFiles(Path.Combine(root, "FusionLegacyBackups"), "*.ig.dat", SearchOption.AllDirectories).Length == 1, "content-addressed backup reused");
            File.WriteAllBytes(backup, new byte[] { 0 }); service.m_saveBuffer[0] = marked;
            Refused<IOException>(() => FusionLegacySave.Prepare(service), "corrupt backup is never accepted or overwritten");
            Check(ReferenceEquals(service.m_saveBuffer[0], marked) && File.ReadAllBytes(target).SequenceEqual(original), "backup failure preserves native file and header");
            Hook(typeof(FusionLegacySaveLoadPatch), "Finalizer", service, true, new IOException("migrated native exception"));
            Check(FusionLegacySave.IsBlocked(platform, service.FileName), "exception after actual migration protects exact old native file");
            FusionLegacySave.Block(service, new IOException("backup failed"));
            Check(FusionLegacySave.IsBlocked(platform, service.FileName) && !FusionLegacySave.IsBlocked(platform, "other.ig.dat") && !FusionLegacySave.IsBlocked(new PlatformService(), service.FileName), "failure guard scopes exact platform plus filename");
            var result = new object?[] { platform, service.FileName, true };
            Check((bool)Hook(typeof(FusionLegacySaveWritePatch), "Prefix", result)! == false && (bool)result[2]! == false, "failed migration blocks only target native write");
            result = new object?[] { platform, service.FileName, true };
            Check((bool)Hook(typeof(FusionLegacySaveDeletePatch), "Prefix", result)! == false && (bool)result[2]! == false, "failed migration blocks target native deletion");
        }
        finally { Directory.Delete(root, true); }
    }
    static void Main()
    {
        Map(); Load(); Save(); Legacy(); FusionProjectileRecords.Disable();
        var board = new Board(); board.AddProjectile(1);
        Check(R.Get(board, R.Kind.Projectile, 1, 100) == null && R.Entries(board, R.Kind.Projectile).Length == 0 && board.m_gridItems.Allocations == 0, "disabled transient backend cannot fall back to native carriers");
        Console.WriteLine($"PASS {checks} production transient-map/load/migration assertions; native game methods are simulated");
    }
}
