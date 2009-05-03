using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise.SqlServer2005;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.EntityBrokers
{
    
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class DuplicateSopEntryEntityBroker : EntityBroker<DuplicateSopReceivedQueue, DuplicateSopReceivedQueueSelectCriteria, DuplicateSopReceivedQueueUpdateColumns>, IDuplicateSopEntryEntityBroker
    {
        public DuplicateSopEntryEntityBroker()
            : base("StudyIntegrityQueue")
        { }
    
    }
}
