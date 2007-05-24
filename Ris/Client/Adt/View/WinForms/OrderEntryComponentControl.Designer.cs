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
            this.components = new System.ComponentModel.Container();
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection2 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection3 = new ClearCanvas.Desktop.Selection();
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
            this._scheduleOrder = new System.Windows.Forms.CheckBox();
            this._diagnosticServiceTree = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
            this.SuspendLayout();
            // 
            // _visitTable
            // 
            this._visitTable.Location = new System.Drawing.Point(20, 22);
            this._visitTable.MenuModel = null;
            this._visitTable.MultiSelect = false;
            this._visitTable.Name = "_visitTable";
            this._visitTable.ReadOnly = false;
            this._visitTable.Selection = selection1;
            this._visitTable.Size = new System.Drawing.Size(620, 127);
            this._visitTable.TabIndex = 0;
            this._visitTable.Table = null;
            this._visitTable.ToolbarModel = null;
            this._visitTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._visitTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select Visit";
            // 
            // _diagnosticService
            // 
            this._diagnosticService.DataSource = null;
            this._diagnosticService.DisplayMember = "";
            this._diagnosticService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._diagnosticService.LabelText = "Diagnostic Service";
            this._diagnosticService.Location = new System.Drawing.Point(20, 179);
            this._diagnosticService.Margin = new System.Windows.Forms.Padding(2);
            this._diagnosticService.Name = "_diagnosticService";
            this._diagnosticService.Size = new System.Drawing.Size(194, 41);
            this._diagnosticService.TabIndex = 2;
            this._diagnosticService.Value = null;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 240);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(153, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Diagnostic Service Breakdown";
            // 
            // _orderingPhysician
            // 
            this._orderingPhysician.DataSource = null;
            this._orderingPhysician.DisplayMember = "";
            this._orderingPhysician.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._orderingPhysician.LabelText = "Ordering Physician";
            this._orderingPhysician.Location = new System.Drawing.Point(250, 240);
            this._orderingPhysician.Margin = new System.Windows.Forms.Padding(2);
            this._orderingPhysician.Name = "_orderingPhysician";
            this._orderingPhysician.Size = new System.Drawing.Size(150, 41);
            this._orderingPhysician.TabIndex = 5;
            this._orderingPhysician.Value = null;
            // 
            // _orderingFacility
            // 
            this._orderingFacility.DataSource = null;
            this._orderingFacility.DisplayMember = "";
            this._orderingFacility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._orderingFacility.LabelText = "Ordering Facility";
            this._orderingFacility.Location = new System.Drawing.Point(250, 308);
            this._orderingFacility.Margin = new System.Windows.Forms.Padding(2);
            this._orderingFacility.Name = "_orderingFacility";
            this._orderingFacility.Size = new System.Drawing.Size(150, 41);
            this._orderingFacility.TabIndex = 6;
            this._orderingFacility.Value = null;
            // 
            // _priority
            // 
            this._priority.DataSource = null;
            this._priority.DisplayMember = "";
            this._priority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._priority.LabelText = "Priority";
            this._priority.Location = new System.Drawing.Point(250, 179);
            this._priority.Margin = new System.Windows.Forms.Padding(2);
            this._priority.Name = "_priority";
            this._priority.Size = new System.Drawing.Size(150, 41);
            this._priority.TabIndex = 7;
            this._priority.Value = null;
            // 
            // _diagnosticServiceBreakdown
            // 
            this._diagnosticServiceBreakdown.AllowDrop = true;
            this._diagnosticServiceBreakdown.Location = new System.Drawing.Point(20, 264);
            this._diagnosticServiceBreakdown.Margin = new System.Windows.Forms.Padding(2);
            this._diagnosticServiceBreakdown.MenuModel = null;
            this._diagnosticServiceBreakdown.Name = "_diagnosticServiceBreakdown";
            this._diagnosticServiceBreakdown.Selection = selection2;
            this._diagnosticServiceBreakdown.ShowToolbar = false;
            this._diagnosticServiceBreakdown.Size = new System.Drawing.Size(194, 160);
            this._diagnosticServiceBreakdown.TabIndex = 8;
            this._diagnosticServiceBreakdown.ToolbarModel = null;
            this._diagnosticServiceBreakdown.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._diagnosticServiceBreakdown.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._diagnosticServiceBreakdown.Tree = null;
            // 
            // _placeOrderButton
            // 
            this._placeOrderButton.Location = new System.Drawing.Point(486, 405);
            this._placeOrderButton.Margin = new System.Windows.Forms.Padding(2);
            this._placeOrderButton.Name = "_placeOrderButton";
            this._placeOrderButton.Size = new System.Drawing.Size(79, 19);
            this._placeOrderButton.TabIndex = 9;
            this._placeOrderButton.Text = "Place Order";
            this._placeOrderButton.UseVisualStyleBackColor = true;
            this._placeOrderButton.Click += new System.EventHandler(this._placeOrderButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(569, 405);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(56, 19);
            this._cancelButton.TabIndex = 10;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _schedulingRequestDateTime
            // 
            this._schedulingRequestDateTime.LabelText = "Requested Schedule Time";
            this._schedulingRequestDateTime.Location = new System.Drawing.Point(250, 383);
            this._schedulingRequestDateTime.Margin = new System.Windows.Forms.Padding(2);
            this._schedulingRequestDateTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._schedulingRequestDateTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._schedulingRequestDateTime.Name = "_schedulingRequestDateTime";
            this._schedulingRequestDateTime.Nullable = false;
            this._schedulingRequestDateTime.ShowTime = true;
            this._schedulingRequestDateTime.Size = new System.Drawing.Size(150, 41);
            this._schedulingRequestDateTime.TabIndex = 11;
            this._schedulingRequestDateTime.Value = null;
            // 
            // _scheduleOrder
            // 
            this._scheduleOrder.AutoSize = true;
            this._scheduleOrder.Location = new System.Drawing.Point(250, 361);
            this._scheduleOrder.Name = "_scheduleOrder";
            this._scheduleOrder.Size = new System.Drawing.Size(100, 17);
            this._scheduleOrder.TabIndex = 12;
            this._scheduleOrder.Text = "Schedule Order";
            this._scheduleOrder.UseVisualStyleBackColor = true;
            // 
            // _diagnosticServiceTree
            // 
            this._diagnosticServiceTree.AllowDrop = true;
            this._diagnosticServiceTree.Location = new System.Drawing.Point(667, 22);
            this._diagnosticServiceTree.Margin = new System.Windows.Forms.Padding(2);
            this._diagnosticServiceTree.MenuModel = null;
            this._diagnosticServiceTree.Name = "_diagnosticServiceTree";
            this._diagnosticServiceTree.Selection = selection3;
            this._diagnosticServiceTree.ShowToolbar = false;
            this._diagnosticServiceTree.Size = new System.Drawing.Size(475, 402);
            this._diagnosticServiceTree.TabIndex = 13;
            this._diagnosticServiceTree.ToolbarModel = null;
            this._diagnosticServiceTree.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._diagnosticServiceTree.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._diagnosticServiceTree.Tree = null;
            // 
            // OrderEntryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._diagnosticServiceTree);
            this.Controls.Add(this._scheduleOrder);
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
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "OrderEntryComponentControl";
            this.Size = new System.Drawing.Size(1175, 450);
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
        private System.Windows.Forms.CheckBox _scheduleOrder;
        private ClearCanvas.Desktop.View.WinForms.BindingTreeView _diagnosticServiceTree;
    }
}
