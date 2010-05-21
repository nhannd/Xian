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

using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// ProcedureEditorComponentBase class
	/// </summary>
	public abstract class ProcedureEditorComponentBase : ApplicationComponent
	{
		private readonly List<FacilitySummary> _facilityChoices;
		private readonly List<DepartmentSummary> _allDepartments;
		private List<DepartmentSummary> _departmentChoices;
		private readonly DepartmentSummary _departmentNone = new DepartmentSummary(null, SR.DummyItemNone, null, null, true);
		private readonly List<EnumValueInfo> _lateralityChoices;
		private readonly List<EnumValueInfo> _schedulingCodeChoices;

		private DateTime? _scheduledDateTime;
		private FacilitySummary _selectedFacility;
		private DepartmentSummary _selectedDepartment;
		private EnumValueInfo _selectedLaterality;
		private EnumValueInfo _selectedSchedulingCode;
		private bool _portableModality;
		private bool _checkedIn;

		protected ProcedureEditorComponentBase(
			List<FacilitySummary> facilityChoices,
			List<DepartmentSummary> departmentChoices,
			List<EnumValueInfo> lateralityChoices,
			List<EnumValueInfo> schedulingCodeChoices)
		{
			Platform.CheckForNullReference(facilityChoices, "facilityChoices");
			Platform.CheckForNullReference(departmentChoices, "departmentChoices");
			Platform.CheckForNullReference(lateralityChoices, "lateralityChoices");
			Platform.CheckForNullReference(schedulingCodeChoices, "schedulingCodeChoices");

			_facilityChoices = facilityChoices;
			_allDepartments = departmentChoices;
			_lateralityChoices = lateralityChoices;
			_schedulingCodeChoices = schedulingCodeChoices;
		}

		public override void Start()
		{
			LoadFromRequisition();

			// update department choices based on selected facility
			UpdateDepartmentChoices();

			base.Start();
		}

		protected abstract void LoadFromRequisition();

		protected abstract void UpdateRequisition();

		#region Presentation Model

		public virtual bool IsScheduledDateTimeEditable
		{
			get { return true; }
			set { }
		}

		public virtual bool IsPerformingFacilityEditable
		{
			get { return true; }
			set { }
		}

		public virtual bool IsPerformingDepartmentEditable
		{
			get { return true; }
			set { }
		}

		public virtual bool IsLateralityEditable
		{
			get { return true; }
			set { }
		}

		public virtual bool IsSchedulingCodeEditable
		{
			get { return true; }
			set { }
		}

		public virtual bool IsPortableEditable
		{
			get { return true; }
			set { }
		}

		public virtual bool IsCheckedInEditable
		{
			get { return true; }
			set { }
		}

		public IList FacilityChoices
		{
			get { return _facilityChoices; }
		}

		public string FormatFacility(object facility)
		{
			return ((FacilitySummary) facility).Name;
		}

		[ValidateNotNull]
		public FacilitySummary SelectedFacility
		{
			get { return _selectedFacility; }
			set
			{
				if (Equals(value, _selectedFacility))
					return;

				_selectedFacility = value;
				NotifyPropertyChanged("SelectedFacility");

				UpdateDepartmentChoices();
				NotifyPropertyChanged("DepartmentChoicesChanged");

				// clear selection
				this.SelectedDepartment = null;
			}
		}

		public IList DepartmentChoices
		{
			get { return _departmentChoices; }
		}

		public string FormatDepartment(object department)
		{
			return (department == _departmentNone || department == null) ? "" : ((DepartmentSummary)department).Name;
		}

		public DepartmentSummary SelectedDepartment
		{
			get { return _selectedDepartment; }
			set
			{
				if (Equals(value, _selectedDepartment))
					return;

				// It's important to convert _departemntNone to null here, in order for "not-null" custom validation rules
				// to behave as expected
				_selectedDepartment = Equals(value, _departmentNone) ? null : value;
				NotifyPropertyChanged("SelectedDepartment");
			}
		}

		public IList LateralityChoices
		{
			get { return _lateralityChoices; }
		}

		public EnumValueInfo SelectedLaterality
		{
			get { return _selectedLaterality; }
			set
			{
				if (Equals(value, _selectedLaterality))
					return;

				_selectedLaterality = value;
				NotifyPropertyChanged("SelectedLaterality");
			}
		}

		public IList SchedulingCodeChoices
		{
			get { return _schedulingCodeChoices; }
		}

		public EnumValueInfo SelectedSchedulingCode
		{
			get { return _selectedSchedulingCode; }
			set
			{
				if (Equals(value, _selectedSchedulingCode))
					return;

				_selectedSchedulingCode = value;
				NotifyPropertyChanged("SelectedSchedulingCode");
			}
		}

		public DateTime? ScheduledDate
		{
			get { return _scheduledDateTime; }
			set
			{
				if (value == _scheduledDateTime)
					return;

				_scheduledDateTime = value;
				NotifyPropertyChanged("ScheduledDate");
				NotifyPropertyChanged("ScheduledTime");
			}
		}

		public DateTime? ScheduledTime
		{
			get { return this.ScheduledDate; }
			set { this.ScheduledDate = value; }
		}

		public bool PortableModality
		{
			get { return _portableModality; }
			set
			{
				if (value == _portableModality)
					return;

				_portableModality = value;
				NotifyPropertyChanged("PortableModality");
			}
		}

		public bool CheckedIn
		{
			get { return _checkedIn; }
			set
			{
				if (value == _checkedIn)
					return;

				_checkedIn = value;
				NotifyPropertyChanged("CheckedIn");
			}
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			UpdateRequisition();

			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		private void UpdateDepartmentChoices()
		{
			_departmentChoices = new List<DepartmentSummary>{_departmentNone};

			// limit department choices to those that are associated with the selected performing facility
			if (_selectedFacility == null)
				return;

			var departmentsForSelectedFacility = CollectionUtils.Select(_allDepartments, d => d.FacilityCode == _selectedFacility.Code);
			_departmentChoices.AddRange(departmentsForSelectedFacility);
		}

	}
}
