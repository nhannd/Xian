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

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.View.WinForms
{
    partial class RecordGeneratorLaunchPadComponentControl
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
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection2 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection3 = new ClearCanvas.Desktop.Selection();
            this._generatorCombo = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._launchButton = new System.Windows.Forms.Button();
            this._numberToBeGenerated = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._settingsTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this._measureSelectionTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._measureSettingsTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // _generatorCombo
            // 
            this._generatorCombo.DataSource = null;
            this._generatorCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._generatorCombo.LabelText = "Generator";
            this._generatorCombo.Location = new System.Drawing.Point(17, 16);
            this._generatorCombo.Margin = new System.Windows.Forms.Padding(2);
            this._generatorCombo.Name = "_generatorCombo";
            this._generatorCombo.Size = new System.Drawing.Size(283, 41);
            this._generatorCombo.TabIndex = 1;
            this._generatorCombo.Value = null;
            // 
            // _launchButton
            // 
            this._launchButton.Location = new System.Drawing.Point(225, 79);
            this._launchButton.Name = "_launchButton";
            this._launchButton.Size = new System.Drawing.Size(75, 23);
            this._launchButton.TabIndex = 2;
            this._launchButton.Text = "Launch New Generator";
            this._launchButton.UseVisualStyleBackColor = true;
            this._launchButton.Click += new System.EventHandler(this._launchButton_Click);
            // 
            // _numberToBeGenerated
            // 
            this._numberToBeGenerated.CausesValidation = false;
            this._numberToBeGenerated.LabelText = "# of Entities";
            this._numberToBeGenerated.Location = new System.Drawing.Point(17, 61);
            this._numberToBeGenerated.Margin = new System.Windows.Forms.Padding(2);
            this._numberToBeGenerated.Mask = "";
            this._numberToBeGenerated.Name = "_numberToBeGenerated";
            this._numberToBeGenerated.Size = new System.Drawing.Size(96, 41);
            this._numberToBeGenerated.TabIndex = 3;
            this._numberToBeGenerated.Value = null;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(17, 108);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._settingsTable);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(283, 481);
            this.splitContainer1.SplitterDistance = 240;
            this.splitContainer1.TabIndex = 5;
            // 
            // _settingsTable
            // 
            this._settingsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._settingsTable.Location = new System.Drawing.Point(0, 0);
            this._settingsTable.MenuModel = null;
            this._settingsTable.Name = "_settingsTable";
            this._settingsTable.ReadOnly = false;
            this._settingsTable.Selection = selection1;
            this._settingsTable.Size = new System.Drawing.Size(283, 240);
            this._settingsTable.TabIndex = 1;
            this._settingsTable.Table = null;
            this._settingsTable.ToolbarModel = null;
            this._settingsTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._settingsTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this._measureSelectionTable);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this._measureSettingsTable);
            this.splitContainer2.Size = new System.Drawing.Size(283, 237);
            this.splitContainer2.SplitterDistance = 115;
            this.splitContainer2.TabIndex = 8;
            // 
            // _measureSelectionTable
            // 
            this._measureSelectionTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._measureSelectionTable.Location = new System.Drawing.Point(0, 0);
            this._measureSelectionTable.MenuModel = null;
            this._measureSelectionTable.Name = "_measureSelectionTable";
            this._measureSelectionTable.ReadOnly = false;
            this._measureSelectionTable.Selection = selection2;
            this._measureSelectionTable.Size = new System.Drawing.Size(283, 115);
            this._measureSelectionTable.TabIndex = 7;
            this._measureSelectionTable.Table = null;
            this._measureSelectionTable.ToolbarModel = null;
            this._measureSelectionTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._measureSelectionTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _measureSettingsTable
            // 
            this._measureSettingsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._measureSettingsTable.Location = new System.Drawing.Point(0, 0);
            this._measureSettingsTable.MenuModel = null;
            this._measureSettingsTable.Name = "_measureSettingsTable";
            this._measureSettingsTable.ReadOnly = false;
            this._measureSettingsTable.Selection = selection3;
            this._measureSettingsTable.Size = new System.Drawing.Size(283, 118);
            this._measureSettingsTable.TabIndex = 9;
            this._measureSettingsTable.Table = null;
            this._measureSettingsTable.ToolbarModel = null;
            this._measureSettingsTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._measureSettingsTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // RecordGeneratorLaunchPadComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this._numberToBeGenerated);
            this.Controls.Add(this._launchButton);
            this.Controls.Add(this._generatorCombo);
            this.Name = "RecordGeneratorLaunchPadComponentControl";
            this.Size = new System.Drawing.Size(315, 602);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _generatorCombo;
        private System.Windows.Forms.Button _launchButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _numberToBeGenerated;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ClearCanvas.Desktop.View.WinForms.TableView _settingsTable;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private ClearCanvas.Desktop.View.WinForms.TableView _measureSelectionTable;
        private ClearCanvas.Desktop.View.WinForms.TableView _measureSettingsTable;
    }
}
