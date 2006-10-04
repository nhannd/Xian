using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    public class ValidatorGroup : IValidator
    {
        private List<IValidator> _validators;

        public ValidatorGroup(IEnumerable<IValidator> validators)
        {
            _validators = new List<IValidator>();
            _validators.AddRange(validators);
        }

        public void Add(IValidator validator)
        {
            _validators.Add(validator);
        }


        #region IValidator Members

        public ValidationResult Result
        {
            get
            {
                bool failure = false;
                List<string> messages = new List<string>();
                foreach (IValidator validator in _validators)
                {
                    ValidationResult result = validator.Result;
                    if (!result.IsValid)
                    {
                        failure = true;
                        messages.AddRange(result.Messages);
                    }
                }

                return new ValidationResult(!failure, messages.ToArray());
            }
        }

        #endregion
    }
}
