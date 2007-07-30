using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a dialog box.
    /// </summary>
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

            public override string Title
            {
                get { return _owner.Title; }
                set { _owner.Title = value; }
            }

        }

        private DesktopWindow _desktopWindow;
        private IApplicationComponent _component;
        private bool _exitRequestedByComponent;
        private Host _host;
        private string _title;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="desktopWindow"></param>
        protected internal DialogBox(DialogBoxCreationArgs args, DesktopWindow desktopWindow)
            :base(args)
        {
            _component = args.Component;
            _desktopWindow = desktopWindow;

            _host = new Host(this, _component);
        }

        /// <summary>
        /// Gets the component hosted by this dialog box.
        /// </summary>
        public object Component
        {
            get { return _component; }
        }

        /// <summary>
        /// Starts the hosted component.
        /// </summary>
        protected override void Initialize()
        {
            _host.StartComponent();

            base.Initialize();
        }

        /// <summary>
        /// Runs this dialog on a modal loop, blocking until the dialog is closed.
        /// </summary>
        /// <returns></returns>
        internal DialogBoxAction RunModal()
        {
            return this.DialogBoxView.RunModal();
        }

        /// <summary>
        /// Terminates the modal loop, closing the dialog box.
        /// </summary>
        /// <param name="action"></param>
        private void EndModal(DialogBoxAction action)
        {
            this.DialogBoxView.EndModal(action);
        }

        /// <summary>
        /// Checks if the hosted component can close.
        /// </summary>
        /// <param name="interactive"></param>
        /// <returns></returns>
        protected internal override bool CanClose(UserInteraction interactive)
        {
            return _exitRequestedByComponent || _host.Component.CanExit(interactive);
        }

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && _host != null)
            {
                _host.StopComponent();
                _host = null;
            }
        }

        /// <summary>
        /// Creates a view for this object.
        /// </summary>
        /// <returns></returns>
        protected sealed override IDesktopObjectView CreateView()
        {
            return _desktopWindow.CreateDialogView(this);
        }

        /// <summary>
        /// Gets the view for this object as a <see cref="IDialogBoxView"/>.
        /// </summary>
        protected IDialogBoxView DialogBoxView
        {
            get { return (IDialogBoxView)this.View; }
        }
    }
}
