using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    public interface IValidator
    {
        bool IsValidatable(DomainObject obj);
        bool IsValid(DomainObject obj, List<string> errors);
    }
}
