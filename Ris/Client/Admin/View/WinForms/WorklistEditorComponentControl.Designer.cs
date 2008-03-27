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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class WorklistEditorComponentControl
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
            this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._type = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._description = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._cancelButton = new System.Windows.Forms.Button();
            this._acceptButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this._procedureTypeGroupsSelector = new ClearCanvas.Desktop.View.WinForms.ListItemSelector();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._fromCheckBox = new System.Windows.Forms.CheckBox();
            this._toCheckBox = new System.Windows.Forms.CheckBox();
            this._toSliding = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._fromSliding = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._toFixed = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._fromFixed = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._fixedWindowRadioButton = new System.Windows.Forms.RadioButton();
            this._slidingWindowRadioButton = new System.Windows.Forms.RadioButton();
            this._priority = new ClearCanvas.Desktop.View.WinForms.DropListPickerField();
            this._patientClass = new ClearCanvas.Desktop.View.WinForms.DropListPickerField();
            this._facilities = new ClearCanvas.Desktop.View.WinForms.DropListPickerField();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this._usersSelector = new ClearCanvas.Desktop.View.WinForms.ListItemSelector();
            this._portable = new ClearCanvas.Desktop.View.WinForms.DropListPickerField();
            this._slidingScale = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // _name
            // 
            this._name.AutoSize = true;
            this._name.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._name.Dock = System.Windows.Forms.DockStyle.Fill;
            this._name.LabelText = "Name";
            this._name.Location = new System.Drawing.Point(2, 2);
            this._name.Margin = new System.Windows.Forms.Padding(2);
            this._name.Mask = "";
            this._name.Name = "_name";
            this._name.PasswordChar = '\0';
            this._name.Size = new System.Drawing.Size(325, 41);
            this._name.TabIndex = 0;
            this._name.ToolTip = null;
            this._name.Value = null;
            // 
            // _type
            // 
            this._type.AutoSize = true;
            this._type.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._type.DataSource = null;
            this._type.DisplayMember = "";
            this._type.Dock = System.Windows.Forms.DockStyle.Fill;
            this._type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._type.LabelText = "Type";
            this._type.Location = new System.Drawing.Point(331, 2);
            this._type.Margin = new System.Windows.Forms.Padding(2);
            this._type.Name = "_type";
            this._type.Size = new System.Drawing.Size(325, 41);
            this._type.TabIndex = 1;
            this._type.Value = null;
            // 
            // _description
            // 
            this._description.AutoSize = true;
            this._description.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.SetColumnSpan(this._description, 2);
            this._description.Dock = System.Windows.Forms.DockStyle.Fill;
            this._description.LabelText = "Description";
            this._description.Location = new System.Drawing.Point(2, 47);
            this._description.Margin = new System.Windows.Forms.Padding(2);
            this._description.Name = "_description";
            this._description.Size = new System.Drawing.Size(654, 69);
            this._description.TabIndex = 2;
            this._description.Value = null;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this._name, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this._type, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this._description, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(658, 118);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this._cancelButton);
            this.flowLayoutPanel1.Controls.Add(this._acceptButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 537);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(658, 29);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(580, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(499, 3);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 0;
            this._acceptButton.Text = "Accept";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(664, 569);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 127);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(658, 404);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this._procedureTypeGroupsSelector);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(650, 378);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Procedure Groups";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // _procedureTypeGroupsSelector
            // 
            this._procedureTypeGroupsSelector.AutoSize = true;
            this._procedureTypeGroupsSelector.AvailableItemsTable = null;
            this._procedureTypeGroupsSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this._procedureTypeGroupsSelector.Location = new System.Drawing.Point(3, 3);
            this._procedureTypeGroupsSelector.Name = "_procedureTypeGroupsSelector";
            this._procedureTypeGroupsSelector.SelectedItemsTable = null;
            this._procedureTypeGroupsSelector.ShowToolbars = false;
            this._procedureTypeGroupsSelector.Size = new System.Drawing.Size(644, 372);
            this._procedureTypeGroupsSelector.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this._portable);
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Controls.Add(this._priority);
            this.tabPage3.Controls.Add(this._patientClass);
            this.tabPage3.Controls.Add(this._facilities);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(650, 378);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Filters";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._slidingScale);
            this.groupBox1.Controls.Add(this._fromCheckBox);
            this.groupBox1.Controls.Add(this._toCheckBox);
            this.groupBox1.Controls.Add(this._toSliding);
            this.groupBox1.Controls.Add(this._fromSliding);
            this.groupBox1.Controls.Add(this._toFixed);
            this.groupBox1.Controls.Add(this._fromFixed);
            this.groupBox1.Controls.Add(this._fixedWindowRadioButton);
            this.groupBox1.Controls.Add(this._slidingWindowRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(19, 202);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(608, 170);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Time Window";
            // 
            // _fromCheckBox
            // 
            this._fromCheckBox.AutoSize = true;
            this._fromCheckBox.Location = new System.Drawing.Point(199, 37);
            this._fromCheckBox.Name = "_fromCheckBox";
            this._fromCheckBox.Size = new System.Drawing.Size(49, 17);
            this._fromCheckBox.TabIndex = 10;
            this._fromCheckBox.Text = "From";
            this._fromCheckBox.UseVisualStyleBackColor = true;
            // 
            // _toCheckBox
            // 
            this._toCheckBox.AutoSize = true;
            this._toCheckBox.Location = new System.Drawing.Point(410, 37);
            this._toCheckBox.Name = "_toCheckBox";
            this._toCheckBox.Size = new System.Drawing.Size(39, 17);
            this._toCheckBox.TabIndex = 9;
            this._toCheckBox.Text = "To";
            this._toCheckBox.UseVisualStyleBackColor = true;
            // 
            // _toSliding
            // 
            this._toSliding.DataSource = null;
            this._toSliding.DisplayMember = "";
            this._toSliding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._toSliding.LabelText = "To";
            this._toSliding.Location = new System.Drawing.Point(425, 57);
            this._toSliding.Margin = new System.Windows.Forms.Padding(2);
            this._toSliding.Name = "_toSliding";
            this._toSliding.Size = new System.Drawing.Size(150, 41);
            this._toSliding.TabIndex = 8;
            this._toSliding.Value = null;
            // 
            // _fromSliding
            // 
            this._fromSliding.DataSource = null;
            this._fromSliding.DisplayMember = "";
            this._fromSliding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._fromSliding.LabelText = "From";
            this._fromSliding.Location = new System.Drawing.Point(213, 57);
            this._fromSliding.Margin = new System.Windows.Forms.Padding(2);
            this._fromSliding.Name = "_fromSliding";
            this._fromSliding.Size = new System.Drawing.Size(150, 41);
            this._fromSliding.TabIndex = 7;
            this._fromSliding.Value = null;
            // 
            // _toFixed
            // 
            this._toFixed.LabelText = "To";
            this._toFixed.Location = new System.Drawing.Point(425, 123);
            this._toFixed.Margin = new System.Windows.Forms.Padding(2);
            this._toFixed.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._toFixed.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._toFixed.Name = "_toFixed";
            this._toFixed.Size = new System.Drawing.Size(150, 41);
            this._toFixed.TabIndex = 6;
            this._toFixed.Value = new System.DateTime(2008, 3, 14, 10, 35, 2, 968);
            // 
            // _fromFixed
            // 
            this._fromFixed.LabelText = "From";
            this._fromFixed.Location = new System.Drawing.Point(213, 123);
            this._fromFixed.Margin = new System.Windows.Forms.Padding(2);
            this._fromFixed.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._fromFixed.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._fromFixed.Name = "_fromFixed";
            this._fromFixed.Size = new System.Drawing.Size(150, 41);
            this._fromFixed.TabIndex = 5;
            this._fromFixed.Value = new System.DateTime(2008, 3, 14, 10, 35, 2, 968);
            // 
            // _fixedWindowRadioButton
            // 
            this._fixedWindowRadioButton.AutoSize = true;
            this._fixedWindowRadioButton.Location = new System.Drawing.Point(23, 137);
            this._fixedWindowRadioButton.Name = "_fixedWindowRadioButton";
            this._fixedWindowRadioButton.Size = new System.Drawing.Size(50, 17);
            this._fixedWindowRadioButton.TabIndex = 3;
            this._fixedWindowRadioButton.TabStop = true;
            this._fixedWindowRadioButton.Text = "Fixed";
            this._fixedWindowRadioButton.UseVisualStyleBackColor = true;
            // 
            // _slidingWindowRadioButton
            // 
            this._slidingWindowRadioButton.AutoSize = true;
            this._slidingWindowRadioButton.Location = new System.Drawing.Point(23, 75);
            this._slidingWindowRadioButton.Name = "_slidingWindowRadioButton";
            this._slidingWindowRadioButton.Size = new System.Drawing.Size(56, 17);
            this._slidingWindowRadioButton.TabIndex = 4;
            this._slidingWindowRadioButton.TabStop = true;
            this._slidingWindowRadioButton.Text = "Sliding";
            this._slidingWindowRadioButton.UseVisualStyleBackColor = true;
            // 
            // _priority
            // 
            this._priority.AutoSize = true;
            this._priority.LabelText = "Order Priority";
            this._priority.Location = new System.Drawing.Point(19, 96);
            this._priority.Margin = new System.Windows.Forms.Padding(2);
            this._priority.Name = "_priority";
            this._priority.Size = new System.Drawing.Size(608, 40);
            this._priority.TabIndex = 2;
            // 
            // _patientClass
            // 
            this._patientClass.AutoSize = true;
            this._patientClass.LabelText = "Patient Class";
            this._patientClass.Location = new System.Drawing.Point(19, 49);
            this._patientClass.Margin = new System.Windows.Forms.Padding(2);
            this._patientClass.Name = "_patientClass";
            this._patientClass.Size = new System.Drawing.Size(608, 40);
            this._patientClass.TabIndex = 1;
            // 
            // _facilities
            // 
            this._facilities.AutoSize = true;
            this._facilities.LabelText = "Facility";
            this._facilities.Location = new System.Drawing.Point(19, 5);
            this._facilities.Margin = new System.Windows.Forms.Padding(2);
            this._facilities.Name = "_facilities";
            this._facilities.Size = new System.Drawing.Size(608, 40);
            this._facilities.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this._usersSelector);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(650, 378);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Users";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // _usersSelector
            // 
            this._usersSelector.AutoSize = true;
            this._usersSelector.AvailableItemsTable = null;
            this._usersSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this._usersSelector.Location = new System.Drawing.Point(3, 3);
            this._usersSelector.Name = "_usersSelector";
            this._usersSelector.SelectedItemsTable = null;
            this._usersSelector.ShowToolbars = false;
            this._usersSelector.Size = new System.Drawing.Size(644, 372);
            this._usersSelector.TabIndex = 0;
            // 
            // _portable
            // 
            this._portable.AutoSize = true;
            this._portable.LabelText = "Portable";
            this._portable.Location = new System.Drawing.Point(19, 147);
            this._portable.Margin = new System.Windows.Forms.Padding(2);
            this._portable.Name = "_portable";
            this._portable.Size = new System.Drawing.Size(304, 40);
            this._portable.TabIndex = 6;
            // 
            // _slidingScale
            // 
            this._slidingScale.DataSource = null;
            this._slidingScale.DisplayMember = "";
            this._slidingScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._slidingScale.LabelText = "Scale";
            this._slidingScale.Location = new System.Drawing.Point(94, 57);
            this._slidingScale.Margin = new System.Windows.Forms.Padding(2);
            this._slidingScale.Name = "_slidingScale";
            this._slidingScale.Size = new System.Drawing.Size(91, 45);
            this._slidingScale.TabIndex = 11;
            this._slidingScale.Value = null;
            // 
            // WorklistEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "WorklistEditorComponentControl";
            this.Size = new System.Drawing.Size(664, 569);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TextField _name;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _type;
        private ClearCanvas.Desktop.View.WinForms.TextAreaField _description;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private ClearCanvas.Desktop.View.WinForms.ListItemSelector _procedureTypeGroupsSelector;
        private ClearCanvas.Desktop.View.WinForms.ListItemSelector _usersSelector;
        private System.Windows.Forms.TabPage tabPage3;
        private ClearCanvas.Desktop.View.WinForms.DropListPickerField _patientClass;
        private ClearCanvas.Desktop.View.WinForms.DropListPickerField _facilities;
        private ClearCanvas.Desktop.View.WinForms.DropListPickerField _priority;
        private System.Windows.Forms.GroupBox groupBox1;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _toFixed;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _fromFixed;
        private System.Windows.Forms.RadioButton _fixedWindowRadioButton;
        private System.Windows.Forms.RadioButton _slidingWindowRadioButton;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _toSliding;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _fromSliding;
        private System.Windows.Forms.CheckBox _fromCheckBox;
        private System.Windows.Forms.CheckBox _toCheckBox;
        private ClearCanvas.Desktop.View.WinForms.DropListPickerField _portable;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _slidingScale;
    }
}
