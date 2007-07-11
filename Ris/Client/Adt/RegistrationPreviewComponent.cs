using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.PreviewService;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint]
    public class RegistrationPreviewToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IRegistrationPreviewToolContext : IToolContext
    {
        RegistrationWorklistItem WorklistItem { get; }
        IDesktopWindow DesktopWindow { get; }
    }

    /// <summary>
    /// Extension point for views onto <see cref="RegistrationPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class RegistrationPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// RegistrationPreviewComponent class
    /// </summary>
    [AssociateView(typeof(RegistrationPreviewComponentViewExtensionPoint))]
    public class RegistrationPreviewComponent : PreviewApplicationComponent
    {
        class RegistrationPreviewToolContext : ToolContext, IRegistrationPreviewToolContext
        {
            private RegistrationPreviewComponent _component;
            public RegistrationPreviewToolContext(RegistrationPreviewComponent component)
            {
                _component = component;
            }

            public RegistrationWorklistItem WorklistItem
            {
                get { return _component.WorklistItem; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }

        private RegistrationWorklistItem _worklistItem;
        private ToolSet _toolSet;

        /// <summary>
        /// Constructor
        /// </summary>
        public RegistrationPreviewComponent()
        {
        }

        public override void Start()
        {
            _toolSet = new ToolSet(new RegistrationPreviewToolExtensionPoint(), new RegistrationPreviewToolContext(this));

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
                            request.PatientProfileRef = _worklistItem.PatientProfileRef;

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
            get { return RegistrationPreviewComponentSettings.Default.DetailsPageUrl; }
        }

        public override ActionModelNode ActionModel
        {
            get { return ActionModelRoot.CreateModel(this.GetType().FullName, "RegistrationPreview-menu", _toolSet.Actions); }
        }

        public RegistrationWorklistItem WorklistItem
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
