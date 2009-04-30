#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Collections.Generic;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.HL7;
using ClearCanvas.HL7.Brokers;
using ClearCanvas.HL7.PreProcessing;
using ClearCanvas.HL7.Processing;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.HL7Admin;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.HL7Admin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IHL7QueueService))]
    public class HL7QueueService : ApplicationServiceBase, IHL7QueueService
    {
        #region IHL7QueueService Members

        [ReadOperation]
        public GetHL7QueueFormDataResponse GetHL7QueueFormData(GetHL7QueueFormDataRequest request)
        {
            GetHL7QueueFormDataResponse response = new GetHL7QueueFormDataResponse();

            response.DirectionChoices = EnumUtils.GetEnumValueList<HL7MessageDirectionEnum>(PersistenceContext);

            response.StatusCodeChoices = EnumUtils.GetEnumValueList<HL7MessageStatusCodeEnum>(PersistenceContext);

            //TODO: fix message type
            response.MessageTypeChoices = new List<string>();
            response.MessageTypeChoices.Add("ADT_A01");
            response.MessageTypeChoices.Add("ADT_A02");
            response.MessageTypeChoices.Add("ADT_A03");
            response.MessageTypeChoices.Add("ADT_A04");
            response.MessageTypeChoices.Add("ADT_A05");
            response.MessageTypeChoices.Add("ORM_O01");
            response.MessageFormatChoices = EnumUtils.GetEnumValueList<HL7MessageFormatEnum>(PersistenceContext);
            response.MessageVersionChoices = EnumUtils.GetEnumValueList<HL7MessageVersionEnum>(PersistenceContext);
			response.PeerChoices = new List<string>(); // TODO: get values of peers from all existing messages

            return response;
        }

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.HL7.QueueMonitor)]
        public ListHL7QueueItemsResponse ListHL7QueueItems(ListHL7QueueItemsRequest request)
        {
            HL7QueueItemAssembler assembler = new HL7QueueItemAssembler();
            HL7QueueItemSearchCriteria criteria = assembler.CreateHL7QueueItemSearchCriteria(request, PersistenceContext);

            return new ListHL7QueueItemsResponse(
                CollectionUtils.Map<HL7QueueItem, HL7QueueItemSummary, List<HL7QueueItemSummary>>(
                    PersistenceContext.GetBroker<IHL7QueueItemBroker>().Find(criteria, request.Page),
                    delegate(HL7QueueItem queueItem)
                    {
                        return assembler.CreateHL7QueueItemSummary(queueItem, PersistenceContext);
                    }));
        }

        [ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.HL7.QueueMonitor)]
		public LoadHL7QueueItemResponse LoadHL7QueueItem(LoadHL7QueueItemRequest request)
        {
            HL7QueueItem queueItem = PersistenceContext.Load<HL7QueueItem>(request.QueueItemRef);
            HL7QueueItemAssembler assembler = new HL7QueueItemAssembler();
            
            return new LoadHL7QueueItemResponse(
                queueItem.GetRef(),
                assembler.CreateHL7QueueItemDetail(queueItem, PersistenceContext));
        }

        [ReadOperation]
        public GetReferencedPatientResponse GetReferencedPatient(GetReferencedPatientRequest request)
        {
            HL7QueueItem queueItem = PersistenceContext.Load<HL7QueueItem>(request.QueueItemRef);

            IList<string> identifiers;
            string assigningAuthority;

            try
            {
				IHL7PeerProcessor processor = HL7ProcessorFactory.GetProcessor(queueItem.Message.Peer);

				if (null == processor)
				{
					// no process was found that can process the message
					// TODO: it will be problematic if queueItem doesn't exist
					throw new System.Exception(string.Format("Cold not find processor for peer: {0}", queueItem.Message.Peer));
				}

				identifiers = processor.ListReferencedPatientIdentifiers();
                if (identifiers.Count == 0)
                {
                    return null;
                }
                
                assigningAuthority = processor.ReferencedPatientIdentifiersAssigningAuthority();
            }
            catch (HL7ProcessingException e)
            {
                throw new RequestValidationException("HL7 processing error: " + e.Message);
            }

            PatientProfileSearchCriteria criteria = new PatientProfileSearchCriteria();
            criteria.Mrn.Id.EqualTo(identifiers[0]);
            criteria.Mrn.AssigningAuthority.EqualTo(PersistenceContext.GetBroker<IEnumBroker>().Find<InformationAuthorityEnum>(assigningAuthority));

            IPatientProfileBroker profileBroker = PersistenceContext.GetBroker<IPatientProfileBroker>();
            IList<PatientProfile> profiles = profileBroker.Find(criteria);

            PatientProfile profile = CollectionUtils.FirstElement(profiles);

            if (profile == null)
            {
                throw new RequestValidationException(string.Format(SR.ExceptionPatientNotFound, identifiers[0], assigningAuthority));
            }

            return new GetReferencedPatientResponse(profile.Patient.GetRef(), profile.GetRef());            
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.HL7.QueueMonitor)]
		public ProcessHL7QueueItemResponse ProcessHL7QueueItem(ProcessHL7QueueItemRequest request)
        {
            HL7QueueItem queueItem = PersistenceContext.Load<HL7QueueItem>(request.QueueItemRef);

            try
            {
				IHL7PeerProcessor processor = HL7ProcessorFactory.GetProcessor(queueItem.Message.Peer);
                processor.Process(PersistenceContext, queueItem.Message);
                PersistenceContext.Lock(queueItem);
                queueItem.SetComplete();
            }
            catch (HL7ProcessingException e)
            {
                // Set the queue item's error description in a different persistence context
                // Ensures queue item's status is updated but domain object changes are rolled back
                using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update, PersistenceScopeOption.RequiresNew))
                {
                    ((IUpdateContext)PersistenceScope.CurrentContext).ChangeSetRecorder.OperationName = this.GetType().FullName;
                    PersistenceScope.CurrentContext.Lock(queueItem);
                    queueItem.SetError(e.Message);
                    scope.Complete();
                }

                throw new RequestValidationException("HL7 processing error: " + e.Message);
            }

            return new ProcessHL7QueueItemResponse();
        }

        #endregion
    }
}


