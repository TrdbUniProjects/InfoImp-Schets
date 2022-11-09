using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Schets.Backend.State;
using Schets.Util;

namespace Schets.UI; 

/// <summary>
/// Options for the selected tool. E.g. fill mode
/// </summary>
public partial class ToolOptions : UserControl {
    public ToolOptions() {
        this.InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// The Fill mode select was changed
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="e">The event arguments</param>
    /// <exception cref="InvalidDataException">Thrown if the selected fill mode is invalid</exception>
    // ReSharper disable once UnusedParameter.Local
    private void FillModeControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) {
        ComboBox box = (ComboBox)sender!;
        CanvasState.FillMode = box.SelectedIndex switch {
            0 => FillMode.Filled,
            1 => FillMode.Outline,
            2 => FillMode.FilledOutline,
            _ => throw new InvalidDataException("Invalid fill mode")
        };
    }

    /// <summary>
    /// The Brush width field was changed
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="e">The event parameters</param>
    // ReSharper disable once UnusedParameter.Local
    private void BrushWidthField_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) {
        TextBox box = (TextBox)sender!;

        if (box.Text == "") {
            return;
        }
        
        if (!NumberUtils.IsStringValidInt(box.Text)) {
            return;
        }

        int parsed = int.Parse(box.Text);
        if (parsed < 0) {
            return;
        }

        CanvasState.BrushWidth = (uint)parsed;
    }
}