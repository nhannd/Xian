#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
    public class DiagnosticServiceAssembler
    {
        public DiagnosticServiceSummary CreateSummary(DiagnosticService diagnosticService)
        {
            return new DiagnosticServiceSummary(
                diagnosticService.GetRef(),
                diagnosticService.Id,
                diagnosticService.Name,
				diagnosticService.Deactivated);
        }

        public DiagnosticServiceDetail CreateDetail(DiagnosticService diagnosticService)
        {
            ProcedureTypeAssembler rptAssembler = new ProcedureTypeAssembler();
            return new DiagnosticServiceDetail(
                diagnosticService.GetRef(),
                diagnosticService.Id,
                diagnosticService.Name,
                CollectionUtils.Map<ProcedureType, ProcedureTypeSummary>(
                    diagnosticService.ProcedureTypes,
                    delegate(ProcedureType rpType)
                    {
						return rptAssembler.CreateSummary(rpType);
                    }),
				diagnosticService.Deactivated);
        }

		public void UpdateDiagnosticService(DiagnosticService ds, DiagnosticServiceDetail detail, IPersistenceContext context)
		{
			ds.Id = detail.Id;
			ds.Name = detail.Name;
			ds.Deactivated = detail.Deactivated;

			ds.ProcedureTypes.Clear();
			ds.ProcedureTypes.AddAll(
				CollectionUtils.Map<ProcedureTypeSummary, ProcedureType>(
					detail.ProcedureTypes,
					delegate(ProcedureTypeSummary pt)
					{
						return context.Load<ProcedureType>(pt.ProcedureTypeRef, EntityLoadFlags.Proxy);
					}));
		}
    }
}
