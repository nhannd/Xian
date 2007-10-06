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
    /// 
    // TH (Oct 5, 2007): All entity objects should be serializable to use in ASP.NET app
    // All parent classes must be serializable
    [Serializable] 
    public abstract class Entity : DomainObject
    {
        private object _oid;
        private int _version;

        public Entity()
        {
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
        /// Gets a <see cref="EntityRef"/> that represents this entity.
        /// </summary>
        /// <returns></returns>
        public virtual EntityRef GetRef()
        {
            if (_oid == null)
                throw new InvalidOperationException("Cannot generate entity ref on transient entity");

            return new EntityRef(GetClass(), _oid, _version);
        }


        /// <summary>
        /// Performs a downcast on this object to the specified subclass type.  If this object is a proxy,
        /// a regular C# downcast operation will fail.  Therefore, application code should always use this method
        /// to perform a safe downcast.
        /// </summary>
        /// <typeparam name="TSubclass"></typeparam>
        /// <returns></returns>
        public TSubclass Downcast<TSubclass>()
            where TSubclass : Entity
        {
            return (TSubclass)GetRawInstance();
        }

        /// <summary>
        /// Subsitute for the C# 'is' operator.  If this object is a proxy, the C# 'is' operator will never
        /// return true.  Therefore, application code must use this method instead.
        /// </summary>
        /// <typeparam name="TSubclass"></typeparam>
        /// <returns></returns>
        public bool Is<TSubclass>()
            where TSubclass : Entity
        {
            return GetRawInstance() is TSubclass;
        }

        /// <summary>
        /// Subsitute for the C# 'as' operator.  If this object is a proxy, the C# 'as' operator will fail.
        /// Therefore, application code must use this method instead.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public TSubclass As<TSubclass>()
            where TSubclass : Entity
        {
            return GetRawInstance() as TSubclass;
        }
    }
}
