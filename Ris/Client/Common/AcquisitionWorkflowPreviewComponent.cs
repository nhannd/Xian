using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Common
{
    /// <summary>
    /// Extension point for views onto <see cref="AcquisitionWorkflowPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class AcquisitionWorkflowPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// AcquisitionWorkflowPreviewComponent class
    /// </summary>
    [AssociateView(typeof(AcquisitionWorkflowPreviewComponentViewExtensionPoint))]
    public class AcquisitionWorkflowPreviewComponent : ApplicationComponent
    {
        private AcquisitionWorklistItem _worklistItem;
        private ScheduledProcedureStep _scheduledProcStep;
        private PatientProfile _patientProfile;

        private IAcquisitionWorkflowService _workflowService;
        private SexEnumTable _sexChoices;
        private OrderPriorityEnumTable _orderPriorities;

        /// <summary>
        /// Constructor
        /// </summary>
        public AcquisitionWorkflowPreviewComponent()
        {
        }

        public AcquisitionWorklistItem WorklistItem
        {
            get { return _worklistItem; }
            set
            {
                _worklistItem = value;
                Refresh();
            }
        }

        public override void Start()
        {
            _workflowService = ApplicationContext.GetService<IAcquisitionWorkflowService>();
            _sexChoices = _workflowService.GetSexEnumTable();
            _orderPriorities = _workflowService.GetOrderPriorityEnumTable();

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        private void Refresh()
        {
            if (_worklistItem != null)
            {
                _scheduledProcStep = _workflowService.LoadWorklistItemPreview(_worklistItem);

                _patientProfile = CollectionUtils.SelectFirst<PatientProfile>(
                    _scheduledProcStep.RequestedProcedure.Order.Patient.Profiles,
                    delegate(PatientProfile pp) { return _worklistItem.PatientProfile.RefersTo(pp); });

            }

            NotifyAllPropertiesChanged();
        }

        #region Presentation Model

        public string Name
        {
            get { return _patientProfile.Name.Format(); }
        }

        public string DateOfBirth
        {
            get { return ClearCanvas.Desktop.Format.Date(_patientProfile.DateOfBirth); }
        }

        public string Mrn
        {
            get { return _patientProfile.Mrn.Format(); }
        }

        public string Healthcard
        {
            get { return _patientProfile.Healthcard.Format(); }
        }

        public string Sex
        {
            get { return _sexChoices[_patientProfile.Sex].Value; }
        }

        public string AccessionNumber
        {
            get { return _scheduledProcStep.RequestedProcedure.Order.AccessionNumber; }
        }

        public string DiagnosticServiceName
        {
            get { return _scheduledProcStep.RequestedProcedure.Order.DiagnosticService.Name; }
       }

        public string OrderingFacility
        {
            get { return _scheduledProcStep.RequestedProcedure.Order.OrderingFacility.Name; }
        }

        public string OrderPriority
        {
            get { return _orderPriorities[_scheduledProcStep.RequestedProcedure.Order.Priority].Value; }
        }

        public string ProcedureType
        {
            get { return _scheduledProcStep.RequestedProcedure.Type.Name; }
        }

        #endregion
    }
}
