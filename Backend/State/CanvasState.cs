using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Media;

namespace Schets.Backend.State;

public static class CanvasState {
    public static List<TemplateShapeDescriptor> Layers { get; private set; } = new List<TemplateShapeDescriptor>();
    public static TemplateShapeDescriptor? DrawingShape;

    public static SelectedTool SelectedTool = SelectedTool.Rectangle;
    public static FillMode FillMode = FillMode.Filled;
    public static uint PrimaryColor = Colors.Black.ToUint32();
    public static uint SecondaryColor = Colors.Red.ToUint32();
    public static uint BrushWidth = 2;

    public static void SetLayers(IEnumerable<TemplateShapeDescriptor> shapes) {
        Layers = new List<TemplateShapeDescriptor>(shapes);
    }

    public static void AddLayer(TemplateShapeDescriptor shape) {
        Layers.Add(shape);
    }

    public static void RemoveLayer(int idx) {
        if (idx > Layers.Count) {
            throw new WarningException($"Index {idx} out of bounds for _layers. Length = {Layers.Count}");
        }

        Layers.RemoveAt(idx);
    }
}