using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    public class DefaultExceptionLogger : IExceptionLogger
    {
        public DefaultExceptionLogger()
        {

        }

        #region IExceptionLogger Members

        public ExceptionLogEntry CreateExceptionLogEntry(Exception e)
        {
            // TODO: create details XML
            return new ExceptionLogEntry(e, null);
        }

        #endregion
    }
}
