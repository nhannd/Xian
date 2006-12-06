using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    public interface IValidationRule
    {
        string PropertyName { get; }
        ValidationResult Result { get; }
    }
}
