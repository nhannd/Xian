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
				Platform.CheckForNullReference(owner, "owner");
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

            public IDesktopWindow DesktopWindow
            {
                get { return _owner._desktopWindow; }
            }

            public CommandHistory CommandHistory
            {
                get
                {
                    // for now this is not supported
                    // if there is a need to support this in future, then this could be implemented
                    // perhaps to somehow access the command history of the active workspace?
                    throw new NotSupportedException();
                }
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
            _component.SetHost(new Host(this));
            _component.Start();

            // create the component's view
            IApplicationComponentView componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(_component.GetType());

            componentView.SetComponent(_component);

            // create the dialog
            _dialogBox = Application.CreateDialogBox();
            _dialogBox.Initialize(_title, componentView);
            _dialogBox.DialogClosing += new EventHandler<ClosingEventArgs>(_dialogBox_DialogClosing);
        
            // run the dialog as modal
            _dialogBox.RunModal();
            return _component.ExitCode;
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
