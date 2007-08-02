using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Database
{
    public interface IProcedureUpdateBroker<TInput> : IPersistenceBroker
        where TInput : ProcedureParameters
    {
        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        bool Execute(TInput criteria);
    }
}
