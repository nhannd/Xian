using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Database.SqlServer2005
{
    public abstract class ProcedureUpdateBroker<TInput> : Broker, IProcedureUpdateBroker<TInput>
        where TInput : ProcedureSearchCriteria
    {
        private String _procedureName;

        protected ProcedureUpdateBroker(String procedureName)
        {
            _procedureName = procedureName;
        }

        #region IProcedureUpdateBroker<TInput> Members

        public bool Execute(TInput criteria)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Execute(TInput[] criteria)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
