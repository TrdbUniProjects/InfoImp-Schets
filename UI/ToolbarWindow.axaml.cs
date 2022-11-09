using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Schets.Backend.State;
using Schets.Util;

namespace Schets.UI;

/// <summary>
/// The toolbar window
/// </summary>
public partial class ToolbarWindow : Window {

    /// <summary>
    /// The button associated with the selected tool
    /// </summary>
    private Button? _selectedTool;
    /// <summary>
    /// The default (unselected) background
    /// </summary>
    private IBrush? _defaultBackground;
    
    public ToolbarWindow() {
        this.InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);

        Button? defaultTool = this.GetControl<Button>("DefaultTool");
        if (defaultTool == null) {
            return;
        }
        
        this._selectedTool = defaultTool;
        ImmutableSolidColorBrush brush = (ImmutableSolidColorBrush)defaultTool.Background!;

        // Show the default tool as being selected
        this._defaultBackground = defaultTool.Background;
        defaultTool.Background = new SolidColorBrush {
            Color = ColorUtil.AdjustHue(brush.Color)
        };
    }

    /// <summary>
    /// The rectangle tool was clicked
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="e">The event arguments</param>
    // ReSharper disable once UnusedParameter.Local
    private void Tool_RectangleClicked(object? sender, RoutedEventArgs e) {
        this.UpdateSelectedToolColor(sender!);
        CanvasState.SelectedTool = SelectedTool.Rectangle;
    }

    /// <summary>
    /// The ellipse tool was clicked
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="e">The event arguments</param>
    // ReSharper disable once UnusedParameter.Local
    private void Tool_EllipseClicked(object? sender, RoutedEventArgs e) {
        this.UpdateSelectedToolColor(sender!);
        CanvasState.SelectedTool = SelectedTool.Ellipse;
    }

    /// <summary>
    /// Update a button's color to indicate that it is currently selected
    /// </summary>
    /// <param name="newTool">The new tool</param>
    private void UpdateSelectedToolColor(object newTool) {
        Button newToolBtn = (Button)newTool;
        if (this._selectedTool != null) {
            // 'reset' the background color of the old tool
            this._selectedTool.Background = this._defaultBackground ?? newToolBtn.Background;
            this._selectedTool.InvalidateVisual();
        }

        // Get the current background color
        if (newToolBtn.Background! is ImmutableSolidColorBrush) {
            ImmutableSolidColorBrush background = (ImmutableSolidColorBrush)newToolBtn.Background!;
            newToolBtn.Background = new SolidColorBrush {
                Color = ColorUtil.AdjustHue(background.Color)
            };
        } else {
            SolidColorBrush background = (SolidColorBrush)newToolBtn.Background!;
            newToolBtn.Background = new SolidColorBrush {
                Color = ColorUtil.AdjustHue(background.Color)
            };
        }

        newToolBtn.InvalidateVisual();
        this._selectedTool = newToolBtn;
    }

    /// <summary>
    /// The line tool was clicked
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="e">The event arguments</param>
    // ReSharper disable once UnusedParameter.Local
    private void Tool_LineClicked(object? sender, RoutedEventArgs e) {
        this.UpdateSelectedToolColor(sender!);
        CanvasState.SelectedTool = SelectedTool.Line;
    }

    /// <summary>
    /// The eraser tool was clicked
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="e">The event parameters</param>
    // ReSharper disable once UnusedParameter.Local
    private void Tool_EraserClicked(object? sender, RoutedEventArgs e) {
        this.UpdateSelectedToolColor(sender!);
        CanvasState.SelectedTool = SelectedTool.Eraser;
    }
}