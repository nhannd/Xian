#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Registration
{
	public class Operations
	{
		public abstract class RegistrationOperation
		{
		}

		public class CheckIn : RegistrationOperation
		{
			public void Execute(Procedure rp, Staff checkInStaff, DateTime? checkInTime, IWorkflow workflow)
			{
				rp.CheckIn(checkInStaff, checkInTime);
			}
		}

		public class ReconcilePatient : RegistrationOperation
		{
			public void Execute(List<Patient> patientsToReconcile, IWorkflow workflow)
			{
				// reconcile all patients
				for (var i = 1; i < patientsToReconcile.Count; i++)
				{
					Reconcile(patientsToReconcile[0], patientsToReconcile[i], workflow);
				}
			}

			/// <summary>
			/// Reconciles the specified patient to this patient
			/// </summary>
			/// <param name="thisPatient"></param>
			/// <param name="otherPatient"></param>
			/// <param name="workflow"></param>
			private static void Reconcile(Patient thisPatient, Patient otherPatient, IWorkflow workflow)
			{
				if (PatientIdentifierConflictsFound(thisPatient, otherPatient))
					throw new PatientReconciliationException("assigning authority conflict - cannot reconcile");

				// copy the collection to iterate
				var otherProfiles = new List<PatientProfile>(otherPatient.Profiles);
				foreach (var profile in otherProfiles)
				{
					thisPatient.AddProfile(profile);
				}

				// copy the collection to iterate
				var otherNotes = new List<PatientNote>(otherPatient.Notes);
				foreach (var note in otherNotes)
				{
					thisPatient.AddNote(note);
				}

				// copy the collection to iterate
				var otherAttachments = new List<PatientAttachment>(otherPatient.Attachments);
				foreach (var attachment in otherAttachments)
				{
					otherPatient.Attachments.Remove(attachment);
					thisPatient.Attachments.Add(attachment);
				}

				// copy the collection to iterate
				var otherAllergies = new List<Allergy>(otherPatient.Allergies);
				foreach (var allergy in otherAllergies)
				{
					otherPatient.Allergies.Remove(allergy);
					thisPatient.Allergies.Add(allergy);
				}

				var visitCriteria = new VisitSearchCriteria();
				visitCriteria.Patient.EqualTo(otherPatient);
				var otherVisits = workflow.GetBroker<IVisitBroker>().Find(visitCriteria);
				foreach (var visit in otherVisits)
				{
					visit.Patient = thisPatient;
				}

				var orderCriteria = new OrderSearchCriteria();
				orderCriteria.Patient.EqualTo(otherPatient);
				var otherOrders = workflow.GetBroker<IOrderBroker>().Find(orderCriteria);
				foreach (var order in otherOrders)
				{
					order.Patient = thisPatient;
				}
			}

			/// <summary>
			/// Check if any of the profiles in each patient have an Mrn with the same assigning authority.
			/// </summary>
			/// <param name="thisPatient"></param>
			/// <param name="otherPatient"></param>
			/// <returns>Returns true if any profiles for the other patient and any profiles for this patient have an Mrn with the same assigning authority.</returns>
			private static bool PatientIdentifierConflictsFound(Patient thisPatient, Patient otherPatient)
			{
				foreach (var x in thisPatient.Profiles)
					foreach (var y in otherPatient.Profiles)
						if (x.Mrn.AssigningAuthority.Equals(y.Mrn.AssigningAuthority))
							return true;

				return false;
			}
		}
	}
}
