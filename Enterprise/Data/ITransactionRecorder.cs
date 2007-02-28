using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Data
{
    public interface ITransactionRecorder
    {
        TransactionRecord CreateTransactionRecord(ICollection<EntityChange> changeSet);
    }
}
