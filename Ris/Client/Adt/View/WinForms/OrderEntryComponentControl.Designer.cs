namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class OrderEntryComponentControl
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
            this._visitTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.label1 = new System.Windows.Forms.Label();
            this._diagnosticService = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this.label2 = new System.Windows.Forms.Label();
            this._orderingPhysician = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._orderingFacility = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._priority = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._diagnosticServiceBreakdown = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
            this._placeOrderButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._schedulingRequestDateTime = new ClearCanvas.Controls.WinForms.DateTimeField();
            this.SuspendLayout();
            // 
            // _visitTable
            // 
            this._visitTable.Location = new System.Drawing.Point(26, 27);
            this._visitTable.Margin = new System.Windows.Forms.Padding(4);
            this._visitTable.MenuModel = null;
            this._visitTable.MultiSelect = false;
            this._visitTable.Name = "_visitTable";
            this._visitTable.ReadOnly = false;
            this._visitTable.Selection = selection1;
            this._visitTable.Size = new System.Drawing.Size(826, 156);
            this._visitTable.TabIndex = 0;
            this._visitTable.Table = null;
            this._visitTable.ToolbarModel = null;
            this._visitTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._visitTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select Visit";
            // 
            // _diagnosticService
            // 
            this._diagnosticService.DataSource = null;
            this._diagnosticService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._diagnosticService.LabelText = "Diagnostic Service";
            this._diagnosticService.Location = new System.Drawing.Point(26, 220);
            this._diagnosticService.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._diagnosticService.Name = "_diagnosticService";
            this._diagnosticService.Size = new System.Drawing.Size(259, 50);
            this._diagnosticService.TabIndex = 2;
            this._diagnosticService.Value = null;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 296);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(199, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Diagnostic Service Breakdown";
            // 
            // _orderingPhysician
            // 
            this._orderingPhysician.DataSource = null;
            this._orderingPhysician.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._orderingPhysician.LabelText = "Ordering Physician";
            this._orderingPhysician.Location = new System.Drawing.Point(334, 296);
            this._orderingPhysician.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._orderingPhysician.Name = "_orderingPhysician";
            this._orderingPhysician.Size = new System.Drawing.Size(200, 50);
            this._orderingPhysician.TabIndex = 5;
            this._orderingPhysician.Value = null;
            // 
            // _orderingFacility
            // 
            this._orderingFacility.DataSource = null;
            this._orderingFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._orderingFacility.LabelText = "Ordering Facility";
            this._orderingFacility.Location = new System.Drawing.Point(334, 379);
            this._orderingFacility.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._orderingFacility.Name = "_orderingFacility";
            this._orderingFacility.Size = new System.Drawing.Size(200, 50);
            this._orderingFacility.TabIndex = 6;
            this._orderingFacility.Value = null;
            // 
            // _priority
            // 
            this._priority.DataSource = null;
            this._priority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._priority.LabelText = "Priority";
            this._priority.Location = new System.Drawing.Point(334, 220);
            this._priority.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._priority.Name = "_priority";
            this._priority.Size = new System.Drawing.Size(200, 50);
            this._priority.TabIndex = 7;
            this._priority.Value = null;
            // 
            // _diagnosticServiceBreakdown
            // 
            this._diagnosticServiceBreakdown.Location = new System.Drawing.Point(26, 325);
            this._diagnosticServiceBreakdown.Name = "_diagnosticServiceBreakdown";
            this._diagnosticServiceBreakdown.Selection = selection2;
            this._diagnosticServiceBreakdown.ShowToolbar = false;
            this._diagnosticServiceBreakdown.Size = new System.Drawing.Size(259, 197);
            this._diagnosticServiceBreakdown.TabIndex = 8;
            this._diagnosticServiceBreakdown.Tree = null;
            // 
            // _placeOrderButton
            // 
            this._placeOrderButton.Location = new System.Drawing.Point(648, 498);
            this._placeOrderButton.Name = "_placeOrderButton";
            this._placeOrderButton.Size = new System.Drawing.Size(105, 23);
            this._placeOrderButton.TabIndex = 9;
            this._placeOrderButton.Text = "Place Order";
            this._placeOrderButton.UseVisualStyleBackColor = true;
            this._placeOrderButton.Click += new System.EventHandler(this._placeOrderButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(759, 498);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 10;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _schedulingRequestDateTime
            // 
            this._schedulingRequestDateTime.LabelText = "Schedule For";
            this._schedulingRequestDateTime.Location = new System.Drawing.Point(334, 471);
            this._schedulingRequestDateTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._schedulingRequestDateTime.Name = "_schedulingRequestDateTime";
            this._schedulingRequestDateTime.Nullable = false;
            this._schedulingRequestDateTime.ShowTime = true;
            this._schedulingRequestDateTime.Size = new System.Drawing.Size(200, 50);
            this._schedulingRequestDateTime.TabIndex = 11;
            this._schedulingRequestDateTime.Value = null;
            // 
            // OrderEntryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._schedulingRequestDateTime);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._placeOrderButton);
            this.Controls.Add(this._diagnosticServiceBreakdown);
            this.Controls.Add(this._priority);
            this.Controls.Add(this._orderingFacility);
            this.Controls.Add(this._orderingPhysician);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._diagnosticService);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._visitTable);
            this.Name = "OrderEntryComponentControl";
            this.Size = new System.Drawing.Size(887, 554);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _visitTable;
        private System.Windows.Forms.Label label1;
        private ClearCanvas.Controls.WinForms.ComboBoxField _diagnosticService;
        private System.Windows.Forms.Label label2;
        private ClearCanvas.Controls.WinForms.ComboBoxField _orderingPhysician;
        private ClearCanvas.Controls.WinForms.ComboBoxField _orderingFacility;
        private ClearCanvas.Controls.WinForms.ComboBoxField _priority;
        private ClearCanvas.Desktop.View.WinForms.BindingTreeView _diagnosticServiceBreakdown;
        private System.Windows.Forms.Button _placeOrderButton;
        private System.Windows.Forms.Button _cancelButton;
        private ClearCanvas.Controls.WinForms.DateTimeField _schedulingRequestDateTime;
    }
}
