using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Schets.Backend.IO; 

public class JsonHandler {

    public static IoResult<T> Deserialize<T>(byte[] bytes) {
        string stringContents;
        try {
            stringContents = Encoding.UTF8.GetString(bytes);
        } catch (ArgumentException e) {
            return IoResult<T>.Fail(e.ToString());
        }

        List<string> errors = new List<string>();
        T? value = JsonConvert.DeserializeObject<T>(
            stringContents,
            new JsonSerializerSettings() {
                Error = delegate(object? _, ErrorEventArgs args) {
                    string? errorString = args.ToString();
                    string errorStringNonNull = errorString ?? "An error occured while deserializing JSON";
                    
                    errors.Add(errorStringNonNull);
                    args.ErrorContext.Handled = true;
                }
            }
        );

        if (errors.Count != 0) {
            return IoResult<T>.Fail(errors[0]);
        }

        if (value == null) {
            return IoResult<T>.Fail("An error occured while deserializing JSON");
        }
        
        return IoResult<T>.Ok(value);
    }

    public static byte[] Serialize<T>(T value) {
        string serialized = JsonConvert.SerializeObject(value);
        return Encoding.UTF8.GetBytes(serialized);
    }
}