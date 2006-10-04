using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    public abstract class ValidationAttribute : Attribute
    {
        public abstract IValidator CreateValidator(TestValueCallbackDelegate testValueCallback);
    }
}
