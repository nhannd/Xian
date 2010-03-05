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
    partial class SummaryComponentBaseControl
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
			this._summaryTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._buttonsPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.tableLayoutPanel1.SuspendLayout();
			this._buttonsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _summaryTableView
			// 
			this._summaryTableView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._summaryTableView.FilterTextBoxVisible = true;
			this._summaryTableView.Location = new System.Drawing.Point(3, 3);
			this._summaryTableView.Name = "_summaryTableView";
			this._summaryTableView.ReadOnly = false;
			this._summaryTableView.Size = new System.Drawing.Size(644, 451);
			this._summaryTableView.TabIndex = 0;
			this._summaryTableView.ItemDoubleClicked += new System.EventHandler(this._staffGroupTableView_ItemDoubleClicked);
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(566, 3);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 2;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(485, 3);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 1;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._buttonsPanel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._summaryTableView, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(650, 492);
			this.tableLayoutPanel1.TabIndex = 3;
			// 
			// _buttonsPanel
			// 
			this._buttonsPanel.Controls.Add(this._cancelButton);
			this._buttonsPanel.Controls.Add(this._okButton);
			this._buttonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._buttonsPanel.Location = new System.Drawing.Point(3, 460);
			this._buttonsPanel.Name = "_buttonsPanel";
			this._buttonsPanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this._buttonsPanel.Size = new System.Drawing.Size(644, 29);
			this._buttonsPanel.TabIndex = 0;
			// 
			// SummaryComponentBaseControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "SummaryComponentBaseControl";
			this.Size = new System.Drawing.Size(650, 492);
			this.tableLayoutPanel1.ResumeLayout(false);
			this._buttonsPanel.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _summaryTableView;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.FlowLayoutPanel _buttonsPanel;
    }
}