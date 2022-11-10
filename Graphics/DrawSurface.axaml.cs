using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Schets.Backend;
using Schets.Backend.Exceptions;
using Schets.Backend.State;
using Schets.Util;

namespace Schets.Graphics; 

/// <summary>
/// The surface the user can draw their shapes on
/// </summary>
public partial class DrawSurface : UserControl {
    /// <summary>
    /// The point the user started drawing at
    /// </summary>
    private Point? _drawStartPoint;
    /// <summary>
    /// Whether the pointer (i.e the primary mouse button) is currently pressed
    /// </summary>
    private bool _isPointerDown;
    
    static DrawSurface() {
        AffectsRender<DrawSurface>();
    }
    
    public DrawSurface() {
        this.InitializeComponent();
    }

    /// <summary>
    /// Initialize this component
    /// </summary>
    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);

        this.PointerPressed += this.OnPointerPressedEvent;
        this.PointerReleased += this.OnPointerReleasedEvent;
        this.PointerMoved += this.OnPointerMovedEvent;
    }

    /// <summary>
    /// Render all shapes to the context
    /// </summary>
    /// <param name="context">The context to render to</param>
    public override void Render(DrawingContext context) {
        foreach(AbstractShape shape in GetShapes()) {
            shape.Render(context);
        }

        if (CanvasState.DrawingShape != null) {
            GetShapeFromLayer(CanvasState.DrawingShape.GetValueOrDefault()).Render(context);
        }
    }

    /// <summary>
    /// Event fired when the pointer is moused
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="args">The arguments to this event</param>
    private void OnPointerMovedEvent(object? sender, PointerEventArgs args) {
        // If the mouse is not currently down,
        // the user isn't drawing. We dont care
        if (!this._isPointerDown) {
            return;
        }
        
        // Show what the user will be drawing as they
        // are moving their mouse
        if (CanvasState.DrawingShape != null) {
            TemplateShapeDescriptor shape = CanvasState.DrawingShape.Value;

            if (GetActiveShapeType() == TemplateShapeType.Line) {
                TemplateCoordinate b = shape.B;

                b.X = args.GetPosition(this).X;
                b.Y = args.GetPosition(this).Y;
                    
                shape.B = b;
            } else {
                (TemplateCoordinate topLeft, TemplateCoordinate bottomRight) =
                    CalculateShapeCoordinates(this._drawStartPoint!.Value, args.GetPosition(this));
                shape.A = topLeft;
                shape.B = bottomRight;
            }
                
            CanvasState.DrawingShape = shape;
        } else {
            TemplateShapeDescriptor descriptor;

            if (GetActiveShapeType() == TemplateShapeType.Line) {
                descriptor = new TemplateShapeDescriptor {
                    ShapeType = GetActiveShapeType(),
                    A = new TemplateCoordinate {
                        X = this._drawStartPoint!.Value.X,
                        Y = this._drawStartPoint!.Value.Y
                    },
                    B = new TemplateCoordinate {
                        X = args.GetPosition(this).X,
                        Y = args.GetPosition(this).Y
                    },
                    Outline = new TemplateShapeOutline {
                        Color = CanvasState.PrimaryColor,
                        Thickness = CanvasState.BrushWidth,
                    }
                };
            } else {
                (TemplateCoordinate topLeft, TemplateCoordinate bottomRight) =
                    CalculateShapeCoordinates(this._drawStartPoint!.Value, args.GetPosition(this));

                descriptor = new TemplateShapeDescriptor {
                    ShapeType = GetActiveShapeType(),
                    A = topLeft,
                    B = bottomRight
                };
            }

            switch (CanvasState.FillMode) {
                case FillMode.Filled:
                    descriptor.BackgroundColor = CanvasState.PrimaryColor;
                    break;
                case FillMode.Outline:
                    descriptor.Outline = new TemplateShapeOutline {
                        Color = CanvasState.PrimaryColor,
                        Thickness = CanvasState.BrushWidth
                    };
                    break;
                case FillMode.FilledOutline:
                    descriptor.Outline = new TemplateShapeOutline {
                        Color = CanvasState.SecondaryColor,
                        Thickness = CanvasState.BrushWidth
                    };
                    descriptor.BackgroundColor = CanvasState.PrimaryColor;
                    break;
            }

            CanvasState.DrawingShape = descriptor;
        }
        
        this.InvalidateVisual();
    }

    /// <summary>
    /// Event fired when the mouse pointer is released
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="args">The arguments to this event</param>
    private void OnPointerPressedEvent(object? sender, PointerPressedEventArgs args) {
        // We only care for the primary button
        if (!args.Pointer.IsPrimary) {
            return;
        }

        if (CanvasState.SelectedTool == SelectedTool.Eraser) {
            int? idxToRemove = null;
            for (int idx = 0; idx < CanvasState.Layers.Count; idx++) {
                TemplateShapeDescriptor descriptor = CanvasState.Layers[idx];
                if (HitUtil.HitTest(descriptor, args.GetPosition(this), new Size(this.Width, this.Height))) {
                    idxToRemove = idx;
                }
            }

            if (idxToRemove == null) {
                return;
            }
            
            CanvasState.RemoveLayer(idxToRemove.Value);
            this.InvalidateVisual();

            return;
        }

        this._drawStartPoint = args.GetPosition(this);
        this._isPointerDown = true;
    }

    /// <summary>
    /// Event fired when the pointer is released
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="args">The arguments to this event</param>
    private void OnPointerReleasedEvent(object? sender, PointerReleasedEventArgs args) {
        if (CanvasState.SelectedTool == SelectedTool.Eraser) {
            return;
        }
        
        if (this._isPointerDown) {
            this._isPointerDown = false;
            CanvasState.DrawingShape = null;
        }
        
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
                    throw new CaseNotImplementedException($"The fill mode {CanvasState.FillMode} is not implemented");
            }
        }

        CanvasState.AddLayer(descriptor);
        this.InvalidateVisual();
    }

    /// <summary>
    /// Calculate the top left and bottom right coordinates of the shape
    /// </summary>
    /// <param name="start">The draw start point</param>
    /// <param name="end">The draw end point</param>
    /// <returns>A tuple of the top left and bottom right coordinates</returns>
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

    /// <summary>
    /// Get the shape type currently being drawn
    /// </summary>
    /// <returns>The shape currently being drawn</returns>
    /// <exception cref="InvalidDataException">Thrown when a tool is selected which is not a shape</exception>
    /// <exception cref="CaseNotImplementedException">Thrown when a tool is selected which is not implemented</exception>
    private static TemplateShapeType GetActiveShapeType() {
        return CanvasState.SelectedTool switch {
            SelectedTool.Rectangle => TemplateShapeType.Rectangle,
            SelectedTool.Ellipse => TemplateShapeType.Ellipse,
            SelectedTool.Line => TemplateShapeType.Line,
            SelectedTool.Eraser => throw new InvalidDataException("This tool creates no shape"),
            _ => throw new CaseNotImplementedException($"Tool {CanvasState.SelectedTool} is not implemented")
        };
    }

    /// <summary>
    /// Get all layers as shapes
    /// </summary>
    /// <returns>All shapes</returns>
    private static IEnumerable<AbstractShape> GetShapes() {
        return CanvasState.Layers.Select(GetShapeFromLayer);
    }

    /// <summary>
    /// Convert a layer to shape object
    /// </summary>
    /// <param name="layer">The layer to covert</param>
    /// <returns>The shape</returns>
    /// <exception cref="CaseNotImplementedException">When the layer has a shape that is not implemented</exception>
    private static AbstractShape GetShapeFromLayer(TemplateShapeDescriptor layer) {
        AbstractShape shape = layer.ShapeType switch {
            TemplateShapeType.Rectangle => new Rectangle(),
            TemplateShapeType.Ellipse => new Ellipse(),
            TemplateShapeType.Line => new Line(),
            _ => throw new CaseNotImplementedException($"Shape {layer.ShapeType} is not implemented")
        };
        
        SetShapeProperties(ref shape, layer);
        return shape;
    }

    /// <summary>
    /// Set the properties of the shape.
    /// This includes:
    /// - The A coordinate
    /// - The B coordinate
    /// - The outline
    /// - The fill
    /// </summary>
    /// <param name="shape">The shape</param>
    /// <param name="layer">The layer</param>
    /// <typeparam name="T">The type of the shape</typeparam>
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