using Avalonia;
using Avalonia.Media;

namespace Schets.Graphics; 

public abstract class AbstractShape {
    
    public Point TopLeft { get; set; }
    public Point BottomRight { get; set; }

    public Pen Outline { get; set; } = new() {
        Brush = Brushes.Black,
        Thickness = 1f
    };

    public IBrush? Fill { get; set; }
    
    public abstract void Render(DrawingContext context);
}