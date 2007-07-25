using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.PreviewService;
using ClearCanvas.Ris.Application.Common.Jsml;

namespace ClearCanvas.Ris.Client.Reporting
{
    [ExtensionPoint]
    public class ReportingPreviewToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IReportingPreviewToolContext : IToolContext
    {
        ReportingWorklistItem WorklistItem { get; }
        IDesktopWindow DesktopWindow { get; }
    }
    
    /// <summary>
    /// Extension point for views onto <see cref="ReportingPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ReportingPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ReportingPreviewComponent class
    /// </summary>
    [AssociateView(typeof(ReportingPreviewComponentViewExtensionPoint))]
    public class ReportingPreviewComponent : DHtmlComponent
    {
        class ReportingPreviewToolContext : ToolContext, IReportingPreviewToolContext
        {
            private ReportingPreviewComponent _component;
            public ReportingPreviewToolContext(ReportingPreviewComponent component)
            {
                _component = component;
            }

            public ReportingWorklistItem WorklistItem
            {
                get { return _component.WorklistItem; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }

        private ReportingWorklistItem _worklistItem;
        private ToolSet _toolSet;

        /// <summary>
        /// Constructor
        /// </summary>
        public ReportingPreviewComponent()
        {
        }

        public override void Start()
        {
            _toolSet = new ToolSet(new ReportingPreviewToolExtensionPoint(), new ReportingPreviewToolContext(this));

            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            base.Stop();
        }

        #region Presentation Model

        public override string GetJsmlData(string requestJsml)
        {
            string responseJsml = "";

            try
            {
                if (_worklistItem != null && String.IsNullOrEmpty(requestJsml) == false)
                {
                    Platform.GetService<IPreviewService>(
                        delegate(IPreviewService service)
                        {
                            GetDataRequest request = JsmlSerializer.Deserialize<GetDataRequest>(requestJsml);
                            request.ProcedureStepRef = _worklistItem.ProcedureStepRef;

                            GetDataResponse response = service.GetData(request);
                            responseJsml = JsmlSerializer.Serialize(response, "response");
                        });
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            return responseJsml;
        }

        public override string DetailsPageUrl
        {
            get { return ReportingPreviewComponentSettings.Default.DetailsPageUrl; }
        }

        public override ActionModelNode ActionModel
        {
            get { return ActionModelRoot.CreateModel(this.GetType().FullName, "ReportingPreview-menu", _toolSet.Actions); }
        }

        public ReportingWorklistItem WorklistItem
        {
            get { return _worklistItem; }
            set
            {
                _worklistItem = value;
                NotifyAllPropertiesChanged();
            }
        }

        #endregion
    }
}
