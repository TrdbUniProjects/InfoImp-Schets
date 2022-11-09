using System;

namespace Schets.Backend.Exceptions; 

/// <summary>
/// Exception thrown when a switch case is not implemented,
/// either on purpose or because the unimplemented case was added afterwqrds.
/// </summary>
public class CaseNotImplementedException : Exception {
    public CaseNotImplementedException(string msg) : base(msg) {}
}