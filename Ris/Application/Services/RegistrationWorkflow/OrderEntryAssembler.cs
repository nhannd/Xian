using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Services.Admin;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    public class OrderEntryAssembler
    {
        public OrderSummary CreateOrderSummary(WorklistQueryResult result, IPersistenceContext context)
        {
            OrderSummary summary = new OrderSummary();

            summary.OrderRef = result.Order;
            summary.MrnId = result.Mrn.Id;
            summary.MrnAssigningAuthority = result.Mrn.AssigningAuthority;
            summary.VisitId = result.VisitNumber.Id;
            summary.VisitAssigningAuthority = result.VisitNumber.AssigningAuthority;
            summary.AccessionNumber = result.AccessionNumber;
            summary.DiagnosticServiceName = result.DiagnosticService;
            summary.RequestedProcedureName = result.RequestedProcedureName;
            summary.ModalityProcedureStepName = result.ModalityProcedureStepName;
            summary.ModalityName = result.ModalityName;

            PersonNameAssembler personNameAssembler = new PersonNameAssembler();
            summary.PatientName = personNameAssembler.CreatePersonNameDetail(result.PatientName);

            OrderPriorityEnumTable priorityEnumTable = context.GetBroker<IOrderPriorityEnumBroker>().Load();
            summary.OrderPriority = priorityEnumTable[result.Priority].Values;

            return summary;
        }

        public DiagnosticServiceSummary CreateDiagnosticServiceSummary(DiagnosticService diagnosticService)
        {
            DiagnosticServiceSummary summary = new DiagnosticServiceSummary();

            summary.DiagnosticServiceRef = diagnosticService.GetRef();
            summary.Id = diagnosticService.Id;
            summary.Name = diagnosticService.Name;

            return summary;            
        }

        public DiagnosticServiceDetail CreateDiagnosticServiceDetail(DiagnosticService diagnosticService)
        {
            DiagnosticServiceDetail detail = new DiagnosticServiceDetail();

            detail.Id = diagnosticService.Id;
            detail.Name = diagnosticService.Name;
            detail.RequestedProcedureTypes =
                CollectionUtils.Map<RequestedProcedureType, RequestedProcedureTypeDetail, List<RequestedProcedureTypeDetail>>(
                    diagnosticService.RequestedProcedureTypes,
                    delegate(RequestedProcedureType rpType)
                    {
                        return CreateRequestedProcedureTypeDetail(rpType);
                    });

            return detail;
        }

        public RequestedProcedureTypeDetail CreateRequestedProcedureTypeDetail(RequestedProcedureType requestedProcedureType)
        {
            RequestedProcedureTypeDetail detail = new RequestedProcedureTypeDetail();

            detail.Id = requestedProcedureType.Id;
            detail.Name = requestedProcedureType.Name;
            detail.ModalityProcedureStepTypes =
                CollectionUtils.Map<ModalityProcedureStepType, ModalityProcedureStepTypeDetail, List<ModalityProcedureStepTypeDetail>>(
                    requestedProcedureType.ModalityProcedureStepTypes,
                    delegate(ModalityProcedureStepType mpsType)
                    {
                        return CreateModalityProcedureStepTypeDetail(mpsType);
                    });

            return detail;
        }

        public ModalityProcedureStepTypeDetail CreateModalityProcedureStepTypeDetail(ModalityProcedureStepType modalityProcedureStepType)
        {
            ModalityProcedureStepTypeDetail detail = new ModalityProcedureStepTypeDetail();
            ModalityAssembler assembler = new ModalityAssembler();

            detail.Id = modalityProcedureStepType.Id;
            detail.Name = modalityProcedureStepType.Name;
            detail.Modalities =
                CollectionUtils.Map<Modality, ModalityDetail, List<ModalityDetail>>(
                    modalityProcedureStepType.DefaultModality,
                    delegate(Modality m)
                    {
                        return assembler.CreateModalityDetail(m);
                    });

            return detail;
        }
    }
}
