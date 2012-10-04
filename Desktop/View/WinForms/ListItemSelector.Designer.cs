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
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this._addItemButton = new System.Windows.Forms.Button();
			this._removeItemButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._availableItems = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._selectedItems = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.tableLayoutPanel3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.ColumnCount = 3;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.Controls.Add(this._addItemButton, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this._removeItemButton, 1, 1);
			this.tableLayoutPanel3.Controls.Add(this.groupBox1, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this.groupBox2, 2, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 2;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(555, 420);
			this.tableLayoutPanel3.TabIndex = 0;
			// 
			// _addItemButton
			// 
			this._addItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._addItemButton.AutoSize = true;
			this._addItemButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._addItemButton.Location = new System.Drawing.Point(263, 184);
			this._addItemButton.Name = "_addItemButton";
			this._addItemButton.Size = new System.Drawing.Size(29, 23);
			this._addItemButton.TabIndex = 0;
			this._addItemButton.Text = ">>";
			this._addItemButton.UseVisualStyleBackColor = true;
			this._addItemButton.Click += new System.EventHandler(this.AddSelection);
			// 
			// _removeItemButton
			// 
			this._removeItemButton.AutoSize = true;
			this._removeItemButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._removeItemButton.Location = new System.Drawing.Point(263, 213);
			this._removeItemButton.Name = "_removeItemButton";
			this._removeItemButton.Size = new System.Drawing.Size(29, 23);
			this._removeItemButton.TabIndex = 1;
			this._removeItemButton.Text = "<<";
			this._removeItemButton.UseVisualStyleBackColor = true;
			this._removeItemButton.Click += new System.EventHandler(this.RemoveSelection);
			// 
			// groupBox1
			// 
			this.groupBox1.AutoSize = true;
			this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBox1.Controls.Add(this._availableItems);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.tableLayoutPanel3.SetRowSpan(this.groupBox1, 2);
			this.groupBox1.Size = new System.Drawing.Size(254, 414);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Available";
			// 
			// _availableItems
			// 
			this._availableItems.AutoSize = true;
			this._availableItems.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._availableItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this._availableItems.FilterTextBoxVisible = true;
			this._availableItems.Location = new System.Drawing.Point(3, 16);
			this._availableItems.Name = "_availableItems";
			this._availableItems.ReadOnly = false;
			this._availableItems.Size = new System.Drawing.Size(248, 395);
			this._availableItems.TabIndex = 0;
			this._availableItems.ItemDoubleClicked += new System.EventHandler(this.AddSelection);
			// 
			// groupBox2
			// 
			this.groupBox2.AutoSize = true;
			this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBox2.Controls.Add(this._selectedItems);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(298, 3);
			this.groupBox2.Name = "groupBox2";
			this.tableLayoutPanel3.SetRowSpan(this.groupBox2, 2);
			this.groupBox2.Size = new System.Drawing.Size(254, 414);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Selected";
			// 
			// _selectedItems
			// 
			this._selectedItems.AutoSize = true;
			this._selectedItems.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._selectedItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this._selectedItems.FilterTextBoxVisible = true;
			this._selectedItems.Location = new System.Drawing.Point(3, 16);
			this._selectedItems.Name = "_selectedItems";
			this._selectedItems.ReadOnly = false;
			this._selectedItems.Size = new System.Drawing.Size(248, 395);
			this._selectedItems.TabIndex = 0;
			this._selectedItems.ItemDoubleClicked += new System.EventHandler(this.RemoveSelection);
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
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button _addItemButton;
        private System.Windows.Forms.Button _removeItemButton;
        private ClearCanvas.Desktop.View.WinForms.TableView _availableItems;
        private ClearCanvas.Desktop.View.WinForms.TableView _selectedItems;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}