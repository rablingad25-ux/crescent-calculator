namespace HollowCrescentMoonCalculator;

/// <summary>
/// Computes the volume of a Hollow Crescent Moon formed by two spheres
/// (two circles revolved around the same axis). The OUTER sphere has
/// radius R; the CUTTER sphere has radius r and its center is offset
/// from the outer sphere's center by distance d along the +x direction.
/// The 3D crescent solid (a "spherical lune") is the outer sphere with
/// the cutter sphere subtracted from it. The hollow version removes a
/// similar inner crescent scaled by (R - t)/R so the convex outer wall
/// has uniform thickness t.
///
/// All sub-volumes use the exact closed-form formula for the volume of
/// intersection of two spheres:
///     V_intersection = pi * (R + r - d)^2
///                      * (d^2 + 2*d*(R + r) - 3*(R - r)^2) / (12 * d)
/// valid when |R - r| < d < R + r (true partial overlap, true crescent).
/// </summary>
public static class VolumeCalculator
{
    public static MoonResult Compute(
        double outerRadius,
        double cutterRadius,
        double centerOffset,
        double wallThickness)
    {
        ValidateInputs(outerRadius, cutterRadius, centerOffset, wallThickness);

        double outerSphereVolume = (4.0 / 3.0) * Math.PI * Math.Pow(outerRadius, 3);
        double overlapVolume = SphereIntersectionVolume(outerRadius, cutterRadius, centerOffset);
        double crescentVolume = outerSphereVolume - overlapVolume;

        double scale = (outerRadius - wallThickness) / outerRadius;
        double innerR = outerRadius * scale;
        double innerCutter = cutterRadius * scale;
        double innerOffset = centerOffset * scale;

        double innerSphereVolume = (4.0 / 3.0) * Math.PI * Math.Pow(innerR, 3);
        double innerOverlap = SphereIntersectionVolume(innerR, innerCutter, innerOffset);
        double innerCrescentVolume = innerSphereVolume - innerOverlap;

        double shellVolume = crescentVolume - innerCrescentVolume;

        return new MoonResult(
            OuterSphereVolume: Round(outerSphereVolume),
            OverlapVolume: Round(overlapVolume),
            CrescentVolume: Round(crescentVolume),
            InnerCavityVolume: Round(innerCrescentVolume),
            ShellVolume: Round(shellVolume),
            OuterRadius: outerRadius,
            CutterRadius: cutterRadius,
            CenterOffset: centerOffset,
            WallThickness: wallThickness,
            Notes: "Two spheres (circles revolved around a shared axis). Crescent = outer sphere minus offset cutter sphere. Volumes computed in closed form."
        );
    }

    /// <summary>
    /// Closed-form volume of the lens-shaped intersection of two spheres
    /// of radii R and r whose centers are separated by distance d.
    /// </summary>
    private static double SphereIntersectionVolume(double R, double r, double d)
    {
        if (d >= R + r) return 0.0;                 // no overlap
        if (d + r <= R) return (4.0 / 3.0) * Math.PI * Math.Pow(r, 3);  // cutter fully inside outer
        if (d + R <= r) return (4.0 / 3.0) * Math.PI * Math.Pow(R, 3);  // outer fully inside cutter
        double term = (R + r - d) * (R + r - d)
                      * (d * d + 2.0 * d * (R + r) - 3.0 * (R - r) * (R - r));
        return Math.PI * term / (12.0 * d);
    }

    private static void ValidateInputs(double R, double r, double d, double t)
    {
        if (R <= 0) throw new ArgumentException("Outer radius must be positive.");
        if (r <= 0) throw new ArgumentException("Cutter radius must be positive.");
        if (d <= 0) throw new ArgumentException("Center offset must be positive.");
        if (t <= 0 || t >= R) throw new ArgumentException("Wall thickness must be between 0 and the outer radius.");
        if (d >= R + r) throw new ArgumentException("Cutter sphere does not intersect the outer sphere (no crescent formed).");
        if (d + r <= R) throw new ArgumentException("Cutter sphere is fully inside the outer sphere (this would be a sphere with a bubble, not a crescent).");
        if (d + R <= r) throw new ArgumentException("Outer sphere is fully inside the cutter sphere (crescent is empty).");
    }

    private static double Round(double v) => Math.Round(v, 6);
}

public record MoonResult(
    double OuterSphereVolume,
    double OverlapVolume,
    double CrescentVolume,
    double InnerCavityVolume,
    double ShellVolume,
    double OuterRadius,
    double CutterRadius,
    double CenterOffset,
    double WallThickness,
    string Notes
);
