using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Services
{
    [ExtensionOf(typeof(ClearCanvas.Enterprise.ServiceLayerExtensionPoint))]
    public class OrderEntryService : HealthcareServiceLayer, IOrderEntryService
    {
        #region IOrderEntryService Members

        [ReadOperation]
        public PatientProfile LoadPatientProfile(EntityRef<PatientProfile> profileRef)
        {
            IPatientProfileBroker broker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = broker.Load(profileRef);
            broker.LoadPatientForPatientProfile(profile);
            return profile;
        }

        [ReadOperation]
        public IList<Visit> ListActiveVisits(EntityRef<Patient> patientRef)
        {
            // ensure that the profiles collection is loaded
            IPatientBroker patientBroker = this.CurrentContext.GetBroker<IPatientBroker>();
            Patient patient = patientBroker.Load(patientRef, EntityLoadFlags.Proxy);

            VisitSearchCriteria criteria = new VisitSearchCriteria();
            criteria.Patient.EqualTo(patient);
            criteria.VisitStatus.NotEqualTo(VisitStatus.Discharged);

            IVisitBroker visitBroker = this.CurrentContext.GetBroker<IVisitBroker>();
            return visitBroker.Find(criteria);
        }

        [ReadOperation]
        public IList<DiagnosticService> ListDiagnosticServiceChoices()
        {
            return this.CurrentContext.GetBroker<IDiagnosticServiceBroker>().FindAll();
        }

        [ReadOperation]
        public DiagnosticService LoadDiagnosticServiceBreakdown(EntityRef<DiagnosticService> diagnosticServiceRef)
        {
            IDiagnosticServiceBroker dsBroker = this.CurrentContext.GetBroker<IDiagnosticServiceBroker>();
            IRequestedProcedureTypeBroker rptBroker = this.CurrentContext.GetBroker<IRequestedProcedureTypeBroker>();

            DiagnosticService diagnosticService = dsBroker.Load(diagnosticServiceRef);
            foreach (RequestedProcedureType rpt in diagnosticService.RequestedProcedureTypes)
            {
                rptBroker.LoadModalityProcedureStepTypesForRequestedProcedureType(rpt);
            }
            return diagnosticService;
        }

        [ReadOperation]
        public IList<Facility> ListOrderingFacilityChoices()
        {
            return this.CurrentContext.GetBroker<IFacilityBroker>().FindAll();
        }

        [ReadOperation]
        public IList<Practitioner> ListOrderingPhysicianChoices()
        {
            // TODO: figure out how to determine which physicians are "ordering" physicians
            return this.CurrentContext.GetBroker<IPractitionerBroker>().FindAll();
        }

        [ReadOperation]
        public IList<ModalityWorklistQueryResult> GetOrdersWorklist(ModalityProcedureStepSearchCriteria criteria)
        {
            IModalityWorklistBroker broker = this.CurrentContext.GetBroker<IModalityWorklistBroker>();
            return broker.GetWorklist(criteria, "UHN");
        }

/*
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public string GenerateNewAccessionNumber()
        {
            // note that we have declared this method as requiring a new persistence context
            // it is safer to generate accession numbers in a separate transaction always
            // if an accession number were generated in a shared transaction, and that transaction was rolled-back,
            // the accession sequence would rollback as well, leaving the application with the possibility of a
            // duplicate accession number
            IAccessionNumberBroker broker = this.CurrentContext.GetBroker<IAccessionNumberBroker>();
            return broker.GetNextAccessionNumber();
        }
*/
        [UpdateOperation]
        public void PlaceOrder(
            Patient patient,
            Visit visit,
            DiagnosticService diagnosticService,
            OrderPriority priority,
            Practitioner orderingPhysician,
            Facility orderingFacility,
            DateTime schedulingRequestTime)
        {
            Platform.CheckForNullReference(patient, "patient");
            Platform.CheckForNullReference(visit, "visit");
            Platform.CheckForNullReference(diagnosticService, "diagnosticService");
            Platform.CheckForNullReference(priority, "priority");
            Platform.CheckForNullReference(orderingPhysician, "orderingPhysician");
            Platform.CheckForNullReference(orderingFacility, "orderingFacility");

            this.CurrentContext.Lock(patient);
            this.CurrentContext.Lock(visit);
            this.CurrentContext.Lock(diagnosticService);
            this.CurrentContext.Lock(orderingPhysician);
            this.CurrentContext.Lock(orderingFacility);

            IAccessionNumberBroker broker = this.CurrentContext.GetBroker<IAccessionNumberBroker>();
            string accNum = broker.GetNextAccessionNumber();

            Order order = Order.NewOrder(
                accNum, patient, visit, diagnosticService, schedulingRequestTime, orderingPhysician, orderingFacility, priority);

            this.CurrentContext.Lock(order, DirtyState.New);
        }

        #endregion
    }
}
