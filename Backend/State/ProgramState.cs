namespace Schets.Backend.State; 

/// <summary>
/// The one true state of the program
/// </summary>
public static class ProgramState {
    /// <summary>
    /// The path to the template currently openened.
    /// </summary>
    public static string? OpenedFilePath { get; set; }
}