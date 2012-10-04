#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Healthcare.Owls
{
	using UpdateViewItemDelegate = UpdateViewItemDelegate<WorklistViewItemBase>;


	/// <summary>
	/// ReportingWorklistViewItem entity
	/// </summary>
	public partial class ReportingWorklistViewItem
	{
		private static readonly Dictionary<WorklistItemField, UpdateViewItemDelegate> _fieldMappings
			= new Dictionary<WorklistItemField, UpdateViewItemDelegate>();

		static ReportingWorklistViewItem()
		{
			_fieldMappings.Add(WorklistItemField.ReportPart,
				(item, value, updateReferences) => ((ReportingWorklistViewItem)item).SetReportPartInfo((ReportPart)value));

			_fieldMappings.Add(WorklistItemField.Report,
				(item, value, updateReferences) => ((ReportingWorklistViewItem)item).SetReportInfo((Report)value));
		}

		public override void SetProcedureStepInfo(ProcedureStep step, bool updateReferences)
		{
			base.SetProcedureStepInfo(step, updateReferences);

			// the ReportPart is not neceesarily created when the reporting step is created (eg if it is an interpretation step)
			// hence we need to check and see if the report part info has never been set, then it needs to be set here
			var rps = step.Downcast<ReportingProcedureStep>();
			if (_reportPart == null && rps.ReportPart != null)
			{
				SetReportPartInfo(rps.ReportPart);
				SetReportInfo(rps.Report);
			}
		}

		public virtual void SetReportPartInfo(ReportPart rpp)
		{
			// rpp might be null because a reporting worklist item may not yet have an associated report, but broker does a left join
			if (rpp == null)
				return;

			_reportPart = new WorklistViewItemReportPartInfo(
				rpp,
				rpp.Version,
				rpp.Interpreter,
				rpp.Verifier,
				rpp.Supervisor,
				rpp.Transcriber,
				rpp.CompletedTime,
				rpp.PreliminaryTime,
				rpp.Index,
				rpp.Status);
		}

		public virtual void SetReportInfo(Report r)
		{
			// r might be null because a reporting worklist item may not yet have an associated report, but broker does a left join
			if (r == null)
				return;

			_report = new WorklistViewItemReportInfo(r, r.Version, r.Status);
		}

		protected override UpdateViewItemDelegate GetFieldUpdater(WorklistItemField field)
		{
			UpdateViewItemDelegate updater;
			return _fieldMappings.TryGetValue(field, out updater) ? updater : base.GetFieldUpdater(field);
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}