using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.HL7;
using ClearCanvas.Ris.Application.Common.Admin.HL7Admin;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    class HL7QueueItemAssembler
    {
        public HL7QueueItemSummary CreateHL7QueueItemSummary(HL7QueueItem queueItem)
        {
            HL7QueueItemSummary summary = new HL7QueueItemSummary();

            //TODO: Handle enumerations properly
            summary.Direction = queueItem.Direction.ToString();

            summary.StatusCode = queueItem.Status.Code.ToString();
            summary.StatusDescription = queueItem.Status.Description;
            summary.CreationDateTime = queueItem.Status.CreationDateTime;
            summary.UpdateDateTime = queueItem.Status.UpdateDateTime;

            summary.Peer = queueItem.Message.Peer.ToString();
            summary.MessageType = queueItem.Message.MessageType;
            summary.MessageEvent = queueItem.Message.Event;
            summary.MessageVersion = queueItem.Message.Version.ToString();
            summary.MessageFormat = queueItem.Message.Format.ToString();

            return summary;
        }

        public HL7QueueItemDetail CreateHL7QueueItemDetail(HL7QueueItem queueItem)
        {
            HL7QueueItemDetail detail = new HL7QueueItemDetail();

            //TODO: Handle enumerations properly
            detail.Direction = queueItem.Direction.ToString();

            detail.StatusCode = queueItem.Status.Code.ToString();
            detail.StatusDescription = queueItem.Status.Description;
            detail.CreationDateTime = queueItem.Status.CreationDateTime;
            detail.UpdateDateTime = queueItem.Status.UpdateDateTime;

            detail.Peer = queueItem.Message.Peer.ToString();
            detail.MessageType = queueItem.Message.MessageType;
            detail.MessageEvent = queueItem.Message.Event;
            detail.MessageVersion = queueItem.Message.Version.ToString();
            detail.MessageFormat = queueItem.Message.Format.ToString();
            detail.MessageText = queueItem.Message.Text;

            return detail;
        }

        public HL7QueueItemSearchCriteria CreateHL7QueueItemSearchCriteria(ListHL7QueueItemsRequest request)
        {
            //TODO: handle enumerations

            HL7QueueItemSearchCriteria criteria = new HL7QueueItemSearchCriteria();

            criteria.Direction.EqualTo(request.Direction);

            criteria.Status = new HL7QueueItemStatusSearchCriteria();
            //criteria.Status.Code.EqualTo(request.StatusCode);

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

            //criteria.Message.MessageType.EqualTo(request.MessageType);
            //criteria.Message.Peer.EqualTo(request.Peer);

            return criteria;
        }
    }
}
