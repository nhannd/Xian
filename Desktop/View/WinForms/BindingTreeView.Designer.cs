namespace ClearCanvas.Desktop.View.WinForms
{
    partial class BindableTreeView
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
            this._treeCtrl = new System.Windows.Forms.TreeView();
            this._iconImageList = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1.SuspendLayout();
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
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(486, 372);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _toolStrip
            // 
            this._toolStrip.Location = new System.Drawing.Point(0, 0);
            this._toolStrip.Name = "_toolStrip";
            this._toolStrip.Size = new System.Drawing.Size(486, 25);
            this._toolStrip.TabIndex = 0;
            this._toolStrip.Text = "toolStrip1";
            // 
            // _treeCtrl
            // 
            this._treeCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._treeCtrl.HideSelection = false;
            this._treeCtrl.ImageIndex = 0;
            this._treeCtrl.ImageList = this._iconImageList;
            this._treeCtrl.Location = new System.Drawing.Point(3, 28);
            this._treeCtrl.Name = "_treeCtrl";
            this._treeCtrl.SelectedImageIndex = 0;
            this._treeCtrl.ShowNodeToolTips = true;
            this._treeCtrl.Size = new System.Drawing.Size(480, 341);
            this._treeCtrl.TabIndex = 1;
            this._treeCtrl.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this._treeCtrl_BeforeExpand);
            this._treeCtrl.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._treeCtrl_AfterSelect);
            // 
            // _iconImageList
            // 
            this._iconImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this._iconImageList.ImageSize = new System.Drawing.Size(16, 16);
            this._iconImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // BindableTreeView
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "BindableTreeView";
            this.Size = new System.Drawing.Size(486, 372);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip _toolStrip;
        private System.Windows.Forms.TreeView _treeCtrl;
        private System.Windows.Forms.ImageList _iconImageList;
    }
}
