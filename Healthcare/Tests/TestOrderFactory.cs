using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.Tests
{
    internal static class TestOrderFactory
    {
        internal static Order CreateOrder(int numRequestedProcedures, int numMpsPerRequestedProcedure, bool scheduleOrder)
        {
            DateTime scheduleTime = DateTime.Now;

            Patient patient = TestPatientFactory.CreatePatient();
            Visit visit = TestVisitFactory.CreateVisit(patient);
            DiagnosticService ds = TestDiagnosticServiceFactory.CreateDiagnosticService(numRequestedProcedures, numMpsPerRequestedProcedure);
            string accession = "10000001";
            ExternalPractitioner orderingPrac = TestExternalPractitionerFactory.CreatePractitioner();
            Facility facility = TestFacilityFactory.CreateFacility();

            return Order.NewOrder(
                accession,
                patient,
                visit,
                ds,
                scheduleTime,
                orderingPrac,
                facility,
                OrderPriority.R,
                scheduleOrder);
        }
    }
}
