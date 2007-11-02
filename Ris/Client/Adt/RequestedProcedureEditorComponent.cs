using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.Collections;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

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
        private List<RequestedProcedureTypeSummary> _procedureTypeChoices;
        private DefaultSuggestionProvider<RequestedProcedureTypeSummary> _procedureTypeSuggestionProvider;
        private RequestedProcedureTypeSummary _selectedProcedureType;
        private DateTime? _scheduledTime;

        private ProcedureRequisition _requisition;
        private List<FacilitySummary> _facilityChoices;
        private List<EnumValueInfo> _lateralityChoices;
        private FacilitySummary _selectedFacility;
        private EnumValueInfo _selectedLaterality;

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

        public ISuggestionProvider ProcedureTypeSuggestionProvider
        {
            get { return _procedureTypeSuggestionProvider; }
        }

        public string FormatProcedureType(object item)
        {
            RequestedProcedureTypeSummary rpt = (RequestedProcedureTypeSummary)item;
            return string.Format("{0} ({1})", rpt.Name, rpt.Id);
        }

        public RequestedProcedureTypeSummary SelectedProcedure
        {
            get { return _selectedProcedureType; }
            set
            {
                if (!object.Equals(value, _selectedProcedureType))
                {
                    _selectedProcedureType = value;
                    NotifyPropertyChanged("SelectedProcedure");
                }
            }
        }

        public IList FacilityChoices
        {
            get { return _facilityChoices;  }
        }

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

        public void Accept()
        {
            _requisition.ProcedureType = _selectedProcedureType;
            _requisition.ScheduledTime = _scheduledTime;
            _requisition.Laterality = _selectedLaterality;
            _requisition.PerformingFacility = _selectedFacility;

            this.Exit(ApplicationComponentExitCode.Normal);
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.Cancelled);
        }

        #endregion
    }
}
