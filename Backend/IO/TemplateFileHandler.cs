namespace Schets.Backend.IO;

public static class TemplateFileHandler {

    public static IoResult<Template> OpenTemplate(string path) {
        IoResult<byte[]> openResult = FileHandler.OpenFile(path);
        if (!openResult.IsOk) {
            return openResult.Any<Template>();
        }

        byte[] contents = openResult.Value!;
        return JsonHandler.Deserialize<Template>(contents);
    }

    public static IoResult<object> SaveTemplate(string path, Template template) {
        byte[] serializedData = JsonHandler.Serialize(template);
        return FileHandler.WriteFile(path, serializedData);
    }
}