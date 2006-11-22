using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.HL7;
using ClearCanvas.HL7.Brokers;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

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
            return this.CurrentContext.GetBroker<IHL7QueueItemBroker>().FindAll();
        }

        [ReadOperation]
        public IList<HL7QueueItem> GetFilteredHL7QueueItems(HL7QueueItemSearchCriteria criteria)
        {
            return this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Find(criteria);
        }

        [UpdateOperation]
        public IList<Patient> ProcessHL7QueueItem(HL7QueueItem hl7QueueItem)
        {
            if (hl7QueueItem.Status.Code == HL7MessageStatusCode.C)
            {
                //throw new Exception("Queue item has already been processed");
            }

            IList<Patient> referencedPatients = null;

            try
            {
                using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
                {
                    referencedPatients = ProcessHL7QueueItemMessage(hl7QueueItem.Message);

                    foreach (Patient patient in referencedPatients)
                    {
                        this.CurrentContext.Lock(patient, DirtyState.New);
                    }

                    this.CurrentContext.Lock(hl7QueueItem);
                    hl7QueueItem.SetComplete();

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                Platform.Log("Unable to process HL7 queue item: " + hl7QueueItem.ToString());
                Platform.Log("Exception thrown: " + e.Message);

                this.CurrentContext.Lock(hl7QueueItem);
                hl7QueueItem.SetError(e.Message);

                if (referencedPatients != null)
                {
                    referencedPatients.Clear();
                }
            }

            return referencedPatients;
        }

        [UpdateOperation]
        public void EnqueueHL7QueueItem(ClearCanvas.HL7.HL7QueueItem item)
        {
            this.CurrentContext.Lock(item, DirtyState.Dirty);
        }

        [UpdateOperation]
        public void SyncExternalQueue()
        {
            IList<HL7ExternalQueueItem> toBeSynched = this.CurrentContext.GetBroker<IHL7ExternalQueueItemBroker>().GetUnsynchedItems();
            foreach (HL7ExternalQueueItem externalQueueItem in toBeSynched)
            {
                HL7QueueItem queueItem = externalQueueItem.GetHL7QueueItem();
                this.CurrentContext.Lock(queueItem, DirtyState.New);

                this.CurrentContext.GetBroker<IHL7ExternalQueueItemBroker>().FlagItemAsSynched(externalQueueItem.Guid);
            }
        }

        #endregion

        #region Private Methods

		private IList<Patient> ProcessHL7QueueItemMessage(HL7QueueItemMessage hl7QueueItemMessage)
        {
            //IHL7MessageProvider messageProvider = HL7MessageProviderFactory.GetMessageProvider(hl7QueueItemMessage.Version);
            //IHL7Message message = messageProvider.GetMessage(hl7QueueItemMessage.MessageType, hl7QueueItemMessage.Format, hl7QueueItemMessage.Text);

            //IHL7MessageMapping mapping = HL7MessageMappingFactory.GetMapping(message, hl7QueueItemMessage.Peer);

            //IHL7Processor processor = HL7ProcessorFactory.GetProcessor(mapping);

            IHL7Processor processor = HL7ProcessorFactory.GetProcessor(hl7QueueItemMessage);

            IList<string> identifiers = processor.ListReferencedPatientIdentifiers();
            string assigningAuthority = processor.ReferencedPatientIdentifiersAssigningAuthority();

            IList<Patient> referencedPatients = LoadOrCreatePatientsFromMrn(identifiers, assigningAuthority);
            processor.UpdatePatients(referencedPatients);

            return referencedPatients;
        }

        private IList<Patient> LoadOrCreatePatientsFromMrn(IList<string> mrns, string assigningAuthority)
        {
            IList<Patient> loadedPatients = new List<Patient>();

            foreach(string mrn in mrns)
            {
                PatientProfileSearchCriteria criteria = new PatientProfileSearchCriteria();
                criteria.Mrn.Id.EqualTo(mrn);
                criteria.Mrn.AssigningAuthority.EqualTo(assigningAuthority);

                IList<PatientProfile> profiles = this.CurrentContext.GetBroker<IPatientProfileBroker>().Find(criteria);
                if (profiles.Count > 0)
                {
                    loadedPatients.Add(profiles[0].Patient);
                }
                else
                {
                    Patient patient = new Patient();
                    PatientProfile profile = new PatientProfile();

                    profile.Mrn.Id = mrn;
                    profile.Mrn.AssigningAuthority = assigningAuthority;

                    patient.AddProfile(profile);

                    loadedPatients.Add(patient);
                }
            }

            return loadedPatients;
        }

	    #endregion    
    }
}


