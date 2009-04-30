#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.Common;
using System.Drawing;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// An enumeration that can be used to provide a <see cref="DialogBox"/> with a hint for what size it should display.
    /// </summary>
    public enum DialogSizeHint
    {
        /// <summary>
        /// Indicates that the dialog should size itself to the content.
        /// </summary>
        Auto,

        /// <summary>
        /// Indicates that the dialog should be small.
        /// </summary>
        Small,

        /// <summary>
        /// Indicates that the dialog should be medium.
        /// </summary>
        Medium,

        /// <summary>
        /// Indicatest that the dialog should be large.
        /// </summary>
        Large
    }

    /// <summary>
    /// Represents a dialog box.
    /// </summary>
    public class DialogBox : DesktopObject
    {
        // implements the host interface, which is exposed to the hosted application component
        private class Host : ApplicationComponentHost, IDialogBoxHost
        {
            private readonly DialogBox _owner;

            internal Host(DialogBox owner, IApplicationComponent component)
                :base(component)
            {
				Platform.CheckForNullReference(owner, "owner");
                _owner = owner;
            }

            public override void Exit()
            {
                _owner._exitRequestedByComponent = true;

                // close the dialog
                _owner.Close(UserInteraction.Allowed, CloseReason.Program);
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

        private readonly DesktopWindow _desktopWindow;
        private readonly IApplicationComponent _component;
        private bool _exitRequestedByComponent;
        private Host _host;
        private readonly DialogSizeHint _dialogSize;
        private readonly Size _size;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="args">Creation args for the dialog box.</param>
        /// <param name="desktopWindow">The <see cref="DesktopWindow"/> that owns the dialog box.</param>
        protected internal DialogBox(DialogBoxCreationArgs args, DesktopWindow desktopWindow)
            :base(args)
        {
            _component = args.Component;
            _dialogSize = args.SizeHint;
            _size = args.Size;
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
        /// Gets the dialog size hint.
        /// </summary>
        public DialogSizeHint DialogSizeHint
        {
            get { return _dialogSize; }
        }

        /// <summary>
        /// Gets the explicit size of the dialog, if specified.
        /// </summary>
        public Size Size
        {
            get { return _size; }
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
        /// Checks if the hosted component can close without user interaction.
        /// </summary>
        protected internal override bool CanClose()
        {
            return _exitRequestedByComponent || _host.Component.CanExit();
        }

        /// <summary>
        /// Gives the hosted component a chance to prepare for a forced exit.
        /// </summary>
        protected override bool PrepareClose(CloseReason reason)
        {
            base.PrepareClose(reason);

            return _exitRequestedByComponent || _host.Component.PrepareExit();
        }

        /// <summary>
        /// Disposes of this object.
        /// </summary>
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
