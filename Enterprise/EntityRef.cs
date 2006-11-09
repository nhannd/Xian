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
        /// Checks whether this reference refers to the specified entity.  Note that the version is not
        /// considered in the comparison.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>True if this reference refers to the specified entity</returns>
        public bool RefersTo(TEntity entity)
        {
            // cannot include the Type in this comparison, because the entity in question may just be a proxy
            // rather than the real entity, however that shouldn't matter because the parameter is strongly typed

            // also cannot check version here, because if the entity is a proxy, the Version property will not
            // be initialized
            return entity.OID == this.EntityOID;
        }
    }
}
