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
        {
            _entityClass = entityClass.AssemblyQualifiedName;
            _entityOid = entityOid;
            _version = version;
        }

        /// <summary>
        /// Returns the class of the entity that this reference refers to
        /// </summary>
        [DataMember]
        internal string Class
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
