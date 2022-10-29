using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Schets.UI;

public partial class ToolbarWindow : Window {
    public ToolbarWindow() {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}