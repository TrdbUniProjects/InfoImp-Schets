using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
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

        TemplateShapeDescriptor descriptor;
        if (GetActiveShapeType() == TemplateShapeType.Line) {
            descriptor = new TemplateShapeDescriptor {
                ShapeType = GetActiveShapeType(),
                A = new TemplateCoordinate {
                    X = this._drawStartPoint.GetValueOrDefault().X,
                    Y = this._drawStartPoint.GetValueOrDefault().Y
                },
                B = new TemplateCoordinate {
                    X = drawEndPoint.X,
                    Y = drawEndPoint.Y
                },
                Outline = new TemplateShapeOutline {
                    Color = CanvasState.PrimaryColor,
                    Thickness = CanvasState.BrushWidth,
                }
            };
        } else {
            (TemplateCoordinate topLeft, TemplateCoordinate bottomRight) =
                CalculateShapeCoordinates(this._drawStartPoint.GetValueOrDefault(), drawEndPoint);
        
            descriptor = new TemplateShapeDescriptor {
                ShapeType = GetActiveShapeType(),
                A = topLeft,
                B = bottomRight
            };

            switch (CanvasState.FillMode) {
                case FillMode.Filled:
                    descriptor.BackgroundColor = CanvasState.PrimaryColor;
                    descriptor.Outline = null;
                    break;
                case FillMode.Outline:
                    descriptor.BackgroundColor = null;
                    descriptor.Outline = new TemplateShapeOutline {
                        Color = CanvasState.PrimaryColor,
                        Thickness = CanvasState.BrushWidth
                    };
                    break;
                case FillMode.FilledOutline:
                    descriptor.BackgroundColor = CanvasState.PrimaryColor;
                    descriptor.Outline = new TemplateShapeOutline {
                        Color = CanvasState.SecondaryColor,
                        Thickness = CanvasState.BrushWidth
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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
            SelectedTool.Line => TemplateShapeType.Line,
        };
    }

    private static IEnumerable<AbstractShape> GetShapes() {
        return CanvasState.Layers.Select(GetShapeFromLayer);
    }

    private static AbstractShape GetShapeFromLayer(TemplateShapeDescriptor layer) {
        AbstractShape shape =  layer.ShapeType switch {
            TemplateShapeType.Rectangle => new Rectangle(),
            TemplateShapeType.Ellipse => new Ellipse(),
            TemplateShapeType.Line => new Line(),
            _ => throw new NotImplementedException("Shape not yet implemented")
        };
        
        SetShapeProperties(ref shape, layer);
        return shape;
    }

    private static void SetShapeProperties<T>(ref T shape, TemplateShapeDescriptor layer) where T: AbstractShape {
        shape.A = new Point(layer.A.X, layer.A.Y);
        shape.B = new Point(layer.B.X, layer.B.Y);

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