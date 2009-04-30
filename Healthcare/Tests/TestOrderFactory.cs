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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Tests
{
    internal static class TestOrderFactory
    {
        internal static Order CreateOrder(int numProcedures, int numMpsPerProcedure, bool createProcedureSteps)
        {
            return CreateOrder(numProcedures, numMpsPerProcedure, createProcedureSteps, true);
        }
        internal static Order CreateOrder(int numProcedures, int numMpsPerProcedure, bool createProcedureSteps, bool schedule)
        {
            DateTime? scheduleTime = DateTime.Now;

            Patient patient = TestPatientFactory.CreatePatient();
            Visit visit = TestVisitFactory.CreateVisit(patient);
            DiagnosticService ds = TestDiagnosticServiceFactory.CreateDiagnosticService(numProcedures);
            string accession = "10000001";
            string reasonForStudy = "Test";
            ExternalPractitioner orderingPrac = TestExternalPractitionerFactory.CreatePractitioner();
            Facility facility = TestFacilityFactory.CreateFacility();

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
                new List<ResultRecipient>()));

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
                    foreach (ProcedureStep step in proc.ProcedureSteps)
                    {
                        if(!step.IsPreStep)
                            step.Schedule(dt);
                    }
                }
            }

            return order;
        }

        private static void AddProcedureSteps(Procedure procedure, int numMps)
        {
            Modality m = new Modality("01", "CT");

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
