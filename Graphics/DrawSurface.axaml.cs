using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Schets.Backend;
using Schets.Backend.State;
namespace Schets.Graphics; 

public partial class DrawSurface : UserControl {

    private Point? _drawStartPoint;

    static DrawSurface() {
        AffectsRender<DrawSurface>();
    }
    
    public DrawSurface() {
        this.InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);

        this.PointerPressed += this.OnPointerPressedEvent;
        this.PointerReleased += this.OnPointerReleasedEvent;
    }

    public override void Render(DrawingContext context) {
        foreach(AbstractShape shape in GetShapes()) {
            shape.Render(context);
        }
    }

    private void OnPointerPressedEvent(object? sender, PointerPressedEventArgs args) {
        if (!args.Pointer.IsPrimary) {
            return;
        }

        this._drawStartPoint = args.GetPosition(this);
    }

    private void OnPointerReleasedEvent(object? sender, PointerReleasedEventArgs args) {
        if (this._drawStartPoint == null || !args.Pointer.IsPrimary) {
             this._drawStartPoint = null;
            return;
        }

        Point drawEndPoint = args.GetPosition(this);

        (TemplateCoordinate topLeft, TemplateCoordinate bottomRight) =
            CalculateShapeCoordinates(this._drawStartPoint.GetValueOrDefault(), drawEndPoint);
        
        TemplateShapeDescriptor descriptor = new() {
            ShapeType = GetActiveShapeType(),
            TopLeft = topLeft,
            BottomRight = bottomRight,
            BackgroundColor = 4294901760
        };
        
        CanvasState.AddLayer(descriptor);
        this.InvalidateVisual();
    }

    private static (TemplateCoordinate, TemplateCoordinate) CalculateShapeCoordinates(Point start, Point end) {
        // Shape can go into one of the four quadrants, with `start` at 0,0 of the imaginary surface.
        if (start.X > end.X) {
            // Left half
            
            if (start.Y < end.Y) {
                // Lower left
                // start is the top right corner, end the bottom left
                return (
                    new TemplateCoordinate {
                        X = end.X,
                        Y = start.Y,
                    },
                    new TemplateCoordinate {
                        X = start.X,
                        Y = end.Y,
                    }
                );
            } else {
                // Upper left
                // start is the bottom right corner, end the top left
                return (
                    new TemplateCoordinate {
                        X = end.X,
                        Y = end.Y
                    }, 
                    new TemplateCoordinate {
                        X = start.X,
                        Y = start.Y
                    }
                );
            }
        } else {
            // Right half
            
            if (start.Y < end.Y) {
                // Lower right
                // Start is the top left corner, end the bottom right
                return (
                    new TemplateCoordinate {
                        X = start.X,
                        Y = start.Y
                    },
                    new TemplateCoordinate {
                        X = end.X,
                        Y = end.Y,
                    }
                );
            } else {
                // Upper right
                // Start is the bottom left corner, end the upper right
                
                return (
                    new TemplateCoordinate {
                        X = start.X,
                        Y = end.Y
                    },
                    new TemplateCoordinate {
                        X = end.X,
                        Y = start.Y
                    }
                );
            }
        }
    }

    private static TemplateShapeType GetActiveShapeType() {
        return CanvasState.SelectedTool switch {
            SelectedTool.Rectangle => TemplateShapeType.Rectangle,
            SelectedTool.Ellipse => TemplateShapeType.Ellipse,
        };
    }

    private static IEnumerable<AbstractShape> GetShapes() {
        return CanvasState.Layers.Select(GetShapeFromLayer);
    }

    private static AbstractShape GetShapeFromLayer(TemplateShapeDescriptor layer) {
        AbstractShape shape =  layer.ShapeType switch {
            TemplateShapeType.Rectangle => new Rectangle(),
            TemplateShapeType.Ellipse => new Ellipse(),
            _ => throw new NotImplementedException("Shape not yet implemented")
        };
        
        SetShapeProperties(ref shape, layer);
        return shape;
    }

    private static void SetShapeProperties<T>(ref T shape, TemplateShapeDescriptor layer) where T: AbstractShape {
        shape.TopLeft = new Point(layer.TopLeft.X, layer.TopLeft.Y);
        shape.BottomRight = new Point(layer.BottomRight.X, layer.BottomRight.Y);

        if (layer.Outline != null) {
            shape.Outline = new Pen {
                Thickness = layer.Outline.GetValueOrDefault().Thickness,
                Brush = new SolidColorBrush {
                    Color = Color.FromUInt32(layer.Outline.GetValueOrDefault().Color)
                }
            };
        }

        if (layer.BackgroundColor != null) {
            shape.Fill = new SolidColorBrush() {
                Color = Color.FromUInt32(layer.BackgroundColor.GetValueOrDefault())
            };
        }
    }
}