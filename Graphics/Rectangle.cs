using Avalonia;
using Avalonia.Media;

namespace Schets.Graphics; 

public class Rectangle : AbstractShape {
    
    public override void Render(DrawingContext context) {
        Rect r = new Rect(this.TopLeft, this.BottomRight);
        if (this.Fill == null) {
            context.DrawRectangle(this.Outline, r);
        } else {
            context.FillRectangle(this.Fill, r);
        }
    }
}