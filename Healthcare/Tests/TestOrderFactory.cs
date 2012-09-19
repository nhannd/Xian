#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Tests
{
	public static class TestOrderFactory
    {


		public static Order CreateOrder(int numProcedures, int numMpsPerProcedure, bool createProcedureSteps)
        {
            return CreateOrder(numProcedures, numMpsPerProcedure, createProcedureSteps, true);
        }
		public static Order CreateOrder(int numProcedures, int numMpsPerProcedure, bool createProcedureSteps, bool schedule)
		{
			Patient patient = TestPatientFactory.CreatePatient();
			Visit visit = TestVisitFactory.CreateVisit(patient);
			var facility = TestFacilityFactory.CreateFacility();
			return CreateOrder(patient, visit, facility, "10000001", numProcedures, numMpsPerProcedure, createProcedureSteps, schedule);
		}

		public static Order CreateOrder(Patient patient, Visit visit, string accession, int numProcedures, int numMpsPerProcedure, bool createProcedureSteps, bool schedule)
		{
			var facility = TestFacilityFactory.CreateFacility();
			return CreateOrder(patient, visit, facility, accession, numProcedures, numMpsPerProcedure, createProcedureSteps,
			                   schedule);
		}

		public static Order CreateOrder(Patient patient, Visit visit, Facility facility, string accession, int numProcedures, int numMpsPerProcedure, bool createProcedureSteps, bool schedule)
        {
			var procedureNumberBroker = new TestProcedureNumberBroker();
			var dicomUidBroker = new TestDicomUidBroker();
			DateTime? scheduleTime = DateTime.Now;

            DiagnosticService ds = TestDiagnosticServiceFactory.CreateDiagnosticService(numProcedures);
            string reasonForStudy = "Test";
            ExternalPractitioner orderingPrac = TestExternalPractitionerFactory.CreatePractitioner();

            Order order =  Order.NewOrder(new OrderCreationArgs(
				Platform.Time,
				TestStaffFactory.CreateStaff(new StaffTypeEnum("SCLR", null, null)),
				null,
                accession,
                patient,
                visit,
                ds,
                reasonForStudy,
                OrderPriority.R,
                facility,
                facility,
                scheduleTime,
                orderingPrac,
                new List<ResultRecipient>()),
				procedureNumberBroker,
				dicomUidBroker);

            if(createProcedureSteps)
            {
                foreach (Procedure proc in order.Procedures)
                {
                    AddProcedureSteps(proc, numMpsPerProcedure);
                }
            }

            DateTime dt = DateTime.Now;
            if(schedule)
            {
                foreach (Procedure proc in order.Procedures)
                {
                	proc.Schedule(dt);
                }
            }

            return order;
        }

        private static void AddProcedureSteps(Procedure procedure, int numMps)
        {
            Modality m = new Modality("01", "CT", null, null);

            for (int s = 0; s < numMps; s++)
            {
                ModalityProcedureStep step = new ModalityProcedureStep();
                step.Description = "MPS 10" + s;
                step.Modality = m;
                procedure.AddProcedureStep(step);
            }
        }
    }
}
