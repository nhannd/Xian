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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class HL7QueuePreviewComponentControl
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
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this._queue = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this._updatedOnStart = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._updatedOnEnd = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._directionChkBox = new System.Windows.Forms.CheckBox();
			this._peerChkBox = new System.Windows.Forms.CheckBox();
			this._typeChkBox = new System.Windows.Forms.CheckBox();
			this._statusChkBox = new System.Windows.Forms.CheckBox();
			this._direction = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._peer = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._type = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._status = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			this._searchButton = new System.Windows.Forms.Button();
			this._showAll = new System.Windows.Forms.Button();
			this._createdOnStart = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._createdOnEnd = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._message = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.flowLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(933, 378);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._message);
			this.splitContainer1.Size = new System.Drawing.Size(927, 372);
			this.splitContainer1.SplitterDistance = 596;
			this.splitContainer1.TabIndex = 0;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this._queue, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(590, 366);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// _queue
			// 
			this._queue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._queue.AutoSize = true;
			this._queue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._queue.Location = new System.Drawing.Point(3, 189);
			this._queue.MultiSelect = false;
			this._queue.Name = "_queue";
			this._queue.ReadOnly = false;
			this._queue.Size = new System.Drawing.Size(584, 174);
			this._queue.TabIndex = 1;
			this._queue.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._queue.SelectionChanged += new System.EventHandler(this._queue_SelectionChanged);
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 6;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.Controls.Add(this._updatedOnStart, 2, 2);
			this.tableLayoutPanel3.Controls.Add(this._updatedOnEnd, 3, 2);
			this.tableLayoutPanel3.Controls.Add(this._directionChkBox, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this._peerChkBox, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this._typeChkBox, 0, 2);
			this.tableLayoutPanel3.Controls.Add(this._statusChkBox, 0, 3);
			this.tableLayoutPanel3.Controls.Add(this._direction, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this._peer, 1, 1);
			this.tableLayoutPanel3.Controls.Add(this._type, 1, 2);
			this.tableLayoutPanel3.Controls.Add(this._status, 1, 3);
			this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel2, 5, 0);
			this.tableLayoutPanel3.Controls.Add(this._createdOnStart, 2, 0);
			this.tableLayoutPanel3.Controls.Add(this._createdOnEnd, 3, 0);
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 4;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(584, 180);
			this.tableLayoutPanel3.TabIndex = 0;
			// 
			// _updatedOnStart
			// 
			this._updatedOnStart.AutoSize = true;
			this._updatedOnStart.LabelText = "Processed - Start";
			this._updatedOnStart.Location = new System.Drawing.Point(171, 92);
			this._updatedOnStart.Margin = new System.Windows.Forms.Padding(2);
			this._updatedOnStart.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._updatedOnStart.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._updatedOnStart.Name = "_updatedOnStart";
			this._updatedOnStart.Nullable = true;
			this._updatedOnStart.ShowTime = true;
			this._updatedOnStart.Size = new System.Drawing.Size(150, 40);
			this._updatedOnStart.TabIndex = 8;
			this._updatedOnStart.Value = null;
			// 
			// _updatedOnEnd
			// 
			this._updatedOnEnd.AutoSize = true;
			this._updatedOnEnd.LabelText = "Processed - End";
			this._updatedOnEnd.Location = new System.Drawing.Point(325, 92);
			this._updatedOnEnd.Margin = new System.Windows.Forms.Padding(2);
			this._updatedOnEnd.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._updatedOnEnd.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._updatedOnEnd.Name = "_updatedOnEnd";
			this._updatedOnEnd.Nullable = true;
			this._updatedOnEnd.ShowTime = true;
			this._updatedOnEnd.Size = new System.Drawing.Size(150, 40);
			this._updatedOnEnd.TabIndex = 9;
			this._updatedOnEnd.Value = null;
			// 
			// _directionChkBox
			// 
			this._directionChkBox.AutoSize = true;
			this._directionChkBox.Location = new System.Drawing.Point(3, 3);
			this._directionChkBox.Name = "_directionChkBox";
			this._directionChkBox.Size = new System.Drawing.Size(15, 14);
			this._directionChkBox.TabIndex = 0;
			this._directionChkBox.UseVisualStyleBackColor = true;
			this._directionChkBox.CheckedChanged += new System.EventHandler(this._directionChkBox_CheckedChanged);
			// 
			// _peerChkBox
			// 
			this._peerChkBox.AutoSize = true;
			this._peerChkBox.Location = new System.Drawing.Point(3, 48);
			this._peerChkBox.Name = "_peerChkBox";
			this._peerChkBox.Size = new System.Drawing.Size(15, 14);
			this._peerChkBox.TabIndex = 4;
			this._peerChkBox.UseVisualStyleBackColor = true;
			this._peerChkBox.CheckedChanged += new System.EventHandler(this._peerChkBox_CheckedChanged);
			// 
			// _typeChkBox
			// 
			this._typeChkBox.AutoSize = true;
			this._typeChkBox.Location = new System.Drawing.Point(3, 93);
			this._typeChkBox.Name = "_typeChkBox";
			this._typeChkBox.Size = new System.Drawing.Size(15, 14);
			this._typeChkBox.TabIndex = 6;
			this._typeChkBox.UseVisualStyleBackColor = true;
			this._typeChkBox.CheckedChanged += new System.EventHandler(this._typeChkBox_CheckedChanged);
			// 
			// _statusChkBox
			// 
			this._statusChkBox.AutoSize = true;
			this._statusChkBox.Location = new System.Drawing.Point(3, 138);
			this._statusChkBox.Name = "_statusChkBox";
			this._statusChkBox.Size = new System.Drawing.Size(15, 14);
			this._statusChkBox.TabIndex = 10;
			this._statusChkBox.UseVisualStyleBackColor = true;
			this._statusChkBox.CheckedChanged += new System.EventHandler(this._statusChkBox_CheckedChanged);
			// 
			// _direction
			// 
			this._direction.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._direction.AutoSize = true;
			this._direction.DataSource = null;
			this._direction.DisplayMember = "";
			this._direction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._direction.Enabled = false;
			this._direction.LabelText = "Direction";
			this._direction.Location = new System.Drawing.Point(23, 2);
			this._direction.Margin = new System.Windows.Forms.Padding(2);
			this._direction.Name = "_direction";
			this._direction.Size = new System.Drawing.Size(144, 41);
			this._direction.TabIndex = 1;
			this._direction.Value = null;
			// 
			// _peer
			// 
			this._peer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._peer.AutoSize = true;
			this._peer.DataSource = null;
			this._peer.DisplayMember = "";
			this._peer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._peer.Enabled = false;
			this._peer.LabelText = "Peer";
			this._peer.Location = new System.Drawing.Point(23, 47);
			this._peer.Margin = new System.Windows.Forms.Padding(2);
			this._peer.Name = "_peer";
			this._peer.Size = new System.Drawing.Size(144, 41);
			this._peer.TabIndex = 5;
			this._peer.Value = null;
			// 
			// _type
			// 
			this._type.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._type.AutoSize = true;
			this._type.DataSource = null;
			this._type.DisplayMember = "";
			this._type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._type.Enabled = false;
			this._type.LabelText = "Type";
			this._type.Location = new System.Drawing.Point(23, 92);
			this._type.Margin = new System.Windows.Forms.Padding(2);
			this._type.Name = "_type";
			this._type.Size = new System.Drawing.Size(144, 41);
			this._type.TabIndex = 7;
			this._type.Value = null;
			// 
			// _status
			// 
			this._status.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._status.AutoSize = true;
			this._status.DataSource = null;
			this._status.DisplayMember = "";
			this._status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._status.Enabled = false;
			this._status.LabelText = "Status";
			this._status.Location = new System.Drawing.Point(23, 137);
			this._status.Margin = new System.Windows.Forms.Padding(2);
			this._status.Name = "_status";
			this._status.Size = new System.Drawing.Size(144, 41);
			this._status.TabIndex = 11;
			this._status.Value = null;
			// 
			// flowLayoutPanel2
			// 
			this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel2.AutoSize = true;
			this.flowLayoutPanel2.Controls.Add(this._searchButton);
			this.flowLayoutPanel2.Controls.Add(this._showAll);
			this.flowLayoutPanel2.Location = new System.Drawing.Point(500, 3);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tableLayoutPanel3.SetRowSpan(this.flowLayoutPanel2, 4);
			this.flowLayoutPanel2.Size = new System.Drawing.Size(81, 174);
			this.flowLayoutPanel2.TabIndex = 12;
			// 
			// _searchButton
			// 
			this._searchButton.Location = new System.Drawing.Point(3, 3);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(75, 23);
			this._searchButton.TabIndex = 0;
			this._searchButton.Text = "Search";
			this._searchButton.UseVisualStyleBackColor = true;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// _showAll
			// 
			this._showAll.Location = new System.Drawing.Point(3, 32);
			this._showAll.Name = "_showAll";
			this._showAll.Size = new System.Drawing.Size(75, 23);
			this._showAll.TabIndex = 1;
			this._showAll.Text = "Show All";
			this._showAll.UseVisualStyleBackColor = true;
			this._showAll.Click += new System.EventHandler(this._showAll_Click);
			// 
			// _createdOnStart
			// 
			this._createdOnStart.AutoSize = true;
			this._createdOnStart.LabelText = "Created - Start";
			this._createdOnStart.Location = new System.Drawing.Point(171, 2);
			this._createdOnStart.Margin = new System.Windows.Forms.Padding(2);
			this._createdOnStart.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._createdOnStart.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._createdOnStart.Name = "_createdOnStart";
			this._createdOnStart.Nullable = true;
			this._createdOnStart.ShowTime = true;
			this._createdOnStart.Size = new System.Drawing.Size(150, 40);
			this._createdOnStart.TabIndex = 2;
			this._createdOnStart.Value = null;
			// 
			// _createdOnEnd
			// 
			this._createdOnEnd.AutoSize = true;
			this._createdOnEnd.LabelText = "Created - End";
			this._createdOnEnd.Location = new System.Drawing.Point(325, 2);
			this._createdOnEnd.Margin = new System.Windows.Forms.Padding(2);
			this._createdOnEnd.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._createdOnEnd.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._createdOnEnd.Name = "_createdOnEnd";
			this._createdOnEnd.Nullable = true;
			this._createdOnEnd.ShowTime = true;
			this._createdOnEnd.Size = new System.Drawing.Size(150, 40);
			this._createdOnEnd.TabIndex = 3;
			this._createdOnEnd.Value = null;
			// 
			// _message
			// 
			this._message.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._message.Location = new System.Drawing.Point(3, 3);
			this._message.Multiline = true;
			this._message.Name = "_message";
			this._message.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this._message.Size = new System.Drawing.Size(321, 363);
			this._message.TabIndex = 0;
			// 
			// HL7QueuePreviewComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "HL7QueuePreviewComponentControl";
			this.Size = new System.Drawing.Size(939, 384);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.flowLayoutPanel2.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.TableView _queue;
        private System.Windows.Forms.Button _showAll;
        private System.Windows.Forms.TextBox _message;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _createdOnStart;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _createdOnEnd;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _updatedOnStart;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _updatedOnEnd;
        private System.Windows.Forms.CheckBox _directionChkBox;
        private System.Windows.Forms.CheckBox _peerChkBox;
        private System.Windows.Forms.CheckBox _typeChkBox;
        private System.Windows.Forms.CheckBox _statusChkBox;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _direction;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _peer;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _type;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _status;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button _searchButton;

    }
}
