using System;
using Avalonia;

namespace Schets.Util; 

public static class MathUtil {
    /// <summary>
    /// Calculate the distance from a point to nearest edge of the ellipse
    /// </summary>
    /// <param name="c">The center of the ellipse</param>
    /// <param name="radiusX">The X radius of the ellipse</param>
    /// <param name="radiusY">The Y radius of the ellipse</param>
    /// <param name="p">The point to check from</param>
    /// <returns>The distance</returns>
    public static double DistanceToEllipse(Point c, double radiusX, double radiusY, Point p) {
        // Point on the ellipse intersecting with the line between p and s
        // However, the function assumes the ellipse to be on the origin
        // So we translate the point accordingly
        Point pTranslated = new(
            p.X - c.X,
            p.Y - c.Y
        );
        Point s = FindClosestPointOnEllipse(radiusX, radiusY, pTranslated);
        // s is the point where the line between p and the origin intersects the ellipse CO.
        // CO is centered on the origin, our ellipse C is not, translate S back to the 'real' space
        Point sTranslated = new (
            s.X + c.X,
            s.Y + c.Y
        );

        // Pythahoras between the point on the ellipse and the point clicked
        return Math.Sqrt(
            (sTranslated.X - p.X) * (sTranslated.X - p.Y) 
            + (sTranslated.Y - p.Y) * (sTranslated.Y - p.Y)
        );
    }
    
    /// <summary>
    /// Find the closest point on the edge of an ellipse to a point.
    /// The ellipse is centered on (0,0).
    /// </summary>
    /// <param name="radiusX">The X radius</param>
    /// <param name="radiusY">The Y radius</param>
    /// <param name="p">The point</param>
    /// <returns>The point which lies on the ellipse and is closest to p</returns>
    public static Point FindClosestPointOnEllipse(double radiusX, double radiusY, Point p) {
        // https://stackoverflow.com/a/46007540
        // https://gist.github.com/JohannesMP/777bdc8e84df6ddfeaa4f0ddb1c7adb3
        // https://github.com/0xfaded/ellipse_demo/issues/1
        
        double px = Math.Abs(p.X);
        double py = Math.Abs(p.Y);

        // semimajor
        double a = Math.Max(radiusX, radiusY);
        // semiminor
        double b = Math.Min(radiusX, radiusY);

        double tx = 0.70710678118;
        double ty = 0.70710678118;

        // Square them outside the loop
        // for performance increase
        double aa = a * a;
        double bb = b * b;

        for (int i = 0; i < 3; i++) {
            double x = a * tx;
            double y = b * ty;

            double ex = (aa - bb) * (tx * tx * tx) / a;
            double ey = (bb - aa) * (ty * ty * ty) / b;

            double rx = x - ex;
            double ry = y - ey;

            double qx = px - ex;
            double qy = py - ey;

            double r = Math.Sqrt(rx * rx + ry * ry);
            double q = Math.Sqrt(qy * qy + qx * qx);

            tx = Math.Min(1, Math.Max(0, (qx * r / q + ex) / a));
            ty = Math.Min(1, Math.Max(0, (qy * r / q + ey) / b));

            double t = Math.Sqrt(tx * tx + ty * ty);
            
            tx /= t;
            ty /= t;
        }

        return new Point(
            a * (p.X < 0 ? -tx : tx),
            b * (p.Y < 0 ? -ty : ty)
        );
    }

    /**
     * Calculate the distance from point p0 to the line between p1 and p2.
     *
     * <param name="p1">Point 1 of the line</param>
     * <param name="p2">Point 2 of the line</param>
     * <param name="p0">The point to calculate the distance from</param>
     */
    public static double DistanceToLine(Point p1, Point p2, Point p0) {
        // https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line#Line_defined_by_two_points
        
        // |(x2 - x1) * (y1 - y0) - (x1 - x0) * (y2 - y1)|
        double top = Math.Abs((p2.X - p1.X) * (p1.Y - p0.Y) - (p1.X - p0.X) * (p2.Y - p1.Y));
        // sqrt((x2 - x1)^2 + (y2 - y1)^2)
        double bottom = Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));

        return top / bottom;
    }
}