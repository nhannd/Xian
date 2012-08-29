#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Xml;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
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
			if(procedureType.Plan.IsDefault)
			{
				var modalityAssembler = new ModalityAssembler();
				return new ProcedureTypeDetail(
					procedureType.GetRef(),
					procedureType.Id,
					procedureType.Name,
					procedureType.Plan.DefaultModality == null ? null : modalityAssembler.CreateModalitySummary(procedureType.Plan.DefaultModality),
					procedureType.DefaultDuration,
					procedureType.Deactivated);
			}

			return new ProcedureTypeDetail(
				procedureType.GetRef(),
				procedureType.Id,
				procedureType.Name,
				procedureType.BaseType == null ? null : CreateSummary(procedureType.BaseType),
				procedureType.Plan.ToString(),
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
				if(detail.CustomProcedurePlan)
				{
					procType.Plan = new ProcedurePlan(detail.PlanXml);
				}
				else
				{
					var modality = context.Load<Modality>(detail.DefaultModality.ModalityRef);
					procType.Plan = ProcedurePlan.CreateDefaultPlan(detail.Name, modality);
				}
			}
			catch (XmlException e)
			{
				throw new RequestValidationException(string.Format("Procedure plan XML is invalid: {0}", e.Message));
			}
		}
	}
}
