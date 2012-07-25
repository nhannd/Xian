#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
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

		public ProcedureTypeDetail CreateDetail(ProcedureType procedureType)
		{
			// write plan to string
			string planXml;
			var sb = new StringBuilder();
			using (var writer = new XmlTextWriter(new StringWriter(sb)))
			{
				writer.Formatting = Formatting.Indented;
				procedureType.GetPlanXml().Save(writer);
				planXml = sb.ToString();
			}

			return new ProcedureTypeDetail(
				procedureType.GetRef(),
				procedureType.Id,
				procedureType.Name,
				procedureType.BaseType == null ? null : CreateSummary(procedureType.BaseType),
				planXml,
				procedureType.DefaultDuration,
				procedureType.Deactivated);
		}

		public void UpdateProcedureType(ProcedureType procType, ProcedureTypeDetail detail, IPersistenceContext context)
		{
			procType.Id = detail.Id;
			procType.Name = detail.Name;
			procType.BaseType = detail.BaseType == null
									? null
									: context.Load<ProcedureType>(detail.BaseType.ProcedureTypeRef, EntityLoadFlags.Proxy);
			procType.DefaultDuration = detail.DefaultDuration;
			procType.Deactivated = detail.Deactivated;

			try
			{
				var xmlPlan = new XmlDocument();
				xmlPlan.LoadXml(detail.PlanXml);
				procType.SetPlanXml(xmlPlan);
			}
			catch (XmlException e)
			{
				throw new RequestValidationException(string.Format("Procedure plan XML is invalid: {0}", e.Message));
			}
		}
	}
}
