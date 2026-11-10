using UnityEngine;

namespace FusionMod;

internal static partial class ImportedPeaVisuals
{
    // Scared animates independent axis angles in its sprite material. Two
    // ordinary transform rotations with signed scale reproduce the full 2D
    // matrix, including shear and reflection, without importing its shader.
    private static void PoseAffines(Model model, Transform[] transforms, string clipName, float time)
    {
        foreach (var affine in model.Affines ?? Array.Empty<Affine>())
        {
            float ax = affine.Values[0], ay = affine.Values[1], sx = affine.Values[2], sy = affine.Values[3];
            foreach (var binding in model.Clips[clipName].Bindings)
            {
                if (binding.Node != affine.Node || binding.Attribute < 8 || binding.Attribute > 11) continue;
                float value = Evaluate(binding.Channels[0], time % model.Clips[clipName].Duration);
                switch (binding.Attribute)
                {
                    case 8: ax = value; break;
                    case 9: ay = value; break;
                    case 10: sx = value; break;
                    case 11: sy = value; break;
                }
            }
            // GameUnitShader's compiled vertex program first shears by ax-ay,
            // then rotates by -ay. Its columns therefore use different axes:
            // x' = sx*cos(ay)*x + sy*sin(ax)*y
            // y' = -sx*sin(ay)*x + sy*cos(ax)*y.
            float a = sx * MathF.Cos(ay), b = sy * MathF.Sin(ax);
            float c = -sx * MathF.Sin(ay), d = sy * MathF.Cos(ax);
            float alpha = MathF.Atan2(c - b, a + d), beta = MathF.Atan2(c + b, a - d);
            float r = MathF.Sqrt((a + d) * (a + d) + (c - b) * (c - b));
            float s = MathF.Sqrt((a - d) * (a - d) + (c + b) * (c + b));
            transforms[affine.Outer].localRotation = Quaternion.Euler(0, 0, (alpha + beta) * .5f * Mathf.Rad2Deg);
            transforms[affine.Outer].localScale = new Vector3((r + s) * .5f, (r - s) * .5f, 1);
            transforms[affine.Inner].localRotation = Quaternion.Euler(0, 0, (alpha - beta) * .5f * Mathf.Rad2Deg);
        }
    }
}
