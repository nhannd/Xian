namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class LocationEditorComponentControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this._cancelButton = new System.Windows.Forms.Button();
            this._acceptButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._facility = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._building = new ClearCanvas.Controls.WinForms.TextField();
            this._floor = new ClearCanvas.Controls.WinForms.TextField();
            this._pointOfCare = new ClearCanvas.Controls.WinForms.TextField();
            this._room = new ClearCanvas.Controls.WinForms.TextField();
            this._bed = new ClearCanvas.Controls.WinForms.TextField();
            this._inactiveDate = new ClearCanvas.Controls.WinForms.DateTimeField();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this._active = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(635, 134);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel3.Controls.Add(this._cancelButton);
            this.flowLayoutPanel3.Controls.Add(this._acceptButton);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(71, 99);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel3.Size = new System.Drawing.Size(561, 32);
            this.flowLayoutPanel3.TabIndex = 2;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(483, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(402, 3);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 0;
            this._acceptButton.Text = "Accept";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this._facility);
            this.flowLayoutPanel1.Controls.Add(this._building);
            this.flowLayoutPanel1.Controls.Add(this._floor);
            this.flowLayoutPanel1.Controls.Add(this._pointOfCare);
            this.flowLayoutPanel1.Controls.Add(this._room);
            this.flowLayoutPanel1.Controls.Add(this._bed);
            this.flowLayoutPanel1.Controls.Add(this._inactiveDate);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(629, 90);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // _facility
            // 
            this._facility.DataSource = null;
            this._facility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._facility.LabelText = "Facility";
            this._facility.Location = new System.Drawing.Point(2, 2);
            this._facility.Margin = new System.Windows.Forms.Padding(2);
            this._facility.Name = "_facility";
            this._facility.Size = new System.Drawing.Size(150, 41);
            this._facility.TabIndex = 0;
            this._facility.Value = null;
            // 
            // _building
            // 
            this._building.LabelText = "Building";
            this._building.Location = new System.Drawing.Point(156, 2);
            this._building.Margin = new System.Windows.Forms.Padding(2);
            this._building.Mask = "";
            this._building.Name = "_building";
            this._building.Size = new System.Drawing.Size(150, 41);
            this._building.TabIndex = 1;
            this._building.Value = null;
            // 
            // _floor
            // 
            this._floor.LabelText = "Floor";
            this._floor.Location = new System.Drawing.Point(310, 2);
            this._floor.Margin = new System.Windows.Forms.Padding(2);
            this._floor.Mask = "";
            this._floor.Name = "_floor";
            this._floor.Size = new System.Drawing.Size(150, 41);
            this._floor.TabIndex = 2;
            this._floor.Value = null;
            // 
            // _pointOfCare
            // 
            this._pointOfCare.LabelText = "Point Of Care";
            this._pointOfCare.Location = new System.Drawing.Point(464, 2);
            this._pointOfCare.Margin = new System.Windows.Forms.Padding(2);
            this._pointOfCare.Mask = "";
            this._pointOfCare.Name = "_pointOfCare";
            this._pointOfCare.Size = new System.Drawing.Size(150, 41);
            this._pointOfCare.TabIndex = 3;
            this._pointOfCare.Value = null;
            // 
            // _room
            // 
            this._room.LabelText = "Room";
            this._room.Location = new System.Drawing.Point(2, 47);
            this._room.Margin = new System.Windows.Forms.Padding(2);
            this._room.Mask = "";
            this._room.Name = "_room";
            this._room.Size = new System.Drawing.Size(150, 41);
            this._room.TabIndex = 4;
            this._room.Value = null;
            // 
            // _bed
            // 
            this._bed.LabelText = "Bed";
            this._bed.Location = new System.Drawing.Point(156, 47);
            this._bed.Margin = new System.Windows.Forms.Padding(2);
            this._bed.Mask = "";
            this._bed.Name = "_bed";
            this._bed.Size = new System.Drawing.Size(150, 41);
            this._bed.TabIndex = 5;
            this._bed.Value = null;
            // 
            // _inactiveDate
            // 
            this._inactiveDate.LabelText = "Inactive Date";
            this._inactiveDate.Location = new System.Drawing.Point(310, 47);
            this._inactiveDate.Margin = new System.Windows.Forms.Padding(2);
            this._inactiveDate.Name = "_inactiveDate";
            this._inactiveDate.Nullable = false;
            this._inactiveDate.ShowTime = false;
            this._inactiveDate.Size = new System.Drawing.Size(150, 41);
            this._inactiveDate.TabIndex = 6;
            this._inactiveDate.Value = null;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this._active);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 99);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(62, 32);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // _active
            // 
            this._active.AutoSize = true;
            this._active.Location = new System.Drawing.Point(3, 3);
            this._active.Name = "_active";
            this._active.Size = new System.Drawing.Size(56, 17);
            this._active.TabIndex = 0;
            this._active.Text = "Active";
            this._active.UseVisualStyleBackColor = true;
            // 
            // LocationEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LocationEditorComponentControl";
            this.Size = new System.Drawing.Size(635, 134);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ClearCanvas.Controls.WinForms.ComboBoxField _facility;
        private ClearCanvas.Controls.WinForms.TextField _building;
        private ClearCanvas.Controls.WinForms.TextField _floor;
        private ClearCanvas.Controls.WinForms.TextField _pointOfCare;
        private ClearCanvas.Controls.WinForms.TextField _room;
        private ClearCanvas.Controls.WinForms.TextField _bed;
        private ClearCanvas.Controls.WinForms.DateTimeField _inactiveDate;
        private System.Windows.Forms.CheckBox _active;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
    }
}
