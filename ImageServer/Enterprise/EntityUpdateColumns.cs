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

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// Abstract base class to store a collection of update columns to be used in in a 
    /// non-procedural broker implementing the 
    /// <see cref="IEntityBroker{TServerEntity,TSelectCriteria,TUpdateColumns}"/> interface.
    /// </summary>
    /// <remark>
    /// Each updatable field in the parameter collection is an element of <see cref="SubParameters"/>.
    /// </remark>
    abstract public class EntityUpdateColumns
    {
        #region Private Members

        private readonly string _entityName;       
        private readonly Dictionary<string, EntityColumnBase> _subParameters = new Dictionary<string, EntityColumnBase>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entityName">The name of the <see cref="ServerEntity"/>.</param>
        public EntityUpdateColumns(string entityName)
        {
            _entityName = entityName;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Returns the list of sub-parameters
        /// </summary>
        public IDictionary<string, EntityColumnBase> SubParameters
        {
            get { return _subParameters; }
        }
        
        public virtual bool IsEmpty
        {
            get
            {
                if (_subParameters.Values.Count > 0)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Gets the key corresponding to the parameter/field to be updated.
        /// </summary>
        public String FieldName
        {
            get { return _entityName; }
        }

        #endregion
    }
}
