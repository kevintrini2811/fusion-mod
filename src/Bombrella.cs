using Il2CppReloaded.Gameplay;

namespace FusionMod;

internal static class Bombrella
{
    private static bool Nearby(Plant umbrella, Plant other) => Math.Abs(umbrella.mPlantCol - other.mPlantCol) <= 1
        && Math.Abs(umbrella.mRow - other.mRow) <= 1;

    private static bool Active(Plant umbrella) => FusionPlant.From(umbrella) == FusionPlant.Bombrella
        && ChomperFusions.Active(umbrella);

    internal static void ChargeFromCherryHeal(Plant healed, int previousHealth)
    {
        if (healed.mPlantHealth <= previousHealth) return;
        var identity = FusionPlant.From(healed);
        if (identity != FusionPlant.CherryNut && identity != FusionPlant.CherryPumpkin) return;
        var plants = healed.mBoard.m_plants;
        for (int i = 0; i < plants.Count; ++i)
            if (plants[i] is { } umbrella && Nearby(umbrella, healed) && Active(umbrella))
                umbrella.mTargetX = Math.Min(100, umbrella.mTargetX + 1);
    }

    internal static Plant? Guard(Plant protectedPlant)
    {
        if (protectedPlant.mBoard == null || !ChomperFusions.Active(protectedPlant)) return null;
        var plants = protectedPlant.mBoard.m_plants;
        for (int i = 0; i < plants.Count; ++i)
            if (plants[i] is { mTargetX: >= 2 } umbrella && Nearby(umbrella, protectedPlant) && Active(umbrella))
                return umbrella;
        return null;
    }

    internal static bool TryProtect(Plant plant)
    {
        var umbrella = Guard(plant);
        if (umbrella == null) return false;
        umbrella.mTargetX -= 2;
        return true;
    }

    internal static void Repel(Plant plant, Zombie zombie)
    {
        var board = plant.mBoard;
        uint plantId = plant.DataID, zombieId = zombie.DataID;
        if (zombie.mIsEating) zombie.StopEating();
        int push = PotBoosts.HasSupport(plant, FusionPlant.UmbrellaPot) ? 3 : 2;
        zombie.mPosX = Math.Min(850, zombie.mPosX + push * UmbrellaShell.SourceUnit);
        zombie.mX = (int)zombie.mPosX;
        zombie.UpdateZombiePosition();
        if (plant.DataID != plantId || plant.mBoard?.Pointer != board.Pointer
            || zombie.DataID != zombieId || !UmbrellaShell.CanHit(plant, zombie)) return;

        var rect = zombie.GetZombieRect();
        float x = (rect.xMin + rect.xMax) / 2f, y = (rect.yMin + rect.yMax) / 2f;
        var blast = new CherryBombPlants.Death(board, FusionPlant.Bombrella, plantId,
            plant.mPlantCol, zombie.mRow, x, y, plant.mRenderOrder);
        CherryBombPlants.Explode(blast, sourceRadius: 1.5f * UmbrellaShell.SourceUnit, damage: 300);
    }
}
