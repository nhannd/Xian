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
    /// Abstract base class for <see cref="EntityRef"/>
    /// </summary>
    [DataContract]
    public class EntityRef
    {
        private string _entityClass;
        private object _entityOid;
        private int _version;

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        private EntityRef()
        {

        }

        /// <summary>
        /// Constructs an instance of this class
        /// </summary>
        /// <param name="entityClass"></param>
        /// <param name="entityOid"></param>
        /// <param name="version"></param>
        public EntityRef(Type entityClass, object entityOid, int version)
            :this(entityClass.AssemblyQualifiedName, entityOid, version)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityClassName">The assembly-qualified class name of the entity class.</param>
        /// <param name="entityOid"></param>
        /// <param name="version"></param>
        public EntityRef(string entityClassName, object entityOid, int version)
        {
            _entityClass = entityClassName;
            _entityOid = entityOid;
            _version = version;
        }

        /// <summary>
        /// Returns the class of the entity that this reference refers to
        /// </summary>
        [DataMember]
        internal string ClassName
        {
            get { return _entityClass; }
            private set { _entityClass = value; }
        }

        /// <summary>
        /// Returns the OID that this reference refers to
        /// </summary>
        [DataMember]
        internal object OID
        {
            get { return _entityOid; }
            private set { _entityOid = value; }
        }

        /// <summary>
        /// Returns the version of the entity that this reference refers to
        /// </summary>
        [DataMember]
        internal int Version
        {
            get { return _version; }
            private set { _version = value; }
        }
/*
        /// <summary>
        /// Checks whether this reference refers to the specified entity.  The type of the entity is not considered
        /// in the comparison, but this should not pose a problem assuming OIDs are unique across all entities.
        /// Note as well that the version is not considered in the comparison.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>True if this reference refers to the specified entity</returns>
        public bool RefersTo(Entity entity)
        {
            // cannot include the Type in this comparison, because the entity in question may just be a proxy
            // rather than the real entity, however that shouldn't matter because the parameter is strongly typed

            // also cannot check version here, because if the entity is a proxy, the Version property will not
            // be initialized
            return entity != null && entity.OID.Equals(this.EntityOID);
        }
*/        
        /// <summary>
        /// Compares two instances of this class for value-based equality.  If versionStrict is true, the
        /// version will be considered in the comparison, otherwise it will be ignored.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="versionStrict"></param>
        /// <returns></returns>
        public bool Equals(object obj, bool versionStrict)
        {
            // if null then they can't be equal
            EntityRef that = obj as EntityRef;
            if (that == null)
                return false;

            // compare fields
            return this._entityOid.Equals(that._entityOid)
                && this._entityClass.Equals(that._entityClass)
                && (!versionStrict || this._version.Equals(that._version));
        }

        /// <summary>
        /// Compares two instances of this class for value-based equality.  Note that by default,
        /// the version is not considered in the comparison.  To include version in the comparison,
        /// call the <see cref="Equals"/> overload that accepts a flag indicating whether to include
        /// version in the comparison.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj, false);
        }

        /// <summary>
        /// Overridden comply with <see cref="Equals"/>.  Note that the version is not considered
        /// in the hash-code computation.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _entityOid.GetHashCode() ^ _entityClass.GetHashCode();
        }


        /// <summary>
        /// Provide a string representation of the reference.
        /// </summary>
        /// <returns>Formatted string containing the type and OID of the referenced object</returns>
        public override string ToString()
        {
            return this.ToString(false);
        }

        /// <summary>
        /// Provide a string representation of the reference.
        /// </summary>
        /// <param name="excludeVersion"></param>
        /// <returns>Formatted string containing the type and OID of the referenced object</returns>
        public string ToString(bool excludeVersion)
        {
            if (excludeVersion)
                return String.Format("{0}/{1}", _entityClass.ToString(), _entityOid.ToString());
            else
                return String.Format("{0}/{1}/{2}", _entityClass.ToString(), _entityOid.ToString(), _version.ToString());
        }

        /// <summary>
        /// Compares instances of this class based on value. Entity type and OID are considered,
        /// but version is ignored.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(EntityRef x, EntityRef y)
        {
            // check if they are the same instance, or both null
            if (Object.ReferenceEquals(x, y))
                return true;

            // if either one is null then they can't be equal
            if ((x as object) == null || (y as object) == null)
                return false;

            // compare fields
            return x.Equals(y);
        }

        /// <summary>
        /// Compares instances of this class based on value. Entity type and OID are considered,
        /// but version is ignored.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(EntityRef x, EntityRef y)
        {
            return !(x == y);
        }
    }
}
