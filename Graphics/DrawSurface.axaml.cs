using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Schets.Backend;
using Schets.Backend.State;
namespace Schets.Graphics; 

public partial class DrawSurface : UserControl {
    public DrawSurface() {
        this.InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
    

    static DrawSurface() {
        AffectsRender<DrawSurface>();
    }

    public override void Render(DrawingContext context) {
        foreach(IShape shape in GetShapes()) {
            shape.Render(context);
        }
    }

    private IEnumerable<IShape> GetShapes() {
        return CanvasState.Layers.Select(GetShapeFromLayer);
    }

    private IShape GetShapeFromLayer(TemplateShapeDescriptor layer) {
        switch (layer.ShapeType) {
            case TemplateShapeType.Rectangle:
                Rectangle rectangle = new Rectangle {
                    TopLeft = new Point(layer.TopLeft.X, layer.TopLeft.Y),
                    BottomRight = new Point(layer.BottomRight.X, layer.BottomRight.Y),
                };

                if (layer.Outline != null) {
                    rectangle.Outline = new Pen {
                        Thickness = layer.Outline.GetValueOrDefault().Thickness,
                        Brush = new SolidColorBrush {
                            Color = Color.FromUInt32(layer.Outline.GetValueOrDefault().Color)
                        }
                    };
                }

                if (layer.BackgroundColor != null) {
                    rectangle.Fill = new SolidColorBrush() {
                        Color = Color.FromUInt32(layer.BackgroundColor.GetValueOrDefault())
                    };
                }

                return rectangle;
            case TemplateShapeType.Elipse:
            default:
                throw new NotImplementedException("Shape not yet implemented");
        }
    }
}