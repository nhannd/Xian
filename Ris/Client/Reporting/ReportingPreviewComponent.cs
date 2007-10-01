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
using ClearCanvas.Enterprise.Common;

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

        public ReportingWorklistItem WorklistItem
        {
            get { return _worklistItem; }
            set
            {
                _worklistItem = value;
                NotifyAllPropertiesChanged();
            }
        }

        public override void Start()
        {
            _toolSet = new ToolSet(new ReportingPreviewToolExtensionPoint(), new ReportingPreviewToolContext(this));

            SetUrl(ReportingPreviewComponentSettings.Default.DetailsPageUrl);

            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            base.Stop();
        }

        #region Presentation Model

        protected override ActionModelNode GetActionModel()
        {
            return ActionModelRoot.CreateModel(this.GetType().FullName, "ReportingPreview-menu", _toolSet.Actions);
        }

        protected override object GetWorklistItem()
        {
            return _worklistItem;
        }

        #endregion
    }
}
