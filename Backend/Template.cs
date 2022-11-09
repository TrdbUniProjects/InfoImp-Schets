using Avalonia;

namespace Schets.Backend; 

/// <summary>
/// The structure of a template file
/// </summary>
public struct Template {
    /// <summary>
    /// The size of the template
    /// </summary>
    public TemplateSize Size;
    /// <summary>
    /// The shapes contained in the template
    /// </summary>
    public TemplateShapeDescriptor[] Shapes;
}

/// <summary>
/// The description of a template shape
/// </summary>
public struct TemplateShapeDescriptor {
    /// <summary>
    /// The type of shape
    /// </summary>
    public TemplateShapeType ShapeType { get; set; }
    /// <summary>
    /// The A coordinate of the shape.
    /// What this means is shape-specific.
    /// </summary>
    public TemplateCoordinate A { get; set; }
    /// <summary>
    /// The B coordinate of the shape.
    /// What this means is shape-specific.
    /// </summary>
    public TemplateCoordinate B { get; set; }
    /// <summary>
    /// The shape's outline
    /// </summary>
    public TemplateShapeOutline? Outline { get; set; }
    /// <summary>
    /// The shapes RGBA fill
    /// </summary>
    public uint? BackgroundColor { get; set; }
}

/// <summary>
/// A shape's outline
/// </summary>
public struct TemplateShapeOutline {
    /// <summary>
    /// The RGBA color
    /// </summary>
    public uint Color { get; set; }
    /// <summary>
    /// The thickness of the line
    /// </summary>
    public double Thickness { get; set; }
}

/// <summary>
/// A coordinate used for a shape
/// </summary>
public struct TemplateCoordinate {
    /// <summary>
    /// The X component
    /// </summary>
    public double X { get; set; }
    /// <summary>
    /// The Y componet
    /// </summary>
    public double Y { get; set; }
    
    public override string ToString() {
        return $"(X: {this.X}, Y: {this.Y})";
    }

    /// <summary>
    /// Convert self to a Point
    /// </summary>
    /// <returns>A point representing self</returns>
    public Point ToPoint() {
        return new Point(this.X, this.Y);
    }
}

/// <summary>
/// The type of shape
/// </summary>
public enum TemplateShapeType {
    /// <summary>
    /// A rectangle
    /// </summary>
    Rectangle,
    /// <summary>
    /// An ellipse
    /// </summary>
    Ellipse,
    /// <summary>
    /// A straight line
    /// </summary>
    Line
}

/// <summary>
/// The size of the template
/// </summary>
public struct TemplateSize {
    /// <summary>
    /// The width
    /// </summary>
    public int Width { get; set; }
    /// <summary>
    /// The height
    /// </summary>
    public int Height { get; set; }
}