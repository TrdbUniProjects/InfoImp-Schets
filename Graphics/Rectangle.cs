using Avalonia;
using Avalonia.Media;

namespace Schets.Graphics; 

/// <summary>
/// A rectangular shape
/// </summary>
public class Rectangle : AbstractShape {
    
    public override void Render(DrawingContext context) {
        Rect r = new(this.A, this.B);

        if (this.Fill != null) {
            context.FillRectangle(this.Fill, r);
        }

        if (this.Outline != null) {
            context.DrawRectangle(this.Outline, r);
        }
    }
}