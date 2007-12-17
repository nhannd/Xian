#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Used by class <see cref="EntityChange"/> to record the type of change made to an entity.
    /// </summary>
    [Serializable]
    public enum EntityChangeType
    {
        Update = 0,
        Create = 1,
        Delete = 2
    }
    
    /// <summary>
    /// Acts as a memento of a change made to an entity.
    /// </summary>
    [DataContract]
    public class EntityChange
    {
        private EntityRef _entityRef;
        private EntityChangeType _changeType;

        /// <summary>
        /// Constructor
        /// </summary>
        public EntityChange(EntityRef entityRef, EntityChangeType changeType)
        {
            _entityRef = entityRef;
            _changeType = changeType;
        }

        /// <summary>
        /// The type of change
        /// </summary>
        [DataMember]
        public EntityChangeType ChangeType
        {
            get { return _changeType; }
        }

        /// <summary>
        /// Reference to the entity that changed
        /// </summary>
        [DataMember]
        public EntityRef EntityRef
        {
            get { return _entityRef; }
        }

        /// <summary>
        /// Checks whether this change supercedes the specified other change.  This change supercedes other iff
        /// the version of this change is greater than the version of the other change, the version of this change
        /// is the same as the version of the other change and the <see cref="ChangeType"/>
        /// of this change supercedes the <see cref="ChangeType"/> of the other.  Note that
        /// <see cref="EntityChangeType.Create"/> supercedes <see cref="EntityChangeType.Update"/>, and 
        /// <see cref="EntityChangeType.Delete"/> supercedes both (e.g. a Create followed by an update is fundamentally a Create, and
        /// a Create or Update followed by a Delete is fundamentally a Delete).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Supercedes(EntityChange other)
        {
            if (!_entityRef.ClassName.Equals(other._entityRef.ClassName) || !_entityRef.OID.Equals(other._entityRef.OID))
                throw new ArgumentException("Argument must represent a change to the same entity");

            return _entityRef.Version > other._entityRef.Version || (_entityRef.Version == other._entityRef.Version && _changeType > other._changeType);
        }
    }
}
