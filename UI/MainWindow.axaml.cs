using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Schets.Backend;
using Schets.Backend.IO;
using Schets.Backend.State;
using Schets.Graphics;

namespace Schets.UI; 

public partial class MainWindow : Window {

    private bool _isToolWindowOpened;
    
    public MainWindow() {
        this.InitializeComponent();
        this.Opened += (_, _) => this.OpenToolWindow();
    }

    private void OpenToolWindow() {
        if (this._isToolWindowOpened) {
            return;
        }

        ToolbarWindow window = new() {
            Position = new PixelPoint(
                this.Position.X + 30,
                this.Position.Y + (int)this.Height / 2
            ),
            ShowInTaskbar = false,
        };
        window.Closed += (_, _) => this._isToolWindowOpened = false;    
        
        window.Show(this);
        this._isToolWindowOpened = true;
    }

    // ReSharper disable UnusedParameter.Local
    private void Window_ToolClicked(object? sender, RoutedEventArgs args) => this.OpenToolWindow();


    // ReSharper disable UnusedParameter.Local
    private async void File_NewClicked(object? sender, RoutedEventArgs args) {
        await new CreateNewWindow().ShowDialog(this);
    }
    
    private async void File_OpenClicked(object? sender, RoutedEventArgs args) {
        OpenFileDialog dialog = new() {
            AllowMultiple = false,
            Title = "Select template",
            Filters = new List<FileDialogFilter> {
                new FileDialogFilter {
                    Name = "Schets Templates",
                    Extensions = {
                        Constants.TemplateFileExtension
                    }
                }
            },
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        };
        
        string[]? selectedFiles = await dialog.ShowAsync(this);
        if (selectedFiles == null) {
            return;
        }

        IoResult<Template> templateResult = TemplateFileHandler.OpenTemplate(selectedFiles[0]);
        if (!templateResult.IsOk) {
            throw new NotImplementedException("IO error");   
        }

        ProgramState.OpenedFilePath = selectedFiles[0];
        this.Title = $"{selectedFiles[0]} - Schets";
        
        Template template = templateResult.Value;
        CanvasState.SetLayers(template.Shapes);

        DrawSurface surface = this.FindControl<DrawSurface>("DrawSurface")!;
        surface.Width = template.Size.Width;
        surface.Height = template.Size.Height;
        surface.InvalidateMeasure();
        surface.InvalidateVisual();
    }

    private void File_ExitClicked(object? sender, RoutedEventArgs e) {
        this.Close();
    }

    private async void File_SaveClicked(object? sender, RoutedEventArgs e) {
        string path;

        if (ProgramState.OpenedFilePath != null) {
            path = ProgramState.OpenedFilePath;
        } else {
            SaveFileDialog saveFileDialog = new() {
                DefaultExtension = Constants.TemplateFileExtension,
                Directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            };
            string? selectedPath = await saveFileDialog.ShowAsync(this);

            if (selectedPath == null) {
                return;
            }

            path = selectedPath;
        }

        ProgramState.OpenedFilePath = path;
        this.Title = $"{path} - Schets";
        
        DrawSurface surface = this.FindControl<DrawSurface>("DrawSurface")!;
        Template t = new() {
            Size = new TemplateSize() {
                Width = (int)surface.Width,
                Height = (int)surface.Height
            },
            Shapes = CanvasState.Layers.ToArray()
        };

        TemplateFileHandler.SaveTemplate(path, t);
    }

    private async void File_SaveAsClicked(object? sender, RoutedEventArgs e) {
        string? path = await new SaveFileDialog() {
            DefaultExtension = "png",
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        }.ShowAsync(this);

        if (path == null) {
            return;
        }
        
        DrawSurface surface = this.FindControl<DrawSurface>("DrawSurface")!;

        RenderTargetBitmap bitmap = new RenderTargetBitmap(new PixelSize((int) surface.Width, (int) surface.Height));
        bitmap.Render(surface);

        try {
            bitmap.Save(path);
        } catch (IOException) {
            throw new NotImplementedException("IO error");
        }
    }
}