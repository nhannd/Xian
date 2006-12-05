using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{
    public class ValidationSystem : IValidation
    {
        private List<IValidator> _validators;

        public ValidationSystem()
        {
            _validators = new List<IValidator>();
        }

        public void Add(IValidator validator)
        {
            _validators.Add(validator);
        }

        public void Remove(IValidator validator)
        {
            _validators.Remove(validator);
        }

        public List<ValidationResult> GetResults()
        {
            return GetResults(_validators);
        }

        public List<ValidationResult> GetResults(string propertyName)
        {
            return GetResults(_validators.FindAll(delegate(IValidator v) { return v.PropertyName == propertyName; }));
        }

        private List<ValidationResult> GetResults(List<IValidator> validators)
        {
            return validators.ConvertAll<ValidationResult>(delegate(IValidator v) { return v.Result; });
        }
    }
}
