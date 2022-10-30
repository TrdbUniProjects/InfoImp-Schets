using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Schets.Backend;
using Schets.Backend.State;
using Schets.Graphics;
using Schets.Util;

namespace Schets.UI;

public partial class CreateNewWindow : Window {
    
    public CreateNewWindow() {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void OkButtonClicked(object? sender, RoutedEventArgs e) {
        TextBox widthField = this.FindControl<TextBox>("WidthField");
        TextBox heightField = this.FindControl<TextBox>("HeightField");

        if (!NumberUtils.IsStringValidInt(widthField.Text)) {
            this.ShowInvalidValueDialog(widthField.Text);
            return;
        }

        if (!NumberUtils.IsStringValidInt(heightField.Text)) {
            this.ShowInvalidValueDialog(heightField.Text);
            return;
        }

        CanvasState.SetLayers(Array.Empty<TemplateShapeDescriptor>());
        
        DrawSurface drawSurface = this.Owner.FindControl<DrawSurface>("DrawSurface")!;

        drawSurface.Width = int.Parse(widthField.Text);
        drawSurface.Height = int.Parse(heightField.Text);
        
        drawSurface.InvalidateMeasure();
        drawSurface.InvalidateVisual();
        
        this.Close();
    }

    private async void ShowInvalidValueDialog(string value) {
        await MessageBox.Show(
            this,
            $"The value {value} is not valid!",
            "Invalid value",
            MessageBox.MessageBoxButtons.Ok
        );
    }

    private void CancelButtonClicked(object? sender, RoutedEventArgs e) {
        this.Close();
    }
}