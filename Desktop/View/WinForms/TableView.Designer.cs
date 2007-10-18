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
	partial class TableView
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer _components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (_components != null))
			{
				UnsubscribeFromOldTable();
				_components.Dispose();
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableView));
            this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._selectionChangeTimer = new System.Windows.Forms.Timer(this.components);
            this._dataGridView = new ClearCanvas.Desktop.View.WinForms.DataGridViewWithDragSupport();
            this._toolStrip = new System.Windows.Forms.ToolStrip();
            this._sortToolStrip = new System.Windows.Forms.ToolStripDropDownButton();
            this._sortAscendingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._sortDescendingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._sortToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this._bindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).BeginInit();
            this._toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _contextMenu
            // 
            this._contextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this._contextMenu.Name = "_contextMenu";
            this._contextMenu.Size = new System.Drawing.Size(61, 4);
            this._contextMenu.Opened += new System.EventHandler(this._contextMenu_Opened);
            this._contextMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this._contextMenu_Closed);
            this._contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this._contextMenu_Opening);
            this._contextMenu.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this._contextMenu_Closing);
            // 
            // _selectionChangeTimer
            // 
            this._selectionChangeTimer.Interval = 50;
            this._selectionChangeTimer.Tick += new System.EventHandler(this._selectionChangeTimer_Tick);
            // 
            // _dataGridView
            // 
            this._dataGridView.AllowUserToAddRows = false;
            this._dataGridView.AllowUserToDeleteRows = false;
            this._dataGridView.AllowUserToOrderColumns = true;
            this._dataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._dataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this._dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dataGridView.ContextMenuStrip = this._contextMenu;
            this._dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dataGridView.Location = new System.Drawing.Point(0, 25);
            this._dataGridView.Name = "_dataGridView";
            this._dataGridView.RowHeadersVisible = false;
            this._dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._dataGridView.Size = new System.Drawing.Size(540, 203);
            this._dataGridView.TabIndex = 2;
            this._dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._dataGridView_CellDoubleClick);
            this._dataGridView.ItemDrag += new System.EventHandler<System.Windows.Forms.ItemDragEventArgs>(this._dataGridView_ItemDrag);
            this._dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this._dataGridView_CurrentCellDirtyStateChanged);
            this._dataGridView.SelectionChanged += new System.EventHandler(this._dataGridView_SelectionChanged);
            // 
            // _toolStrip
            // 
            this._toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._sortToolStrip});
            this._toolStrip.Location = new System.Drawing.Point(0, 0);
            this._toolStrip.Name = "_toolStrip";
            this._toolStrip.Size = new System.Drawing.Size(540, 25);
            this._toolStrip.TabIndex = 1;
            this._toolStrip.Text = "toolStrip1";
            // 
            // _sortToolStrip
            // 
            this._sortToolStrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._sortToolStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._sortAscendingToolStripMenuItem,
            this._sortDescendingToolStripMenuItem,
            this._sortToolStripSeparator});
            this._sortToolStrip.Image = ((System.Drawing.Image)(resources.GetObject("_sortToolStrip.Image")));
            this._sortToolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._sortToolStrip.Name = "_sortToolStrip";
            this._sortToolStrip.Size = new System.Drawing.Size(55, 22);
            this._sortToolStrip.Text = "Sort By";
            this._sortToolStrip.Visible = false;
            // 
            // _sortAscendingToolStripMenuItem
            // 
            this._sortAscendingToolStripMenuItem.Name = "_sortAscendingToolStripMenuItem";
            this._sortAscendingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this._sortAscendingToolStripMenuItem.Text = "Sort Ascending";
            this._sortAscendingToolStripMenuItem.Click += new System.EventHandler(this.sortAscendingToolStripMenuItem_Click);
            // 
            // _sortDescendingToolStripMenuItem
            // 
            this._sortDescendingToolStripMenuItem.Name = "_sortDescendingToolStripMenuItem";
            this._sortDescendingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this._sortDescendingToolStripMenuItem.Text = "Sort Descending";
            this._sortDescendingToolStripMenuItem.Click += new System.EventHandler(this.sortDescendingToolStripMenuItem_Click);
            // 
            // _sortToolStripSeparator
            // 
            this._sortToolStripSeparator.Name = "_sortToolStripSeparator";
            this._sortToolStripSeparator.Size = new System.Drawing.Size(149, 6);
            // 
            // TableView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._dataGridView);
            this.Controls.Add(this._toolStrip);
            this.Name = "TableView";
            this.Size = new System.Drawing.Size(540, 228);
            this.Load += new System.EventHandler(this.TableView_Load);
            ((System.ComponentModel.ISupportInitialize)(this._bindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._dataGridView)).EndInit();
            this._toolStrip.ResumeLayout(false);
            this._toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.BindingSource _bindingSource;
        private System.Windows.Forms.ContextMenuStrip _contextMenu;
		private System.ComponentModel.IContainer components;
		private ClearCanvas.Desktop.View.WinForms.DataGridViewWithDragSupport _dataGridView;
        private System.Windows.Forms.Timer _selectionChangeTimer;
        private System.Windows.Forms.ToolStrip _toolStrip;
        private System.Windows.Forms.ToolStripDropDownButton _sortToolStrip;
        private System.Windows.Forms.ToolStripMenuItem _sortAscendingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _sortDescendingToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator _sortToolStripSeparator;
	}
}
