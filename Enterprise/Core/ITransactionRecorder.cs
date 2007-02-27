using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    public interface ITransactionRecorder
    {
        TransactionRecord CreateTransactionRecord(ICollection<EntityChange> changeSet);
    }
}
