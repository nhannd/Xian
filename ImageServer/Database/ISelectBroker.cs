using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Database
{
    public delegate void SelectCallback<T>(T row);

    public interface ISelectBroker<TInput, TOutput> : IPersistenceBroker
        where TInput : SelectCriteria
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

    }
}
