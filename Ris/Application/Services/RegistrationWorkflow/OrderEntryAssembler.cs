using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Services.Admin;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    public class OrderEntryAssembler
    {
        public OrderSummary CreateOrderSummary(WorklistItem item, IPersistenceContext context)
        {
            OrderSummary summary = new OrderSummary();

            //summary.OrderRef = item.Order;
            //summary.MrnId = item.Mrn.Id;
            //summary.MrnAssigningAuthority = item.Mrn.AssigningAuthority;
            //summary.VisitId = item.VisitNumber.Id;
            //summary.VisitAssigningAuthority = item.VisitNumber.AssigningAuthority;
            //summary.AccessionNumber = item.AccessionNumber;
            //summary.DiagnosticServiceName = item.DiagnosticService;
            //summary.RequestedProcedureName = item.RequestedProcedureName;
            //summary.ModalityProcedureStepName = item.ModalityProcedureStepName;
            //summary.ModalityName = item.ModalityName;

            //PersonNameAssembler personNameAssembler = new PersonNameAssembler();
            //summary.PatientName = personNameAssembler.CreatePersonNameDetail(item.PatientName);

            //OrderPriorityEnumTable priorityEnumTable = context.GetBroker<IOrderPriorityEnumBroker>().Load();
            //summary.OrderPriority = new EnumValueInfo(item.Priority.ToString(), priorityEnumTable[item.Priority].Value);

            return summary;
        }

        public DiagnosticServiceSummary CreateDiagnosticServiceSummary(DiagnosticService diagnosticService)
        {
            return new DiagnosticServiceSummary(
                diagnosticService.GetRef(),
                diagnosticService.Id,
                diagnosticService.Name);
        }

        public DiagnosticServiceDetail CreateDiagnosticServiceDetail(DiagnosticService diagnosticService)
        {
            return new DiagnosticServiceDetail(
                diagnosticService.Id,
                diagnosticService.Name,
                CollectionUtils.Map<RequestedProcedureType, RequestedProcedureTypeDetail, List<RequestedProcedureTypeDetail>>(
                    diagnosticService.RequestedProcedureTypes,
                    delegate(RequestedProcedureType rpType)
                    {
                        return CreateRequestedProcedureTypeDetail(rpType);
                    }));
        }

        public RequestedProcedureTypeDetail CreateRequestedProcedureTypeDetail(RequestedProcedureType requestedProcedureType)
        {
            return new RequestedProcedureTypeDetail(
                requestedProcedureType.Id,
                requestedProcedureType.Name,
                CollectionUtils.Map<ModalityProcedureStepType, ModalityProcedureStepTypeDetail, List<ModalityProcedureStepTypeDetail>>(
                    requestedProcedureType.ModalityProcedureStepTypes,
                    delegate(ModalityProcedureStepType mpsType)
                    {
                        return CreateModalityProcedureStepTypeDetail(mpsType);
                    }));
        }

        public ModalityProcedureStepTypeDetail CreateModalityProcedureStepTypeDetail(ModalityProcedureStepType modalityProcedureStepType)
        {
            ModalityAssembler assembler = new ModalityAssembler();
            return new ModalityProcedureStepTypeDetail(
                modalityProcedureStepType.Id,
                modalityProcedureStepType.Name,
                assembler.CreateModalityDetail(modalityProcedureStepType.DefaultModality));
        }
    }
}
