using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// This helper class is used internally to aid in creation of generic entity refs
    /// </summary>
    internal static class EntityRefFactory
    {
        public static EntityRefBase CreateReference(Type entityClass, object oid, int version)
        {
            Type typedEntityRef = typeof(EntityRef<>).MakeGenericType(new Type[] { entityClass });

            return (EntityRefBase)Activator.CreateInstance(
                typedEntityRef,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null,
                new object[] { oid, version },
                null);
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
        internal EntityRef(object oid, int version)
            : base(typeof(TEntity), oid, version)
        {
        }

        /// <summary>
        /// Cast this entity reference to a reference based on the specified entity class.  The assumption
        /// is that the specified class is either a superclass or a subclass of the entity class that this reference
        /// is based on.
        /// </summary>
        /// <param name="entityClass"></param>
        /// <returns></returns>
        public EntityRefBase Cast(Type entityClass)
        {
            return EntityRefFactory.CreateReference(entityClass, this.EntityOID, this.Version);
        }

        /// <summary>
        /// Cast this entity reference to a reference based on the specified entity class.  The assumption
        /// is that the specified class is either a superclass or a subclass of the entity class that this reference
        /// is based on.
        /// </summary>
        /// <returns></returns>
        public EntityRef<TCast> Cast<TCast>()
            where TCast : Entity
        {
            return new EntityRef<TCast>(this.EntityOID, this.Version);
        }

        /// <summary>
        /// Provide a string representation of the reference
        /// </summary>
        /// <returns>Formatted string containing the type and OID of the referenced object</returns>
        public override string ToString()
        {
            return String.Format("Class: {0}, OID: {1}", this.EntityClass.ToString(), this.EntityOID.ToString());
        }
    }
}
