namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class SettingsManagementComponentControl
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
            this._valueTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._settingsGroupTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _valueTableView
            // 
            this._valueTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._valueTableView.Location = new System.Drawing.Point(0, 0);
            this._valueTableView.Margin = new System.Windows.Forms.Padding(4);
            this._valueTableView.MenuModel = null;
            this._valueTableView.Name = "_valueTableView";
            this._valueTableView.ReadOnly = false;
            this._valueTableView.Selection = selection1;
            this._valueTableView.Size = new System.Drawing.Size(879, 304);
            this._valueTableView.TabIndex = 1;
            this._valueTableView.Table = null;
            this._valueTableView.ToolbarModel = null;
            this._valueTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this._valueTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // _settingsGroupTableView
            // 
            this._settingsGroupTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._settingsGroupTableView.Location = new System.Drawing.Point(0, 0);
            this._settingsGroupTableView.Margin = new System.Windows.Forms.Padding(4);
            this._settingsGroupTableView.MenuModel = null;
            this._settingsGroupTableView.Name = "_settingsGroupTableView";
            this._settingsGroupTableView.ReadOnly = false;
            this._settingsGroupTableView.Selection = selection2;
            this._settingsGroupTableView.ShowToolbar = false;
            this._settingsGroupTableView.Size = new System.Drawing.Size(879, 307);
            this._settingsGroupTableView.TabIndex = 2;
            this._settingsGroupTableView.Table = null;
            this._settingsGroupTableView.ToolbarModel = null;
            this._settingsGroupTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._settingsGroupTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._settingsGroupTableView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._valueTableView);
            this.splitContainer1.Size = new System.Drawing.Size(879, 615);
            this.splitContainer1.SplitterDistance = 307;
            this.splitContainer1.TabIndex = 3;
            // 
            // SettingsManagementComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "SettingsManagementComponentControl";
            this.Size = new System.Drawing.Size(879, 615);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _valueTableView;
        private ClearCanvas.Desktop.View.WinForms.TableView _settingsGroupTableView;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
