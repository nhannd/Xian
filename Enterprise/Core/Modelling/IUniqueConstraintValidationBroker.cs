#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

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
        /// <param name="entityClass">The class of entity to which the constraint applies.</param>
        /// <param name="uniqueConstraintMembers">The properties of the object that form the unique key.
        /// These may be compound property expressions (e.g. Name.FirstName, Name.LastName).
        /// </param>
        /// <returns></returns>
        bool IsUnique(DomainObject obj, Type entityClass, string[] uniqueConstraintMembers);
    }
}
