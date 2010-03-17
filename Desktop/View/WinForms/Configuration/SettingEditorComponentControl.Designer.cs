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

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	partial class SettingEditorComponentControl
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._defaultValue = new System.Windows.Forms.RichTextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._currentValue = new System.Windows.Forms.RichTextBox();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 521F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 9F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(638, 552);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(2, 2);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
			this.splitContainer1.Size = new System.Drawing.Size(634, 517);
			this.splitContainer1.SplitterDistance = 252;
			this.splitContainer1.SplitterWidth = 3;
			this.splitContainer1.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._defaultValue);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(634, 252);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Default Value";
			// 
			// _defaultValue
			// 
			this._defaultValue.AcceptsTab = true;
			this._defaultValue.AutoWordSelection = true;
			this._defaultValue.DetectUrls = false;
			this._defaultValue.Dock = System.Windows.Forms.DockStyle.Fill;
			this._defaultValue.Location = new System.Drawing.Point(3, 16);
			this._defaultValue.Name = "_defaultValue";
			this._defaultValue.ReadOnly = true;
			this._defaultValue.Size = new System.Drawing.Size(628, 233);
			this._defaultValue.TabIndex = 0;
			this._defaultValue.Text = "";
			this._defaultValue.WordWrap = false;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this._currentValue);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(0, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(634, 262);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Current Value";
			// 
			// _currentValue
			// 
			this._currentValue.AcceptsTab = true;
			this._currentValue.DetectUrls = false;
			this._currentValue.Dock = System.Windows.Forms.DockStyle.Fill;
			this._currentValue.Location = new System.Drawing.Point(3, 16);
			this._currentValue.Name = "_currentValue";
			this._currentValue.Size = new System.Drawing.Size(628, 243);
			this._currentValue.TabIndex = 0;
			this._currentValue.Text = "";
			this._currentValue.WordWrap = false;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this._okButton);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 523);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.flowLayoutPanel1.Size = new System.Drawing.Size(634, 27);
			this.flowLayoutPanel1.TabIndex = 1;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(576, 2);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(516, 2);
			this._okButton.Margin = new System.Windows.Forms.Padding(2);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 1;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// SettingEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "SettingEditorComponentControl";
			this.Size = new System.Drawing.Size(638, 552);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RichTextBox _defaultValue;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RichTextBox _currentValue;
	}
}