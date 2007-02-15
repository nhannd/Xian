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
        public IList<HL7QueueItem> GetHL7QueueItems(HL7QueueItemSearchCriteria criteria, SearchResultPage page)
        {
            return this.CurrentContext.GetBroker<IHL7QueueItemBroker>().Find(criteria, page);
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

            IHL7Processor processor = HL7ProcessorFactory.GetProcessor(preProcessedQueueItem.Message);

            processor.Patients = LoadOrCreatePatientsFromMrn(
                processor.ListReferencedPatientIdentifiers(),
                processor.ReferencedPatientIdentifiersAssigningAuthority());
            processor.Visits = LoadOrCreateVisitFromVisitNumber(
                processor.ListReferencedVisitIdentifiers(),
                processor.ReferencedVisitIdentifierAssigningAuthority());

            processor.Process();

            foreach (EntityAccess<Patient> patientAccess in processor.Patients)
            {
                this.CurrentContext.Lock(patientAccess.Entity, patientAccess.State);
            }
            foreach (EntityAccess<Visit> visitAccess in processor.Visits)
            {
                this.CurrentContext.Lock(visitAccess.Entity, visitAccess.State);
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

        #endregion

        #region Private Methods

        private IList<EntityAccess<Patient>> LoadOrCreatePatientsFromMrn(IList<string> mrns, string assigningAuthority)
        {
            IList<EntityAccess<Patient>> loadedPatients = new List<EntityAccess<Patient>>();

            foreach (string mrn in mrns)
            {
                PatientProfileSearchCriteria criteria = new PatientProfileSearchCriteria();
                criteria.Mrn.Id.EqualTo(mrn);
                criteria.Mrn.AssigningAuthority.EqualTo(assigningAuthority);

                IList<PatientProfile> profiles = this.CurrentContext.GetBroker<IPatientProfileBroker>().Find(criteria);
                if (profiles.Count > 0)
                {
                    loadedPatients.Add(new EntityAccess<Patient>(profiles[0].Patient, DirtyState.Dirty));
                }
                else
                {
                    Patient patient = new Patient();
                    PatientProfile profile = new PatientProfile();

                    profile.Mrn.Id = mrn;
                    profile.Mrn.AssigningAuthority = assigningAuthority;

                    patient.AddProfile(profile);

                    loadedPatients.Add(new EntityAccess<Patient>(patient, DirtyState.New));
                }
            }

            return loadedPatients;
        }

        private IList<EntityAccess<Visit>> LoadOrCreateVisitFromVisitNumber(IList<string> visitNumbers, string assigningAuthority)
        {
            IList<EntityAccess<Visit>> loadedVisits = new List<EntityAccess<Visit>>();

            if (visitNumbers.Count == 0) return loadedVisits;

            foreach (string visitNumber in visitNumbers)
            {
                VisitSearchCriteria criteria = new VisitSearchCriteria();
                criteria.VisitNumber.Id.EqualTo(visitNumber);
                criteria.VisitNumber.AssigningAuthority.EqualTo(assigningAuthority);

                IList<Visit> visits = this.CurrentContext.GetBroker<IVisitBroker>().Find(criteria);
                if (visits.Count > 0)
                {
                    loadedVisits.Add(new EntityAccess<Visit>(visits[0], DirtyState.Dirty));
                }
                else
                {
                    Visit visit = new Visit();

                    visit.VisitNumber.Id = visitNumber;
                    visit.VisitNumber.AssigningAuthority = assigningAuthority;

                    IList<Facility> facilities = this.CurrentContext.GetBroker<IFacilityBroker>().FindAll();
                    visit.Facility = facilities[0];

                    loadedVisits.Add(new EntityAccess<Visit>(visit, DirtyState.New));
                }
            }

            return loadedVisits;
        }

        #endregion    
    }
}


