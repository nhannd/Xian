namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class ProtocolGroupSummaryComponentControl
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
            this._protocolGroupsTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _protocolGroupsTable
            // 
            this._protocolGroupsTable.AutoSize = true;
            this._protocolGroupsTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._protocolGroupsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._protocolGroupsTable.Location = new System.Drawing.Point(0, 0);
            this._protocolGroupsTable.Name = "_protocolGroupsTable";
            this._protocolGroupsTable.ReadOnly = false;
            this._protocolGroupsTable.Size = new System.Drawing.Size(150, 150);
            this._protocolGroupsTable.TabIndex = 0;
            this._protocolGroupsTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._protocolGroupsTable.ItemDoubleClicked += new System.EventHandler(this._protocolGroupsTable_ItemDoubleClicked);
            // 
            // ProtocolGroupSummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._protocolGroupsTable);
            this.Name = "ProtocolGroupSummaryComponentControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _protocolGroupsTable;
    }
}
