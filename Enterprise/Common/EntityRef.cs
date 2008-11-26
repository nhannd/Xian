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
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Abstract base class for <see cref="EntityRef"/>
    /// </summary>
    [DataContract]
    public class EntityRef : IVersionedEquatable<EntityRef>
    {
		/// <summary>
		/// Provides a null-safe means of checking for equality, optionally ignoring the version.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="ignoreVersion"></param>
		/// <returns></returns>
		public static bool Equals(EntityRef x, EntityRef y, bool ignoreVersion)
		{
			if (ReferenceEquals(x, y))
				return true;
			if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
				return false;

			return x.Equals(y, ignoreVersion);
		}


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
		/// Deserialization constructor
		/// </summary>
		/// <param name="value">The serialized EntityRef value.</param>
		public EntityRef(string value)
		{
			Deserialize(value);
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

        /// <summary>
        /// Compares two instances of this class for value-based equality, including
        /// the version in the comparison.  To exclude version in the comparison,
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
        /// Overridden to comply with <see cref="Equals"/>.  Version is included in the hashcode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _entityOid.GetHashCode() ^ _entityClass.GetHashCode() ^ _version.GetHashCode();
        }


        /// <summary>
        /// Provide a string representation of the reference.
        /// </summary>
        /// <returns>Formatted string containing the type, OID and version of the referenced object</returns>
        public override string ToString()
        {
            return this.ToString(true, true);
        }

        /// <summary>
        /// Provide a string representation of the reference.
        /// </summary>
        /// <param name="includeVersion"></param>
        /// <returns>Formatted string containing the type, OID and version of the referenced object</returns>
        public string ToString(bool includeVersion)
        {
            return ToString(includeVersion, true);
        }

        /// <summary>
        /// Provide a string representation of the reference.
        /// </summary>
        /// <param name="includeVersion"></param>
        /// <param name="includeEntityClass"></param>
        /// <returns>Formatted string containing the type, OID and version of the referenced object</returns>
        public string ToString(bool includeVersion, bool includeEntityClass)
        {
            if (includeVersion)
            {
                if (includeEntityClass)
                    return String.Format("{0}/{1}/{2}", _entityClass.ToString(), _entityOid.ToString(), _version.ToString());
                else
                    return String.Format("{0}/{1}", _entityOid.ToString(), _version.ToString());
            }
            else
            {
                if (includeEntityClass)
                    return String.Format("{0}/{1}", _entityClass.ToString(), _entityOid.ToString());
                else
                    return _entityOid.ToString();
            }
        }

        /// <summary>
        /// Compares instances of this class based on value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(EntityRef x, EntityRef y)
        {
            // check if they are the same instance, or both null
            if (ReferenceEquals(x, y))
                return true;

            // if either one is null then they can't be equal
            if ((x as object) == null || (y as object) == null)
                return false;

            // compare fields
            return x.Equals(y);
        }

        /// <summary>
        /// Compares instances of this class based on value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(EntityRef x, EntityRef y)
        {
            return !(x == y);
		}

		#region Serialization / Deserialization Helper

		public string Serialize()
		{
			return string.Format("{0}:{1}:{2}:{3}",
				EntityRefUtils.GetClassName(this),
				EntityRefUtils.GetOID(this).GetType().AssemblyQualifiedName,
				EntityRefUtils.GetOID(this),
				EntityRefUtils.GetVersion(this));
		}

		private void Deserialize(string value)
		{
			Platform.CheckForNullReference(value, "value");

			string[] parts = value.Split(':');
			if (parts.Length != 4)
				throw new SerializationException("Invalid EntityRef string");

			string entityClassName = parts[0];
			Type oidType = Type.GetType(parts[1], true);
			string oidValue = parts[2];
			int version = int.Parse(parts[3]);

			object oid = null;
			if (oidType == typeof(int))
			{
				oid = int.Parse(oidValue);
			}
			else if (oidType == typeof(long))
			{
				oid = long.Parse(oidValue);
			}
			else if (oidType == typeof(string))
			{
				oid = oidValue;
			}
			else if (oidType == typeof(Guid))
			{
				oid = new Guid(oidValue);
			}
			else
				throw new SerializationException("Invalid EntityRef string");

			this.ClassName = entityClassName;
			this.OID = oid;
			this.Version = version;
		}

		#endregion

		#region IVersionedEquatable

		/// <summary>
		/// Compares two instances of this class for value-based equality.  If <paramref name="ignoreVersion"/>
		/// is true, the version will not be considered in the comparison.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="ignoreVersion"></param>
		/// <returns></returns>
		public bool Equals(object obj, bool ignoreVersion)
		{
			EntityRef that = obj as EntityRef;
			return Equals(that, ignoreVersion);
		}

		#endregion

		#region IVersionedEquatable<EntityRef> Members

		public bool Equals(EntityRef other, bool ignoreVersion)
		{
			if (other == null)
				return false;

			// compare fields
			return this._entityOid.Equals(other._entityOid)
				&& this._entityClass.Equals(other._entityClass)
				&& (ignoreVersion || this._version.Equals(other._version));
		}

		#endregion

		#region IEquatable<EntityRef> Members

		public bool Equals(EntityRef other)
		{
			return Equals(other, false);
		}

		#endregion
	}
}
