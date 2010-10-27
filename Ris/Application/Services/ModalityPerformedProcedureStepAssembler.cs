#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Application.Services
{
	public class ModalityPerformedProcedureStepAssembler
	{
		public ModalityPerformedProcedureStepDetail CreateModalityPerformedProcedureStepDetail(ModalityPerformedProcedureStep mpps, IPersistenceContext context)
		{
			var assembler = new ModalityProcedureStepAssembler();

			// include the details of each MPS in the mpps summary
			var mpsDetails = CollectionUtils.Map(mpps.Activities,
				(ProcedureStep mps) => assembler.CreateProcedureStepSummary(mps.As<ModalityProcedureStep>(), context));

			var dicomSeriesAssembler = new DicomSeriesAssembler();
			var dicomSeries = dicomSeriesAssembler.GetDicomSeriesDetails(mpps.DicomSeries);

			StaffSummary mppsPerformer = null;
			var performer = mpps.Performer as ProcedureStepPerformer;
			if (performer != null)
			{
				var staffAssembler = new StaffAssembler();
				mppsPerformer = staffAssembler.CreateStaffSummary(performer.Staff, context);
			}

			return new ModalityPerformedProcedureStepDetail(
				mpps.GetRef(),
				EnumUtils.GetEnumValueInfo(mpps.State, context),
				mpps.StartTime,
				mpps.EndTime,
				mppsPerformer,
				mpsDetails,
				dicomSeries,
				ExtendedPropertyUtils.Copy(mpps.ExtendedProperties));
		}
	}
}
