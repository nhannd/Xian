using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Database
{
    public delegate void ProcedureSelectCallback<T>(T row);

    /// <summary>
    /// Interface used to define stored procedures that that input parameters and return resultant rows.
    /// </summary>
    /// <typeparam name="TInput">Input parameters</typeparam>
    /// <typeparam name="TOutput">The return type</typeparam>
    public interface IProcedureSelectBroker<TInput, TOutput> : IPersistenceBroker
        where TInput : ProcedureParameters
        where TOutput : ServerEntity, new()
    {
        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<TOutput> Execute(TInput criteria);

        /// <summary>
        /// Retrieves all entities matching the specified criteria,
        /// constrained by the specified page constraint.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        void Execute(TInput criteria, SelectCallback<TOutput> callback);

    }
}
