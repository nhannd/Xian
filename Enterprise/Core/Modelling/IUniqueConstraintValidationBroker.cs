using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// Defines the interface to a specialized broker that can validate a unique constraint for an entity.
    /// </summary>
    public interface IUniqueConstraintValidationBroker : IPersistenceBroker
    {
        /// <summary>
        /// Tests whether the specified object satisfies the specified unique constraint.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <param name="uniqueConstraintMembers">The properties of the object that form the unique key.
        /// These may be compound property expressions (e.g. Name.FirstName, Name.LastName).
        /// </param>
        /// <returns></returns>
        bool IsUnique(DomainObject obj, string[] uniqueConstraintMembers);
    }
}
