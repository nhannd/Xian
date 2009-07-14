#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
    public class ProcedureEditorComponent : ApplicationComponent
    {
        private readonly List<ProcedureTypeSummary> _procedureTypeChoices;
        private DefaultSuggestionProvider<ProcedureTypeSummary> _procedureTypeSuggestionProvider;
        private ProcedureTypeSummary _selectedProcedureType;
        private DateTime? _scheduledTime;

        private readonly ProcedureRequisition _requisition;
        private readonly List<FacilitySummary> _facilityChoices;
        private readonly List<EnumValueInfo> _lateralityChoices;
        private FacilitySummary _selectedFacility;
        private EnumValueInfo _selectedLaterality;
        private bool _portableModality;
        private bool _checkedIn;
        private readonly bool _isCheckedInEnabled;

        /// <summary>
        /// Constructor for add mode.
        /// </summary>
        public ProcedureEditorComponent(ProcedureRequisition requisition, List<FacilitySummary> facilityChoices, List<EnumValueInfo> lateralityChoices, List<ProcedureTypeSummary> procedureTypeChoices)
        {
            Platform.CheckForNullReference(requisition, "requisition");
            Platform.CheckForNullReference(procedureTypeChoices, "procedureTypeChoices");

            _requisition = requisition;
            _procedureTypeChoices = procedureTypeChoices;
            _facilityChoices = facilityChoices;
            _lateralityChoices = lateralityChoices;

            // if the requisition's procedure type is null, then it is a new procedure and checked in can be edited.
            _isCheckedInEnabled = _requisition.Status == null || _requisition.Status.Code == "SC";
        }

        /// <summary>
        /// Constructor for edit mode.
        /// </summary>
        public ProcedureEditorComponent(ProcedureRequisition requisition,
            List<FacilitySummary> facilityChoices, List<EnumValueInfo> lateralityChoices)
            : this(requisition, facilityChoices, lateralityChoices, new List<ProcedureTypeSummary>())
        {
        }

        public override void Start()
        {
            this.Validation.Add(new ValidationRule("CheckedIn",
                delegate
                {

                    // This validation does not apply if the procedure is not checked in
                    if (!_checkedIn)
                        return new ValidationResult(true, "");

                    DateTime now = Platform.Time;
                    DateTime earlyBound = now.AddMinutes(-CheckInSettings.Default.EarlyCheckInWarningThreshold);
                    DateTime lateBound = now.AddMinutes(CheckInSettings.Default.LateCheckInWarningThreshold);
                    bool newScheduledTimeTooEarly = _scheduledTime < earlyBound;
                    bool newScheduledTimeTooLate = _scheduledTime > lateBound;
                    string alertMessage = "";
                    if (newScheduledTimeTooEarly)
                        alertMessage = SR.MessageAlertScheduledTimeTooEarly;
                    else if (newScheduledTimeTooLate)
                        alertMessage = SR.MessageAlertScheduledTimeTooLate;

                    return new ValidationResult(!(newScheduledTimeTooEarly || newScheduledTimeTooLate), alertMessage);
                }));

            _procedureTypeChoices.Sort(
                    delegate(ProcedureTypeSummary x, ProcedureTypeSummary y)
                    {
                        return x.Name.CompareTo(y.Name);
                    });

            _procedureTypeSuggestionProvider =
                new DefaultSuggestionProvider<ProcedureTypeSummary>(_procedureTypeChoices, FormatProcedureType);

            _selectedProcedureType = _requisition.ProcedureType;
            _scheduledTime = _requisition.ScheduledTime;
            _selectedFacility = _requisition.PerformingFacility;
            _selectedLaterality = _requisition.Laterality;
            _portableModality = _requisition.PortableModality;
            _checkedIn = _requisition.CheckedIn;

            base.Start();
        }

        #region Presentation Model

        public bool IsProcedureTypeEditable
        {
            get { return _procedureTypeChoices.Count > 0; }
        }

        public bool IsPerformingFacilityEditable
        {
            get { return _requisition.Status == null || _requisition.Status.Code == "SC"; }
        }

        public bool IsScheduledTimeEditable
        {
            get { return _requisition.Status == null || _requisition.Status.Code == "SC"; }
        }

        public ISuggestionProvider ProcedureTypeSuggestionProvider
        {
            get { return _procedureTypeSuggestionProvider; }
        }

        public string FormatProcedureType(object item)
        {
            ProcedureTypeSummary rpt = (ProcedureTypeSummary)item;
            return string.Format("{0} ({1})", rpt.Name, rpt.Id);
        }

        [ValidateNotNull]
        public ProcedureTypeSummary SelectedProcedureType
        {
            get { return _selectedProcedureType; }
            set
            {
                if (!object.Equals(value, _selectedProcedureType))
                {
                    _selectedProcedureType = value;
                    NotifyPropertyChanged("SelectedProcedureType");
                }
            }
        }

        public IList FacilityChoices
        {
            get { return _facilityChoices; }
        }

        public string FormatFacility(object facility)
        {
            return (facility as FacilitySummary).Name;
        }

        [ValidateNotNull]
        public FacilitySummary SelectedFacility
        {
            get { return _selectedFacility; }
            set
            {
                if (!Equals(value, _selectedFacility))
                {
                    _selectedFacility = value;
                    NotifyPropertyChanged("SelectedFacility");
                }
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
                if (!Equals(value, _selectedLaterality))
                {
                    _selectedLaterality = value;
                    NotifyPropertyChanged("SelectedLaterality");
                }
            }
        }

        public DateTime? ScheduledTime
        {
            get { return _scheduledTime; }
            set
            {
                if (value != _scheduledTime)
                {
                    _scheduledTime = value;
                    NotifyPropertyChanged("ScheduledTime");
                }
            }
        }

        public bool PortableModality
        {
            get { return _portableModality; }
            set
            {
                if (value != _portableModality)
                {
                    _portableModality = value;
                    NotifyPropertyChanged("PortableModality");
                }
            }
        }

        public bool CheckedIn
        {
            get { return _checkedIn; }
            set
            {
                if (value != _checkedIn)
                {
                    _checkedIn = value;
                    NotifyPropertyChanged("CheckedIn");
                }
            }
        }

        public bool IsCheckedInEnabled
        {
            get { return _isCheckedInEnabled; }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            _requisition.ProcedureType = _selectedProcedureType;
            _requisition.ScheduledTime = _scheduledTime;
            _requisition.Laterality = _selectedLaterality;
            _requisition.PerformingFacility = _selectedFacility;
            _requisition.PortableModality = _portableModality;
            _requisition.CheckedIn = _checkedIn;

            this.Exit(ApplicationComponentExitCode.Accepted);
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion
    }
}
