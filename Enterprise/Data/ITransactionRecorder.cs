using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    public interface ITransactionRecorder
    {
        TransactionRecord CreateTransactionRecord(ICollection<EntityChange> changeSet);
    }
}
