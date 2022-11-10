using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Media;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Schets.Backend.State;

/// <summary>
/// Holds the one true state of the canvas
/// </summary>
public static class CanvasState {
    /// <summary>
    /// Shapes that are present on the canvas
    /// </summary>
    public static List<TemplateShapeDescriptor> Layers { get; private set; } = new();
    /// <summary>
    /// When a user is drawing something,
    /// the shape they are currently drawing is held in this variable
    /// </summary>
    public static TemplateShapeDescriptor? DrawingShape;
    /// <summary>
    /// The tool currently selected by the user
    /// </summary>
    public static SelectedTool SelectedTool = SelectedTool.Rectangle;
    /// <summary>
    /// The fill mode to use for the shape being drawn
    /// </summary>
    public static FillMode FillMode = FillMode.Filled;
    /// <summary>
    /// The primary color
    /// </summary>
    public static uint PrimaryColor = Colors.Black.ToUint32();
    /// <summary>
    /// The secondary color
    /// </summary>
    public static uint SecondaryColor = Colors.Red.ToUint32();
    /// <summary>
    /// The width of the brush
    /// </summary>
    public static uint BrushWidth = 2;

    /// <summary>
    /// Set the layers of the canvas
    /// </summary>
    /// <param name="shapes">The layers</param>
    public static void SetLayers(IEnumerable<TemplateShapeDescriptor> shapes) {
        Layers = new List<TemplateShapeDescriptor>(shapes);
    }

    /// <summary>
    /// Add a new layer to the canvas on top
    /// </summary>
    /// <param name="shape">The layer</param>
    public static void AddLayer(TemplateShapeDescriptor shape) {
        Layers.Add(shape);
        ProgramState.ModifiedSinceLastSave = true;
    }

    /// <summary>
    /// Remove a layer from the canvas
    /// </summary>
    /// <param name="idx">The index to remove</param>
    /// <exception cref="WarningException">The index was out of bounds</exception>
    public static void RemoveLayer(int idx) {
        if (idx > Layers.Count) {
            throw new WarningException($"Index {idx} out of bounds for Layers. Length = {Layers.Count}");
        }
        
        Layers.RemoveAt(idx);
        
        ProgramState.ModifiedSinceLastSave = true;
    }
}