// Offline fog adapter. Native array layout is checked separately with unmanaged memory.
namespace Il2CppSystem {
 public class Array { public int GetLength(int dimension) => dimension == 0 ? 12 : 7; }
}
namespace Il2CppReloaded.Gameplay {
 public partial class Plant {
  public void Squish() {}
 }
 public partial class Zombie {
  public void EatPlant(Plant plant) {}
 }
 public sealed class FogArray {
  public IntPtr Pointer;
  public T Cast<T>() where T : new() => new();
 }
 public partial class Board {
  public bool Fog;
  public FogArray? mGridCelFog;
  public bool StageHasFog() => Fog;
 }
}

namespace FusionMod { internal static class RetaliationNuts { internal static IntPtr RadiusBoard, FireBoard; } }
namespace FusionMod { internal static class SpikePot { internal static void Tick(Il2CppReloaded.Gameplay.Plant plant) {} } }
