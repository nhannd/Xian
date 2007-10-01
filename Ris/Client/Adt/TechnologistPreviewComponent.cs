using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.PreviewService;
using ClearCanvas.Ris.Application.Common.Jsml;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint]
    public class TechnologistPreviewToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface ITechnologistPreviewToolContext : IToolContext
    {
        ModalityWorklistItem WorklistItem { get; }
        IDesktopWindow DesktopWindow { get; }
    }

    public class TechnologistPreviewComponent : DHtmlComponent
    {
        class TechnologistPreviewToolContext : ToolContext, ITechnologistPreviewToolContext
        {
            private TechnologistPreviewComponent _component;

            public TechnologistPreviewToolContext(TechnologistPreviewComponent component)
            {
                _component = component;
            }

            #region ITechnologistPreviewToolContext Members

            public ModalityWorklistItem WorklistItem
            {
                get { return _component.WorklistItem; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            #endregion
        }

        private ModalityWorklistItem _worklistItem;
        private ToolSet _toolSet;

        /// <summary>
        /// Constructor
        /// </summary>
        public TechnologistPreviewComponent()
        {
        }

        public override void Start()
        {
            _toolSet = new ToolSet(new TechnologistPreviewToolExtensionPoint(), new TechnologistPreviewToolContext(this));

            SetUrl(TechnologistPreviewComponentSettings.Default.DetailsPageUrl);

            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            base.Stop();
        }

        #region Public Properties

        public ModalityWorklistItem WorklistItem
        {
            get { return _worklistItem; }
            set
            {
                _worklistItem = value;
                NotifyAllPropertiesChanged();
            }
        }


        #endregion

        protected override ActionModelNode GetActionModel()
        {
            return ActionModelRoot.CreateModel(this.GetType().FullName, "TechnologistPreview-menu", _toolSet.Actions);
        }

        protected override object GetWorklistItem()
        {
            return _worklistItem;
        }
    }
}
