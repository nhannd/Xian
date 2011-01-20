#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;

namespace ClearCanvas.Ris.Application.Services
{
    public class ProcedurePlanAssembler
    {
        public ProcedurePlanDetail CreateProcedurePlanSummary(Order order, IPersistenceContext context)
        {
            ProcedurePlanDetail detail = new ProcedurePlanDetail();

            ProcedureAssembler assembler = new ProcedureAssembler();
            StaffAssembler staffAssembler = new StaffAssembler();

            detail.OrderRef = order.GetRef();
            detail.Procedures = CollectionUtils.Map<Procedure, ProcedureDetail>(
                order.Procedures,
                delegate(Procedure rp)
                {
                	return assembler.CreateProcedureDetail(
                		rp,
                		delegate(ProcedureStep ps) { return ps.Is<ModalityProcedureStep>(); }, // only MPS are relevant here
                		false,
                		context);
                });
            detail.DiagnosticServiceSummary =
                new DiagnosticServiceSummary(order.DiagnosticService.GetRef(), order.DiagnosticService.Id, order.DiagnosticService.Name, order.DiagnosticService.Deactivated);


            return detail;
        }
    }
}
