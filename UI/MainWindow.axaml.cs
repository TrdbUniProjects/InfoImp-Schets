using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Schets.Backend;
using Schets.Backend.IO;
using Schets.Backend.State;
using Schets.Graphics;

namespace Schets.UI; 

public partial class MainWindow : Window {
    public MainWindow() {
        this.InitializeComponent();
    }

    private async void FileNewClicked(object? sender, RoutedEventArgs e) {
        await new CreateNewWindow().ShowDialog(this);
        this.Title = "untitled - Schets";
    }
    
    private async void FileOpenClicked(object? sender, RoutedEventArgs args) {
        OpenFileDialog? dialog = new OpenFileDialog() {
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

    private void ExitApplicationClicked(object? sender, RoutedEventArgs e) {
        this.Close();
    }

    private void FileSaveClicked(object? sender, RoutedEventArgs e) {
        throw new System.NotImplementedException();
    }

    private void FileSaveAsClicked(object? sender, RoutedEventArgs e) {
        throw new System.NotImplementedException();
    }
}