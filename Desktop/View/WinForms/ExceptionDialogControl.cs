#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
    internal partial class ExceptionDialogControl : CustomUserControl
    {
    	private readonly Exception _exception;

		internal ExceptionDialogControl(Exception exception, string message, ExceptionDialogActions buttonActions, ClickHandlerDelegate ok, ClickHandlerDelegate quit)
		{
        	_exception = exception;

			InitializeComponent();

			_description.Text = message;

			AcceptButton = _okButton;
			CancelButton = _okButton;

			EventHandler okClick = delegate
			                       	{
			                       		Result = ExceptionDialogAction.Ok;
			                       		ok();
			                       	};

			EventHandler quitClick = delegate
			                         	{
			                         		Result = ExceptionDialogAction.Quit;
			                         		quit();
			                         	};

			if (buttonActions == ExceptionDialogActions.Ok)
			{
				if (ok == null)
					throw new ArgumentException("Ok method must be supplied", "ok");

				_okButton.Click += okClick;
				_quitButton.Dispose();
			}
			else if (buttonActions == ExceptionDialogActions.Quit)
			{
				if (quit == null)
					throw new ArgumentException("Quit method must be supplied", "quit");

				_quitButton.Click += quitClick;
				AcceptButton = _quitButton;
				CancelButton = _quitButton;
				_okButton.Dispose();
			}
			else
			{
				if (ok == null)
					throw new ArgumentException("Ok method must be supplied", "ok");
				if (quit == null)
					throw new ArgumentException("Quit method must be supplied", "quit");

				_okButton.Click += okClick;
				_okButton.Text = "&Continue";
				_quitButton.Click += quitClick;
			}

			if (_exception != null)
			{
				// Update Exceptions detail tree
				_detailTree.BeginUpdate();
				BuildTreeFromException(null, _exception);
				_detailTree.ExpandAll();
				_detailTree.EndUpdate();
			}
			else
			{
				_detailButton.Dispose();
			}

        	// Hide the details when dialog first startup
            HideDetails();
        }

		public ExceptionDialogAction Result { get; private set; }

        #region Event functions

        private void _detailButton_Click(object sender, EventArgs e)
        {
            if (_detailTree.Visible)
                HideDetails();
            else
                ShowDetails();
        }

        private void _detailTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Show ContextMenu when right click anywhere on the TreeView
                Point mousePoint = PointToScreen(new Point(e.X, e.Y));
                contextMenuStrip1.Show(mousePoint.X + _detailTree.Left, mousePoint.Y + _detailTree.Top);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Copy exception details to clipboard
			string clipboardMessage = SR.ExceptionHandlerMessagePrefix + _description.Text + "\r\n\r\n";
            clipboardMessage += BuildMessageFromException(_exception);
            Clipboard.SetText(clipboardMessage);
        }

        #endregion

        #region Helper functions

        private void HideDetails()
        {
            _detailTree.Hide();
            _detailButton.Text = _detailButton.Text.Replace("<", ">");

            // Shrink the user control
            Rectangle thisBounds = Bounds;
            thisBounds.Height = _quitButton.Bounds.Bottom - thisBounds.Top + 10;
            Bounds = thisBounds;
        }

        private void ShowDetails()
        {
            _detailTree.Show();
            _detailButton.Text = _detailButton.Text.Replace(">", "<");

            // Expand the user control
            Rectangle thisBounds = Bounds;
            thisBounds.Height = _detailTree.Bounds.Bottom - thisBounds.Top + 10;
            Bounds = thisBounds;
        }

        private void BuildTreeFromException(TreeNode thisNode, Exception e)
        {
            // Special case for the root node
            if (thisNode == null)
                thisNode = _detailTree.Nodes.Add(e.Message);
            else
                thisNode.Nodes.Add(e.Message);

            if (e.StackTrace != null)
            {
                // Add a new node for each level of StackTrace
                const string lineBreak = "\r\n";
                int prevIndex = 0;
                int startIndex = e.StackTrace.IndexOf(lineBreak, prevIndex);
                while (startIndex != -1)
                {
                    thisNode.Nodes.Add(e.StackTrace.Substring(prevIndex, startIndex - prevIndex));
                    prevIndex = startIndex + lineBreak.Length;
                    startIndex = e.StackTrace.IndexOf(lineBreak, prevIndex);
                }
                thisNode.Nodes.Add(e.StackTrace.Substring(prevIndex));
            }

            // Recursively add inner exception to the tree
            if (e.InnerException != null)
            {
                TreeNode childNode = thisNode.Nodes.Add("InnerException");
                BuildTreeFromException(childNode, e.InnerException);
            }
        }

        private static string BuildMessageFromException(Exception e)
        {
            string message = "";

            message += e.Source + ": " + e.Message + "\r\n";
            message += e.StackTrace + "\r\n";

            // Recursively add inner exception to the message
            if (e.InnerException != null)
            {
                message += "\r\n";
				message += SR.ExceptionHandlerInnerExceptionText + "\r\n";
                message += BuildMessageFromException(e.InnerException);
            }

            return message;
        }

        #endregion
    }
}
