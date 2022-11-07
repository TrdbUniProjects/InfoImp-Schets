using System;

namespace Schets.Backend.Exceptions; 

public class CaseNotImplementedException : Exception {
    public CaseNotImplementedException(string msg) : base(msg) {}
}