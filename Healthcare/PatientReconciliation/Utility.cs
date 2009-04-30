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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.PatientReconciliation
{
	public class Utility
	{
		static public bool PatientIdentifiersFromIdenticalInformationAuthority(Patient thisPatient, Patient otherPatient)
		{
			foreach (PatientProfile x in thisPatient.Profiles)
				foreach (PatientProfile y in otherPatient.Profiles)
					if (x.Mrn.AssigningAuthority.Equals(y.Mrn.AssigningAuthority))
						return true;

			return false;
		}

		static public void ReconcileBetweenInformationAuthorities(Patient thisPatient, Patient otherPatient, IPersistenceContext context)
		{
			if (Utility.PatientIdentifiersFromIdenticalInformationAuthority(thisPatient, otherPatient))
				throw new PatientReconciliationException("assigning authorities not identical conflict - cannot reconcile");

			// copy patient profiles over
			foreach (PatientProfile profile in otherPatient.Profiles)
				thisPatient.AddProfile(profile);

			ReconnectRelatedPatientInformation(thisPatient, otherPatient, context);
		}

		static public void ReconcileWithinInformationAuthority(Patient thisPatient, Patient otherPatient, IPersistenceContext context)
		{
			ReconnectRelatedPatientInformation(thisPatient, otherPatient, context);
		}

		/// <summary>
		/// All pertinent data other than the Profiles gets copied from otherPatient to thisPatient
		/// </summary>
		/// <param name="thisPatient"></param>
		/// <param name="otherPatient"></param>
		/// <param name="context"></param>
		static private void ReconnectRelatedPatientInformation(Patient thisPatient, Patient otherPatient, IPersistenceContext context)
		{
			foreach (PatientNote note in otherPatient.Notes)
			{
				thisPatient.AddNote(note);
			}

			OrderSearchCriteria orderCriteria = new OrderSearchCriteria();
			orderCriteria.Patient.EqualTo(otherPatient);
			IList<Order> otherOrders = context.GetBroker<IOrderBroker>().Find(orderCriteria);
			foreach (Order order in otherOrders)
			{
				order.Patient = thisPatient;
			}

			VisitSearchCriteria visitCriteria = new VisitSearchCriteria();
			visitCriteria.Patient.EqualTo(otherPatient);
			IList<Visit> otherVisits = context.GetBroker<IVisitBroker>().Find(visitCriteria);
			foreach (Visit visit in otherVisits)
			{
				visit.Patient = thisPatient;
			}

			// TODO: delete the otherPatient
		}

	}
}
