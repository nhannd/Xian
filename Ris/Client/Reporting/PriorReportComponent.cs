using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Extension point for views onto <see cref="PriorReportComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PriorReportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// PriorReportComponent class
    /// </summary>
    [AssociateView(typeof(PriorReportComponentViewExtensionPoint))]
    public class PriorReportComponent : ApplicationComponent
    {
        private EntityRef _reportingStepRef;

        private ReportSummaryTable _reportList;
        private ReportSummary _selectedReport;

        /// <summary>
        /// Constructor
        /// </summary>
        public PriorReportComponent(EntityRef reportingStepRef)
        {
            _reportingStepRef = reportingStepRef;
            _reportList = new ReportSummaryTable();
        }

        public override void Start()
        {
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    GetPriorReportResponse response = service.GetPriorReport(new GetPriorReportRequest(_reportingStepRef));
                    _reportList.Items.AddRange(response.Reports);
                });

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public ITable Reports
        {
            get { return _reportList; }
        }

        public ISelection SelectedReport
        {
            get { return _selectedReport == null ? Selection.Empty : new Selection(_selectedReport); }
            set
            {
                ReportSummary newSelection = (ReportSummary)value.Item;
                if (_selectedReport != newSelection)
                {
                    _selectedReport = newSelection;
                    ReportSelectionChanged();
                }
            }
        }

        public string ReportContent
        {
            get { return _selectedReport == null ? null : _selectedReport.Format(); }
        }

        private void ReportSelectionChanged()
        {
            NotifyAllPropertiesChanged();
        }
    }
}
