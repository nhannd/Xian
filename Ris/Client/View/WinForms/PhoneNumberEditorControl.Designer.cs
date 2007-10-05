namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class PhoneNumberEditorControl
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
            this._acceptButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._countryCode = new ClearCanvas.Controls.WinForms.TextField();
            this._extension = new ClearCanvas.Controls.WinForms.TextField();
            this._number = new ClearCanvas.Controls.WinForms.TextField();
            this._areaCode = new ClearCanvas.Controls.WinForms.TextField();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this._phoneType = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this._validFrom = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._validUntil = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(244, 3);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 0;
            this._acceptButton.Text = "Accept";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(325, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this._countryCode, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._extension, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this._number, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this._areaCode, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(409, 184);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _countryCode
            // 
            this._countryCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._countryCode.LabelText = "Country Code";
            this._countryCode.Location = new System.Drawing.Point(2, 53);
            this._countryCode.Margin = new System.Windows.Forms.Padding(2);
            this._countryCode.Name = "_countryCode";
            this._countryCode.Size = new System.Drawing.Size(77, 43);
            this._countryCode.TabIndex = 1;
            this._countryCode.Value = null;
            // 
            // _extension
            // 
            this._extension.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._extension.LabelText = "Extension";
            this._extension.Location = new System.Drawing.Point(327, 53);
            this._extension.Margin = new System.Windows.Forms.Padding(2);
            this._extension.Name = "_extension";
            this._extension.Size = new System.Drawing.Size(80, 43);
            this._extension.TabIndex = 4;
            this._extension.Value = null;
            // 
            // _number
            // 
            this._number.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._number.LabelText = "Number";
            this._number.Location = new System.Drawing.Point(165, 54);
            this._number.Mask = "";
            this._number.Name = "_number";
            this._number.Size = new System.Drawing.Size(157, 41);
            this._number.TabIndex = 3;
            this._number.Value = null;
            // 
            // _areaCode
            // 
            this._areaCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._areaCode.LabelText = "Area Code";
            this._areaCode.Location = new System.Drawing.Point(83, 53);
            this._areaCode.Margin = new System.Windows.Forms.Padding(2);
            this._areaCode.Name = "_areaCode";
            this._areaCode.Size = new System.Drawing.Size(77, 43);
            this._areaCode.TabIndex = 2;
            this._areaCode.Value = null;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel2, 4);
            this.flowLayoutPanel2.Controls.Add(this._phoneType);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(403, 45);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // _phoneType
            // 
            this._phoneType.DataSource = null;
            this._phoneType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._phoneType.LabelText = "Type";
            this._phoneType.Location = new System.Drawing.Point(2, 2);
            this._phoneType.Margin = new System.Windows.Forms.Padding(2);
            this._phoneType.Name = "_phoneType";
            this._phoneType.Size = new System.Drawing.Size(150, 41);
            this._phoneType.TabIndex = 2;
            this._phoneType.Value = null;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 4);
            this.flowLayoutPanel1.Controls.Add(this._cancelButton);
            this.flowLayoutPanel1.Controls.Add(this._acceptButton);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 152);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(403, 29);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel3, 4);
            this.flowLayoutPanel3.Controls.Add(this._validFrom);
            this.flowLayoutPanel3.Controls.Add(this._validUntil);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 101);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(403, 45);
            this.flowLayoutPanel3.TabIndex = 5;
            // 
            // _validFrom
            // 
            this._validFrom.LabelText = "Valid From";
            this._validFrom.Location = new System.Drawing.Point(2, 2);
            this._validFrom.Margin = new System.Windows.Forms.Padding(2);
            this._validFrom.Name = "_validFrom";
            this._validFrom.Nullable = true;
            this._validFrom.ShowTime = false;
            this._validFrom.Size = new System.Drawing.Size(157, 41);
            this._validFrom.TabIndex = 0;
            this._validFrom.Value = null;
            // 
            // _validUntil
            // 
            this._validUntil.LabelText = "Valid Until";
            this._validUntil.Location = new System.Drawing.Point(163, 2);
            this._validUntil.Margin = new System.Windows.Forms.Padding(2);
            this._validUntil.Name = "_validUntil";
            this._validUntil.Nullable = true;
            this._validUntil.ShowTime = false;
            this._validUntil.Size = new System.Drawing.Size(160, 41);
            this._validUntil.TabIndex = 1;
            this._validUntil.Value = null;
            // 
            // PhoneNumberEditorControl
            // 
            this.AcceptButton = this._acceptButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PhoneNumberEditorControl";
            this.Size = new System.Drawing.Size(409, 184);
            this.Load += new System.EventHandler(this.PhoneNumberEditorControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ClearCanvas.Controls.WinForms.TextField _countryCode;
        private ClearCanvas.Controls.WinForms.TextField _extension;
        private ClearCanvas.Controls.WinForms.TextField _areaCode;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _validFrom;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _validUntil;
        private ClearCanvas.Controls.WinForms.ComboBoxField _phoneType;
        private System.Windows.Forms.ToolTip toolTip1;
        private ClearCanvas.Controls.WinForms.TextField _number;
    }
}
