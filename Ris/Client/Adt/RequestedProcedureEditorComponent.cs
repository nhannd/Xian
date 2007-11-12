using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.Collections;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="RequestedProcedureEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class RequestedProcedureEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// RequestedProcedureEditorComponent class
    /// </summary>
    [AssociateView(typeof(RequestedProcedureEditorComponentViewExtensionPoint))]
    public class RequestedProcedureEditorComponent : ApplicationComponent
    {
        private readonly List<RequestedProcedureTypeSummary> _procedureTypeChoices;
        private DefaultSuggestionProvider<RequestedProcedureTypeSummary> _procedureTypeSuggestionProvider;
        private RequestedProcedureTypeSummary _selectedProcedureType;
        private DateTime? _scheduledTime;

        private readonly ProcedureRequisition _requisition;
        private readonly List<FacilitySummary> _facilityChoices;
        private readonly List<EnumValueInfo> _lateralityChoices;
        private FacilitySummary _selectedFacility;
        private EnumValueInfo _selectedLaterality;
        private bool _portableModality;

        /// <summary>
        /// Constructor for add mode.
        /// </summary>
        public RequestedProcedureEditorComponent(ProcedureRequisition requisition, List<FacilitySummary> facilityChoices, List<EnumValueInfo> lateralityChoices, List<RequestedProcedureTypeSummary> procedureTypeChoices)
        {
            Platform.CheckForNullReference(requisition, "requisition");
            Platform.CheckForNullReference(procedureTypeChoices, "procedureTypeChoices");

            _requisition = requisition;
            _procedureTypeChoices = procedureTypeChoices;
            _facilityChoices = facilityChoices;
            _lateralityChoices = lateralityChoices;
        }

        /// <summary>
        /// Constructor for edit mode.
        /// </summary>
        public RequestedProcedureEditorComponent(ProcedureRequisition requisition,
            List<FacilitySummary> facilityChoices, List<EnumValueInfo> lateralityChoices)
            :this(requisition, facilityChoices, lateralityChoices, new List<RequestedProcedureTypeSummary>())
        {
        }

        public override void Start()
        {
            _procedureTypeSuggestionProvider = new DefaultSuggestionProvider<RequestedProcedureTypeSummary>(
                _procedureTypeChoices, FormatProcedureType,
                delegate(RequestedProcedureTypeSummary x, RequestedProcedureTypeSummary y)
                    {
                        return x.Name.CompareTo(y.Name);
                    });

            _selectedProcedureType = _requisition.ProcedureType;
            _scheduledTime = _requisition.ScheduledTime;
            _selectedFacility = _requisition.PerformingFacility;
            _selectedLaterality = _requisition.Laterality;
            _portableModality = _requisition.PortableModality;

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
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
            RequestedProcedureTypeSummary rpt = (RequestedProcedureTypeSummary)item;
            return string.Format("{0} ({1})", rpt.Name, rpt.Id);
        }

        [ValidateNotNull]
        public RequestedProcedureTypeSummary SelectedProcedureType
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
            get { return _facilityChoices;  }
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
                if(!Equals(value, _selectedFacility))
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
                if(value != _portableModality)
                {
                    _portableModality = value;
                    NotifyPropertyChanged("PortableModality");
                }
            }
        }

        public void Accept()
        {
            if(this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            _requisition.ProcedureType = _selectedProcedureType;
            _requisition.ScheduledTime = _scheduledTime;
            _requisition.Laterality = _selectedLaterality;
            _requisition.PerformingFacility = _selectedFacility;
            _requisition.PortableModality = _portableModality;

            this.Exit(ApplicationComponentExitCode.Accepted);
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion
    }
}
