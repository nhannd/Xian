namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class ContactPersonEditorComponentControl
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._cancelButton = new System.Windows.Forms.Button();
            this._acceptButton = new System.Windows.Forms.Button();
            this._businessPhone = new ClearCanvas.Controls.WinForms.TextField();
            this._homePhone = new ClearCanvas.Controls.WinForms.TextField();
            this._address = new ClearCanvas.Controls.WinForms.TextField();
            this._name = new ClearCanvas.Controls.WinForms.TextField();
            this._type = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._relationship = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this._businessPhone, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this._homePhone, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._address, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._name, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._type, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._relationship, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(460, 305);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this._cancelButton);
            this.flowLayoutPanel1.Controls.Add(this._acceptButton);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 273);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(454, 29);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(376, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 0;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(295, 3);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 1;
            this._acceptButton.Text = "Accept";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // _businessPhone
            // 
            this._businessPhone.AutoSize = true;
            this._businessPhone.Dock = System.Windows.Forms.DockStyle.Fill;
            this._businessPhone.LabelText = "Business Phone";
            this._businessPhone.Location = new System.Drawing.Point(240, 137);
            this._businessPhone.Margin = new System.Windows.Forms.Padding(10, 2, 18, 2);
            this._businessPhone.Mask = "";
            this._businessPhone.Name = "_businessPhone";
            this._businessPhone.Size = new System.Drawing.Size(202, 40);
            this._businessPhone.TabIndex = 4;
            this._businessPhone.Value = null;
            // 
            // _homePhone
            // 
            this._homePhone.AutoSize = true;
            this._homePhone.Dock = System.Windows.Forms.DockStyle.Fill;
            this._homePhone.LabelText = "Home Phone";
            this._homePhone.Location = new System.Drawing.Point(18, 137);
            this._homePhone.Margin = new System.Windows.Forms.Padding(18, 2, 10, 2);
            this._homePhone.Mask = "";
            this._homePhone.Name = "_homePhone";
            this._homePhone.Size = new System.Drawing.Size(202, 40);
            this._homePhone.TabIndex = 3;
            this._homePhone.Value = null;
            // 
            // _address
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._address, 2);
            this._address.Dock = System.Windows.Forms.DockStyle.Fill;
            this._address.LabelText = "Address";
            this._address.Location = new System.Drawing.Point(18, 92);
            this._address.Margin = new System.Windows.Forms.Padding(18, 2, 18, 2);
            this._address.Mask = "";
            this._address.Name = "_address";
            this._address.Size = new System.Drawing.Size(424, 41);
            this._address.TabIndex = 2;
            this._address.Value = null;
            // 
            // _name
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._name, 2);
            this._name.Dock = System.Windows.Forms.DockStyle.Fill;
            this._name.LabelText = "Name";
            this._name.Location = new System.Drawing.Point(18, 47);
            this._name.Margin = new System.Windows.Forms.Padding(18, 2, 18, 2);
            this._name.Mask = "";
            this._name.Name = "_name";
            this._name.Size = new System.Drawing.Size(424, 41);
            this._name.TabIndex = 1;
            this._name.Value = null;
            // 
            // _type
            // 
            this._type.DataSource = null;
            this._type.DisplayMember = "";
            this._type.Dock = System.Windows.Forms.DockStyle.Fill;
            this._type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._type.LabelText = "Type";
            this._type.Location = new System.Drawing.Point(18, 2);
            this._type.Margin = new System.Windows.Forms.Padding(18, 2, 10, 2);
            this._type.Name = "_type";
            this._type.Size = new System.Drawing.Size(202, 41);
            this._type.TabIndex = 5;
            this._type.Value = null;
            // 
            // _relationship
            // 
            this._relationship.DataSource = null;
            this._relationship.DisplayMember = "";
            this._relationship.Dock = System.Windows.Forms.DockStyle.Fill;
            this._relationship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._relationship.LabelText = "Relationship";
            this._relationship.Location = new System.Drawing.Point(240, 2);
            this._relationship.Margin = new System.Windows.Forms.Padding(10, 2, 18, 2);
            this._relationship.Name = "_relationship";
            this._relationship.Size = new System.Drawing.Size(202, 41);
            this._relationship.TabIndex = 6;
            this._relationship.Value = null;
            // 
            // ContactPersonEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ContactPersonEditorComponentControl";
            this.Size = new System.Drawing.Size(460, 305);
            this.Load += new System.EventHandler(this.ContactPersonEditorComponentControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private ClearCanvas.Controls.WinForms.TextField _businessPhone;
        private ClearCanvas.Controls.WinForms.TextField _homePhone;
        private ClearCanvas.Controls.WinForms.TextField _address;
        private ClearCanvas.Controls.WinForms.TextField _name;
        private ClearCanvas.Controls.WinForms.ComboBoxField _type;
        private ClearCanvas.Controls.WinForms.ComboBoxField _relationship;
    }
}
