#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// WinForms implementation of <see cref="IDialogBoxView"/>. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class may subclassed if customization is desired.  In this case, the <see cref="DesktopWindowView"/>
    /// class must also be subclassed in order to instantiate the subclass from 
    /// its <see cref="DesktopWindowView.CreateDialogBoxView"/> method.
    /// </para>
    /// <para>
    /// Reasons for subclassing may include: overriding the <see cref="CreateDialogBoxForm"/> factory method to supply
    /// a custom subclass of the <see cref="DialogBoxForm"/> class.
    /// </para>
    /// </remarks>
    public class DialogBoxView : DesktopObjectView, IDialogBoxView
    {
        private DialogBoxForm _form;
        private IWin32Window _owner;
        private bool _reallyClose;
		private bool _showingModal;

        private IApplicationComponent _component;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dialogBox"></param>
        /// <param name="owner"></param>
        protected internal DialogBoxView(DialogBox dialogBox, DesktopWindowView owner)
        {
            IApplicationComponentView componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(dialogBox.Component.GetType());
            componentView.SetComponent((IApplicationComponent)dialogBox.Component);

            // cache the app component - we'll need it later to get the ExitCode
            _component = (IApplicationComponent)dialogBox.Component;

            _form = CreateDialogBoxForm(dialogBox, (Control)componentView.GuiElement);
            _form.FormClosing += new FormClosingEventHandler(_form_FormClosing);

            _owner = owner.DesktopForm;
        }

        #region DesktopObjectView overrides

        /// <summary>
        /// Not used.
        /// </summary>
        public override void Open()
        {
            // do nothing
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public override void Show()
        {
            // do nothing
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public override void Hide()
        {
            // do nothing
        }

        /// <summary>
        /// Not used.
        /// </summary>
        public override void Activate()
        {
            // do nothing
        }

        /// <summary>
        /// Sets the title of the dialog box.
        /// </summary>
        /// <param name="title"></param>
        public override void SetTitle(string title)
        {
            _form.Text = title;
        }

        #endregion

        #region IDialogBoxView Members

        /// <summary>
        /// Runs the modal dialog.
        /// </summary>
        /// <returns></returns>
        public DialogBoxAction RunModal()
        {
			_showingModal = true;
        	try
			{
				DialogResult result = _form.ShowDialog(_owner);
				switch (result)
				{
					case DialogResult.Cancel:
						return DialogBoxAction.Cancel;
					case DialogResult.OK:
						return DialogBoxAction.Ok;
					case DialogResult.No:
						return DialogBoxAction.No;
					case DialogResult.Yes:
						return DialogBoxAction.Yes;
					default:
						throw new NotSupportedException();
				}
			}
			finally
			{
				_showingModal = false;
				DisposeForm();
			}
        }

		private void DisposeForm()
		{
			if (_form != null)
			{
				_form.Dispose();
				_form = null;
			}
		}

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
				if (_showingModal)
				{
					_reallyClose = true;

					// need to call a special DelayedClose method here,
					// since calling Close directly has no effect here (since we're already in the scope of the FormClosing event)
					_form.DelayedClose(_component.ExitCode == ApplicationComponentExitCode.Accepted ? DialogBoxAction.Ok : DialogBoxAction.Cancel);
				}
				else
				{
					DisposeForm();
				}
            }

            base.Dispose(disposing);
        }

        #endregion

        /// <summary>
        /// Called to create an instance of a <see cref="DialogBoxForm"/> for use by this view.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected virtual DialogBoxForm CreateDialogBoxForm(DialogBox dialogBox, Control content)
        {
            return new DialogBoxForm(dialogBox, content);
        }
    
        private void _form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_reallyClose)
            {
                e.Cancel = true;

                // raise the close requested event
                // if this results in an actual close, the Dispose method will be called, setting the _reallyClose flag
                RaiseCloseRequested();
            }
        }
    }
}
