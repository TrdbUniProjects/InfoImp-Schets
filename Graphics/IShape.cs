using Avalonia.Media;

namespace Schets.Graphics; 

public interface IShape {
    public void Render(DrawingContext context);
}