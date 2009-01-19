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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class OrderNoteConversationComponentControl
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
            this._componentTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._cancelButton = new System.Windows.Forms.Button();
            this._completeButton = new System.Windows.Forms.Button();
            this._replyCheckBox = new System.Windows.Forms.CheckBox();
            this._componentSplitContainer = new System.Windows.Forms.SplitContainer();
            this._notesGroupBox = new System.Windows.Forms.GroupBox();
            this._orderNotesPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._replyGroupBox = new System.Windows.Forms.GroupBox();
            this._replyTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this._replyBody = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this._onBehalf = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._urgent = new System.Windows.Forms.CheckBox();
            this._notifyToGroupBox = new System.Windows.Forms.GroupBox();
            this._recipientsTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this._staffRecipientLookup = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
            this._groupRecipientLookup = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
            this._groupRecipientAddButton = new System.Windows.Forms.Button();
            this._staffRecipientAddButton = new System.Windows.Forms.Button();
            this._recipients = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._componentTableLayout.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this._componentSplitContainer.Panel1.SuspendLayout();
            this._componentSplitContainer.Panel2.SuspendLayout();
            this._componentSplitContainer.SuspendLayout();
            this._notesGroupBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this._replyGroupBox.SuspendLayout();
            this._replyTableLayout.SuspendLayout();
            this.panel1.SuspendLayout();
            this._notifyToGroupBox.SuspendLayout();
            this._recipientsTableLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // _componentTableLayout
            // 
            this._componentTableLayout.AutoSize = true;
            this._componentTableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._componentTableLayout.ColumnCount = 1;
            this._componentTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._componentTableLayout.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this._componentTableLayout.Controls.Add(this._componentSplitContainer, 0, 0);
            this._componentTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._componentTableLayout.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this._componentTableLayout.Location = new System.Drawing.Point(0, 0);
            this._componentTableLayout.Margin = new System.Windows.Forms.Padding(0);
            this._componentTableLayout.Name = "_componentTableLayout";
            this._componentTableLayout.RowCount = 2;
            this._componentTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._componentTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._componentTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._componentTableLayout.Size = new System.Drawing.Size(843, 695);
            this._componentTableLayout.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this._cancelButton);
            this.flowLayoutPanel1.Controls.Add(this._completeButton);
            this.flowLayoutPanel1.Controls.Add(this._replyCheckBox);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 663);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(837, 29);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(759, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 2;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _completeButton
            // 
            this._completeButton.AutoSize = true;
            this._completeButton.Location = new System.Drawing.Point(671, 3);
            this._completeButton.Name = "_completeButton";
            this._completeButton.Size = new System.Drawing.Size(82, 23);
            this._completeButton.TabIndex = 1;
            this._completeButton.Text = "Acknowledge";
            this._completeButton.UseVisualStyleBackColor = true;
            this._completeButton.Click += new System.EventHandler(this._completeButton_Click);
            // 
            // _replyCheckBox
            // 
            this._replyCheckBox.AutoSize = true;
            this._replyCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._replyCheckBox.Location = new System.Drawing.Point(592, 3);
            this._replyCheckBox.Name = "_replyCheckBox";
            this._replyCheckBox.Padding = new System.Windows.Forms.Padding(0, 3, 20, 0);
            this._replyCheckBox.Size = new System.Drawing.Size(73, 20);
            this._replyCheckBox.TabIndex = 0;
            this._replyCheckBox.Text = "Reply";
            this._replyCheckBox.UseVisualStyleBackColor = true;
            // 
            // _componentSplitContainer
            // 
            this._componentSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._componentSplitContainer.Location = new System.Drawing.Point(3, 3);
            this._componentSplitContainer.Name = "_componentSplitContainer";
            this._componentSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _componentSplitContainer.Panel1
            // 
            this._componentSplitContainer.Panel1.Controls.Add(this._notesGroupBox);
            // 
            // _componentSplitContainer.Panel2
            // 
            this._componentSplitContainer.Panel2.Controls.Add(this.tableLayoutPanel1);
            this._componentSplitContainer.Size = new System.Drawing.Size(837, 654);
            this._componentSplitContainer.SplitterDistance = 348;
            this._componentSplitContainer.TabIndex = 1;
            this._componentSplitContainer.TabStop = false;
            // 
            // _notesGroupBox
            // 
            this._notesGroupBox.AutoSize = true;
            this._notesGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._notesGroupBox.Controls.Add(this._orderNotesPanel);
            this._notesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._notesGroupBox.Location = new System.Drawing.Point(0, 0);
            this._notesGroupBox.Name = "_notesGroupBox";
            this._notesGroupBox.Size = new System.Drawing.Size(837, 348);
            this._notesGroupBox.TabIndex = 0;
            this._notesGroupBox.TabStop = false;
            this._notesGroupBox.Text = "Conversation History";
            // 
            // _orderNotesPanel
            // 
            this._orderNotesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._orderNotesPanel.Location = new System.Drawing.Point(3, 16);
            this._orderNotesPanel.Name = "_orderNotesPanel";
            this._orderNotesPanel.Size = new System.Drawing.Size(831, 329);
            this._orderNotesPanel.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.00717F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.99283F));
            this.tableLayoutPanel1.Controls.Add(this._replyGroupBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._notifyToGroupBox, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(837, 302);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _replyGroupBox
            // 
            this._replyGroupBox.AutoSize = true;
            this._replyGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._replyGroupBox.Controls.Add(this._replyTableLayout);
            this._replyGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._replyGroupBox.Location = new System.Drawing.Point(3, 3);
            this._replyGroupBox.Name = "_replyGroupBox";
            this._replyGroupBox.Size = new System.Drawing.Size(513, 296);
            this._replyGroupBox.TabIndex = 0;
            this._replyGroupBox.TabStop = false;
            this._replyGroupBox.Text = "Add a note";
            // 
            // _replyTableLayout
            // 
            this._replyTableLayout.AutoSize = true;
            this._replyTableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._replyTableLayout.ColumnCount = 2;
            this._replyTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._replyTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._replyTableLayout.Controls.Add(this._replyBody, 0, 1);
            this._replyTableLayout.Controls.Add(this.panel1, 0, 0);
            this._replyTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._replyTableLayout.Location = new System.Drawing.Point(3, 16);
            this._replyTableLayout.Margin = new System.Windows.Forms.Padding(0);
            this._replyTableLayout.Name = "_replyTableLayout";
            this._replyTableLayout.RowCount = 2;
            this._replyTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._replyTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._replyTableLayout.Size = new System.Drawing.Size(507, 277);
            this._replyTableLayout.TabIndex = 0;
            // 
            // _replyBody
            // 
            this._replyBody.AcceptsTab = true;
            this._replyBody.AutoWordSelection = true;
            this._replyBody.DetectUrls = false;
            this._replyBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this._replyBody.Location = new System.Drawing.Point(3, 59);
            this._replyBody.Name = "_replyBody";
            this._replyBody.Size = new System.Drawing.Size(501, 215);
            this._replyBody.TabIndex = 1;
            this._replyBody.Text = "";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._onBehalf);
            this.panel1.Controls.Add(this._urgent);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(501, 50);
            this.panel1.TabIndex = 0;
            // 
            // _onBehalf
            // 
            this._onBehalf.AutoSize = true;
            this._onBehalf.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._onBehalf.DataSource = null;
            this._onBehalf.DisplayMember = "";
            this._onBehalf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._onBehalf.LabelText = "On Behalf Of The Group";
            this._onBehalf.Location = new System.Drawing.Point(2, 2);
            this._onBehalf.Margin = new System.Windows.Forms.Padding(2);
            this._onBehalf.Name = "_onBehalf";
            this._onBehalf.Size = new System.Drawing.Size(317, 41);
            this._onBehalf.TabIndex = 0;
            this._onBehalf.Value = null;
            // 
            // _urgent
            // 
            this._urgent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._urgent.AutoSize = true;
            this._urgent.Location = new System.Drawing.Point(415, 21);
            this._urgent.Name = "_urgent";
            this._urgent.Size = new System.Drawing.Size(58, 17);
            this._urgent.TabIndex = 1;
            this._urgent.Text = "Urgent";
            this._urgent.UseVisualStyleBackColor = true;
            // 
            // _notifyToGroupBox
            // 
            this._notifyToGroupBox.Controls.Add(this._recipientsTableLayout);
            this._notifyToGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._notifyToGroupBox.Location = new System.Drawing.Point(522, 3);
            this._notifyToGroupBox.Name = "_notifyToGroupBox";
            this._notifyToGroupBox.Size = new System.Drawing.Size(312, 296);
            this._notifyToGroupBox.TabIndex = 0;
            this._notifyToGroupBox.TabStop = false;
            this._notifyToGroupBox.Text = "Parties to Notify";
            // 
            // _recipientsTableLayout
            // 
            this._recipientsTableLayout.AutoSize = true;
            this._recipientsTableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._recipientsTableLayout.ColumnCount = 2;
            this._recipientsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._recipientsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._recipientsTableLayout.Controls.Add(this._staffRecipientLookup, 0, 1);
            this._recipientsTableLayout.Controls.Add(this._groupRecipientLookup, 0, 0);
            this._recipientsTableLayout.Controls.Add(this._groupRecipientAddButton, 1, 0);
            this._recipientsTableLayout.Controls.Add(this._staffRecipientAddButton, 1, 1);
            this._recipientsTableLayout.Controls.Add(this._recipients, 0, 2);
            this._recipientsTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._recipientsTableLayout.Location = new System.Drawing.Point(3, 16);
            this._recipientsTableLayout.Margin = new System.Windows.Forms.Padding(0);
            this._recipientsTableLayout.Name = "_recipientsTableLayout";
            this._recipientsTableLayout.RowCount = 3;
            this._recipientsTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._recipientsTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._recipientsTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._recipientsTableLayout.Size = new System.Drawing.Size(306, 277);
            this._recipientsTableLayout.TabIndex = 1;
            // 
            // _staffRecipientLookup
            // 
            this._staffRecipientLookup.AutoSize = true;
            this._staffRecipientLookup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._staffRecipientLookup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._staffRecipientLookup.LabelText = "Staff to Notify";
            this._staffRecipientLookup.Location = new System.Drawing.Point(0, 41);
            this._staffRecipientLookup.Margin = new System.Windows.Forms.Padding(0);
            this._staffRecipientLookup.Name = "_staffRecipientLookup";
            this._staffRecipientLookup.Size = new System.Drawing.Size(264, 41);
            this._staffRecipientLookup.TabIndex = 2;
            this._staffRecipientLookup.Value = null;
            // 
            // _groupRecipientLookup
            // 
            this._groupRecipientLookup.AutoSize = true;
            this._groupRecipientLookup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._groupRecipientLookup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._groupRecipientLookup.LabelText = "Group to Notify";
            this._groupRecipientLookup.Location = new System.Drawing.Point(0, 0);
            this._groupRecipientLookup.Margin = new System.Windows.Forms.Padding(0);
            this._groupRecipientLookup.Name = "_groupRecipientLookup";
            this._groupRecipientLookup.Size = new System.Drawing.Size(264, 41);
            this._groupRecipientLookup.TabIndex = 0;
            this._groupRecipientLookup.Value = null;
            // 
            // _groupRecipientAddButton
            // 
            this._groupRecipientAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._groupRecipientAddButton.AutoSize = true;
            this._groupRecipientAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._groupRecipientAddButton.Location = new System.Drawing.Point(267, 15);
            this._groupRecipientAddButton.Name = "_groupRecipientAddButton";
            this._groupRecipientAddButton.Size = new System.Drawing.Size(36, 23);
            this._groupRecipientAddButton.TabIndex = 1;
            this._groupRecipientAddButton.Text = "Add";
            this._groupRecipientAddButton.UseVisualStyleBackColor = true;
            this._groupRecipientAddButton.Click += new System.EventHandler(this._groupRecipientAddButton_Click);
            // 
            // _staffRecipientAddButton
            // 
            this._staffRecipientAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._staffRecipientAddButton.AutoSize = true;
            this._staffRecipientAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._staffRecipientAddButton.Location = new System.Drawing.Point(267, 56);
            this._staffRecipientAddButton.Name = "_staffRecipientAddButton";
            this._staffRecipientAddButton.Size = new System.Drawing.Size(36, 23);
            this._staffRecipientAddButton.TabIndex = 3;
            this._staffRecipientAddButton.Text = "Add";
            this._staffRecipientAddButton.UseVisualStyleBackColor = true;
            this._staffRecipientAddButton.Click += new System.EventHandler(this._staffRecipientAddButton_Click);
            // 
            // _recipients
            // 
            this._recipients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._recipients.AutoSize = true;
            this._recipients.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._recipientsTableLayout.SetColumnSpan(this._recipients, 2);
            this._recipients.Location = new System.Drawing.Point(3, 85);
            this._recipients.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
            this._recipients.Name = "_recipients";
            this._recipients.ReadOnly = false;
            this._recipients.ShowToolbar = false;
            this._recipients.Size = new System.Drawing.Size(288, 189);
            this._recipients.TabIndex = 4;
            // 
            // OrderNoteConversationComponentControl
            // 
            this.AcceptButton = this._completeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this._componentTableLayout);
            this.Name = "OrderNoteConversationComponentControl";
            this.Size = new System.Drawing.Size(843, 695);
            this._componentTableLayout.ResumeLayout(false);
            this._componentTableLayout.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this._componentSplitContainer.Panel1.ResumeLayout(false);
            this._componentSplitContainer.Panel1.PerformLayout();
            this._componentSplitContainer.Panel2.ResumeLayout(false);
            this._componentSplitContainer.ResumeLayout(false);
            this._notesGroupBox.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this._replyGroupBox.ResumeLayout(false);
            this._replyGroupBox.PerformLayout();
            this._replyTableLayout.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this._notifyToGroupBox.ResumeLayout(false);
            this._notifyToGroupBox.PerformLayout();
            this._recipientsTableLayout.ResumeLayout(false);
            this._recipientsTableLayout.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel _componentTableLayout;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _completeButton;
		private System.Windows.Forms.TableLayoutPanel _replyTableLayout;
		private System.Windows.Forms.GroupBox _notesGroupBox;
		private System.Windows.Forms.GroupBox _replyGroupBox;
		private System.Windows.Forms.RichTextBox _replyBody;
		private System.Windows.Forms.GroupBox _notifyToGroupBox;
		private System.Windows.Forms.TableLayoutPanel _recipientsTableLayout;
		private ClearCanvas.Desktop.View.WinForms.TableView _recipients;
		private LookupField _staffRecipientLookup;
		private LookupField _groupRecipientLookup;
		private System.Windows.Forms.Button _groupRecipientAddButton;
		private System.Windows.Forms.Button _staffRecipientAddButton;
		private System.Windows.Forms.CheckBox _urgent;
		private System.Windows.Forms.Panel panel1;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _onBehalf;
        private System.Windows.Forms.SplitContainer _componentSplitContainer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox _replyCheckBox;
		private System.Windows.Forms.Panel _orderNotesPanel;
    }
}
