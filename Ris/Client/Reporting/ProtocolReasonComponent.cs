using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Extension point for views onto <see cref="ProtocolReasonComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProtocolReasonComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProtocolReasonComponent class
    /// </summary>
    [AssociateView(typeof(ProtocolReasonComponentViewExtensionPoint))]
    public class ProtocolReasonComponent : ApplicationComponent
    {
        private EnumValueInfo _selectedReason;
        private List<EnumValueInfo> _availableReasons;

        public override void Start()
        {
            Platform.GetService<IProtocollingWorkflowService>(
                delegate(IProtocollingWorkflowService service)
                {
                    GetSuspendRejectReasonChoicesResponse response =
                        service.GetSuspendRejectReasonChoices(new GetSuspendRejectReasonChoicesRequest());
                    _availableReasons = response.SuspendRejectReasonChoices;
                });

            // TODO prepare the component for its live phase
            base.Start();
        }

        #region PresentationModel

        public EnumValueInfo Reason
        {
            get { return _selectedReason; }
        }

        public string SelectedReasonChoice
        {
            get { return _selectedReason == null ? "" : _selectedReason.Value; }
            set
            {
                _selectedReason = (value == "")
                    ? null
                    : CollectionUtils.SelectFirst<EnumValueInfo>(
                        _availableReasons,
                        delegate(EnumValueInfo reason) { return reason.Value == value; });
            }
        }

        public List<string> ReasonChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_availableReasons);  }
        }

        public bool OkayEnabled
        {
            get { return _selectedReason != null; }
        }

        public void Okay()
        {
            this.Exit(ApplicationComponentExitCode.Accepted);
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion



    }
}
