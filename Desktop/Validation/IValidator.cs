using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    public interface IValidator
    {
        string PropertyName { get; }
        ValidationResult Result { get; }
    }
}
