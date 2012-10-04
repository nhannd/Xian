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
    partial class RecordGeneratorComponentControl
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
            this._start = new System.Windows.Forms.Button();
            this._stop = new System.Windows.Forms.Button();
            this._splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._statTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._measureTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._statExport = new System.Windows.Forms.Button();
            this._measureExport = new System.Windows.Forms.Button();
            this._splitContainer1.Panel1.SuspendLayout();
            this._splitContainer1.Panel2.SuspendLayout();
            this._splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _start
            // 
            this._start.Location = new System.Drawing.Point(3, 5);
            this._start.Name = "_start";
            this._start.Size = new System.Drawing.Size(75, 23);
            this._start.TabIndex = 2;
            this._start.Text = "Start";
            this._start.UseVisualStyleBackColor = true;
            this._start.Click += new System.EventHandler(this._start_Click);
            // 
            // _stop
            // 
            this._stop.Enabled = false;
            this._stop.Location = new System.Drawing.Point(84, 5);
            this._stop.Name = "_stop";
            this._stop.Size = new System.Drawing.Size(75, 23);
            this._stop.TabIndex = 3;
            this._stop.Text = "Stop";
            this._stop.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this._stop.UseVisualStyleBackColor = true;
            this._stop.Click += new System.EventHandler(this._stop_Click);
            // 
            // _splitContainer1
            // 
            this._splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._splitContainer1.Location = new System.Drawing.Point(0, 34);
            this._splitContainer1.Name = "_splitContainer1";
            this._splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _splitContainer1.Panel1
            // 
            this._splitContainer1.Panel1.Controls.Add(this._statTableView);
            // 
            // _splitContainer1.Panel2
            // 
            this._splitContainer1.Panel2.Controls.Add(this._measureTableView);
            this._splitContainer1.Size = new System.Drawing.Size(631, 450);
            this._splitContainer1.SplitterDistance = 208;
            this._splitContainer1.TabIndex = 4;
            // 
            // _statTableView
            // 
            this._statTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._statTableView.Location = new System.Drawing.Point(0, 0);
            this._statTableView.MenuModel = null;
            this._statTableView.Name = "_statTableView";
            this._statTableView.ReadOnly = false;
            this._statTableView.Selection = selection1;
            this._statTableView.Size = new System.Drawing.Size(631, 208);
            this._statTableView.TabIndex = 1;
            this._statTableView.Table = null;
            this._statTableView.ToolbarModel = null;
            this._statTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._statTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _measureTableView
            // 
            this._measureTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._measureTableView.Location = new System.Drawing.Point(0, 0);
            this._measureTableView.MenuModel = null;
            this._measureTableView.Name = "_measureTableView";
            this._measureTableView.ReadOnly = false;
            this._measureTableView.Selection = selection2;
            this._measureTableView.Size = new System.Drawing.Size(631, 238);
            this._measureTableView.TabIndex = 2;
            this._measureTableView.Table = null;
            this._measureTableView.ToolbarModel = null;
            this._measureTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._measureTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _statExport
            // 
            this._statExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._statExport.Location = new System.Drawing.Point(373, 5);
            this._statExport.Name = "_statExport";
            this._statExport.Size = new System.Drawing.Size(116, 23);
            this._statExport.TabIndex = 5;
            this._statExport.Text = "Export Statistics";
            this._statExport.UseVisualStyleBackColor = true;
            this._statExport.Click += new System.EventHandler(this._statExport_Click);
            // 
            // _measureExport
            // 
            this._measureExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._measureExport.Location = new System.Drawing.Point(495, 5);
            this._measureExport.Name = "_measureExport";
            this._measureExport.Size = new System.Drawing.Size(116, 23);
            this._measureExport.TabIndex = 6;
            this._measureExport.Text = "Export Measures";
            this._measureExport.UseVisualStyleBackColor = true;
            this._measureExport.Click += new System.EventHandler(this._measureExport_Click);
            // 
            // RecordGeneratorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._measureExport);
            this.Controls.Add(this._statExport);
            this.Controls.Add(this._splitContainer1);
            this.Controls.Add(this._stop);
            this.Controls.Add(this._start);
            this.Name = "RecordGeneratorComponentControl";
            this.Size = new System.Drawing.Size(634, 484);
            this._splitContainer1.Panel1.ResumeLayout(false);
            this._splitContainer1.Panel2.ResumeLayout(false);
            this._splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _start;
        private System.Windows.Forms.Button _stop;
        private System.Windows.Forms.SplitContainer _splitContainer1;
        private ClearCanvas.Desktop.View.WinForms.TableView _statTableView;
        private ClearCanvas.Desktop.View.WinForms.TableView _measureTableView;
        private System.Windows.Forms.Button _statExport;
        private System.Windows.Forms.Button _measureExport;

    }
}
