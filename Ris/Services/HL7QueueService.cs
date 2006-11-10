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
            HL7QueueItemSearchCriteria criteria = new HL7QueueItemSearchCriteria();
            return this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Find(criteria);
        }

        [UpdateOperation]
        public IList<Patient> ProcessHL7QueueItem(HL7QueueItem hl7QueueItem)
        {
            if (hl7QueueItem.Status.Code == HL7MessageStatusCode.C)
            {
                //throw new Exception("Queue item has already been processed");
            }

            HL7MessageStatusCode result = HL7MessageStatusCode.C;
            string statusDescription = null;
            IList<Patient> referencedPatients = null;

            try
            {
                referencedPatients = ProcessHL7QueueItemHelper(hl7QueueItem);
                foreach (Patient patient in referencedPatients)
                {
                    this.CurrentContext.Lock(patient);
                    //this.CurrentContext.GetBroker<IPatientBroker>().Store(patient);
                }
            }
            catch (Exception e)
            {
                Platform.Log("Unable to process HL7 queue item: " + hl7QueueItem.ToString());
                Platform.Log("Exception thrown: " + e.Message);
                statusDescription = e.Message;
                result = HL7MessageStatusCode.E;

                if (referencedPatients != null)
                {
                    referencedPatients.Clear();
                }
                //can transactions be rolled back here yet have the QueueItemUpdate proceed?
            }
            finally
            {
                UpdateQueueItemStatus(hl7QueueItem, result, statusDescription);
            }

            return referencedPatients;
        }

        private IList<Patient> ProcessHL7QueueItemHelper(HL7QueueItem hl7QueueItem)
        {
            HL7MessageProviderFactory providerFactory = new HL7MessageProviderFactory();
            IHL7MessageProvider messageProvider = providerFactory.GetMessageProvider(hl7QueueItem.Message.Version);
            IHL7Message message = messageProvider.GetMessage(hl7QueueItem.Message.MessageType, hl7QueueItem.Message.Format, hl7QueueItem.Message.Text);

            HL7MessageMappingFactory mappingFactory = new HL7MessageMappingFactory();
            IHL7MessageMapping mapping = mappingFactory.GetMapping(message, hl7QueueItem.Message.Peer);

            HL7ProcessorFactory processorFactory = new HL7ProcessorFactory();
            IHL7Processor processor = processorFactory.GetProcessor(mapping);

            IList<string> referencedPatientProfiles = processor.GetReferencedPatientIdentifiers();
            string assigningAuthority = processor.GetReferencedPatientIdentifiersAssigningAuthority();

            IList<Patient> referencedPatients = LoadOrCreatePatients(referencedPatientProfiles, assigningAuthority);
            processor.UpdatePatients(referencedPatients);

            return referencedPatients;
        }

        [UpdateOperation]
        public void EnqueueHL7QueueItem(ClearCanvas.HL7.HL7QueueItem item)
        {
            this.CurrentContext.Lock(item);
            //this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Store(item);
        }

        [UpdateOperation]
        public void SyncExternalQueue()
        {
            IList<HL7ExternalQueueItem> toBeSynched = this.CurrentContext.GetBroker<IHL7ExternalQueueItemBroker>().GetUnsynchedItems();
            foreach (HL7ExternalQueueItem externalQueueItem in toBeSynched)
            {
                HL7QueueItem queueItem = externalQueueItem.GetHL7QueueItem();
                this.CurrentContext.Lock(queueItem);
                //this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Store(queueItem);
                this.CurrentContext.GetBroker<IHL7ExternalQueueItemBroker>().FlagItemAsSynched(externalQueueItem.Guid);
            }
        }

        #endregion

        private void UpdateQueueItemStatus(ClearCanvas.HL7.HL7QueueItem item, ClearCanvas.HL7.HL7MessageStatusCode status, string statusDescription)
        {
            if (item == null) return;

            //HL7QueueItem reloaded = this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Find(item.OID);

            //if (reloaded != null)
            //{
            //    reloaded.Status.Code = status;
            //    reloaded.Status.Description = statusDescription;
            //    reloaded.Status.UpdateDateTime = Platform.Time;

            //    this.CurrentContext.Lock(reloaded);
            //    //this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Store(reloaded);
            //}
            item.Status.Code = status;
            item.Status.Description = statusDescription;
            item.Status.UpdateDateTime = Platform.Time;

            this.CurrentContext.Lock(item);
        }

        private IList<Patient> LoadOrCreatePatients(IList<string> mrns, string assigningAuthority)
        {
            IList<Patient> patients = new List<Patient>();
            foreach(string mrn in mrns)
            {
                PatientProfileSearchCriteria criteria = new PatientProfileSearchCriteria();
                criteria.Mrn.Id.EqualTo(mrn);
                //criteria.MRN.AssigningAuthority.EqualTo(assigningAuthority);

                IList<PatientProfile> profiles = this.CurrentContext.GetBroker<IPatientProfileBroker>().Find(criteria);
                if (profiles.Count > 0)
                {
                    patients.Add(profiles[0].Patient);
                }
                else
                {
                    Patient patient = new Patient();
                    PatientProfile profile = new PatientProfile();
                    profile.Mrn.Id = mrn;
                    profile.Mrn.AssigningAuthority = "TODO";
                    profile.Patient = patient;
                    patient.Profiles.Add(profile);

                    patients.Add(patient);
                }
            }
            return patients;
        }
    }
}


