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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
	public class OrderEntryAssembler
	{
		public OrderRequisition CreateOrderRequisition(Order order, IPersistenceContext context)
		{
			var visitAssembler = new VisitAssembler();
			var pracAssembler = new ExternalPractitionerAssembler();
			var facilityAssembler = new FacilityAssembler();
			var dsAssembler = new DiagnosticServiceAssembler();
			var attachmentAssembler = new OrderAttachmentAssembler();
			var noteAssembler = new OrderNoteAssembler();
			var resultRecipientAssembler = new ResultRecipientAssembler();

			var requisition = new OrderRequisition
				{
					Patient = order.Patient.GetRef(),
					Visit = visitAssembler.CreateVisitSummary(order.Visit, context),
					DiagnosticService = dsAssembler.CreateSummary(order.DiagnosticService),
					SchedulingRequestTime = order.SchedulingRequestTime,
					OrderingPractitioner = pracAssembler.CreateExternalPractitionerSummary(order.OrderingPractitioner, context),
					OrderingFacility = facilityAssembler.CreateFacilitySummary(order.OrderingFacility),
					ReasonForStudy = order.ReasonForStudy,
					Priority = EnumUtils.GetEnumValueInfo(order.Priority, context),
					ResultRecipients = CollectionUtils.Map<ResultRecipient, ResultRecipientDetail>(
						order.ResultRecipients,
						r => resultRecipientAssembler.CreateResultRecipientDetail(r, context)),
					Procedures = CollectionUtils.Map<Procedure, ProcedureRequisition>(
						order.Procedures,
						procedure => CreateProcedureRequisition(procedure, context)),
					Attachments = CollectionUtils.Map<OrderAttachment, OrderAttachmentSummary>(
						order.Attachments,
						attachment => attachmentAssembler.CreateOrderAttachmentSummary(attachment, context)),
					Notes = CollectionUtils.Map<OrderNote, OrderNoteDetail>(
						OrderNote.GetNotesForOrder(order),
						note => noteAssembler.CreateOrderNoteDetail(note, context)),
					ExtendedProperties = ExtendedPropertyUtils.Copy(order.ExtendedProperties)
				};

			return requisition;
		}

		public void UpdateOrderFromRequisition(Order order, OrderRequisition requisition, Staff currentStaff, IPersistenceContext context)
		{
			// only certain properties of an order may be updated from a requisition
			// Patient cannot not be updated
			// DiagnosticService cannot be updated
			// OrderingFacility cannot be updated

			// do not update the individual procedures, as this is done separately - see UpdateProcedureFromRequisition

			// Some properties cannot be updated if the procedure is terminated
			if (!order.IsTerminated)
			{
				order.Visit = context.Load<Visit>(requisition.Visit.VisitRef, EntityLoadFlags.Proxy);

				order.SchedulingRequestTime = requisition.SchedulingRequestTime;
				order.OrderingPractitioner = context.Load<ExternalPractitioner>(
					requisition.OrderingPractitioner.PractitionerRef, EntityLoadFlags.Proxy);
				order.ReasonForStudy = requisition.ReasonForStudy;
				order.Priority = EnumUtils.GetEnumValue<OrderPriority>(requisition.Priority);

				// wipe out and reset the result recipients
				order.ResultRecipients.Clear();

				CollectionUtils.Map<ResultRecipientDetail, ResultRecipient>(
					requisition.ResultRecipients,
					s => new ResultRecipient(
							context.Load<ExternalPractitionerContactPoint>(s.ContactPoint.ContactPointRef, EntityLoadFlags.Proxy),
							EnumUtils.GetEnumValue<ResultCommunicationMode>(s.PreferredCommunicationMode))).ForEach(r => order.ResultRecipients.Add(r));
			}

			// synchronize Order.Attachments from order requisition
			var attachmentAssembler = new OrderAttachmentAssembler();
			attachmentAssembler.Synchronize(order.Attachments, requisition.Attachments, currentStaff, context);

			// synchronize Order.Notes from order requisition
			var noteAssembler = new OrderNoteAssembler();
			noteAssembler.SynchronizeOrderNotes(order, requisition.Notes, currentStaff, context);

			if (requisition.ExtendedProperties != null)
			{
				ExtendedPropertyUtils.Update(order.ExtendedProperties, requisition.ExtendedProperties);
			}
		}

		public ProcedureRequisition CreateProcedureRequisition(Procedure procedure, IPersistenceContext context)
		{
			var procedureTypeAssembler = new ProcedureTypeAssembler();
			var facilityAssembler = new FacilityAssembler();
			var departmentAssembler = new DepartmentAssembler();

			// create requisition
			return new ProcedureRequisition(
				procedureTypeAssembler.CreateSummary(procedure.Type),
				procedure.Index,
				procedure.ScheduledStartTime,
				EnumUtils.GetEnumValueInfo(procedure.SchedulingCode),
				procedure.PerformingFacility == null ? null : facilityAssembler.CreateFacilitySummary(procedure.PerformingFacility),
				procedure.PerformingDepartment == null ? null : departmentAssembler.CreateSummary(procedure.PerformingDepartment, context),
				EnumUtils.GetEnumValueInfo(procedure.Laterality, context),
				procedure.Portable,
				procedure.ProcedureCheckIn.IsPreCheckIn == false,
				EnumUtils.GetEnumValueInfo(procedure.Status, context),
				IsProcedureModifiable(procedure),
				procedure.Status == ProcedureStatus.CA || procedure.Status == ProcedureStatus.DC);
		}

		public void UpdateProcedureFromRequisition(Procedure procedure, ProcedureRequisition requisition, IPersistenceContext context)
		{
			// check if the procedure was cancelled
			if (requisition.Cancelled)
			{
				if (procedure.Status == ProcedureStatus.SC)
				{
					// if RP is still scheduled, cancel it
					procedure.Cancel();
				}
				else if (procedure.Status == ProcedureStatus.IP)
				{
					// if RP in-progress, discontinue it
					procedure.Discontinue();
				}

				// early exit - nothing else to update
				return;
			}

			procedure.Schedule(requisition.ScheduledTime);
			procedure.SchedulingCode = EnumUtils.GetEnumValue<SchedulingCodeEnum>(requisition.SchedulingCode, context);

			procedure.PerformingFacility = context.Load<Facility>(requisition.PerformingFacility.FacilityRef, EntityLoadFlags.Proxy);
			procedure.PerformingDepartment = requisition.PerformingDepartment == null ? null
				: context.Load<Department>(requisition.PerformingDepartment.DepartmentRef, EntityLoadFlags.Proxy);

			procedure.Laterality = EnumUtils.GetEnumValue<Laterality>(requisition.Laterality);
			procedure.Portable = requisition.PortableModality;

			if (requisition.CheckedIn && procedure.ProcedureCheckIn.IsPreCheckIn)
			{
				procedure.ProcedureCheckIn.CheckIn(null);
			}
			else if (!requisition.CheckedIn && procedure.ProcedureCheckIn.IsCheckedIn)
			{
				procedure.ProcedureCheckIn.RevertCheckIn();
			}
		}

		// arguably this is a business logic decision that shouldn't go here, but there is really no
		// better place to put it right now
		// note that the notion of "modifiable" here is specific to the idea of a "requisition"
		// The "requisition" is modifiable only as long as the procedure is in the SC status
		private bool IsProcedureModifiable(Procedure procedure)
		{
			return procedure.Status == ProcedureStatus.SC;
		}
	}
}
