using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    public interface IValidation
    {
        void Add(IValidator validator);
        void Remove(IValidator validator);
        List<ValidationResult> GetResults();
        List<ValidationResult> GetResults(string propertyName);
    }
}
