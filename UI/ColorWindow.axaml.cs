using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Schets.Backend.State;
using Schets.Util;

namespace Schets.UI;

public partial class ColorWindow : Window {
    public ColorWindow() {
        this.InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    /// <summary>
    /// Initialize the component
    /// </summary>
    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// The primary R changed
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="args">The event arguments</param>
    // ReSharper disable once UnusedParameter.Local
    private void OnPrimaryRChanged(object? sender, AvaloniaPropertyChangedEventArgs args) {
        string text = ((TextBox?)sender)!.Text;
        if (text == "" || !IsColorComponentValid(text)) {
            return;
        }

        Color c = Color.FromUInt32(CanvasState.PrimaryColor);
        CanvasState.PrimaryColor = new Color(255, byte.Parse(text), c.G, c.B).ToUint32();
    }

    /// <summary>
    /// The primary G changed
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="args">The event arguments</param>
    // ReSharper disable once UnusedParameter.Local
    private void OnPrimaryGChanged(object? sender, AvaloniaPropertyChangedEventArgs args) {
        string text = ((TextBox?)sender)!.Text;
        if (text == "" || !IsColorComponentValid(text)) {
            return;
        }

        Color c = Color.FromUInt32(CanvasState.PrimaryColor);
        CanvasState.PrimaryColor = new Color(255, c.R, byte.Parse(text), c.B).ToUint32();
    }

    /// <summary>
    /// The primary B changed
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="args">The event arguments</param>
    // ReSharper disable once UnusedParameter.Local
    private void OnPrimaryBChanged(object? sender, AvaloniaPropertyChangedEventArgs args) {
        string text = ((TextBox?)sender)!.Text;
        if (text == "" || !IsColorComponentValid(text)) {
            return;
        }

        Color c = Color.FromUInt32(CanvasState.PrimaryColor);
        CanvasState.PrimaryColor = new Color(255, c.R, c.G, byte.Parse(text)).ToUint32();    
    }

    /// <summary>
    /// The secondary R changed
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="args">The event arguments</param>
    // ReSharper disable once UnusedParameter.Local
    private void OnSecondaryRChanged(object? sender, AvaloniaPropertyChangedEventArgs args) {
        string text = ((TextBox?)sender)!.Text;
        if (text == "" || !IsColorComponentValid(text)) {
            return;
        }

        Color c = Color.FromUInt32(CanvasState.SecondaryColor);
        CanvasState.SecondaryColor = new Color(255, byte.Parse(text), c.G, c.B).ToUint32();    
    }

    /// <summary>
    /// The secondary G changed
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="args">The event arguments</param>
    // ReSharper disable once UnusedParameter.Local
    private void OnSecondaryGChanged(object? sender, AvaloniaPropertyChangedEventArgs args) {
        string text = ((TextBox?)sender)!.Text;
        if (text == "" || !IsColorComponentValid(text)) {
            return;
        }

        Color c = Color.FromUInt32(CanvasState.SecondaryColor);
        CanvasState.SecondaryColor = new Color(255, c.R, byte.Parse(text), c.B).ToUint32();    
    }

    /// <summary>
    /// The secondary B changed
    /// </summary>
    /// <param name="sender">The object from which this event originates</param>
    /// <param name="args">The event arguments</param>
    // ReSharper disable once UnusedParameter.Local
    private void OnSecondaryBChanged(object? sender, AvaloniaPropertyChangedEventArgs args) {
        string text = ((TextBox?)sender)!.Text;
        if (text == "" || !IsColorComponentValid(text)) {
            return;
        }

        Color c = Color.FromUInt32(CanvasState.SecondaryColor);
        CanvasState.SecondaryColor = new Color(255, c.R, c.G, byte.Parse(text)).ToUint32();
        
    }

    /// <summary>
    /// Check if the provided string is a valid component of RGB
    /// </summary>
    /// <param name="input">The input string</param>
    /// <returns>True if the input is valid</returns>
    private static bool IsColorComponentValid(string input) {
        if (!NumberUtils.IsStringValidInt(input)) {
            return false;
        }

        int parsed = int.Parse(input);
        return parsed is >= 0 and <= 255;
    }
}