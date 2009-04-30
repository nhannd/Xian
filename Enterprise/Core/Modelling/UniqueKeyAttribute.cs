#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
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
