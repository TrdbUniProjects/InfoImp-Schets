using System;
using Avalonia;
using Avalonia.Media;

namespace Schets.Graphics; 

public class Rectangle : IShape {
    
    public Point TopLeft { get; set; }
    public Point BottomRight { get; set; }

    public Pen Outline { get; set; } = new Pen() {
        Brush = Brushes.Black,
        Thickness = 1f
    };

    public IBrush? Fill { get; set; }
    
    public void Render(DrawingContext context) {
        Rect r = new Rect(this.TopLeft, this.BottomRight);
        if (this.Fill == null) {
            context.DrawRectangle(this.Outline, r);
        } else {
            context.FillRectangle(this.Fill, r);
        }
    }
}