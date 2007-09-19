using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    public class DefaultEntityValidator : IValidator
    {
        #region IEntityValidator Members

        public bool IsValidatable(DomainObject entity)
        {
            return true;
        }

        public bool IsValid(DomainObject entity, List<string> errors)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
