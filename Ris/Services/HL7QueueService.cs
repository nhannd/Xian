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
        public IList<HL7QueueItem> GetNextInboundItemBatch()
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
        public IList<HL7QueueItem> GetAllItems()
        {
            HL7QueueItemSearchCriteria criteria = new HL7QueueItemSearchCriteria();
            return this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Find(criteria);
        }
     
        [UpdateOperation]
        public void UpdateItemStatus(ClearCanvas.HL7.HL7QueueItem item, ClearCanvas.HL7.HL7MessageStatusCode status)
        {
            UpdateItemStatusHelper(item, status, null);
        }

        [UpdateOperation]
        public void UpdateItemStatus(ClearCanvas.HL7.HL7QueueItem item, ClearCanvas.HL7.HL7MessageStatusCode status, string statusDescription)
        {
            UpdateItemStatusHelper(item, status, statusDescription);
        }

        //[UpdateOperation]
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

        [UpdateOperation]
        public void EnqueueItem(ClearCanvas.HL7.HL7QueueItem item)
        {
            this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Store(item);
        }

        #endregion
    }
}
