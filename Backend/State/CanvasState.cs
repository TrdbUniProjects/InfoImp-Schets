using System.Collections.Generic;
using System.ComponentModel;

namespace Schets.Backend.State; 

public static class CanvasState {

    public static List<TemplateShapeDescriptor> Layers { get; private set; }= new List<TemplateShapeDescriptor>();

    public static SelectedTool SelectedTool = SelectedTool.Rectangle;
    
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