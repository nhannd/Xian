using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    public interface IExceptionLogger
    {
        ExceptionLogEntry CreateExceptionLogEntry(Exception e);
    }
}
