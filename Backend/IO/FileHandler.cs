using System.IO;

namespace Schets.Backend.IO; 

/// <summary>
/// File IO handler
/// </summary>
public static class FileHandler {

    /// <summary>
    /// Open a file and read its contents.
    /// </summary>
    /// <param name="path">The path read from</param>
    /// <returns>
    /// The contents of the file.
    /// An error result is returned if:
    /// - The path does not exist
    /// - The path is not a file
    /// - An IOException occurs while reading the file
    /// - The entire file could not be read
    /// </returns>
    public static IoResult<byte[]> OpenFile(string path) {
        if (!File.Exists(path)) {
            return IoResult<byte[]>.Fail("File does not exist");
        }

        FileAttributes attr = File.GetAttributes(path);
        if ((attr & FileAttributes.Directory) != FileAttributes.Directory) {            
            return IoResult<byte[]>.Fail("Path is a directory");
        }

        FileStream s;
        try {
            s = new FileStream(path, FileMode.Open);
        } catch (IOException e) {
            return IoResult<byte[]>.Fail(e.ToString());
        }

        byte[] buffer = new byte[s.Length];
        int bytesRead;
        try {
            bytesRead = s.Read(buffer);
        } catch (IOException e) {
            return IoResult<byte[]>.Fail(e.ToString());
        }

        return bytesRead != s.Length ? IoResult<byte[]>.Fail($"Failed to read entire file. Read {bytesRead} out of {s.Length} total bytes") : IoResult<byte[]>.Ok(buffer);
    }

    /// <summary>
    /// Write a file
    /// </summary>
    /// <param name="path">The path to write to.</param>
    /// <param name="contents">The contents of the file to write</param>
    /// <returns>
    /// An error condition is returned if:
    /// - The file could not be created
    /// - An IOException occurs while writing the contents
    /// </returns>
    public static IoResult<object> WriteFile(string path, byte[] contents) {
        try {
            FileStream s = new(path, FileMode.Create);
            s.Write(contents);
        } catch (IOException e) {
            return IoResult<object>.Fail(e.ToString());  
        }
        
        return IoResult<object>.Ok(null);
    }
    
}