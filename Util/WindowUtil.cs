using Avalonia.Controls;

namespace Schets.Util; 

public class WindowUtil {

    public static Window? FindParentWindow(IControl control) {
        if (control.Parent != null && control.Parent!.GetType() == typeof(Window)) {
            return control.Parent! as Window;
        } else {
            return FindParentWindow(control.Parent!);
        }
    }
    
}