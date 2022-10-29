using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Schets.UI;

public partial class MessageBox : Window {
    public enum MessageBoxButtons {
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel
    }

    public enum MessageBoxResult {
        Ok,
        Cancel,
        Yes,
        No
    }

    public MessageBox() {
        AvaloniaXamlLoader.Load(this);
    }
    
    public static Task Show(Window parent, string text, string title, MessageBoxButtons buttons) {
        MessageBox msgbox = new MessageBox() {
            Title = title
        };
        msgbox.FindControl<TextBlock>("Text").Text = text;
        StackPanel buttonPanel = msgbox.FindControl<StackPanel>("Buttons");

        MessageBoxResult res = MessageBoxResult.Ok;

        void AddButton(string caption, MessageBoxResult r, bool def = false) {
            Button btn = new Button {Content = caption};
            btn.Click += (_, __) => { 
                res = r;
                msgbox.Close();
            };
            buttonPanel.Children.Add(btn);
            if (def) {
                res = r;   
            }
        }

        switch (buttons) {
            case MessageBoxButtons.Ok:
            case MessageBoxButtons.OkCancel:
                AddButton("Ok", MessageBoxResult.Ok, true);
                break;
            case MessageBoxButtons.YesNo:
            case MessageBoxButtons.YesNoCancel:
                AddButton("Yes", MessageBoxResult.Yes);
                AddButton("No", MessageBoxResult.No, true);
                break;
        }

        if (buttons == MessageBoxButtons.OkCancel || buttons == MessageBoxButtons.YesNoCancel) {
            AddButton("Cancel", MessageBoxResult.Cancel, true);
        }

        TaskCompletionSource<MessageBoxResult> tcs = new TaskCompletionSource<MessageBoxResult>();
        msgbox.Closed += delegate { tcs.TrySetResult(res); };
        if (parent != null) {
            msgbox.ShowDialog(parent);
        } else {
            msgbox.Show();
        }

        return tcs.Task;
    }
}