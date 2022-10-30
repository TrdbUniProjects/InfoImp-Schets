using Avalonia;
using Avalonia.Media;

namespace Schets.Graphics; 

public class Ellipse : AbstractShape {
    public override void Render(DrawingContext context) {
        double height = this.BottomRight.Y - this.TopLeft.Y;
        double width = this.BottomRight.X - this.TopLeft.X;
        
        context.DrawEllipse(
            this.Fill,
            this.Outline,
            new Point(this.TopLeft.X + width / 2d, this.TopLeft.Y + height / 2d),
            width / 2d,
            height / 2d
        );
    }
}