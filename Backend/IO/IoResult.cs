namespace Schets.Backend.IO; 

/// <summary>
/// The result of an IO function
/// </summary>
/// <typeparam name="T">The contained return type if an OK condition is returned</typeparam>
public class IoResult<T> {

    /// <summary>
    /// The return value on an OK condition
    /// </summary>
    public T? Value { get; private init; }
    /// <summary>
    /// Whether the result is OK
    /// </summary>
    public bool IsOk { get; private init; }
    /// <summary>
    /// The error message if the result is not OK
    /// </summary>
    public string? Error { get; private init;  }

    private IoResult() {}

    /// <summary>
    /// The IO operation succeeded
    /// </summary>
    /// <param name="value">The value to return</param>
    /// <returns>A Result with an OK value</returns>
    public static IoResult<T> Ok(T? value) {
        return new IoResult<T>() {
            IsOk = true,
            Value = value
        };
    }

    /// <summary>
    /// The IO operation failed
    /// </summary>
    /// <param name="error">The error message</param>
    /// <returns>A Result with an error value</returns>
    public static IoResult<T> Fail(string error) {
        return new IoResult<T>() {
            Error = error,
            IsOk = false
        };
    }

    /// <summary>
    /// Helper function to transform between different generic types.
    /// This is useful for bubbling up results with different OK types in case of an error.
    /// </summary>
    /// <typeparam name="TA">The wanted OK value type</typeparam>
    /// <returns>A result with the same OK state as self</returns>
    public IoResult<TA> Any<TA>() {
        return new IoResult<TA>() {
            Error = this.Error,
            IsOk = this.IsOk,
            Value = default
        };
    }
}