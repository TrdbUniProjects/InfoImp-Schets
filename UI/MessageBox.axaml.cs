using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Schets.UI;

/// <summary>
/// A message box for informing the user about something and asking for their input.
/// </summary>
public partial class MessageBox : Window {
    /// <summary>
    /// The buttons shown to the user
    /// </summary>
    public enum MessageBoxButtons {
        /// <summary>
        /// OK button
        /// </summary>
        Ok,
        /// <summary>
        /// OK and Cancel button
        /// </summary>
        OkCancel,
        /// <summary>
        /// Yes and No button
        /// </summary>
        YesNo,
        /// <summary>
        /// Yes, No and Cancel button
        /// </summary>
        YesNoCancel
    }

    /// <summary>
    /// The result returned from the message box.
    /// </summary>
    public enum MessageBoxResult {
        /// <summary>
        /// OK was clicked
        /// </summary>
        Ok,
        /// <summary>
        /// Cancel was clicked
        /// </summary>
        Cancel,
        /// <summary>
        /// Yes was clicked
        /// </summary>
        Yes,
        /// <summary>
        /// No was clicked
        /// </summary>
        No
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public MessageBox() {
        AvaloniaXamlLoader.Load(this);
    }
    
    /// <summary>
    /// Show the message box
    /// </summary>
    /// <param name="parent">The parent window</param>
    /// <param name="text">The text to show</param>
    /// <param name="title">The title of the window</param>
    /// <param name="buttons">The buttons to show</param>
    /// <returns>The Task containing the result of the message box</returns>
    public static Task<MessageBoxResult> Show(Window? parent, string text, string title, MessageBoxButtons buttons) {
        MessageBox msgbox = new() {
            Title = title,
            TransparencyLevelHint = WindowTransparencyLevel.None
        };
        msgbox.FindControl<TextBlock>("Text").Text = text;
        StackPanel buttonPanel = msgbox.FindControl<StackPanel>("Buttons");

        MessageBoxResult res = MessageBoxResult.Ok;

        void AddButton(string caption, MessageBoxResult r, bool def = false) {
            Button btn = new() {
                Content = caption
            };
            
            btn.Click += (_, _) => { 
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
                AddButton("Yes", MessageBoxResult.Yes);
                break;
            case MessageBoxButtons.YesNoCancel:
                AddButton("Yes", MessageBoxResult.Yes);
                AddButton("No", MessageBoxResult.No, true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null);
        }

        if (buttons is MessageBoxButtons.OkCancel or MessageBoxButtons.YesNoCancel) {
            AddButton("Cancel", MessageBoxResult.Cancel, true);
        }

        TaskCompletionSource<MessageBoxResult> tcs = new TaskCompletionSource<MessageBoxResult>();
        msgbox.Closed += (_, _) => { tcs.TrySetResult(res); };
        if (parent != null) {
            msgbox.ShowDialog(parent).Wait();
        } else {
            msgbox.Show();
        }

        return tcs.Task;
    }
}