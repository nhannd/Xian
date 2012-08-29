#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using System.Linq;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using System.Xml;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
	public class ProcedureTypeAssembler
	{
		public ProcedureTypeSummary CreateSummary(ProcedureType rpt)
		{
			return new ProcedureTypeSummary(rpt.GetRef(), rpt.Name, rpt.Id, rpt.DefaultDuration, rpt.Deactivated);
		}

		public ProcedureTypeDetail CreateDetail(ProcedureType procedureType, IPersistenceContext context)
		{
			Modality defaultModality;
			if(IsDefaultPlan(context, procedureType.GetPlanXml(), out defaultModality))
			{
				var modalityAssembler = new ModalityAssembler();
				return new ProcedureTypeDetail(
					procedureType.GetRef(),
					procedureType.Id,
					procedureType.Name,
					defaultModality == null ? null : modalityAssembler.CreateModalitySummary(defaultModality),
					procedureType.DefaultDuration,
					procedureType.Deactivated);
			}

			return new ProcedureTypeDetail(
				procedureType.GetRef(),
				procedureType.Id,
				procedureType.Name,
				procedureType.BaseType == null ? null : CreateSummary(procedureType.BaseType),
				WritePlanToString(procedureType),
				procedureType.DefaultDuration,
				procedureType.Deactivated);
		}

		public void UpdateProcedureType(ProcedureType procType, ProcedureTypeDetail detail, IPersistenceContext context)
		{
			procType.Id = detail.Id;
			procType.Name = detail.Name;
			procType.BaseType = detail.CustomProcedurePlan && detail.BaseType != null
									? context.Load<ProcedureType>(detail.BaseType.ProcedureTypeRef, EntityLoadFlags.Proxy)
									: null;
			procType.DefaultDuration = detail.DefaultDuration;
			procType.Deactivated = detail.Deactivated;

			try
			{
				procType.SetPlanXml(GetProcedurePlan(detail));
			}
			catch (XmlException e)
			{
				throw new RequestValidationException(string.Format("Procedure plan XML is invalid: {0}", e.Message));
			}
		}

		private static bool IsDefaultPlan(IPersistenceContext context, XmlDocument planXml, out Modality defaultModality)
		{
			var type = planXml.DocumentElement == null ? null : planXml.DocumentElement.GetAttribute("type");
			if(type == "default")
			{
				var mpsNodes = planXml.SelectNodes("procedure-plan/procedure-steps/procedure-step[@class='ClearCanvas.Healthcare.ModalityProcedureStep']");
				var mpsNode = mpsNodes == null ? null : mpsNodes.Cast<XmlElement>().FirstOrDefault();
				if(mpsNode != null)
				{
					var where = new ModalitySearchCriteria();
					where.Id.EqualTo(mpsNode.GetAttribute("modality"));
					defaultModality = context.GetBroker<IModalityBroker>().FindOne(where);
					return true;
				}
			}
			defaultModality = null;
			return false;
		}

		private static string WritePlanToString(ProcedureType procedureType)
		{
			string planXml;
			var sb = new StringBuilder();
			using (var writer = new XmlTextWriter(new StringWriter(sb)))
			{
				writer.Formatting = Formatting.Indented;
				procedureType.GetPlanXml().Save(writer);
				planXml = sb.ToString();
			}
			return planXml;
		}

		private static XmlDocument GetProcedurePlan(ProcedureTypeDetail detail)
		{
			var xmlPlan = new XmlDocument();
			if(detail.CustomProcedurePlan)
			{
				xmlPlan.LoadXml(detail.PlanXml);
				if(xmlPlan.DocumentElement != null)
				{
					xmlPlan.DocumentElement.SetAttribute("type", "custom");	// this is a custom plan
				}
			}
			else
			{
				if(detail.DefaultModality == null)
					throw new RequestValidationException("Default Modality must be specified.");

				// create the default procedure plan, which has exactly one MPS referencing the default modality
				var planNode = xmlPlan.CreateElement("procedure-plan");
				xmlPlan.AppendChild(planNode);
				planNode.SetAttribute("type", "default");	// this is the default plan

				var stepsNode = xmlPlan.CreateElement("procedure-steps");
				planNode.AppendChild(stepsNode);
				var stepNode = xmlPlan.CreateElement("procedure-step");
				stepsNode.AppendChild(stepNode);
				stepNode.SetAttribute("class", "ClearCanvas.Healthcare.ModalityProcedureStep");
				stepNode.SetAttribute("description", detail.Name);
				stepNode.SetAttribute("modality", detail.DefaultModality.Id);
			}
			return xmlPlan;
		}
	}
}
