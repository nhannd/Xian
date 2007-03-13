using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.HL7;
using ClearCanvas.Ris.Application.Common.Admin.HL7Admin;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    class HL7QueueItemAssembler
    {
        public HL7QueueItemSummary CreateHL7QueueItemSummary(HL7QueueItem queueItem, IPersistenceContext context)
        {
            HL7QueueItemSummary summary = new HL7QueueItemSummary();

            summary.Direction = context.GetBroker<HL7MessageDirectionEnumTable>()[queueItem.Direction];

            summary.StatusCode = context.GetBroker<HL7MessageStatusCodeEnumTable>()[queueItem.Status.Code];
            summary.StatusDescription = queueItem.Status.Description;
            summary.CreationDateTime = queueItem.Status.CreationDateTime;
            summary.UpdateDateTime = queueItem.Status.UpdateDateTime;

            summary.Peer = context.GetBroker<HL7MessagePeerEnumTable>()[queueItem.Message.Peer];
            summary.MessageType = queueItem.Message.MessageType;
            summary.MessageEvent = queueItem.Message.Event;
            summary.MessageVersion = context.GetBroker<HL7MessageVersionEnumTable>()[queueItem.Message.Version];
            summary.MessageFormat = context.GetBroker<HL7MessageFormatEnumTable>()[queueItem.Message.Format];

            return summary;
        }

        public HL7QueueItemDetail CreateHL7QueueItemDetail(HL7QueueItem queueItem, IPersistenceContext context)
        {
            HL7QueueItemDetail detail = new HL7QueueItemDetail();

            detail.Direction = context.GetBroker<HL7MessageDirectionEnumTable>()[queueItem.Direction];

            detail.StatusCode = context.GetBroker<HL7MessageStatusCodeEnumTable>()[queueItem.Status.Code];
            detail.StatusDescription = queueItem.Status.Description;
            detail.CreationDateTime = queueItem.Status.CreationDateTime;
            detail.UpdateDateTime = queueItem.Status.UpdateDateTime;

            detail.Peer = context.GetBroker<HL7MessagePeerEnumTable>()[queueItem.Message.Peer];
            detail.MessageType = queueItem.Message.MessageType;
            detail.MessageEvent = queueItem.Message.Event;
            detail.MessageVersion = context.GetBroker<HL7MessageVersionEnumTable>()[queueItem.Message.Version];
            detail.MessageFormat = context.GetBroker<HL7MessageFormatEnumTable>()[queueItem.Message.Format];
            detail.MessageText = queueItem.Message.Text;

            return detail;
        }

        public HL7QueueItemSearchCriteria CreateHL7QueueItemSearchCriteria(ListHL7QueueItemsRequest request, IPersistenceContext context)
        {
            HL7QueueItemSearchCriteria criteria = new HL7QueueItemSearchCriteria();

            criteria.Direction.EqualTo(context.GetBroker<HL7MessageDirectionEnumTable>()[request.Direction]);

            criteria.Status = new HL7QueueItemStatusSearchCriteria();
            criteria.Status.Code.EqualTo(context.GetBroker<HL7MessageStatusCodeEnumTable>()[request.StatusCode]);

            if(request.StartingCreationDateTime.HasValue && request.EndingCreationDateTime.HasValue)
                criteria.Status.CreationDateTime.Between(request.StartingCreationDateTime.Value, request.EndingCreationDateTime);
            else if (request.StartingCreationDateTime.HasValue)
                criteria.Status.CreationDateTime.MoreThanOrEqualTo(request.StartingCreationDateTime.Value);
            else if (request.EndingCreationDateTime.HasValue)
                criteria.Status.CreationDateTime.LessThanOrEqualTo(request.StartingCreationDateTime.Value);

            if (request.StartingUpdateDateTime.HasValue && request.EndingUpdateDateTime.HasValue)
                criteria.Status.UpdateDateTime.Between(request.StartingUpdateDateTime.Value, request.EndingUpdateDateTime);
            else if (request.StartingUpdateDateTime.HasValue)
                criteria.Status.UpdateDateTime.MoreThanOrEqualTo(request.StartingUpdateDateTime.Value);
            else if (request.EndingUpdateDateTime.HasValue)
                criteria.Status.UpdateDateTime.LessThanOrEqualTo(request.StartingUpdateDateTime.Value);

            criteria.Message = new HL7QueueItemMessageSearchCriteria();

            criteria.Message.MessageType.EqualTo(request.MessageType);
            criteria.Message.Peer.EqualTo(context.GetBroker<HL7MessagePeerEnumTable>()[request.Peer]);

            return criteria;
        }
    }
}
