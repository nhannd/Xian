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
        class Host : IApplicationComponentHost
        {
            private ApplicationComponentHostDialog _owner;

            internal Host(ApplicationComponentHostDialog owner)
            {
                _owner = owner;
            }

            public void Exit()
            {
                _owner._exitRequestedByComponent = true;
                DialogBoxAction action = _owner._component.ExitCode == ApplicationComponentExitCode.Cancelled ? DialogBoxAction.Cancel : DialogBoxAction.Ok;
                _owner._dialogBox.EndModal(action);
            }

            public DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
            {
                return Platform.ShowMessageBox(message, buttons);
            }

        }

        private IApplicationComponent _component;
        private string _title;
        private IDialogBox _dialogBox;
        private bool _exitRequestedByComponent;

        internal ApplicationComponentHostDialog(string title, IApplicationComponent component)
        {
            _title = title;
            _component = component;
        }

        internal ApplicationComponentExitCode RunModal()
        {
            // start the component
            _component.SetHost(new Host(this));
            _component.Start();

            // create the component's view
            IExtensionPoint viewExtensionPoint = ApplicationComponent.GetViewExtensionPoint(_component.GetType());
            IApplicationComponentView componentView = (IApplicationComponentView)ViewFactory.CreateView(viewExtensionPoint);

            componentView.SetComponent(_component);

            // create the dialog
            _dialogBox = DesktopApplication.CreateDialogBox();
            _dialogBox.Initialize(_title, componentView);
            _dialogBox.DialogClosing += new EventHandler<ClosingEventArgs>(_dialogBox_DialogClosing);
        
            // run the dialog as modal
            DialogBoxAction action = _dialogBox.RunModal();
            return action == DialogBoxAction.Ok ? ApplicationComponentExitCode.Normal : ApplicationComponentExitCode.Cancelled;
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
