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
		private readonly List<ProcedureTypeSummary> _procedureTypeChoices;
		private DefaultSuggestionProvider<ProcedureTypeSummary> _procedureTypeSuggestionProvider;
		private ProcedureTypeSummary _selectedProcedureType;

		private readonly ProcedureRequisition _requisition;

		/// <summary>
		/// Constructor for add mode.
		/// </summary>
		public ProcedureEditorComponent(
			ProcedureRequisition requisition,
			List<FacilitySummary> facilityChoices,
			List<DepartmentSummary> departmentChoices,
			List<EnumValueInfo> lateralityChoices,
			List<EnumValueInfo> schedulingCodeChoices,
			List<ProcedureTypeSummary> procedureTypeChoices)
			: base(facilityChoices, departmentChoices, lateralityChoices, schedulingCodeChoices)
		{
			Platform.CheckForNullReference(requisition, "requisition");
			Platform.CheckForNullReference(procedureTypeChoices, "procedureTypeChoices");

			_requisition = requisition;
			_procedureTypeChoices = procedureTypeChoices;
		}

		/// <summary>
		/// Constructor for edit mode.
		/// </summary>
		public ProcedureEditorComponent(
			ProcedureRequisition requisition,
			List<FacilitySummary> facilityChoices,
			List<DepartmentSummary> departmentChoices,
			List<EnumValueInfo> lateralityChoices,
			List<EnumValueInfo> schedulingCodeChoices)
			: this(requisition, facilityChoices, departmentChoices, lateralityChoices, schedulingCodeChoices, new List<ProcedureTypeSummary>())
		{
		}

		public override void Start()
		{
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

			_procedureTypeChoices.Sort((x, y) => x.Name.CompareTo(y.Name));

			_procedureTypeSuggestionProvider = new DefaultSuggestionProvider<ProcedureTypeSummary>(_procedureTypeChoices, FormatProcedureType);

			base.Start();
		}

		protected override void LoadFromRequisition()
		{
			_selectedProcedureType = _requisition.ProcedureType;

			this.ScheduledTime = _requisition.ScheduledTime;
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
			get { return _procedureTypeChoices.Count > 0; }
		}

		public override bool IsScheduledDateTimeEditable
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

		public ISuggestionProvider ProcedureTypeSuggestionProvider
		{
			get { return _procedureTypeSuggestionProvider; }
		}

		public string FormatProcedureType(object item)
		{
			var rpt = (ProcedureTypeSummary)item;
			return string.Format("{0} ({1})", rpt.Name, rpt.Id);
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
