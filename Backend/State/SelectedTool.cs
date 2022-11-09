namespace Schets.Backend.State; 

/// <summary>
/// The drawing tool currently selected
/// </summary>
public enum SelectedTool {
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
    Line,
    /// <summary>
    /// Eraser tool to remove existing shapes
    /// </summary>
    Eraser,
}