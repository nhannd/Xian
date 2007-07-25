using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Database.SqlServer2005
{
    public abstract class ProcedureEntitySelectBroker<TInput,TOutput> : Broker,IProcedureEntitySelectBroker<TInput,TOutput>
        where TInput : ProcedureSearchCriteria
        where TOutput : ProcedureEntity
    {
        #region IProcedureEntitySelectBroker<TInput,TOutput> Members

        public IList<TOutput> Execute(TInput criteria)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IList<TOutput> Execute(TInput[] criteria)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Execute(TInput criteria, SelectCallback callback)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Execute(TInput[] criteria, SelectCallback page)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
