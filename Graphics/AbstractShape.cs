using Avalonia;
using Avalonia.Media;

namespace Schets.Graphics; 

/// <summary>
/// Base of every shape
/// </summary>
public abstract class AbstractShape {
    /// <summary>
    /// The A coordinate of the shape.
    /// What this does depends on the shape
    /// </summary>
    public Point A { get; set; }
    /// <summary>
    /// The B coordinate of the shape.
    /// What this does depends on the shape.
    /// </summary>
    public Point B { get; set; }
    /// <summary>
    /// The outline pen.
    /// If this is null, the shape has no outline
    /// </summary>
    public Pen? Outline { get; set; }
    /// <summary>
    /// The brush for the fill of the shape.
    /// If this is null, the shape has no fill
    /// </summary>
    public IBrush? Fill { get; set; }
    
    /// <summary>
    /// Render the shape to the screen
    /// </summary>
    /// <param name="context"></param>
    public abstract void Render(DrawingContext context);
}