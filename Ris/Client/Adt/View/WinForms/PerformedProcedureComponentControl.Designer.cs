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
            ClearCanvas.Desktop.Selection selection2 = new ClearCanvas.Desktop.Selection();
            this.tableLayoutPanelDocumentationDetails = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this._buttonCompleteDocumentationDetails = new System.Windows.Forms.Button();
            this.splitContainerDocumentationDetails = new System.Windows.Forms.SplitContainer();
            this._mppsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._mppsDetailsPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanelDocumentationDetails.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.splitContainerDocumentationDetails.Panel1.SuspendLayout();
            this.splitContainerDocumentationDetails.Panel2.SuspendLayout();
            this.splitContainerDocumentationDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelDocumentationDetails
            // 
            this.tableLayoutPanelDocumentationDetails.AutoSize = true;
            this.tableLayoutPanelDocumentationDetails.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelDocumentationDetails.ColumnCount = 1;
            this.tableLayoutPanelDocumentationDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelDocumentationDetails.Controls.Add(this.flowLayoutPanel2, 0, 1);
            this.tableLayoutPanelDocumentationDetails.Controls.Add(this.splitContainerDocumentationDetails, 0, 0);
            this.tableLayoutPanelDocumentationDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelDocumentationDetails.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelDocumentationDetails.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanelDocumentationDetails.Name = "tableLayoutPanelDocumentationDetails";
            this.tableLayoutPanelDocumentationDetails.RowCount = 2;
            this.tableLayoutPanelDocumentationDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelDocumentationDetails.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelDocumentationDetails.Size = new System.Drawing.Size(1014, 627);
            this.tableLayoutPanelDocumentationDetails.TabIndex = 1;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this._buttonCompleteDocumentationDetails);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(4, 587);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel2.Size = new System.Drawing.Size(1006, 36);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // _buttonCompleteDocumentationDetails
            // 
            this._buttonCompleteDocumentationDetails.Location = new System.Drawing.Point(902, 4);
            this._buttonCompleteDocumentationDetails.Margin = new System.Windows.Forms.Padding(4);
            this._buttonCompleteDocumentationDetails.Name = "_buttonCompleteDocumentationDetails";
            this._buttonCompleteDocumentationDetails.Size = new System.Drawing.Size(100, 28);
            this._buttonCompleteDocumentationDetails.TabIndex = 0;
            this._buttonCompleteDocumentationDetails.Text = "Complete";
            this._buttonCompleteDocumentationDetails.UseVisualStyleBackColor = true;
            this._buttonCompleteDocumentationDetails.Click += new System.EventHandler(this._buttonCompleteDocumentationDetails_Click);
            // 
            // splitContainerDocumentationDetails
            // 
            this.splitContainerDocumentationDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerDocumentationDetails.Location = new System.Drawing.Point(4, 4);
            this.splitContainerDocumentationDetails.Margin = new System.Windows.Forms.Padding(4);
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
            this.splitContainerDocumentationDetails.Size = new System.Drawing.Size(1006, 575);
            this.splitContainerDocumentationDetails.SplitterDistance = 195;
            this.splitContainerDocumentationDetails.SplitterWidth = 5;
            this.splitContainerDocumentationDetails.TabIndex = 1;
            // 
            // _mppsTableView
            // 
            this._mppsTableView.AutoSize = true;
            this._mppsTableView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._mppsTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mppsTableView.Location = new System.Drawing.Point(0, 0);
            this._mppsTableView.Margin = new System.Windows.Forms.Padding(5);
            this._mppsTableView.MenuModel = null;
            this._mppsTableView.Name = "_mppsTableView";
            this._mppsTableView.ReadOnly = false;
            this._mppsTableView.Selection = selection2;
            this._mppsTableView.Size = new System.Drawing.Size(1006, 195);
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
            this._mppsDetailsPanel.Margin = new System.Windows.Forms.Padding(4);
            this._mppsDetailsPanel.Name = "_mppsDetailsPanel";
            this._mppsDetailsPanel.Size = new System.Drawing.Size(1006, 375);
            this._mppsDetailsPanel.TabIndex = 0;
            // 
            // PerformedProcedureComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelDocumentationDetails);
            this.Name = "PerformedProcedureComponentControl";
            this.Size = new System.Drawing.Size(1014, 627);
            this.tableLayoutPanelDocumentationDetails.ResumeLayout(false);
            this.tableLayoutPanelDocumentationDetails.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.splitContainerDocumentationDetails.Panel1.ResumeLayout(false);
            this.splitContainerDocumentationDetails.Panel1.PerformLayout();
            this.splitContainerDocumentationDetails.Panel2.ResumeLayout(false);
            this.splitContainerDocumentationDetails.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDocumentationDetails;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button _buttonCompleteDocumentationDetails;
        private System.Windows.Forms.SplitContainer splitContainerDocumentationDetails;
        private ClearCanvas.Desktop.View.WinForms.TableView _mppsTableView;
        private System.Windows.Forms.Panel _mppsDetailsPanel;

    }
}
