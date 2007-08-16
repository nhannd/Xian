using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Database
{
    /// <summary>
    /// Interface for retrieving enumerated values from the database.
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    public interface IEnumBroker<TOutput> : IPersistenceBroker
        where TOutput : ServerEnum, new()
    {
        /// <summary>
        /// Retrieves all enums.
        /// </summary>
        /// <returns></returns>
        IList<TOutput> Execute();
    }
}
