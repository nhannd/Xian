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

namespace ClearCanvas.Desktop.View.WinForms
{
    partial class ListItemSelector
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
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this._addItemButton = new System.Windows.Forms.Button();
            this._removeItemButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._availableItems = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._selectedItems = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._filterColumn = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._filterValue = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._clearFilterButton = new System.Windows.Forms.Button();
            this._applyFilterButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this._addItemButton, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this._removeItemButton, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label2, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this._availableItems, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this._selectedItems, 2, 2);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(555, 420);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // _addItemButton
            // 
            this._addItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._addItemButton.Location = new System.Drawing.Point(240, 233);
            this._addItemButton.Name = "_addItemButton";
            this._addItemButton.Size = new System.Drawing.Size(75, 23);
            this._addItemButton.TabIndex = 4;
            this._addItemButton.Text = ">>";
            this._addItemButton.UseVisualStyleBackColor = true;
            this._addItemButton.Click += new System.EventHandler(this.AddSelection);
            // 
            // _removeItemButton
            // 
            this._removeItemButton.Location = new System.Drawing.Point(240, 262);
            this._removeItemButton.Name = "_removeItemButton";
            this._removeItemButton.Size = new System.Drawing.Size(75, 23);
            this._removeItemButton.TabIndex = 5;
            this._removeItemButton.Text = "<<";
            this._removeItemButton.UseVisualStyleBackColor = true;
            this._removeItemButton.Click += new System.EventHandler(this.RemoveSelection);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Available";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(321, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Selected";
            // 
            // _availableItems
            // 
            this._availableItems.AutoSize = true;
            this._availableItems.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._availableItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this._availableItems.Location = new System.Drawing.Point(3, 102);
            this._availableItems.MenuModel = null;
            this._availableItems.Name = "_availableItems";
            this._availableItems.ReadOnly = false;
            this.tableLayoutPanel3.SetRowSpan(this._availableItems, 2);
            this._availableItems.Selection = selection1;
            this._availableItems.ShowToolbar = false;
            this._availableItems.Size = new System.Drawing.Size(231, 315);
            this._availableItems.TabIndex = 2;
            this._availableItems.Table = null;
            this._availableItems.ToolbarModel = null;
            this._availableItems.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._availableItems.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._availableItems.ItemDoubleClicked += new System.EventHandler(this.AddSelection);
            // 
            // _selectedItems
            // 
            this._selectedItems.AutoSize = true;
            this._selectedItems.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._selectedItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this._selectedItems.Location = new System.Drawing.Point(321, 102);
            this._selectedItems.MenuModel = null;
            this._selectedItems.Name = "_selectedItems";
            this._selectedItems.ReadOnly = false;
            this.tableLayoutPanel3.SetRowSpan(this._selectedItems, 2);
            this._selectedItems.Selection = selection2;
            this._selectedItems.ShowToolbar = false;
            this._selectedItems.Size = new System.Drawing.Size(231, 315);
            this._selectedItems.TabIndex = 3;
            this._selectedItems.Table = null;
            this._selectedItems.ToolbarModel = null;
            this._selectedItems.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._selectedItems.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._selectedItems.ItemDoubleClicked += new System.EventHandler(this.RemoveSelection);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel3.SetColumnSpan(this.tableLayoutPanel1, 3);
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this._filterColumn, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._filterValue, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(549, 80);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // _filterColumn
            // 
            this._filterColumn.DataSource = null;
            this._filterColumn.DisplayMember = "";
            this._filterColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filterColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._filterColumn.LabelText = "Filter By";
            this._filterColumn.Location = new System.Drawing.Point(2, 2);
            this._filterColumn.Margin = new System.Windows.Forms.Padding(2);
            this._filterColumn.Name = "_filterColumn";
            this._filterColumn.Size = new System.Drawing.Size(270, 41);
            this._filterColumn.TabIndex = 0;
            this._filterColumn.Value = null;
            // 
            // _filterValue
            // 
            this._filterValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filterValue.LabelText = "Value";
            this._filterValue.Location = new System.Drawing.Point(276, 2);
            this._filterValue.Margin = new System.Windows.Forms.Padding(2);
            this._filterValue.Mask = "";
            this._filterValue.Name = "_filterValue";
            this._filterValue.Size = new System.Drawing.Size(271, 41);
            this._filterValue.TabIndex = 1;
            this._filterValue.Value = null;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this._clearFilterButton);
            this.flowLayoutPanel1.Controls.Add(this._applyFilterButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 48);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(543, 29);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // _clearFilterButton
            // 
            this._clearFilterButton.Location = new System.Drawing.Point(465, 3);
            this._clearFilterButton.Name = "_clearFilterButton";
            this._clearFilterButton.Size = new System.Drawing.Size(75, 23);
            this._clearFilterButton.TabIndex = 0;
            this._clearFilterButton.Text = "Clear";
            this._clearFilterButton.UseVisualStyleBackColor = true;
            this._clearFilterButton.Click += new System.EventHandler(this._clearFilterButton_Click);
            // 
            // _applyFilterButton
            // 
            this._applyFilterButton.Location = new System.Drawing.Point(384, 3);
            this._applyFilterButton.Name = "_applyFilterButton";
            this._applyFilterButton.Size = new System.Drawing.Size(75, 23);
            this._applyFilterButton.TabIndex = 1;
            this._applyFilterButton.Text = "Apply";
            this._applyFilterButton.UseVisualStyleBackColor = true;
            this._applyFilterButton.Click += new System.EventHandler(this._applyFilterButton_Click);
            // 
            // ListItemSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "ListItemSelector";
            this.Size = new System.Drawing.Size(555, 420);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button _addItemButton;
        private System.Windows.Forms.Button _removeItemButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private ClearCanvas.Desktop.View.WinForms.TableView _availableItems;
        private ClearCanvas.Desktop.View.WinForms.TableView _selectedItems;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _filterColumn;
        private ClearCanvas.Desktop.View.WinForms.TextField _filterValue;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _clearFilterButton;
        private System.Windows.Forms.Button _applyFilterButton;
    }
}