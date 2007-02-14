using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Abstract base class for all entities in the domain model.
    /// </summary>
    public abstract class Entity : DomainObject
    {
        private object _oid;
        private int _version;

        /// <summary>
        /// OID is short for object identifier, and is used to store the surrogate key that uniquely identifies the 
        /// object in the database.  This property is public for compatibility with NHibernate proxies.  It should
        /// not be used by application code.
        /// </summary>
        [DataMember]
        public virtual object OID
        {
            get { return _oid; }
            private set { _oid = value; }
        }

        /// <summary>
        /// Keeps track of the object version for optimistic concurrency.  This property is public for compatibility
        /// with NHibernate proxies.  It should not be used by application code.
        /// </summary>
        [DataMember]
        public virtual int Version
        {
            get { return _version; }
            private set { _version = value; }
        }
    }
}
