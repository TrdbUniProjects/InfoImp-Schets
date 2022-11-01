using Avalonia;
using Avalonia.Media;

namespace Schets.Graphics; 

public abstract class AbstractShape {
    
    public Point A { get; set; }
    public Point B { get; set; }

    public Pen? Outline { get; set; }

    public IBrush? Fill { get; set; }
    
    public abstract void Render(DrawingContext context);
}