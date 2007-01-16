using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.HL7;
using ClearCanvas.HL7.Brokers;
using ClearCanvas.HL7.PreProcessing;
using ClearCanvas.HL7.Processing;
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
        public HL7QueueItem LoadHL7QueueItem(EntityRef<HL7QueueItem> queueItemRef)
        {
            IHL7QueueItemBroker broker = this.CurrentContext.GetBroker<IHL7QueueItemBroker>();
            return broker.Load(queueItemRef);
        }

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

        [ReadOperation]
        public PatientProfile GetReferencedPatient(EntityRef<HL7QueueItem> hl7QueueItemRef)
        {
            IHL7QueueItemBroker broker = this.CurrentContext.GetBroker<IHL7QueueItemBroker>();
            HL7QueueItem queueItem = broker.Load(hl7QueueItemRef);

            IHL7Processor processor = HL7ProcessorFactory.GetProcessor(queueItem.Message);

            IList<string> identifiers = processor.ListReferencedPatientIdentifiers();
            if (identifiers.Count == 0)
            {
                return null;
            }
            string assigningAuthority = processor.ReferencedPatientIdentifiersAssigningAuthority();

            PatientProfileSearchCriteria criteria = new PatientProfileSearchCriteria();
            criteria.Mrn.Id.EqualTo(identifiers[0]);
            criteria.Mrn.AssigningAuthority.EqualTo(assigningAuthority);

            IPatientProfileBroker profileBroker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            IList<PatientProfile> profiles = profileBroker.Find(criteria);

            if (profiles.Count == 0)
            {
                return null;
            }
            else
            {
                return profiles[0];
            }
        }

        [UpdateOperation]
        public IList<Patient> ProcessHL7QueueItem(HL7QueueItem hl7QueueItem)
        {
            IList<Patient> referencedPatients = null;

            IHL7PreProcessor preProcessor = new HL7PreProcessor();
            HL7QueueItem preProcessedQueueItem = preProcessor.ApplyAll(hl7QueueItem);

            //IHL7Processor processor = HL7ProcessorFactory.GetProcessor(hl7QueueItem.Message);
            IHL7Processor processor = HL7ProcessorFactory.GetProcessor(preProcessedQueueItem.Message);

            processor.Patients = LoadOrCreatePatientsFromMrn(
                processor.ListReferencedPatientIdentifiers(), 
                processor.ReferencedPatientIdentifiersAssigningAuthority());
            processor.Visits = LoadOrCreateVisitFromVisitNumber(
                processor.ListReferencedVisitIdentifiers(),
                processor.ReferencedVisitIdentifierAssigningAuthority());

            processor.Process();

            //foreach (Entity newEntity in processor.NewEntities)
            foreach (Entity newEntity in processor.Patients)
            {
                this.CurrentContext.Lock(newEntity, DirtyState.New);
            }
            foreach (Visit newEntity in processor.Visits)
            {
                this.CurrentContext.Lock(newEntity, DirtyState.New);
            }
            foreach (Entity dirtyEntity in processor.DirtyEntities)
            {
                this.CurrentContext.Lock(dirtyEntity, DirtyState.Dirty);
            }
            
            return referencedPatients;
        }

        [UpdateOperation]
        public void SetHL7QueueItemComplete(HL7QueueItem item)
        {
            this.CurrentContext.Lock(item);
            item.SetComplete();
        }

        [UpdateOperation]
        public void SetHL7QueueItemError(HL7QueueItem item, string errorMessage)
        {
            this.CurrentContext.Lock(item);
            item.SetError(errorMessage);
        }

        [UpdateOperation]
        public void EnqueueHL7QueueItem(ClearCanvas.HL7.HL7QueueItem item)
        {
            this.CurrentContext.Lock(item, DirtyState.New);
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

        private IList<Visit> LoadOrCreateVisitFromVisitNumber(IList<string> visitNumbers, string assigningAuthority)
        {
            IList<Visit> loadedVisits = new List<Visit>();

            if (visitNumbers.Count == 0) return loadedVisits;

            foreach (string visitNumber in visitNumbers)
            {
                VisitSearchCriteria criteria = new VisitSearchCriteria();
                criteria.VisitNumber.Id.EqualTo(visitNumber);
                criteria.VisitNumber.AssigningAuthority.EqualTo(assigningAuthority);

                IList<Visit> visits = this.CurrentContext.GetBroker<IVisitBroker>().Find(criteria);
                if (visits.Count > 0)
                {
                    loadedVisits.Add(visits[0]);
                }
                else
                {
                    Visit visit = new Visit();

                    visit.VisitNumber.Id = visitNumber;
                    visit.VisitNumber.AssigningAuthority = assigningAuthority;

                    IList<Facility> facilities = this.CurrentContext.GetBroker<IFacilityBroker>().FindAll();
                    visit.Facility = facilities[0];

                    loadedVisits.Add(visit);
                }
            }

            return loadedVisits;
        }

        #endregion    
    }
}


