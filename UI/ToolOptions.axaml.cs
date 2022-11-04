using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Schets.Backend.State;
using Schets.Util;

namespace Schets.UI; 

public partial class ToolOptions : UserControl {
    public ToolOptions() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void FillModeControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) {
        ComboBox box = (ComboBox)sender!;
        CanvasState.FillMode = box.SelectedIndex switch {
            0 => FillMode.Filled,
            1 => FillMode.Outline,
            2 => FillMode.FilledOutline,
            _ => throw new InvalidDataException("Invalid fill mode")
        };
    }

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