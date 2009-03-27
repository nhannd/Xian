using System;
using System.Xml;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
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

	internal class DefaultPatientReconciliationStrategy : IPatientAutoReconciliationService
	{
		private readonly XmlActionsApplicator _applicator;

		public DefaultPatientReconciliationStrategy()
		{
			_applicator = new XmlActionsApplicator(DefaultActions.GetStandardActions());
		}

		#region IPatientAutoReconciliationService Members

		public PatientInformation ReconcilePatient(PatientInformation patient)
		{
			PatientInformation returnPatient = patient.Clone();
			if (String.IsNullOrEmpty(patient.PatientId))
				return returnPatient;

			returnPatient.PatientId = returnPatient.PatientId.Trim();

			XmlDocument document = DefaultPatientReconciliationSettings.Default.RulesXml;
			XmlElement rulesNode = document.SelectSingleNode("//patient-reconciliation-rules") as XmlElement;
			if (rulesNode != null)
			{
				foreach(XmlNode ruleNode in rulesNode.SelectNodes("rule"))
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

		#endregion
	}
}
