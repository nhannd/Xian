#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Linq;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Services.ReportingWorkflow
{
	class ReportingWorkflowServiceRecorder
	{
		static class Operations
		{
			public const string Preliminary = "Report:Preliminary";
			public const string Verified = "Report:Verified";
			public const string Corrected = "Report:Corrected";
			public const string Revised = "Report:Revised";
			public const string Discarded = "Report:Discarded";
		}

		// CompleteInterpretationForTranscription
		// CompleteInterpretationForVerification
		internal class CompleteInterpretation : RisServiceOperationRecorderBase
		{
			protected override bool ShouldCapture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (SaveReportRequest)recorderContext.Request;
				var rps = persistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.None);

				// only relevant for the main report, not for addendums (i.e. we are still in P status)
				return rps.ReportPart.Report.Status == ReportStatus.P && base.ShouldCapture(recorderContext, persistenceContext);
			}

			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (SaveReportRequest)recorderContext.Request;
				var rps = persistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.None);
				return ReportingWorkflowServiceRecorder.Capture(Operations.Preliminary, rps);
			}
		}

		// CompleteVerification
		// CompleteInterpretationAndVerify
		internal class CompleteVerification : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (SaveReportRequest)recorderContext.Request;
				var rps = persistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.None);

				// report will not have changed to F status yet, since it is not yet published
				// if we are in P status, this is a verification of the main report
				// if we are in F status, this is a verification of an addendum, hence a correction
				var operation = rps.ReportPart.Report.Status == ReportStatus.P ? Operations.Verified : Operations.Corrected;
				return ReportingWorkflowServiceRecorder.Capture(operation, rps);
			}
		}

		// ReviseUnpublishedReport
		internal class Revise : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (ReviseUnpublishedReportRequest)recorderContext.Request;
				var rps = persistenceContext.Load<ReportingProcedureStep>(request.PublicationStepRef, EntityLoadFlags.None);
				return ReportingWorkflowServiceRecorder.Capture(Operations.Revised, rps);
			}
		}

		// CancelReportingStep
		internal class Discard : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (CancelReportingStepRequest)recorderContext.Request;
				var rps = persistenceContext.Load<ReportingProcedureStep>(request.ReportingStepRef, EntityLoadFlags.None);
				return ReportingWorkflowServiceRecorder.Capture(Operations.Discarded, rps);
			}
		}

		private static RisServiceOperationRecorderBase.OperationData Capture(string operation, ReportingProcedureStep rps)
		{
			var reportPart = rps.ReportPart;
			var procedures = reportPart.Report.Procedures;
			var order = procedures.First().Order;
			var patientProfile = procedures.First().PatientProfile;

			return new RisServiceOperationRecorderBase.OperationData(operation, patientProfile, order, procedures);
		}
	}
}
