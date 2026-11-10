// Only supplies the reflection target. Randomness in test plants is owned by
// each family's existing Board double; no game assembly is executed.
namespace Il2CppReloaded.Gameplay;
public static class Common
{
    public static int Rand(int range) => range / 2;
    public static float Rand(float range) => range / 2;
}
