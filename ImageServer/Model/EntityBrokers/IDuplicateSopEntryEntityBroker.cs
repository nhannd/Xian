using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    public interface IDuplicateSopEntryEntityBroker : IEntityBroker<DuplicateSopReceivedQueue, DuplicateSopReceivedQueueSelectCriteria, DuplicateSopReceivedQueueUpdateColumns>
    {
    }
}
