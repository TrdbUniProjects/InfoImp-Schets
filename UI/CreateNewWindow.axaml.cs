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
        this.InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Event fired when the OK button is clicked
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="e">The arguments to this event</param>
    // ReSharper disable UnusedParameter.Local
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

        // Clear the canvas layers
        CanvasState.SetLayers(Array.Empty<TemplateShapeDescriptor>());
        
        DrawSurface drawSurface = this.Owner.FindControl<DrawSurface>("DrawSurface")!;

        drawSurface.Width = int.Parse(widthField.Text);
        drawSurface.Height = int.Parse(heightField.Text);
        
        drawSurface.InvalidateMeasure();
        drawSurface.InvalidateVisual();
        
        this.Close();
    }

    /// <summary>
    /// Show a dialog informing the user that a value is invalid
    /// </summary>
    /// <param name="value">The invalid value</param>
    private async void ShowInvalidValueDialog(string value) {
        await MessageBox.Show(
            this,
            $"The value {value} is not valid!",
            "Invalid value",
            MessageBox.MessageBoxButtons.Ok
        );
    }

    /// <summary>
    /// Event fired when the Cancel button is clicked
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="e">The arguments to this event</param>
    // ReSharper disable UnusedParameter.Local+
    private void CancelButtonClicked(object? sender, RoutedEventArgs e) {
        this.Close();
    }
}