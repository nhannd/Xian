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
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	//TODO: at some point in the future, expand to a full blown auto reconciler that just wraps the Ris' reconciliation service.

	[Cloneable(true)]
	public class PatientInformation
	{
		private string _patientId;
		//private string _patientsName;
		//private string _patientsBirthDate;
		//private string _patientsBirthTime;
		//private string _patientsSex;

		internal PatientInformation()
		{
		}

		internal PatientInformation(Patient patient)
		{
			_patientId = patient.PatientId;
			//_patientsName = patient.PatientsName;
			//_patientsSex = patient.PatientsSex;
		}

		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}

		//public string PatientsName
		//{
		//    get { return _patientsName; }
		//    set { _patientsName = value; }
		//}

		//public string PatientsBirthDate
		//{
		//    get { return _patientsBirthDate; }
		//    set { _patientsBirthDate = value; }
		//}

		//public string PatientsBirthTime
		//{
		//    get { return _patientsBirthTime; }
		//    set { _patientsBirthTime = value; }
		//}

		//public string PatientsSex
		//{
		//    get { return _patientsSex; }
		//    set { _patientsSex = value; }
		//}

		public PatientInformation Clone()
		{
			return CloneBuilder.Clone(this) as PatientInformation;
		}
	}

	internal interface IReconciliationAction : IXmlAction
	{
		void Apply(XmlElement context, PatientInformation item);
	}

	internal abstract class ReconciliationAction<TActionContext> : XmlAction<PatientInformation, TActionContext>, IReconciliationAction where TActionContext : class, new()
	{
		protected ReconciliationAction()
		{
		}

		#region IReconciliationAction Members

		public abstract void Apply(XmlElement actionElement, PatientInformation item);

		#endregion
	}

	internal class DefaultPatientReconciliationStrategy
	{
		private readonly XmlActionsApplicator _applicator;

		public DefaultPatientReconciliationStrategy()
		{
			_applicator = new XmlActionsApplicator(DefaultActions.GetStandardActions());
		}

		public PatientInformation ReconcileSearchCriteria(PatientInformation patient)
		{
			return Reconcile(patient, DefaultPatientReconciliationSettings.Default.SearchReconciliationRulesXml, "search-reconciliation-rules");
		}

		public PatientInformation ReconcilePatientInformation(PatientInformation patient)
		{
			return Reconcile(patient, DefaultPatientReconciliationSettings.Default.PatientReconciliationRulesXml, "patient-reconciliation-rules");
		}

		private PatientInformation Reconcile(PatientInformation patient, XmlDocument rulesDocument, string rulesElementName)
		{
			PatientInformation returnPatient = patient.Clone();
			if (String.IsNullOrEmpty(patient.PatientId))
				return returnPatient;

			returnPatient.PatientId = returnPatient.PatientId.Trim();

			XmlElement rulesNode = rulesDocument.SelectSingleNode("//" + rulesElementName) as XmlElement;
			if (rulesNode != null)
			{
				foreach (XmlNode ruleNode in rulesNode.SelectNodes("rule"))
				{
					XmlElement ruleElement = ruleNode as XmlElement;
					if (ruleElement != null)
					{
						if (_applicator.Apply(ruleElement, returnPatient))
							break;
					}
				}
			}

			return returnPatient;
		}
	}
}
