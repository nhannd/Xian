using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    public interface IValidator
    {
        ValidationResult Result { get; }
    }
}
