#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// When applied to an entity class, indicates that a specified set of properties on the class
    /// form a unique key for instances of that class within the set of persistent instances.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class UniqueKeyAttribute : Attribute
    {
        private readonly string[] _memberProperties;
        private readonly string _logicalName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logicalName">The logical name of the key.</param>
        /// <param name="memberProperties">
        /// An array of property names that form the unique key for the class.  For example, a Person class
        /// might have a unique key consisting of "FirstName" and "LastName" properties.  Note that compound
        /// property expressions may be used, e.g. for a Person class with a Name property that itself has First
        /// and Last properties, the unique key members might be "Name.First" and "Name.Last".
        /// </param>
        public UniqueKeyAttribute(string logicalName, string[] memberProperties)
        {
            _logicalName = logicalName;
            _memberProperties = memberProperties;
        }

        public string[] MemberProperties
        {
            get { return _memberProperties; }
        }

        public string LogicalName
        {
            get { return _logicalName; }
        }
    }
}
