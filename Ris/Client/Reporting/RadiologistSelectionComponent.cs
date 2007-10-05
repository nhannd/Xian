using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Extension point for views onto <see cref="RadiologistSelectionComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class RadiologistSelectionComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(RadiologistSelectionComponentViewExtensionPoint))]
    public class RadiologistSelectionComponent : ApplicationComponent
    {
        private bool _makeDefault;
        private StaffSummary _selectedRadiologist;
        private StaffSummaryTable _radiologistTable;

        public override void Start()
        {
            _radiologistTable = new StaffSummaryTable();

            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    GetRadiologistListResponse response = service.GetRadiologistList(new GetRadiologistListRequest());
                    _radiologistTable.Items.AddRange(response.Radiologists);

                    // Choose the default supervisor
                    if (String.IsNullOrEmpty(SupervisorSettings.Default.SupervisorID) == false)
                    {
                        _selectedRadiologist = CollectionUtils.SelectFirst<StaffSummary>(
                            response.Radiologists,
                            delegate(StaffSummary staff)
                                {
                                    return staff.StaffId == SupervisorSettings.Default.SupervisorID;
                                });
                        NotifyPropertyChanged("RadiologistSelection");
                    }
                });

            base.Start();
        }

        public ITable Radiologists
        {
            get { return _radiologistTable; }
        }

        public ISelection RadiologistSelection
        {
            get { return _selectedRadiologist == null ? Selection.Empty : new Selection(_selectedRadiologist); }
            set { _selectedRadiologist = (StaffSummary)value.Item; }
        }

        public bool AcceptEnabled
        {
            get { return _selectedRadiologist != null; }
        }

        public bool MakeDefaultChecked
        {
            get { return _makeDefault; }
            set { _makeDefault = value; }
        }

        public StaffSummary SelectedRadiologist
        {
            get { return _selectedRadiologist; }
        }

        public void Accept()
        {
            if (_makeDefault && _selectedRadiologist != null)
                SupervisorSettings.Default.SupervisorID = _selectedRadiologist.StaffId;

            this.ExitCode = ApplicationComponentExitCode.Normal;
            this.Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }
    }
}
