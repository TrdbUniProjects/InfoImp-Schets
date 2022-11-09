using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Schets.Backend.IO; 

/// <summary>
/// JSON handler
/// </summary>
public static class JsonHandler {

    /// <summary>
    /// Deserialize JSON from bytes
    /// </summary>
    /// <param name="bytes">The bytes to deserialize</param>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <returns>
    /// The deserialized value.
    /// Returns an error condition if:
    /// - The bytes are not valid UTF-8
    /// - The string is not valid JSON
    /// - The JSON does not match the type provided
    /// </returns>
    public static IoResult<T> Deserialize<T>(byte[] bytes) {
        string stringContents;
        try {
            stringContents = Encoding.UTF8.GetString(bytes);
        } catch (ArgumentException e) {
            return IoResult<T>.Fail(e.ToString());
        }

        List<string> errors = new();
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

    /// <summary>
    /// Serialize to JSON
    /// </summary>
    /// <param name="value">The value to serialize</param>
    /// <typeparam name="T">The type of the value to serialize</typeparam>
    /// <returns>
    /// The serialized value encoded in UTF-8
    /// </returns>
    public static byte[] Serialize<T>(T value) {
        string serialized = JsonConvert.SerializeObject(value);
        return Encoding.UTF8.GetBytes(serialized);
    }
}