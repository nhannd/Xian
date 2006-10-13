using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.HL7;
using ClearCanvas.HL7.Brokers;

namespace ClearCanvas.Ris.Services
{
    [ExtensionOf(typeof(ClearCanvas.Enterprise.ServiceLayerExtensionPoint))]
    public class HL7QueueService : HL7ServiceLayer, IHL7QueueService
    {
        private readonly int numResults = 50;

        public HL7QueueService()
        {
        }

        #region IHL7QueueService Members

        [ReadOperation]
        public IList<HL7QueueItem> GetNextInboundHL7QueueItemBatch()
        {
            // Find the first pending item in the queue
            HL7QueueItemSearchCriteria criteria = new HL7QueueItemSearchCriteria();
            criteria.Status.Code.EqualTo(HL7MessageStatusCode.P);
            criteria.Status.CreationDateTime.SortAsc(0);

            SearchResultPage page = new SearchResultPage();
            page.FirstRow = 0;
            page.MaxRows = numResults;

            return this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Find(criteria, page);
        }

        [ReadOperation]
        public IList<HL7QueueItem> GetAllHL7QueueItems()
        {
            HL7QueueItemSearchCriteria criteria = new HL7QueueItemSearchCriteria();
            return this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Find(criteria);
        }

        [UpdateOperation]
        public void ProcessHL7QueueItem(ClearCanvas.HL7.HL7QueueItem item)
        {
            UpdateItemStatusHelper(item, HL7MessageStatusCode.C, null);
        }

        [UpdateOperation]
        public void EnqueueHL7QueueItem(ClearCanvas.HL7.HL7QueueItem item)
        {
            this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Store(item);
        }

        [UpdateOperation]
        public void SyncExternalQueue()
        {
            IList<HL7ExternalQueueItem> toBeSynched = this.CurrentContext.GetBroker<IHL7ExternalQueueItemBroker>().GetUnsynchedItems();
            foreach (HL7ExternalQueueItem externalQueueItem in toBeSynched)
            {
                HL7QueueItem queueItem = externalQueueItem.GetHL7QueueItem();
                this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Store(queueItem);
                this.CurrentContext.GetBroker<IHL7ExternalQueueItemBroker>().FlagItemAsSynched(externalQueueItem.Guid);
            }
        }

        #endregion

        private void UpdateItemStatusHelper(ClearCanvas.HL7.HL7QueueItem item, ClearCanvas.HL7.HL7MessageStatusCode status, string statusDescription)
        {
            if (item == null) return;

            HL7QueueItem reloaded = this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Find(item.OID);

            if (reloaded != null)
            {
                reloaded.Status.Code = status;

                if (statusDescription != null)
                {
                    reloaded.Status.Description = statusDescription;
                }

                reloaded.Status.UpdateDateTime = Platform.Time;

                this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Store(reloaded);
            }
        }

    }
}
