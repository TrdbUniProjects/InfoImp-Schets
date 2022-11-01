namespace Schets.Backend; 

public struct Template {
    public TemplateSize Size;
    public TemplateShapeDescriptor[] Shapes;
}

public struct TemplateShapeDescriptor {
    public TemplateShapeType ShapeType { get; set; }
    
    public TemplateCoordinate A { get; set; }
    public TemplateCoordinate B { get; set; }
    
    public TemplateShapeOutline? Outline { get; set; }
    public uint? BackgroundColor { get; set; }
}

public struct TemplateShapeOutline {
    public uint Color { get; set; }
    public double Thickness { get; set; }
}

public struct TemplateCoordinate {
    public double X { get; set; }
    public double Y { get; set; }

    public override string ToString() {
        return $"(X: {this.X}, Y: {this.Y})";
    }
}

public enum TemplateShapeType {
    Rectangle,
    Ellipse,
    Line
}

public struct TemplateSize {
    public int Width { get; set; }
    public int Height { get; set; }
}