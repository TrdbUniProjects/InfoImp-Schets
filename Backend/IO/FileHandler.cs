using System.IO;

namespace Schets.Backend.IO; 

public static class FileHandler {

    public static IoResult<byte[]> OpenFile(string path) {
        if (!File.Exists(path)) {
            return IoResult<byte[]>.Fail("File does not exist");
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

        if (bytesRead != s.Length) {
            return IoResult<byte[]>.Fail($"Failed to read entire file. Read {bytesRead} out of {s.Length} total bytes");
        }

        return IoResult<byte[]>.Ok(buffer);
    }

    public static IoResult<object> WriteFile(string path, byte[] contents) {
        try {
            FileStream s = new FileStream(path, FileMode.Create);
            s.Write(contents);
        } catch (IOException e) {
            return IoResult<object>.Fail(e.ToString());  
        }
        
        return IoResult<object>.Ok(null);
    }
    
}