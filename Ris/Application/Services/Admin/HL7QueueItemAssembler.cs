#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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

            summary.QueueItemRef = queueItem.GetRef();

            summary.Direction = EnumUtils.GetValue(queueItem.Direction, context);
            summary.StatusCode = EnumUtils.GetValue(queueItem.Status.Code, context);
            summary.StatusDescription = queueItem.Status.Description;
            summary.CreationDateTime = queueItem.Status.CreationDateTime;
            summary.UpdateDateTime = queueItem.Status.UpdateDateTime;

            summary.Peer = EnumUtils.GetValue(queueItem.Message.Peer, context);
            summary.MessageType = queueItem.Message.MessageType;
            summary.MessageEvent = queueItem.Message.Event;
            summary.MessageVersion = EnumUtils.GetValue(queueItem.Message.Version, context);
            summary.MessageFormat = EnumUtils.GetValue(queueItem.Message.Format, context);

            return summary;
        }

        public HL7QueueItemDetail CreateHL7QueueItemDetail(HL7QueueItem queueItem, IPersistenceContext context)
        {
            HL7QueueItemDetail detail = new HL7QueueItemDetail();

            detail.QueueItemRef = queueItem.GetRef();

            detail.Direction = EnumUtils.GetEnumValueInfo(queueItem.Direction, context);
            detail.StatusCode = EnumUtils.GetEnumValueInfo(queueItem.Status.Code, context);
            detail.StatusDescription = queueItem.Status.Description;
            detail.CreationDateTime = queueItem.Status.CreationDateTime;
            detail.UpdateDateTime = queueItem.Status.UpdateDateTime;

            detail.Peer = EnumUtils.GetEnumValueInfo(queueItem.Message.Peer, context);

            detail.MessageType = queueItem.Message.MessageType;
            detail.MessageEvent = queueItem.Message.Event;
            detail.MessageVersion = EnumUtils.GetEnumValueInfo(queueItem.Message.Version, context);
            detail.MessageFormat = EnumUtils.GetEnumValueInfo(queueItem.Message.Format, context);
            detail.MessageText = queueItem.Message.Text;

            return detail;
        }

        public HL7QueueItemSearchCriteria CreateHL7QueueItemSearchCriteria(ListHL7QueueItemsRequest request, IPersistenceContext context)
        {
            HL7QueueItemSearchCriteria criteria = new HL7QueueItemSearchCriteria();

            if (request.Direction != null)
                criteria.Direction.EqualTo(EnumUtils.GetEnumValue<HL7MessageDirection>(request.Direction));

            if (request.StatusCode != null)
                criteria.Status.Code.EqualTo(EnumUtils.GetEnumValue<HL7MessageStatusCode>(request.StatusCode));

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

            if (request.MessageType != null)
                criteria.Message.MessageType.EqualTo(request.MessageType);
    
            if (request.Peer != null)
                criteria.Message.Peer.EqualTo(EnumUtils.GetEnumValue<HL7MessagePeer>(request.Peer));

            return criteria;
        }
    }
}
