using System;
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

namespace Schets.Graphics; 

public partial class DrawSurface : UserControl {
    private Point? _drawStartPoint;
    private bool _isPointerDown;
    
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
        this.PointerMoved += this.OnPointerMovedEvent;
    }

    public override void Render(DrawingContext context) {
        foreach(AbstractShape shape in GetShapes()) {
            shape.Render(context);
        }

        if (CanvasState.DrawingShape != null) {
            GetShapeFromLayer(CanvasState.DrawingShape.GetValueOrDefault()).Render(context);
        }
    }

    private void OnPointerMovedEvent(object? sender, PointerEventArgs args) {
        if (!this._isPointerDown) {
            return;
        }

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
                        Color = CanvasState.PrimaryColor,
                        Thickness = CanvasState.BrushWidth
                    };
                    descriptor.BackgroundColor = CanvasState.PrimaryColor;
                    break;
            }

            CanvasState.DrawingShape = descriptor;
        }
        
        this.InvalidateVisual();
    }

    private void OnPointerPressedEvent(object? sender, PointerPressedEventArgs args) {
        if (!args.Pointer.IsPrimary) {
            return;
        }

        if (CanvasState.SelectedTool == SelectedTool.Eraser) {
            int? idxToRemove = null;
            for (int idx = 0; idx < CanvasState.Layers.Count; idx++) {
                TemplateShapeDescriptor descriptor = CanvasState.Layers[idx];
                if (this.HitTest(descriptor, args.GetPosition(this))) {
                    idxToRemove = idx;
                }
            }

            if (idxToRemove == null) {
                return;
            }
            CanvasState.Layers.RemoveAt(idxToRemove.Value);
            this.InvalidateVisual();

            return;
        }

        this._drawStartPoint = args.GetPosition(this);
        this._isPointerDown = true;
    }

    private bool HitTest(TemplateShapeDescriptor descriptor, Point point) {
        return descriptor.ShapeType switch {
            TemplateShapeType.Rectangle => this.HitTestSquare(descriptor, point),
            TemplateShapeType.Ellipse => this.HitTestEllipse(descriptor, point),
            TemplateShapeType.Line =>
                // TODO
                false,
            _ => throw new CaseNotImplementedException($"No hit-test implemented for shape type {descriptor.ShapeType}")
        };
    }

    private bool HitTestEllipse(TemplateShapeDescriptor descriptor, Point point) {
        double height = Math.Abs(descriptor.A.Y - descriptor.B.Y);
        double width = Math.Abs(descriptor.A.X - descriptor.B.X);
        
        if (descriptor.BackgroundColor == null) { // Outline only
            Point center = new(descriptor.A.X + width / 2d, descriptor.A.Y + height / 2d);

            double distance = DistanceToEllipse(center, width / 2, height / 2, point);
            double factor = Math.Max(this.Width * 0.05, this.Height * 0.05);

            return distance < factor;
        } else { // Filled
            // Andreas (https://math.stackexchange.com/users/317854/andreas), Point-Ellipse collision test., URL (version: 2017-01-26): https://math.stackexchange.com/q/2114902
            
            double x = point.X;
            double y = point.Y;
                
            double mx = descriptor.A.X + width / 2d;
            double my = descriptor.A.Y + height / 2d;
            double sigmaX = Math.Abs(-descriptor.A.X + descriptor.B.X) / 2    ;
            double sigmaY = Math.Abs(-descriptor.A.Y + descriptor.B.Y) / 2;

            double resultX = (x - mx) * (x - mx) / (sigmaX * sigmaX);
            double resultY = (y - my) * (y - my) / (sigmaY * sigmaY);
            double result = resultX + resultY;

            return result < 1;
        }
    }
    
    private bool HitTestSquare(TemplateShapeDescriptor descriptor, Point point) {
        if (descriptor.BackgroundColor == null) { // Shape is outline only
            Point topLeft = descriptor.A.ToPoint();
            Point bottomRight = descriptor.B.ToPoint();

            Point topRight = new (bottomRight.X, topLeft.Y);
            Point bottomLeft = new (topLeft.X, bottomRight.Y);

            // A rectangle is 4 sides, so we'll calculate the distance from
            // the point to each side. If the distance is within the threshold 
            // on any side, we'll consider it a hit

            double distLeft = DistanceToLine(topLeft, bottomLeft, point);
            double distTop = DistanceToLine(topLeft, topRight, point);
            double distRight = DistanceToLine(topRight, bottomRight, point);
            double distBottom = DistanceToLine(bottomRight, bottomLeft, point);

            double wThreshold = this.Width * 0.05;
            double hThreshold = this.Height * 0.05;
            
            return distLeft < wThreshold
                   || distTop < hThreshold
                   || distRight < wThreshold
                   || distBottom < hThreshold;
            
        } else { // Shape is filled
            Rect r = new Rect(descriptor.A.ToPoint(), descriptor.B.ToPoint()); 
            return r.Contains(point);
        }
    }

    private static double DistanceToEllipse(Point c, double radiusX, double radiusY, Point p) {
        
        // Point on the ellipse intersecting with the line between p and s
        // However, the function assumes the ellipse to be on the origin
        // So we translate the point accordingly
        Point pTranslated = new(
            p.X - c.X,
            p.Y - c.Y
        );
        Point s = FindClosestPointOnEllipse(radiusX, radiusY, pTranslated);
        // s is the point where the line between p and the origin intersects the ellipse CO.
        // CO is centered on the origin, our ellipse C is not, translate S back to the 'real' space
        Point sTranslated = new (
            s.X + c.X,
            s.Y + c.Y
        );

        // Pythahoras between the point on the ellipse and the point clicked
        return Math.Sqrt(
            (sTranslated.X - p.X) * (sTranslated.X - p.Y) 
            + (sTranslated.Y - p.Y) * (sTranslated.Y - p.Y)
        );
    }
    
    private static Point FindClosestPointOnEllipse(double radiusX, double radiusY, Point p) {
        // https://stackoverflow.com/a/46007540
        // https://gist.github.com/JohannesMP/777bdc8e84df6ddfeaa4f0ddb1c7adb3
        // https://github.com/0xfaded/ellipse_demo/issues/1
        
        double px = Math.Abs(p.X);
        double py = Math.Abs(p.Y);

        // semimajor
        double a = Math.Max(radiusX, radiusY);
        // semiminor
        double b = Math.Min(radiusX, radiusY);

        double tx = 0.70710678118;
        double ty = 0.70710678118;

        double aa = a * a;
        double bb = b * b;

        for (int i = 0; i < 3; i++) {
            double x = a * tx;
            double y = b * ty;

            double ex = (aa - bb) * (tx * tx * tx) / a;
            double ey = (bb - aa) * (ty * ty * ty) / b;

            double rx = x - ex;
            double ry = y - ey;

            double qx = px - ex;
            double qy = py - ey;

            double r = Math.Sqrt(rx * rx + ry * ry);
            double q = Math.Sqrt(qy * qy + qx * qx);

            tx = Math.Min(1, Math.Max(0, (qx * r / q + ex) / a));
            ty = Math.Min(1, Math.Max(0, (qy * r / q + ey) / b));

            double t = Math.Sqrt(tx * tx + ty * ty);
            
            tx /= t;
            ty /= t;
        }

        return new Point(
            a * (p.X < 0 ? -tx : tx),
            b * (p.Y < 0 ? -ty : ty)
        );
    }

    /**
     * Calculate the distance from point p0 to the line between p1 and p2.
     *
     * <param name="p1">Point 1 of the line</param>
     * <param name="p2">Point 2 of the line</param>
     * <param name="p0">The point to calculate the distance from</param>
     */
    private static double DistanceToLine(Point p1, Point p2, Point p0) {
        // https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line#Line_defined_by_two_points
        
        // |(x2 - x1) * (y1 - y0) - (x1 - x0) * (y2 - y1)|
        double top = Math.Abs((p2.X - p1.X) * (p1.Y - p0.Y) - (p1.X - p0.X) * (p2.Y - p1.Y));
        // sqrt((x2 - x1)^2 + (y2 - y1)^2)
        double bottom = Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));

        return top / bottom;
    }

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
            SelectedTool.Eraser => throw new InvalidDataException("This tool creates no shape"),
            _ => throw new CaseNotImplementedException($"Tool {CanvasState.SelectedTool} is not implemented")
        };
    }

    private static IEnumerable<AbstractShape> GetShapes() {
        return CanvasState.Layers.Select(GetShapeFromLayer);
    }

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