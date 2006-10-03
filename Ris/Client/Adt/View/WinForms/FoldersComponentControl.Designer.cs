namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class FoldersComponentControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FoldersComponentControl));
            this._folderTree = new System.Windows.Forms.TreeView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._toolbar = new System.Windows.Forms.ToolStrip();
            this._folderTreeImageList = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _folderTree
            // 
            this._folderTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._folderTree.HideSelection = false;
            this._folderTree.ImageIndex = 0;
            this._folderTree.ImageList = this._folderTreeImageList;
            this._folderTree.Location = new System.Drawing.Point(3, 28);
            this._folderTree.Name = "_folderTree";
            this._folderTree.SelectedImageIndex = 1;
            this._folderTree.ShowRootLines = false;
            this._folderTree.Size = new System.Drawing.Size(228, 472);
            this._folderTree.TabIndex = 0;
            this._folderTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._folderTree_AfterSelect);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this._folderTree, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._toolbar, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(234, 503);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // _toolbar
            // 
            this._toolbar.Location = new System.Drawing.Point(0, 0);
            this._toolbar.Name = "_toolbar";
            this._toolbar.Size = new System.Drawing.Size(234, 25);
            this._toolbar.TabIndex = 1;
            this._toolbar.Text = "toolStrip1";
            // 
            // _folderTreeImageList
            // 
            this._folderTreeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_folderTreeImageList.ImageStream")));
            this._folderTreeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this._folderTreeImageList.Images.SetKeyName(0, "FolderClosedMedium.png");
            this._folderTreeImageList.Images.SetKeyName(1, "FolderOpenMedium.png");
            // 
            // FoldersComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FoldersComponentControl";
            this.Size = new System.Drawing.Size(234, 503);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView _folderTree;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip _toolbar;
        private System.Windows.Forms.ImageList _folderTreeImageList;
    }
}
