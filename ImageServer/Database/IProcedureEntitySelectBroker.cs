using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Database
{
    public delegate void SelectCallback<T>(T row);

    public interface IProcedureEntitySelectBroker<TInput, TOutput> : IPersistenceBroker
        where TInput : ProcedureSearchCriteria
        where TOutput : ProcedureEntity, new()
    {
        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<TOutput> Execute(TInput criteria);

        /// <summary>
        /// Retrieves all entities matching any of the specified criteria (the criteria are combined using OR).
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<TOutput> Execute(TInput[] criteria);

        /// <summary>
        /// Retrieves all entities matching the specified criteria,
        /// constrained by the specified page constraint.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        void Execute(TInput criteria, SelectCallback<TOutput> callback);

        /// <summary>
        /// Retrieves all entities matching any of the specified criteria (the criteria are combined using OR),
        /// constrained by the specified page constraint.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        void Execute(TInput[] criteria, SelectCallback<TOutput> page);

    }
}
