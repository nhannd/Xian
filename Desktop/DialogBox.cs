using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public class DialogBox : DesktopObject
    {
        // implements the host interface, which is exposed to the hosted application component
        class Host : ApplicationComponentHost
        {
            private DialogBox _owner;

            internal Host(DialogBox owner, IApplicationComponent component)
                :base(component)
            {
				Platform.CheckForNullReference(owner, "owner");
                _owner = owner;
            }

            public override void Exit()
            {
                _owner._exitRequestedByComponent = true;
                DialogBoxAction action = _owner._component.ExitCode == ApplicationComponentExitCode.Cancelled ? DialogBoxAction.Cancel : DialogBoxAction.Ok;
                _owner.EndModal(action);
            }

            public override DesktopWindow DesktopWindow
            {
                get { return _owner._desktopWindow; }
            }

            public override void SetTitle(string title)
            {
                _owner.SetTitle(title);
            }

        }

        private DesktopWindow _desktopWindow;
        private IApplicationComponent _component;
        private bool _exitRequestedByComponent;
        private Host _host;

        protected internal DialogBox(DialogBoxCreationArgs args, DesktopWindow desktopWindow)
            :base(args)
        {
            _component = args.Component;
            _desktopWindow = desktopWindow;

            _host = new Host(this, _component);
        }

        public object Component
        {
            get { return _component; }
        }

        protected override void Initialize()
        {
            _host.StartComponent();

            base.Initialize();
        }

        internal DialogBoxAction RunModal()
        {
            return this.DialogBoxView.RunModal();
        }

        private void EndModal(DialogBoxAction action)
        {
            this.DialogBoxView.EndModal(action);
        }

        private void SetTitle(string title)
        {
            this.DialogBoxView.SetTitle(title);
        }

        protected internal override bool CanClose(UserInteraction interactive)
        {
            return _exitRequestedByComponent || _host.Component.CanExit(interactive);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && _host != null)
            {
                _host.StopComponent();
                _host = null;
            }
        }

        protected override IDesktopObjectView CreateView()
        {
            return _desktopWindow.CreateDialogView(this);
        }

        protected IDialogBoxView DialogBoxView
        {
            get { return (IDialogBoxView)this.View; }
        }
    }
}
