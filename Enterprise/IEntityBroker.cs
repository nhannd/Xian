using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Base interface for all entity broker interfaces.  An entity broker manages the movement of a
    /// particular domain entity class to and from a persistent store.
    /// 
    /// This interface should not be implemented directly.
    /// Instead, a sub-interface should be defined that extends this interface for a given entity.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="Entity"/> sub-class on which this broker acts</typeparam>
    /// <typeparam name="TSearchCriteria">The <see cref="SearchCriteria"/> subclass corresponding to the entity</typeparam>
    public interface IEntityBroker<TEntity, TSearchCriteria> : IPersistenceBroker
        where TEntity: Entity
        where TSearchCriteria : SearchCriteria
    {
        /// <summary>
        /// Retrieves the specified entity from the persistent store.
        /// </summary>
        /// <param name="oid">The object ID of the entity to retrieve</param>
        /// <returns>The entity instance</returns>
        TEntity Find(long oid);

        /// <summary>
        /// Retrieves a list of all entities matching the specified criteria.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<TEntity> Find(TSearchCriteria criteria);

        /// <summary>
        /// Returns the number of entities matching the specified criteria.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        long Count(TSearchCriteria criteria);

        /// <summary>
        /// Forces loading of a lazy property of the specified entity.
        /// </summary>
        /// <param name="entity">The entity on which to act</param>
        /// <param name="property">The lazy property of the entity to load</param>
        void LoadRelated(TEntity entity, object property);

        /// <summary>
        /// Stores the given entity instance in the persistent store.  If the specified entity is transient,
        /// it will be made persistent (added to the persistent store).  If the specified entity is already persistent,
        /// the persistent store will be updated.
        /// </summary>
        /// <param name="entity">The entity instance to store</param>
        void Store(TEntity entity);

        /// <summary>
        /// Makes the specified entity transient (removes it from the persistent store).
        /// </summary>
        /// <param name="entity">The entity instance to remove</param>
        void Delete(TEntity entity);
    }
}
