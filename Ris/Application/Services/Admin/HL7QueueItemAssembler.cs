using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.HL7;
using ClearCanvas.Ris.Application.Common.Admin.HL7Admin;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.HL7.Brokers;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    class HL7QueueItemAssembler
    {
        public HL7QueueItemSummary CreateHL7QueueItemSummary(HL7QueueItem queueItem, IPersistenceContext context)
        {
            HL7QueueItemSummary summary = new HL7QueueItemSummary();

            summary.Direction = context.GetBroker<IHL7MessageDirectionEnumBroker>().Load()[queueItem.Direction].Value;

            summary.StatusCode = context.GetBroker<IHL7MessageStatusCodeEnumBroker>().Load()[queueItem.Status.Code].Value;
            summary.StatusDescription = queueItem.Status.Description;
            summary.CreationDateTime = queueItem.Status.CreationDateTime;
            summary.UpdateDateTime = queueItem.Status.UpdateDateTime;

            summary.Peer = context.GetBroker<IHL7MessagePeerEnumBroker>().Load()[queueItem.Message.Peer].Value;
            summary.MessageType = queueItem.Message.MessageType;
            summary.MessageEvent = queueItem.Message.Event;
            summary.MessageVersion = context.GetBroker<IHL7MessageVersionEnumBroker>().Load()[queueItem.Message.Version].Value;
            summary.MessageFormat = context.GetBroker<IHL7MessageFormatEnumBroker>().Load()[queueItem.Message.Format].Value;

            return summary;
        }

        public HL7QueueItemDetail CreateHL7QueueItemDetail(HL7QueueItem queueItem, IPersistenceContext context)
        {
            HL7QueueItemDetail detail = new HL7QueueItemDetail();

            HL7MessageDirectionEnum direction = context.GetBroker<IHL7MessageDirectionEnumBroker>().Load()[queueItem.Direction];
            detail.Direction = new EnumValueInfo(
                direction.Code.ToString(),
                direction.Value);

            HL7MessageStatusCodeEnum status = context.GetBroker<IHL7MessageStatusCodeEnumBroker>().Load()[queueItem.Status.Code];
            detail.StatusCode = new EnumValueInfo(
                status.Code.ToString(),
                status.Value);

            detail.StatusDescription = queueItem.Status.Description;
            detail.CreationDateTime = queueItem.Status.CreationDateTime;
            detail.UpdateDateTime = queueItem.Status.UpdateDateTime;

            HL7MessagePeerEnum peer = context.GetBroker<IHL7MessagePeerEnumBroker>().Load()[queueItem.Message.Peer];
            detail.Peer = new EnumValueInfo(
                peer.Code.ToString(),
                peer.Value);

            detail.MessageType = queueItem.Message.MessageType;
            detail.MessageEvent = queueItem.Message.Event;

            HL7MessageVersionEnum version = context.GetBroker<IHL7MessageVersionEnumBroker>().Load()[queueItem.Message.Version];
            detail.MessageVersion = new EnumValueInfo(
                version.Code.ToString(),
                version.Value);

            HL7MessageFormatEnum format = context.GetBroker<IHL7MessageFormatEnumBroker>().Load()[queueItem.Message.Format];
            detail.MessageFormat = new EnumValueInfo(
                format.Code.ToString(),
                format.Value);
            
            detail.MessageText = queueItem.Message.Text;

            return detail;
        }

        public HL7QueueItemSearchCriteria CreateHL7QueueItemSearchCriteria(ListHL7QueueItemsRequest request, IPersistenceContext context)
        {
            HL7QueueItemSearchCriteria criteria = new HL7QueueItemSearchCriteria();

            criteria.Direction.EqualTo((HL7MessageDirection)Enum.Parse(typeof(HL7MessageDirection), request.Direction.Code));

            criteria.Status.Code.EqualTo((HL7MessageStatusCode)Enum.Parse(typeof(HL7MessageStatusCode), request.StatusCode.Code));

            if (request.StartingCreationDateTime.HasValue && request.EndingCreationDateTime.HasValue)
                criteria.Status.CreationDateTime.Between(request.StartingCreationDateTime.Value, request.EndingCreationDateTime.Value);
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

            criteria.Message.MessageType.EqualTo(request.MessageType);
            criteria.Message.Peer.EqualTo((HL7MessagePeer)Enum.Parse(typeof(HL7MessagePeer), request.Peer.Code));

            return criteria;
        }
    }
}
