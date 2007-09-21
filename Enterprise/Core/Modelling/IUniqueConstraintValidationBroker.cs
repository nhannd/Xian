using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public interface IUniqueConstraintValidationBroker : IPersistenceBroker
    {
        bool IsUnique(DomainObject obj, string[] uniqueConstraintMembers);
    }
}
