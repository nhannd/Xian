using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Abstract base class for all entities in the domain model.
    /// </summary>
    public abstract class Entity
    {
        private long _oid;
        private int _version;

        /// <summary>
        /// OID is short for Object ID, and is used to store the surrogate key that uniquely identifies the 
        /// object in the database.  Client code can read this value but cannot set it.
        /// </summary>
        public virtual long OID
        {
            get { return _oid; }
            private set { _oid = value; }
        }

        /// <summary>
        /// Keeps track of the object version for optimistic concurrency.  Not used by client code.
        /// </summary>
        protected virtual int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        /// Returns true if this entity has never been persisted
        /// </summary>
        public bool IsNew
        {
            get { return this.OID == 0; }
        }
    }
}
