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
		static public bool PatientIdentifierConflictsFound(Patient thisPatient, Patient otherPatient)
		{
			foreach (PatientProfile x in thisPatient.Profiles)
				foreach (PatientProfile y in otherPatient.Profiles)
					if (x.Mrn.AssigningAuthority.Equals(y.Mrn.AssigningAuthority))
						return true;

			return false;
		}

		/// <summary>
		/// All pertinent data gets copied from otherPatient to thisPatient
		/// </summary>
		/// <param name="thisPatient"></param>
		/// <param name="otherPatient"></param>
		/// <param name="context"></param>
		static public void Reconcile(Patient thisPatient, Patient otherPatient, IPersistenceContext context)
		{
			if (Utility.PatientIdentifierConflictsFound(thisPatient, otherPatient))
				throw new PatientReconciliationException("assigning authority conflict - cannot reconcile");

			foreach (PatientProfile profile in otherPatient.Profiles)
			{
				thisPatient.AddProfile(profile);
			}

			foreach (PatientNote note in otherPatient.Notes)
			{
				thisPatient.Notes.Add((PatientNote)note.Clone());
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
