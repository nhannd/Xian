using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Hosts an application component in a modal dialog.  See <see cref="ApplicationComponent.LaunchAsDialog"/>.
    /// </summary>
    public class ApplicationComponentHostDialog
    {
        // implements the host interface, which is exposed to the hosted application component
        class Host : ApplicationComponentHost
        {
            private ApplicationComponentHostDialog _owner;

            internal Host(ApplicationComponentHostDialog owner, IApplicationComponent component)
                :base(component)
            {
				Platform.CheckForNullReference(owner, "owner");
                _owner = owner;
            }

            public override void Exit()
            {
                _owner._exitRequestedByComponent = true;
                DialogBoxAction action = _owner._component.ExitCode == ApplicationComponentExitCode.Cancelled ? DialogBoxAction.Cancel : DialogBoxAction.Ok;
                _owner._dialogBox.EndModal(action);
            }

            public override IDesktopWindow DesktopWindow
            {
                get { return _owner._desktopWindow; }
            }

            public override void SetTitle(string title)
            {
                _owner.SetTitle(title);
            }

        }

        private IApplicationComponent _component;
        private string _title;
        private IDialogBox _dialogBox;
        private IDesktopWindow _desktopWindow;
        private bool _exitRequestedByComponent;

        internal ApplicationComponentHostDialog(string title, IApplicationComponent component)
        {
			Platform.CheckForNullReference(component, "component");
            _title = title;
            _component = component;
        }

        internal ApplicationComponentExitCode RunModal(IDesktopWindow desktopWindow)
        {
			Platform.CheckForNullReference(desktopWindow, "desktopWindow");
            _desktopWindow = desktopWindow;

            // start the component
            Host host = new Host(this, _component);
            host.StartComponent();

            // create the dialog
            _dialogBox = Application.CreateDialogBox();
            _dialogBox.Initialize(_title, host.ComponentView);
            _dialogBox.DialogClosing += new EventHandler<ClosingEventArgs>(_dialogBox_DialogClosing);
        
            // run the dialog as modal
            _dialogBox.RunModal();
            return _component.ExitCode;
        }

        internal void SetTitle(string title)
        {

            // if the dialog box was already created, then update it's title
            if (_dialogBox != null)
            {
                _dialogBox.Title = title;
            }
            else
            {
                // otherwise, just remember the title, so the dialog title will be set when it is created
                _title = title;
            }
        }

        private void _dialogBox_DialogClosing(object sender, ClosingEventArgs e)
        {
            if (!_exitRequestedByComponent)
            {
                e.Cancel = !_component.CanExit();
            }
        }

    }
}
