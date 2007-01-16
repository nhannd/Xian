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
            this._settingsGroupTreeView = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
            this.SuspendLayout();
            // 
            // _valueTableView
            // 
            this._valueTableView.Location = new System.Drawing.Point(263, 121);
            this._valueTableView.Margin = new System.Windows.Forms.Padding(4);
            this._valueTableView.MenuModel = null;
            this._valueTableView.Name = "_valueTableView";
            this._valueTableView.ReadOnly = false;
            this._valueTableView.Selection = selection1;
            this._valueTableView.Size = new System.Drawing.Size(587, 306);
            this._valueTableView.TabIndex = 1;
            this._valueTableView.Table = null;
            this._valueTableView.ToolbarModel = null;
            this._valueTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._valueTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _settingsGroupTreeView
            // 
            this._settingsGroupTreeView.AllowDrop = true;
            this._settingsGroupTreeView.Location = new System.Drawing.Point(3, 3);
            this._settingsGroupTreeView.Name = "_settingsGroupTreeView";
            this._settingsGroupTreeView.Selection = selection2;
            this._settingsGroupTreeView.Size = new System.Drawing.Size(225, 609);
            this._settingsGroupTreeView.TabIndex = 2;
            this._settingsGroupTreeView.Tree = null;
            // 
            // SettingsManagementComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._settingsGroupTreeView);
            this.Controls.Add(this._valueTableView);
            this.Name = "SettingsManagementComponentControl";
            this.Size = new System.Drawing.Size(879, 615);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _valueTableView;
        private ClearCanvas.Desktop.View.WinForms.BindingTreeView _settingsGroupTreeView;
    }
}
