#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.Enterprise.Desktop.View.WinForms
{
    partial class AuthorityGroupEditorComponentControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this._authorityGroupName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._tokenTreeView = new Crownwood.DotNetMagic.Controls.TreeControl();
			this.label1 = new System.Windows.Forms.Label();
			this._description = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.SuspendLayout();
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(475, 458);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 4;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			this._acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._acceptButton.Location = new System.Drawing.Point(394, 458);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 3;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _authorityGroupName
			// 
			this._authorityGroupName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._authorityGroupName.AutoSize = true;
			this._authorityGroupName.LabelText = "Name";
			this._authorityGroupName.Location = new System.Drawing.Point(3, 2);
			this._authorityGroupName.Margin = new System.Windows.Forms.Padding(2);
			this._authorityGroupName.Mask = "";
			this._authorityGroupName.Name = "_authorityGroupName";
			this._authorityGroupName.PasswordChar = '\0';
			this._authorityGroupName.Size = new System.Drawing.Size(282, 40);
			this._authorityGroupName.TabIndex = 0;
			this._authorityGroupName.ToolTip = null;
			this._authorityGroupName.Value = null;
			// 
			// _tokenTreeView
			// 
			this._tokenTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._tokenTreeView.AutoEdit = false;
			this._tokenTreeView.CheckStates = Crownwood.DotNetMagic.Controls.CheckStates.ThreeStateCheck;
			this._tokenTreeView.FocusNode = null;
			this._tokenTreeView.HotBackColor = System.Drawing.Color.Empty;
			this._tokenTreeView.HotForeColor = System.Drawing.Color.Empty;
			this._tokenTreeView.Location = new System.Drawing.Point(3, 71);
			this._tokenTreeView.Name = "_tokenTreeView";
			this._tokenTreeView.SelectedNode = null;
			this._tokenTreeView.SelectedNoFocusBackColor = System.Drawing.SystemColors.Control;
			this._tokenTreeView.Size = new System.Drawing.Size(549, 334);
			this._tokenTreeView.TabIndex = 1;
			this._tokenTreeView.Text = "Permissions";
			this._tokenTreeView.AfterSelect += new Crownwood.DotNetMagic.Controls.NodeEventHandler(this._tokenTreeView_AfterSelect);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 52);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(62, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Permissions";
			// 
			// _description
			// 
			this._description.LabelText = "Description of selected permission";
			this._description.Location = new System.Drawing.Point(3, 410);
			this._description.Margin = new System.Windows.Forms.Padding(2);
			this._description.Mask = "";
			this._description.Name = "_description";
			this._description.PasswordChar = '\0';
			this._description.ReadOnly = true;
			this._description.Size = new System.Drawing.Size(547, 41);
			this._description.TabIndex = 2;
			this._description.ToolTip = null;
			this._description.Value = null;
			// 
			// AuthorityGroupEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._description);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._tokenTreeView);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Controls.Add(this._authorityGroupName);
			this.Name = "AuthorityGroupEditorComponentControl";
			this.Size = new System.Drawing.Size(561, 491);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
		private ClearCanvas.Desktop.View.WinForms.TextField _authorityGroupName;
		private Crownwood.DotNetMagic.Controls.TreeControl _tokenTreeView;
		private System.Windows.Forms.Label label1;
		private ClearCanvas.Desktop.View.WinForms.TextField _description;
    }
}
