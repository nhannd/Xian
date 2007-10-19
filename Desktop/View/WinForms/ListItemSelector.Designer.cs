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
            this._availableItems = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._selectedItems = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this._addItemButton, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this._removeItemButton, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this._availableItems, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this._selectedItems, 2, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(555, 420);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // _addItemButton
            // 
            this._addItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._addItemButton.Location = new System.Drawing.Point(240, 190);
            this._addItemButton.Name = "_addItemButton";
            this._addItemButton.Size = new System.Drawing.Size(75, 23);
            this._addItemButton.TabIndex = 4;
            this._addItemButton.Text = ">>";
            this._addItemButton.UseVisualStyleBackColor = true;
            this._addItemButton.Click += new System.EventHandler(this.AddSelection);
            // 
            // _removeItemButton
            // 
            this._removeItemButton.Location = new System.Drawing.Point(240, 219);
            this._removeItemButton.Name = "_removeItemButton";
            this._removeItemButton.Size = new System.Drawing.Size(75, 23);
            this._removeItemButton.TabIndex = 5;
            this._removeItemButton.Text = "<<";
            this._removeItemButton.UseVisualStyleBackColor = true;
            this._removeItemButton.Click += new System.EventHandler(this.RemoveSelection);
            // 
            // _availableItems
            // 
            this._availableItems.AutoSize = true;
            this._availableItems.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._availableItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this._availableItems.FilterTextBoxVisible = true;
            this._availableItems.Location = new System.Drawing.Point(3, 16);
            this._availableItems.MenuModel = null;
            this._availableItems.Name = "_availableItems";
            this._availableItems.ReadOnly = false;
            this.tableLayoutPanel3.SetRowSpan(this._availableItems, 2);
            this._availableItems.Selection = selection1;
            this._availableItems.Size = new System.Drawing.Size(231, 401);
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
            this._selectedItems.FilterTextBoxVisible = true;
            this._selectedItems.Location = new System.Drawing.Point(321, 16);
            this._selectedItems.MenuModel = null;
            this._selectedItems.Name = "_selectedItems";
            this._selectedItems.ReadOnly = false;
            this.tableLayoutPanel3.SetRowSpan(this._selectedItems, 2);
            this._selectedItems.Selection = selection2;
            this._selectedItems.Size = new System.Drawing.Size(231, 401);
            this._selectedItems.TabIndex = 3;
            this._selectedItems.Table = null;
            this._selectedItems.ToolbarModel = null;
            this._selectedItems.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._selectedItems.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._selectedItems.ItemDoubleClicked += new System.EventHandler(this.RemoveSelection);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(321, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Selected";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Available";
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button _addItemButton;
        private System.Windows.Forms.Button _removeItemButton;
        private ClearCanvas.Desktop.View.WinForms.TableView _availableItems;
        private ClearCanvas.Desktop.View.WinForms.TableView _selectedItems;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}