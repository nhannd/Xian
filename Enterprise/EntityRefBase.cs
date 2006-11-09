using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    public abstract class EntityRefBase
    {
        private Type _entityClass;
        private long _entityOid;
        private int _version;

        /// <summary>
        /// Constructs an instance of this class
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="entityOid"></param>
        /// <param name="version"></param>
        protected EntityRefBase(Type entityType, long entityOid, int version)
        {
            _entityClass = entityType;
            _entityOid = entityOid;
            _version = version;
        }

        /// <summary>
        /// Returns the class of the entity that this reference refers to
        /// </summary>
        internal Type EntityClass
        {
            get { return _entityClass; }
        }

        /// <summary>
        /// Returns the OID that this reference refers to
        /// </summary>
        internal long EntityOID
        {
            get { return _entityOid; }
        }

        /// <summary>
        /// Returns the version of the entity that this reference refers to
        /// </summary>
        internal int Version
        {
            get { return _version; }
        }

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
            return entity.OID == this.EntityOID;
        }
        
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
            EntityRefBase that = obj as EntityRefBase;
            if (that == null)
                return false;

            // compare fields
            return this._entityOid == that._entityOid
                && this._entityClass == that._entityClass
                && (!versionStrict || this._version == that._version);
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

        public override int GetHashCode()
        {
            return _entityOid.GetHashCode() ^ _version.GetHashCode() ^ _entityClass.GetHashCode();
        }

        /// <summary>
        /// Compares instances of this class based on value. Entity type and OID are considered,
        /// but version is ignored.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(EntityRefBase x, EntityRefBase y)
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
        public static bool operator !=(EntityRefBase x, EntityRefBase y)
        {
            return !(x == y);
        }
    }
}
