namespace Schets.Backend.IO;

/// <summary>
/// File handler to read and write templates
/// </summary>
public static class TemplateFileHandler {

    /// <summary>
    /// Open a template from the disk
    /// </summary>
    /// <param name="path">The file path to read from</param>
    /// <returns>
    /// The template stored in the file.
    /// Returns an error condition if:
    /// - The path is not file or does not exist
    /// - The contents of the file do not make up a valid template
    /// </returns>
    public static IoResult<Template> OpenTemplate(string path) {
        IoResult<byte[]> openResult = FileHandler.OpenFile(path);
        if (!openResult.IsOk) {
            return openResult.Any<Template>();
        }

        byte[] contents = openResult.Value!;
        return JsonHandler.Deserialize<Template>(contents);
    }

    /// <summary>
    /// Save a template to disk
    /// </summary>
    /// <param name="path">The path to write to</param>
    /// <param name="template">The template to save</param>
    /// <returns>
    /// Returns an error condition if:
    /// - An IO error occurs
    /// </returns>
    public static IoResult<object> SaveTemplate(string path, Template template) {
        byte[] serializedData = JsonHandler.Serialize(template);
        return FileHandler.WriteFile(path, serializedData);
    }
}