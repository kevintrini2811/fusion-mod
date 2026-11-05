using Il2CppReloaded.Gameplay;
using UnityEngine;

namespace FusionMod;

internal static partial class ImportedPeaVisuals
{
    internal static (float X, float Y) ScaredyMuzzle(Plant plant)
    {
        if (plant.mController != null && Views.TryGetValue(plant.mController.Pointer, out var view) && view.IsValid
            && view.TryFumeOrigin(out var origin) && plant.mController is { } controller)
        {
            var anchor = controller.m_transform;
            if (anchor != null && anchor.parent != null)
            {
                var delta = anchor.parent.InverseTransformVector(origin - anchor.position);
                if (float.IsFinite(delta.x) && float.IsFinite(delta.y))
                    return (plant.X + delta.x / 2.82f, plant.Y - delta.y / 2.82f);
            }
        }
        return (plant.mX + plant.mWidth / 2f + 15, plant.mY + plant.mHeight / 2f - 10);
    }

    // LanternLaser is procedural in the source prefab: no sprite, animator or
    // texture is serialized. Its line fades for .3 seconds after Shoot1.
    internal static void Ray(Plant plant) => plant.mTargetY = 30;

    private sealed partial class View
    {
        private SpriteRenderer? ray;
        private readonly List<SpriteRenderer> splitRays = new();
        private void UpdateRay()
        {
            bool lumos = assets.Identity == FusionPlant.LumosCactus;
            bool continuous = FusionPlant.IsBeamShooter(assets.Identity);
            if (assets.Identity != FusionPlant.RayShroom && !lumos && !continuous || root == null) return;
            if (!preview && Plant.mBoard.mPaused) return;
            if (ray != null) ray.enabled = false;
            foreach (var line in splitRays) line.enabled = false;
            int remaining = Plant.mTargetY;
            if (continuous)
            {
                bool target = !preview && (BeamShooters.RelayTarget(Plant) != null
                    || Enumerable.Range(0, BeamShooters.TargetCount(assets.Identity))
                        .Any(slot => BeamShooters.ZombieTarget(Plant, slot) != null));
                remaining = target && Plant.mLaunchCounter > 0 && BeamShooters.Active(Plant) ? 30 : 0;
            }
            if (lumos)
            {
                int elapsed = CactusFusions.AttackTicks(Plant) - Plant.mStartRow;
                int since = elapsed - (Plant.mTargetX != 0 && elapsed >= 50 ? 50 : CactusFusions.EventTick(Plant));
                remaining = Plant.mStartRow > 0 && Plant.mTargetY == 0 && PumpkinFusions.LightLevel(Plant) >= 2
                    && since >= 0 ? Math.Max(0, 30 - since) : 0;
            }
            if (preview || remaining <= 0 || !Plant.mIsOnBoard || Plant.mDead) return;
            bool ground = !lumos || Plant.mTargetX == 0;
            float height = lumos && !ground ? 1.8f : .8f;
            float origin = Plant.mX + Plant.mWidth / 2f;
            var nativeAnchor = Controller.m_transform;
            var boardSpace = nativeAnchor.parent;
            Vector3 AtColumn(Vector3 start, float fromX, float toX) => start
                + boardSpace.TransformVector(new Vector3((toX - fromX) * 2.82f, 0, 0));
            var crossed = ground ? PlanternFusions.Crossed(Plant, 900).OrderBy(p => p.mPlantCol).ToArray() : Array.Empty<Plant>();
            var cold = crossed.FirstOrDefault(p => FusionPlant.From(p) == FusionPlant.AuroraPlantern || FusionPlant.From(p) == FusionPlant.BlacklightPlantern);
            bool purple = cold != null && FusionPlant.From(cold) == FusionPlant.BlacklightPlantern;
            int used = 0;
            void Segment(Vector3 start, Vector3 finish, bool chilled)
            {
                SpriteRenderer line;
                if (used++ == 0) line = ray ??= CreateBeam("LanternLaser");
                else
                {
                    int index = used - 2;
                    if (index >= splitRays.Count) splitRays.Add(CreateBeam("Plantern-Laser"));
                    line = splitRays[index];
                }
                PositionBeam(line, start, finish, .3f);
                line.color = chilled ? (purple ? new Color(.65f, .2f, 1, remaining / 30f) : new Color(.35f, .75f, 1, remaining / 30f)) : new Color(1, 1, 1, remaining / 30f);
                line.enabled = true;
            }
            void Lane(Vector3 start, float columnX)
            {
                var finish = AtColumn(start, columnX, 900);
                float coldX = cold == null ? float.PositiveInfinity : cold.mX + cold.mWidth / 2f;
                if (cold != null && coldX > columnX)
                {
                    var crossing = AtColumn(start, columnX, coldX);
                    Segment(start, crossing, false); Segment(crossing, finish, true);
                }
                else Segment(start, finish, cold != null);
            }
            int muzzle = Array.FindIndex(model.Nodes, n => n.Name == (lumos && !ground ? "Shoot2" : "Shoot"));
            var mouth = muzzle >= 0 ? transforms[muzzle].position : root.transform.TransformPoint(new Vector3(0, height, 0));
            Lane(mouth, origin);
            foreach (var splitter in crossed)
            {
                if (FusionPlant.From(splitter) != FusionPlant.Triplantern) continue;
                for (int head = 1; head <= 2; ++head)
                {
                    Vector3 start = AtColumn(mouth, origin, splitter.mX + splitter.mWidth / 2f)
                        + boardSpace.TransformVector(new Vector3(0, head == 1 ? 40 : -40, 0));
                    if (splitter.mController != null && Views.TryGetValue(splitter.mController.Pointer, out var view))
                    {
                        int node = Array.FindIndex(view.model.Nodes, n => n.Name == "Shoot" + head);
                        if (node >= 0) start = view.transforms[node].position;
                    }
                    Lane(start, splitter.mX + splitter.mWidth / 2f);
                }
            }
        }
    }
}
