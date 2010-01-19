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
    partial class BindingTreeView
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
			this.components = new System.ComponentModel.Container();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this._searchTextBox = new System.Windows.Forms.ToolStripTextBox();
			this._applySearchButton = new System.Windows.Forms.ToolStripButton();
			this._clearSearchButton = new System.Windows.Forms.ToolStripButton();
			this._treeCtrl = new System.Windows.Forms.TreeView();
			this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._imageList = new System.Windows.Forms.ImageList(this.components);
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.tableLayoutPanel1.SuspendLayout();
			this._toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._toolStrip, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._treeCtrl, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(364, 302);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _toolStrip
			// 
			this._toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._searchTextBox,
            this._applySearchButton,
            this._clearSearchButton});
			this._toolStrip.Location = new System.Drawing.Point(0, 0);
			this._toolStrip.Name = "_toolStrip";
			this._toolStrip.Size = new System.Drawing.Size(364, 25);
			this._toolStrip.TabIndex = 0;
			this._toolStrip.Text = "toolStrip1";
			// 
			// _searchTextBox
			// 
			this._searchTextBox.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
			this._searchTextBox.Name = "_searchTextBox";
			this._searchTextBox.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this._searchTextBox.Size = new System.Drawing.Size(100, 23);
			this._searchTextBox.ToolTipText = "Enter text here to search tree";
			this._searchTextBox.Visible = false;
			this._searchTextBox.TextChanged += new System.EventHandler(this._searchTextBox_TextChanged);
			// 
			// _applySearchButton
			// 
			this._applySearchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._applySearchButton.Enabled = false;
			this._applySearchButton.Image = global::ClearCanvas.Desktop.View.WinForms.SR.SearchToolMini;
			this._applySearchButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._applySearchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._applySearchButton.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this._applySearchButton.Name = "_applySearchButton";
			this._applySearchButton.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this._applySearchButton.Size = new System.Drawing.Size(23, 25);
			this._applySearchButton.Text = "Apply Search";
			this._applySearchButton.Visible = false;
			this._applySearchButton.Click += new System.EventHandler(this._applySearchButton_Click);
			// 
			// _clearSearchButton
			// 
			this._clearSearchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._clearSearchButton.Enabled = false;
			this._clearSearchButton.Image = global::ClearCanvas.Desktop.View.WinForms.SR.ClearFilterMini;
			this._clearSearchButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._clearSearchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._clearSearchButton.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this._clearSearchButton.Name = "_clearSearchButton";
			this._clearSearchButton.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			this._clearSearchButton.Size = new System.Drawing.Size(23, 25);
			this._clearSearchButton.Text = "Clear Search";
			this._clearSearchButton.Visible = false;
			this._clearSearchButton.Click += new System.EventHandler(this._clearSearchButton_Click);
			// 
			// _treeCtrl
			// 
			this._treeCtrl.ContextMenuStrip = this._contextMenu;
			this._treeCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._treeCtrl.HideSelection = false;
			this._treeCtrl.ImageIndex = 0;
			this._treeCtrl.ImageList = this._imageList;
			this._treeCtrl.Location = new System.Drawing.Point(2, 27);
			this._treeCtrl.Margin = new System.Windows.Forms.Padding(2);
			this._treeCtrl.Name = "_treeCtrl";
			this._treeCtrl.SelectedImageIndex = 0;
			this._treeCtrl.ShowNodeToolTips = true;
			this._treeCtrl.Size = new System.Drawing.Size(360, 273);
			this._treeCtrl.TabIndex = 1;
			this._treeCtrl.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this._treeCtrl_NodeMouseDoubleClick);
			this._treeCtrl.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this._treeCtrl_AfterCheck);
			this._treeCtrl.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this._treeCtrl_AfterCollapse);
			this._treeCtrl.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this._treeCtrl_AfterLabelEdit);
			this._treeCtrl.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this._treeCtrl_BeforeExpand);
			this._treeCtrl.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._treeCtrl_AfterSelect);
			this._treeCtrl.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this._treeCtrl_NodeMouseClick);
			this._treeCtrl.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this._treeCtrl_BeforeSelect);
			this._treeCtrl.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this._treeCtrl_AfterExpand);
			this._treeCtrl.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this._treeCtrl_ItemDrag);
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
			// _imageList
			// 
			this._imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this._imageList.ImageSize = new System.Drawing.Size(16, 16);
			this._imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// _toolTip
			// 
			this._toolTip.ShowAlways = true;
			// 
			// BindingTreeView
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "BindingTreeView";
			this.Size = new System.Drawing.Size(364, 302);
			this.Load += new System.EventHandler(this.BindingTreeView_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this._toolStrip.ResumeLayout(false);
			this._toolStrip.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip _toolStrip;
        private System.Windows.Forms.TreeView _treeCtrl;
        private System.Windows.Forms.ContextMenuStrip _contextMenu;
        private System.Windows.Forms.ImageList _imageList;
		private System.Windows.Forms.ToolTip _toolTip;
		private System.Windows.Forms.ToolStripButton _clearSearchButton;
		private System.Windows.Forms.ToolStripTextBox _searchTextBox;
		private System.Windows.Forms.ToolStripButton _applySearchButton;
    }
}
