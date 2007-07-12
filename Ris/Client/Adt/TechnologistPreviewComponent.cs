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

    /// <summary>
    /// Extension point for views onto <see cref="TechnologistPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class TechnologistPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// TechnologistPreviewComponent class
    /// </summary>
    [AssociateView(typeof(TechnologistPreviewComponentViewExtensionPoint))]
    public class TechnologistPreviewComponent : PreviewApplicationComponent
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

            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            base.Stop();
        }

        #region Public Properties

        public override string GetPreviewData(string requestJsml)
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
                            responseJsml = JsmlSerializer.Serialize<GetDataResponse>(response);
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
            get { return TechnologistPreviewComponentSettings.Default.DetailsPageUrl; }
        }

        public override ActionModelNode ActionModel
        {
            get { return ActionModelRoot.CreateModel(this.GetType().FullName, "TechnologistPreview-menu", _toolSet.Actions); }
        }

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
    }
}
