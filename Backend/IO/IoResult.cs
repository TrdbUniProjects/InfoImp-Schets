namespace Schets.Backend.IO; 

public class IoResult<T> {

    public T? Value { get; private init; }
    public bool IsOk { get; private init; }
    public string? Error { get; private init;  }

    private IoResult() {}

    public static IoResult<T> Ok(T? value) {
        return new IoResult<T>() {
            IsOk = true,
            Value = value
        };
    }

    public static IoResult<T> Fail(string error) {
        return new IoResult<T>() {
            Error = error,
            IsOk = false
        };
    }

    public IoResult<TA> Any<TA>() {
        return new IoResult<TA>() {
            Error = this.Error,
            IsOk = this.IsOk,
            Value = default
        };
    }
}