using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    public delegate void SelectCallback<T>(T row);

    public interface IEntityBroker<TOutput, TInput, TUpdateBrokerParameters> : IPersistenceBroker
        where TInput : EntitySelectCriteria
        where TUpdateBrokerParameters : EntityUpdateColumns
        where TOutput : ServerEntity, new()
    {
        /// <summary>
        /// Loads the entity referred to by the specified entity reference.
        /// </summary>
        /// <param name="entityRef"></param>
        /// <returns></returns>
        TOutput Load(ServerEntityKey entityRef);

        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<TOutput> Find(TInput criteria);

        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        void Find(TInput criteria, SelectCallback<TOutput> callback);

        /// <summary>
        /// Retrieves a count of the entities within the persistent store that
        /// match the specified criteria.
        /// </summary>
        /// <param name="criteria">The input criteria</param>
        /// <returns>The number or resultant rows.</returns>
        int Count(TInput criteria);

        /// <summary>
        /// Updates the entity specified by the <paramref name="entityKey"/> with values specified in <paramref="parameters"/>.
        /// </summary>
        /// <param name="entityKey">The <see cref="ServerEntityKey"/> object whose <see cref="ServerEntityKey.Key"/> references to the object to be updated.</param>
        /// <param name="parameters">The <see cref="UpdateBrokerParameters"/> specifying the columns to be updated.</param>
        /// <returns></returns>
        bool Update(ServerEntityKey entityKey, TUpdateBrokerParameters parameters);

        /// <summary>
        /// Inserts a new entity with field values indicated in <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters">The <see cref="UpdateBrokerParameters"/> object which specifies the values for the columns in the new entity.</param>
        /// <returns>References to the newly inserted entity.</returns>
        TOutput Insert(TUpdateBrokerParameters parameters);

        /// <summary>
        /// Delete an entity.
        /// </summary>
        /// <param name="entityKey">The key for the entity to delete.</param>
        /// <returns>true on success, false on failure</returns>
        bool Delete(ServerEntityKey entityKey);
    }
}
