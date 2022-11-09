namespace Schets.Backend.State; 

/// <summary>
/// Describes how a shape should be filled
/// </summary>
public enum FillMode {
    /// <summary>
    /// The shape should be filled.
    /// The color should be the primary color.
    /// </summary>
    Filled,
    /// <summary>
    /// The shape should not be filled,
    /// and only have an outline.
    /// The color of the outline should be the primary color.
    /// </summary>
    Outline,
    /// <summary>
    /// The shape should both be filled and have an outline.
    /// The color of the fill should be the primary color
    /// and the color of the outline should be the secondary color.
    /// </summary>
    FilledOutline
}