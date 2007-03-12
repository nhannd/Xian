using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Abstract base class for all entities in the domain model.
    /// </summary>
    public abstract class Entity : DomainObject
    {
        private object _oid;
        private int _version;
        private Type _entityClass;

        public Entity()
        {
            _entityClass = this.GetType();
        }

        /// <summary>
        /// OID is short for object identifier, and is used to store the surrogate key that uniquely identifies the 
        /// object in the database.  This property is public for compatibility with NHibernate proxies.  It should
        /// not be used by application code.
        /// </summary>
        public virtual object OID
        {
            get { return _oid; }
            private set { _oid = value; }
        }

        /// <summary>
        /// Keeps track of the object version for optimistic concurrency.  This property is public for compatibility
        /// with NHibernate proxies.  It should not be used by application code.
        /// </summary>
        public virtual int Version
        {
            get { return _version; }
            private set { _version = value; }
        }

        /// <summary>
        /// Gets the class of this entity.  Note that the class of this entity is not necessarily the same as the
        /// type of this object, because this object may be an NHibernate proxy.  Therefore, use this method rather
        /// than <see cref="GetType"/>.
        /// </summary>
        /// <returns></returns>
        public virtual Type GetClass()
        {
            return _entityClass;
        }

        /// <summary>
        /// Gets a <see cref="EntityRef"/> that represents this entity.
        /// </summary>
        /// <returns></returns>
        public virtual EntityRef GetRef()
        {
            if (_oid == null)
                throw new InvalidOperationException("Cannot generate entity ref on transient entity");

            return new EntityRef(_entityClass, _oid, _version);
        }
    }
}
