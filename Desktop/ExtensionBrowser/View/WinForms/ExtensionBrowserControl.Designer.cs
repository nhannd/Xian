#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.Desktop.ExtensionBrowser.View.WinForms
{
    partial class ExtensionBrowserControl
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
            this._pluginTreeView = new System.Windows.Forms.TreeView();
            this._tabView = new System.Windows.Forms.TabControl();
            this._pluginViewTabPage = new System.Windows.Forms.TabPage();
            this._extPointViewTabPage = new System.Windows.Forms.TabPage();
            this._extPointTreeView = new System.Windows.Forms.TreeView();
            this._tabView.SuspendLayout();
            this._pluginViewTabPage.SuspendLayout();
            this._extPointViewTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // _pluginTree
            // 
            this._pluginTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pluginTreeView.Location = new System.Drawing.Point(3, 3);
            this._pluginTreeView.Name = "_pluginTree";
            this._pluginTreeView.Size = new System.Drawing.Size(424, 277);
            this._pluginTreeView.TabIndex = 0;
            // 
            // _tabView
            // 
            this._tabView.Controls.Add(this._extPointViewTabPage);
            this._tabView.Controls.Add(this._pluginViewTabPage);
            this._tabView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabView.Location = new System.Drawing.Point(0, 0);
            this._tabView.Name = "_tabView";
            this._tabView.SelectedIndex = 0;
            this._tabView.Size = new System.Drawing.Size(438, 309);
            this._tabView.TabIndex = 1;
            // 
            // _pluginViewTabPage
            // 
            this._pluginViewTabPage.Controls.Add(this._pluginTreeView);
            this._pluginViewTabPage.Location = new System.Drawing.Point(4, 22);
            this._pluginViewTabPage.Name = "_pluginViewTabPage";
            this._pluginViewTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._pluginViewTabPage.Size = new System.Drawing.Size(430, 283);
            this._pluginViewTabPage.TabIndex = 0;
            this._pluginViewTabPage.Text = "Plugins";
            this._pluginViewTabPage.UseVisualStyleBackColor = true;
            // 
            // _extPointViewTabPage
            // 
            this._extPointViewTabPage.Controls.Add(this._extPointTreeView);
            this._extPointViewTabPage.Location = new System.Drawing.Point(4, 22);
            this._extPointViewTabPage.Name = "_extPointViewTabPage";
            this._extPointViewTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._extPointViewTabPage.Size = new System.Drawing.Size(430, 283);
            this._extPointViewTabPage.TabIndex = 1;
            this._extPointViewTabPage.Text = "Extension Points";
            this._extPointViewTabPage.UseVisualStyleBackColor = true;
            // 
            // _extPointTree
            // 
            this._extPointTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._extPointTreeView.Location = new System.Drawing.Point(3, 3);
            this._extPointTreeView.Name = "_extPointTree";
            this._extPointTreeView.Size = new System.Drawing.Size(424, 277);
            this._extPointTreeView.TabIndex = 0;
            // 
            // BrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tabView);
            this.Name = "BrowserControl";
            this.Size = new System.Drawing.Size(438, 309);
            this._tabView.ResumeLayout(false);
            this._pluginViewTabPage.ResumeLayout(false);
            this._extPointViewTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView _pluginTreeView;
        private System.Windows.Forms.TabControl _tabView;
        private System.Windows.Forms.TabPage _pluginViewTabPage;
        private System.Windows.Forms.TabPage _extPointViewTabPage;
        private System.Windows.Forms.TreeView _extPointTreeView;
    }
}
