using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Database
{
    public delegate void ReadCallback<T>(T row);

    public interface IProcedureEntityReadBroker<TOutput> : IPersistenceBroker
        where TOutput : ProcedureEntity, new()
    {
        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<TOutput> Execute();

        /// <summary>
        /// Retrieves all entities matching the specified criteria,
        /// constrained by the specified page constraint.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        void Execute(ReadCallback<TOutput> callback);
    }
}
