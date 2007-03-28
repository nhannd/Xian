using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.HL7;
using ClearCanvas.HL7.Brokers;
using ClearCanvas.HL7.PreProcessing;
using ClearCanvas.HL7.Processing;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.HL7Admin;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common;

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

            response.DirectionChoices = CollectionUtils.Map<HL7MessageDirectionEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IHL7MessageDirectionEnumBroker>().Load().Items,
                delegate(HL7MessageDirectionEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            response.StatusCodeChoices = CollectionUtils.Map<HL7MessageStatusCodeEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IHL7MessageStatusCodeEnumBroker>().Load().Items,
                delegate(HL7MessageStatusCodeEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            //TODO: fix message type
            response.MessageTypeChoices = new List<string>();
            response.MessageTypeChoices.Add("ADT_A01");
            response.MessageTypeChoices.Add("ADT_A02");
            response.MessageTypeChoices.Add("ADT_A03");
            response.MessageTypeChoices.Add("ADT_A04");
            response.MessageTypeChoices.Add("ADT_A05");
            response.MessageTypeChoices.Add("ORM_O01");

            response.MessageFormatChoices = CollectionUtils.Map<HL7MessageFormatEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IHL7MessageFormatEnumBroker>().Load().Items,
                delegate(HL7MessageFormatEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            response.MessageVersionChoices = CollectionUtils.Map<HL7MessageVersionEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IHL7MessageVersionEnumBroker>().Load().Items,
                delegate(HL7MessageVersionEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            response.PeerChoices = CollectionUtils.Map<HL7MessagePeerEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IHL7MessagePeerEnumBroker>().Load().Items,
                delegate(HL7MessagePeerEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            return response;
        }

        [ReadOperation]
        public ListHL7QueueItemsResponse ListHL7QueueItems(ListHL7QueueItemsRequest request)
        {
            HL7QueueItemAssembler assembler = new HL7QueueItemAssembler();
            HL7QueueItemSearchCriteria criteria = assembler.CreateHL7QueueItemSearchCriteria(request, PersistenceContext);
            SearchResultPage page = new SearchResultPage(request.FirstRow, request.MaxRows);

            return new ListHL7QueueItemsResponse(
                CollectionUtils.Map<HL7QueueItem, HL7QueueItemSummary, List<HL7QueueItemSummary>>(
                    PersistenceContext.GetBroker<IHL7QueueItemBroker>().Find(criteria, page),
                    delegate(HL7QueueItem queueItem)
                    {
                        return assembler.CreateHL7QueueItemSummary(queueItem, PersistenceContext);
                    }));
        }

        [ReadOperation]
        public LoadHL7QueueItemResponse LoadHL7QueueItem(LoadHL7QueueItemRequest request)
        {
            HL7QueueItem queueItem = (HL7QueueItem)PersistenceContext.Load(request.QueueItemRef);
            HL7QueueItemAssembler assembler = new HL7QueueItemAssembler();
            
            return new LoadHL7QueueItemResponse(
                queueItem.GetRef(),
                assembler.CreateHL7QueueItemDetail(queueItem, PersistenceContext));
        }

        [ReadOperation]
        public GetReferencedPatientResponse GetReferencedPatient(GetReferencedPatientRequest request)
        {
            HL7QueueItem queueItem = (HL7QueueItem)PersistenceContext.Load(request.QueueItemRef);

            //TODO:  Refactor following region
            #region To Be Refactored
            
            IHL7PreProcessor preProcessor = new HL7PreProcessor();
            HL7QueueItem preProcessedQueueItem = preProcessor.ApplyAll(queueItem);

            IHL7Processor processor = HL7ProcessorFactory.GetProcessor(preProcessedQueueItem.Message);

            IList<string> identifiers = processor.ListReferencedPatientIdentifiers();
            if (identifiers.Count == 0)
            {
                return null;
            }
            string assigningAuthority = processor.ReferencedPatientIdentifiersAssigningAuthority();

            #endregion

            PatientProfileSearchCriteria criteria = new PatientProfileSearchCriteria();
            criteria.Mrn.Id.EqualTo(identifiers[0]);
            criteria.Mrn.AssigningAuthority.EqualTo(assigningAuthority);

            IPatientProfileBroker profileBroker = PersistenceContext.GetBroker<IPatientProfileBroker>();
            IList<PatientProfile> profiles = profileBroker.Find(criteria);

            if (profiles.Count == 0)
            {
                return new GetReferencedPatientResponse();
            }
            else
            {
                return new GetReferencedPatientResponse(profiles[0].GetRef());
            }
        }

        [UpdateOperation]
        public ProcessHL7QueueItemResponse ProcessHL7QueueItem(ProcessHL7QueueItemRequest request)
        {
            HL7QueueItem queueItem = (HL7QueueItem)PersistenceContext.Load(request.QueueItemRef);

            try
            {
                IHL7PreProcessor preProcessor = new HL7PreProcessor();
                HL7QueueItem preProcessedQueueItem = preProcessor.ApplyAll(queueItem);

                IHL7Processor processor = HL7ProcessorFactory.GetProcessor(preProcessedQueueItem.Message);
                processor.Process(PersistenceContext);

                PersistenceContext.Lock(queueItem);
                queueItem.SetComplete();
            }
            catch (Exception e)
            {
                // Set the queue item's error description in a different persistence context
                // Ensures queue item's status updates but domain object changes are rolled back
                using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update, PersistenceScopeOption.RequiresNew))
                {
                    PersistenceScope.Current.Lock(queueItem);
                    queueItem.SetError(e.Message);
                    scope.Complete();
                }

                throw e;
            }

            return new ProcessHL7QueueItemResponse();
        }

        #endregion
    }
}


