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

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ExceptionHandlerComponent"/>
    /// </summary>
    public partial class ExceptionHandlerComponentControl : CustomUserControl
    {
        private ExceptionHandlerComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ExceptionHandlerComponentControl(ExceptionHandlerComponent component)
        {
            InitializeComponent();

            _component = component;
            _description.Text = _component.Message;

			base.AcceptButton = this._okButton;
			base.CancelButton = this._okButton;

            // Update Exceptions detail tree
            _detailTree.BeginUpdate();
            BuildTreeFromException(null, _component.Exception);
            _detailTree.ExpandAll();
            _detailTree.EndUpdate();

            // Hide the details when dialog first startup
            HideDetails();
        }

        #region Event functions

        private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

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
            clipboardMessage += BuildMessageFromException(_component.Exception);
            Clipboard.SetText(clipboardMessage);
        }

        #endregion

        #region Helper functions

        private void HideDetails()
        {
            _detailTree.Hide();
            _detailButton.Text = _detailButton.Text.Replace("<", ">");

            // Shrink the user control
            Rectangle thisBounds = this.Bounds;
            thisBounds.Height = _okButton.Bounds.Bottom - thisBounds.Top + 10;
            this.Bounds = thisBounds;
        }

        private void ShowDetails()
        {
            _detailTree.Show();
            _detailButton.Text = _detailButton.Text.Replace(">", "<");

            // Expand the user control
            Rectangle thisBounds = this.Bounds;
            thisBounds.Height = _detailTree.Bounds.Bottom - thisBounds.Top + 10;
            this.Bounds = thisBounds;
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
                string lineBreak = "\r\n";
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

        private string BuildMessageFromException(Exception e)
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
