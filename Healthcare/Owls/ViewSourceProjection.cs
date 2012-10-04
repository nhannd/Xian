#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Defines additional <see cref="WorklistItemProjection"/> instances useful for OWLS views.
	/// </summary>
	public class ViewSourceProjection
	{
		/// <summary>
		/// Base view source projection for procedure-oriented views.
		/// </summary>
		public static readonly WorklistItemProjection ProcedureBase;

		/// <summary>
		/// Base view source projection for procedure-step oriented views.
		/// </summary>
		public static readonly WorklistItemProjection ProcedureStepBase;

		/// <summary>
		/// Base view source projection for the reporting view.
		/// </summary>
		public static readonly WorklistItemProjection Reporting;

		/// <summary>
		/// Base view source projection for the protocol view.
		/// </summary>
		public static readonly WorklistItemProjection Protocol;


		/// <summary>
		/// Type initializer.
		/// </summary>
		static ViewSourceProjection()
		{
			ProcedureBase = new WorklistItemProjection(
				new[]
					{
						WorklistItemField.Patient,
						WorklistItemField.PatientProfile,
						WorklistItemField.Visit,
						WorklistItemField.Order,
						WorklistItemField.DiagnosticService,
						WorklistItemField.Procedure,
						WorklistItemField.ProcedureType
					});

			ProcedureStepBase = ProcedureBase.AddFields(new[] { WorklistItemField.ProcedureCheckIn, WorklistItemField.ProcedureStep });
			Reporting = ProcedureStepBase.AddFields(new[] { WorklistItemField.ReportPart, WorklistItemField.Report });
			Protocol = ProcedureStepBase.AddFields(new[] { WorklistItemField.Protocol });
		}
	}
}
