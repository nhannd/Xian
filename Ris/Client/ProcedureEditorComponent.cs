#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ProcedureEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ProcedureEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ProcedureEditorComponent class
	/// </summary>
	[AssociateView(typeof(ProcedureEditorComponentViewExtensionPoint))]
	public class ProcedureEditorComponent : ProcedureEditorComponentBase
	{
		public enum Mode
		{
			Add,
			Edit
		}

		private readonly ProcedureRequisition _requisition;
		private readonly Mode _mode;

		private ProcedureTypeLookupHandler _procedureTypeLookupHandler;
		private ProcedureTypeSummary _selectedProcedureType;


		public ProcedureEditorComponent(
			ProcedureRequisition requisition,
			Mode mode,
			List<FacilitySummary> facilityChoices,
			List<DepartmentSummary> departmentChoices,
			List<EnumValueInfo> lateralityChoices,
			List<EnumValueInfo> schedulingCodeChoices)
			: base(facilityChoices, departmentChoices, lateralityChoices, schedulingCodeChoices)
		{
			Platform.CheckForNullReference(requisition, "requisition");

			_mode = mode;
			_requisition = requisition;
		}

		public override void Start()
		{
			_procedureTypeLookupHandler = new ProcedureTypeLookupHandler(this.Host.DesktopWindow);
			this.Validation.Add(new ValidationRule("CheckedIn",
				delegate
				{
					// This validation does not apply if the procedure is not checked in
					if (!this.CheckedIn)
						return new ValidationResult(true, "");

					string alertMessage;
					var checkInTime = Platform.Time;
					var success = CheckInSettings.ValidateResult.Success == CheckInSettings.Validate(this.ScheduledTime, checkInTime, out alertMessage);
					return new ValidationResult(success, alertMessage);
				}));

			base.Start();
		}

		protected override void LoadFromRequisition()
		{
			_selectedProcedureType = _requisition.ProcedureType;

			this.ScheduledTime = _requisition.ScheduledTime;
			this.ScheduledDuration = _requisition.ScheduledDuration;
			this.SelectedFacility = _requisition.PerformingFacility;
			this.SelectedDepartment = _requisition.PerformingDepartment;
			this.SelectedLaterality = _requisition.Laterality;
			this.SelectedSchedulingCode = _requisition.SchedulingCode;
			this.PortableModality = _requisition.PortableModality;
			this.CheckedIn = _requisition.CheckedIn;
		}

		protected override void UpdateRequisition()
		{
			_requisition.ProcedureType = _selectedProcedureType;
			_requisition.ScheduledTime = this.ScheduledTime;
			_requisition.ScheduledDuration = this.ScheduledDuration;
			_requisition.Laterality = this.SelectedLaterality;
			_requisition.SchedulingCode = this.SelectedSchedulingCode;
			_requisition.PerformingFacility = this.SelectedFacility;
			_requisition.PerformingDepartment = this.SelectedDepartment;
			_requisition.PortableModality = this.PortableModality;
			_requisition.CheckedIn = this.CheckedIn;
		}

		#region Presentation Model

		public bool IsProcedureTypeEditable
		{
			get { return _mode == Mode.Add; }
		}

		public override bool IsScheduledDateTimeEditable
		{
			get { return _requisition.Status == null || _requisition.Status.Code == "SC"; }
		}

		public override bool IsScheduledDurationEditable
		{
			get { return _requisition.Status == null || _requisition.Status.Code == "SC"; }
		}

		public override bool IsPerformingFacilityEditable
		{
			get { return _requisition.Status == null || _requisition.Status.Code == "SC"; }
		}

		public override bool IsPerformingDepartmentEditable
		{
			get { return _requisition.Status == null || _requisition.Status.Code == "SC"; }
		}

		public override bool IsCheckedInEditable
		{
			get { return _requisition.Status == null || _requisition.Status.Code == "SC"; }
		}

		public ILookupHandler ProcedureTypeLookupHandler
		{
			get { return _procedureTypeLookupHandler; }
		}

		[ValidateNotNull]
		public ProcedureTypeSummary SelectedProcedureType
		{
			get { return _selectedProcedureType; }
			set
			{
				if (Equals(value, _selectedProcedureType))
					return;

				_selectedProcedureType = value;
				NotifyPropertyChanged("SelectedProcedureType");
			}
		}

		#endregion
	}
}
