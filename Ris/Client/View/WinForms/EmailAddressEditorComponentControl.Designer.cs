namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class EmailAddressEditorComponentControl
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
            this._validFrom = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._validUntil = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._address = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._cancelButton = new System.Windows.Forms.Button();
            this._acceptButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _validFrom
            // 
            this._validFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this._validFrom.LabelText = "Valid From";
            this._validFrom.Location = new System.Drawing.Point(2, 47);
            this._validFrom.Margin = new System.Windows.Forms.Padding(2, 2, 22, 2);
            this._validFrom.Name = "_validFrom";
            this._validFrom.Nullable = true;
            this._validFrom.ShowTime = false;
            this._validFrom.Size = new System.Drawing.Size(149, 76);
            this._validFrom.TabIndex = 0;
            this._validFrom.Value = null;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this._validFrom, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._validUntil, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this._address, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(346, 160);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // _validUntil
            // 
            this._validUntil.Dock = System.Windows.Forms.DockStyle.Fill;
            this._validUntil.LabelText = "Valid Until";
            this._validUntil.Location = new System.Drawing.Point(175, 47);
            this._validUntil.Margin = new System.Windows.Forms.Padding(2, 2, 22, 2);
            this._validUntil.Name = "_validUntil";
            this._validUntil.Nullable = true;
            this._validUntil.ShowTime = false;
            this._validUntil.Size = new System.Drawing.Size(149, 76);
            this._validUntil.TabIndex = 1;
            this._validUntil.Value = null;
            // 
            // _address
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._address, 2);
            this._address.Dock = System.Windows.Forms.DockStyle.Fill;
            this._address.LabelText = "Email Address";
            this._address.Location = new System.Drawing.Point(2, 2);
            this._address.Margin = new System.Windows.Forms.Padding(2, 2, 22, 2);
            this._address.Mask = "";
            this._address.Name = "_address";
            this._address.Size = new System.Drawing.Size(322, 41);
            this._address.TabIndex = 2;
            this._address.Value = null;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this._cancelButton);
            this.flowLayoutPanel1.Controls.Add(this._acceptButton);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 128);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 23, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(320, 29);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(242, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 0;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(161, 3);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 1;
            this._acceptButton.Text = "Accept";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // EmailAddressEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "EmailAddressEditorComponentControl";
            this.Size = new System.Drawing.Size(346, 160);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.DateTimeField _validFrom;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _validUntil;
        private ClearCanvas.Desktop.View.WinForms.TextField _address;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
    }
}
