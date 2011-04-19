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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthorityGroupEditorComponentControl));
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
			resources.ApplyResources(this._cancelButton, "_cancelButton");
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			resources.ApplyResources(this._acceptButton, "_acceptButton");
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _authorityGroupName
			// 
			resources.ApplyResources(this._authorityGroupName, "_authorityGroupName");
			this._authorityGroupName.Name = "_authorityGroupName";
			this._authorityGroupName.ToolTip = null;
			this._authorityGroupName.Value = null;
			// 
			// _tokenTreeView
			// 
			resources.ApplyResources(this._tokenTreeView, "_tokenTreeView");
			this._tokenTreeView.AutoEdit = false;
			this._tokenTreeView.CheckStates = Crownwood.DotNetMagic.Controls.CheckStates.ThreeStateCheck;
			this._tokenTreeView.FocusNode = null;
			this._tokenTreeView.HotBackColor = System.Drawing.Color.Empty;
			this._tokenTreeView.HotForeColor = System.Drawing.Color.Empty;
			this._tokenTreeView.Name = "_tokenTreeView";
			this._tokenTreeView.SelectedNode = null;
			this._tokenTreeView.SelectedNoFocusBackColor = System.Drawing.SystemColors.Control;
			this._tokenTreeView.AfterSelect += new Crownwood.DotNetMagic.Controls.NodeEventHandler(this._tokenTreeView_AfterSelect);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// _description
			// 
			resources.ApplyResources(this._description, "_description");
			this._description.Name = "_description";
			this._description.ReadOnly = true;
			this._description.ToolTip = null;
			this._description.Value = null;
			// 
			// AuthorityGroupEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._description);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._tokenTreeView);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Controls.Add(this._authorityGroupName);
			this.Name = "AuthorityGroupEditorComponentControl";
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
