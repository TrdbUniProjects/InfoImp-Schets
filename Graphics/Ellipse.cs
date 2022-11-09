using Avalonia;
using Avalonia.Media;

namespace Schets.Graphics; 

/// <summary>
/// An elliptical shape
/// </summary>
public class Ellipse : AbstractShape {
    public override void Render(DrawingContext context) {
        double height = this.B.Y - this.A.Y;
        double width = this.B.X - this.A.X;
        
        context.DrawEllipse(
            this.Fill,
            this.Outline,
            new Point(this.A.X + width / 2d, this.A.Y + height / 2d),
            width / 2d,
            height / 2d
        );
    }
}