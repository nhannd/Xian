using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Database.SqlServer2005
{
    public abstract class ProcedureEntityReadBroker<TOutput> : Broker, IProcedureEntityReadBroker<TOutput>
        where TOutput : ProcedureEntity
    {
        #region IProcedureEntityReadBroker<TOutput> Members

        public IList<TOutput> Execute()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Execute(ReadCallback callback)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
