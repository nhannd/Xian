using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// This helper class is used internally to aid in creation of generic entity refs
    /// </summary>
    internal abstract class EntityRefFactoryBase
    {
        public abstract EntityRefBase CreateReference(long oid, int version);
    }

    /// <summary>
    /// This helper class is used internally to aid in creation of generic entity refs
    /// </summary>
    internal class EntityRefFactory<TEntity> : EntityRefFactoryBase
        where TEntity : Entity
    {
        public EntityRefFactory()
        {
        }

        public override EntityRefBase CreateReference(long oid, int version)
        {
            return new EntityRef<TEntity>(oid, version);
        }
    }

    /// <summary>
    /// Provides a mechanism to refer to an entity at the enterprise level, independent of any 
    /// <see cref="IPersistenceContext"/> or instance of <see cref="Entity"/>
    /// </summary>
    /// <typeparam name="TEntity">The class of entity this reference refers to</typeparam>
    public class EntityRef<TEntity> : EntityRefBase
        where TEntity : Entity
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entity"></param>
        public EntityRef(TEntity entity)
            :this(entity.OID, entity.Version)
        {
        }

        /// <summary>
        /// Constructor - internal use
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="version"></param>
        internal EntityRef(long oid, int version)
            : base(typeof(TEntity), oid, version)
        {
        }

        /// <summary>
        /// Provide a string representatin of the reference
        /// </summary>
        /// <returns>Formatted string containing the type and OID of the referenced object</returns>
        public override string ToString()
        {
            return String.Format("Class: {0}, OID: {1}", this.EntityClass.ToString(), this.EntityOID.ToString());
        }
    }
}
