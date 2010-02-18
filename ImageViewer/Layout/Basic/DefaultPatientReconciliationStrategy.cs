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
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	[Cloneable(true)]
	public class PatientInformation : IPatientData
	{
		internal PatientInformation()
		{
		}

		internal PatientInformation(IPatientData patientData)
		{
			PatientId = patientData.PatientId;
			PatientsName = patientData.PatientsName;
			PatientsBirthDate = patientData.PatientsBirthDate;
			PatientsBirthTime = patientData.PatientsBirthTime;
			PatientsSex = patientData.PatientsSex;
		}

		#region IPatientData Members

		public string PatientId { get; set; }
		public string PatientsName { get; private set; }
		public string PatientsBirthDate { get; private set; }
		public string PatientsBirthTime { get; private set; }
		public string PatientsSex { get; private set; }

		#endregion

		public PatientInformation Clone()
		{
			return CloneBuilder.Clone(this) as PatientInformation;
		}
	}

	//TODO: at some point in the future, expand to a full blown auto reconciler that just wraps the Ris' reconciliation service.

	internal interface IPatientReconciliationStrategy
	{
		//NOTE: I dislike doing this everywhere - need centralized study management.
		void SetStudyTree(StudyTree studyTree);

		IPatientData ReconcileSearchCriteria(IPatientData patient);
		IPatientData ReconcilePatientInformation(IPatientData patient);
	}

	internal class DefaultPatientReconciliationStrategy : IPatientReconciliationStrategy
	{
		#region PatientInformation class

		#endregion

		private readonly XmlActionsApplicator _applicator;

		public DefaultPatientReconciliationStrategy()
		{
			_applicator = new XmlActionsApplicator(DefaultActions.GetStandardActions());
		}

		private StudyTree StudyTree { get; set; }

		void IPatientReconciliationStrategy.SetStudyTree(StudyTree studyTree)
		{
			StudyTree = studyTree;		
		}

		public IPatientData ReconcileSearchCriteria(IPatientData patientInfo)
		{
			var patientInformation = new PatientInformation{ PatientId = patientInfo.PatientId };
			return Reconcile(patientInformation, DefaultPatientReconciliationSettings.Default.SearchReconciliationRulesXml, "search-reconciliation-rules");
		}

		public IPatientData ReconcilePatientInformation(IPatientData patientInfo)
		{
			Platform.CheckMemberIsSet(StudyTree, "StudyTree");

			var testPatientInformation = new PatientInformation{ PatientId = patientInfo.PatientId };
			testPatientInformation = Reconcile(testPatientInformation, DefaultPatientReconciliationSettings.Default.PatientReconciliationRulesXml, "patient-reconciliation-rules");

			foreach (var patient in StudyTree.Patients)
			{
				var reconciledPatientInfo = new PatientInformation { PatientId = patient.PatientId };
				reconciledPatientInfo = Reconcile(reconciledPatientInfo, DefaultPatientReconciliationSettings.Default.PatientReconciliationRulesXml, "patient-reconciliation-rules");

				if (reconciledPatientInfo.PatientId == testPatientInformation.PatientId)
					return new PatientInformation(patient) { PatientId = reconciledPatientInfo.PatientId };
			}

			return null;
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
