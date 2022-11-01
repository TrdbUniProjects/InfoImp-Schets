using Avalonia;
using Avalonia.Media;

namespace Schets.Graphics; 

public class Line : AbstractShape {
    
    public override void Render(DrawingContext context) {
        context.DrawLine(this.Outline!, this.A, this.B);
    }
}