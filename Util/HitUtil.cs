using System;
using Avalonia;
using Schets.Backend;
using Schets.Backend.Exceptions;

namespace Schets.Util; 

public static class HitUtil {
    public static bool HitTest(TemplateShapeDescriptor descriptor, Point point, Size screenDimensions) {
        return descriptor.ShapeType switch {
            TemplateShapeType.Rectangle => HitTestSquare(descriptor, point, screenDimensions),
            TemplateShapeType.Ellipse => HitTestEllipse(descriptor, point, screenDimensions),
            TemplateShapeType.Line => HitTestLine(descriptor, point, screenDimensions),
            _ => throw new CaseNotImplementedException($"No hit-test implemented for shape type {descriptor.ShapeType}")
        };
    }

    private static bool HitTestLine(TemplateShapeDescriptor descriptor, Point point, Size screenDimensions) {
        double dist = MathUtil.DistanceToLine(
            descriptor.A.ToPoint(),
            descriptor.B.ToPoint(),
            point
        );
        double factor = Math.Max(screenDimensions.Width * 0.05, screenDimensions.Height * 0.05);
        
        return dist < factor;
    }

    private static bool HitTestEllipse(TemplateShapeDescriptor descriptor, Point point, Size screenDimensions) {
        double height = Math.Abs(descriptor.A.Y - descriptor.B.Y);
        double width = Math.Abs(descriptor.A.X - descriptor.B.X);
        
        if (descriptor.BackgroundColor == null) { // Outline only
            Point center = new(descriptor.A.X + width / 2d, descriptor.A.Y + height / 2d);

            double distance = MathUtil.DistanceToEllipse(center, width / 2, height / 2, point);
            double factor = Math.Max(screenDimensions.Width* 0.05, screenDimensions.Height * 0.05);

            return distance < factor;
        } else { // Filled
            // Andreas (https://math.stackexchange.com/users/317854/andreas), Point-Ellipse collision test., URL (version: 2017-01-26): https://math.stackexchange.com/q/2114902
            
            double x = point.X;
            double y = point.Y;
                
            double mx = descriptor.A.X + width / 2d;
            double my = descriptor.A.Y + height / 2d;
            double sigmaX = Math.Abs(-descriptor.A.X + descriptor.B.X) / 2    ;
            double sigmaY = Math.Abs(-descriptor.A.Y + descriptor.B.Y) / 2;

            double resultX = (x - mx) * (x - mx) / (sigmaX * sigmaX);
            double resultY = (y - my) * (y - my) / (sigmaY * sigmaY);
            double result = resultX + resultY;

            return result < 1;
        }
    }
    
    private static bool HitTestSquare(TemplateShapeDescriptor descriptor, Point point, Size screenDimensions) {
        if (descriptor.BackgroundColor == null) { // Shape is outline only
            Point topLeft = descriptor.A.ToPoint();
            Point bottomRight = descriptor.B.ToPoint();

            Point topRight = new (bottomRight.X, topLeft.Y);
            Point bottomLeft = new (topLeft.X, bottomRight.Y);

            // A rectangle is 4 sides, so we'll calculate the distance from
            // the point to each side. If the distance is within the threshold 
            // on any side, we'll consider it a hit

            double distLeft = MathUtil.DistanceToLine(topLeft, bottomLeft, point);
            double distTop = MathUtil.DistanceToLine(topLeft, topRight, point);
            double distRight = MathUtil.DistanceToLine(topRight, bottomRight, point);
            double distBottom = MathUtil.DistanceToLine(bottomRight, bottomLeft, point);

            double wThreshold = screenDimensions.Width * 0.05;
            double hThreshold = screenDimensions.Height * 0.05;
            
            return distLeft < wThreshold
                   || distTop < hThreshold
                   || distRight < wThreshold
                   || distBottom < hThreshold;
            
        } else { // Shape is filled
            Rect r = new(descriptor.A.ToPoint(), descriptor.B.ToPoint()); 
            return r.Contains(point);
        }
    }
}