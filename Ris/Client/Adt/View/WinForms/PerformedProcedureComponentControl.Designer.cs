namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class PerformedProcedureComponentControl
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
            this.splitContainerDocumentationDetails = new System.Windows.Forms.SplitContainer();
            this._mppsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._mppsDetailsPanel = new System.Windows.Forms.Panel();
            this.splitContainerDocumentationDetails.Panel1.SuspendLayout();
            this.splitContainerDocumentationDetails.Panel2.SuspendLayout();
            this.splitContainerDocumentationDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerDocumentationDetails
            // 
            this.splitContainerDocumentationDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerDocumentationDetails.Location = new System.Drawing.Point(0, 0);
            this.splitContainerDocumentationDetails.Name = "splitContainerDocumentationDetails";
            this.splitContainerDocumentationDetails.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerDocumentationDetails.Panel1
            // 
            this.splitContainerDocumentationDetails.Panel1.Controls.Add(this._mppsTableView);
            // 
            // splitContainerDocumentationDetails.Panel2
            // 
            this.splitContainerDocumentationDetails.Panel2.Controls.Add(this._mppsDetailsPanel);
            this.splitContainerDocumentationDetails.Size = new System.Drawing.Size(760, 509);
            this.splitContainerDocumentationDetails.SplitterDistance = 172;
            this.splitContainerDocumentationDetails.TabIndex = 1;
            // 
            // _mppsTableView
            // 
            this._mppsTableView.AutoSize = true;
            this._mppsTableView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._mppsTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mppsTableView.Location = new System.Drawing.Point(0, 0);
            this._mppsTableView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._mppsTableView.MenuModel = null;
            this._mppsTableView.Name = "_mppsTableView";
            this._mppsTableView.ReadOnly = false;
            this._mppsTableView.Selection = selection1;
            this._mppsTableView.Size = new System.Drawing.Size(760, 172);
            this._mppsTableView.TabIndex = 0;
            this._mppsTableView.Table = null;
            this._mppsTableView.ToolbarModel = null;
            this._mppsTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._mppsTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _mppsDetailsPanel
            // 
            this._mppsDetailsPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._mppsDetailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mppsDetailsPanel.Location = new System.Drawing.Point(0, 0);
            this._mppsDetailsPanel.Name = "_mppsDetailsPanel";
            this._mppsDetailsPanel.Size = new System.Drawing.Size(760, 333);
            this._mppsDetailsPanel.TabIndex = 0;
            // 
            // PerformedProcedureComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerDocumentationDetails);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "PerformedProcedureComponentControl";
            this.Size = new System.Drawing.Size(760, 509);
            this.splitContainerDocumentationDetails.Panel1.ResumeLayout(false);
            this.splitContainerDocumentationDetails.Panel1.PerformLayout();
            this.splitContainerDocumentationDetails.Panel2.ResumeLayout(false);
            this.splitContainerDocumentationDetails.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerDocumentationDetails;
        private ClearCanvas.Desktop.View.WinForms.TableView _mppsTableView;
        private System.Windows.Forms.Panel _mppsDetailsPanel;

    }
}
