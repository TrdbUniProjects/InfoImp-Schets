using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Schets.Backend.State;
using Schets.Util;

namespace Schets.UI;

public partial class ToolbarWindow : Window {

    private Button? _selectedTool;
    
    public ToolbarWindow() {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);

        Button? defaultTool = this.GetControl<Button>("DefaultTool");
        if (defaultTool != null) {
            this._selectedTool = defaultTool;
            
            ImmutableSolidColorBrush brush = (ImmutableSolidColorBrush)defaultTool!.Background!;
            defaultTool.Background = new SolidColorBrush {
                Color = DarkenColor(brush.Color)
            };
        }
    }

    private void Tool_RectangleClicked(object? sender, RoutedEventArgs e) {
        this.UpdateSelectedToolColor(sender!);
        CanvasState.SelectedTool = SelectedTool.Rectangle;
    }

    private void Tool_EllipseClicked(object? sender, RoutedEventArgs e) {
        this.UpdateSelectedToolColor(sender!);
        CanvasState.SelectedTool = SelectedTool.Ellipse;
    }

    private void UpdateSelectedToolColor(object newTool) {
        Button newToolBtn = (Button)newTool;
        if (this._selectedTool != null) {
            // 'reset' the background color of the old tool
            this._selectedTool.Background = newToolBtn.Background;
            this._selectedTool.InvalidateVisual();
        }

        // Get the current background color
        ImmutableSolidColorBrush background = (ImmutableSolidColorBrush)newToolBtn.Background!;
        
        newToolBtn.Background = new SolidColorBrush {
            Color = DarkenColor(background.Color)
        };
        
        newToolBtn.InvalidateVisual();
        this._selectedTool = newToolBtn;
    }

    private static Color DarkenColor(Color r, double factor = 0.7d) {
        Rgb originalColor = new() {
            R = r.R,
            G = r.G,
            B = r.B,
        };
        
        // Convert to HSV
        Hsv hsv = ColorUtil.FromRgb(originalColor);

        // Darken the color        
        hsv.V *= factor;
        
        // Convert back to RGB
        Rgb newColor = ColorUtil.FromHsv(hsv);

        return new Color(255, (byte)newColor.R, (byte)newColor.G, (byte)newColor.B);
    }
}