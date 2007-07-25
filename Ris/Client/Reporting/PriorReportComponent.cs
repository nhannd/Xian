using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using System.Runtime.InteropServices;
using ClearCanvas.Ris.Application.Common.Jsml;

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
        /// <summary>
        /// The script callback is an object that is made available to the web browser so that
        /// the javascript code can invoke methods on the host.  It must be COM-visible.
        /// </summary>
        [ComVisible(true)]
        public class ScriptCallback
        {
            private PriorReportComponent _component;

            public ScriptCallback(PriorReportComponent component)
            {
                _component = component;
            }

            public void Alert(string message)
            {
                _component.Host.ShowMessageBox(message, MessageBoxActions.Ok);
            }

            public string GetData(string tag)
            {
                string temp = _component.GetData(tag);
                return temp;
            }
        }

        private ScriptCallback _scriptCallback;

        private EntityRef _reportingStepRef;

        private ReportSummaryTable _reportList;
        private ReportSummary _selectedReport;

        /// <summary>
        /// Constructor
        /// </summary>
        public PriorReportComponent(EntityRef reportingStepRef)
        {
            _scriptCallback = new ScriptCallback(this);

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
                    NotifyAllPropertiesChanged();
                }
            }
        }

        public string PreviewUrl
        {
            get { return ReportEditorComponentSettings.Default.ReportPreviewPageUrl; }
        }

        public ScriptCallback ScriptObject
        {
            get { return _scriptCallback; }
        }

        public string GetData(string tag)
        {
            return JsmlSerializer.Serialize(_selectedReport, "report");
        }

    }
}
