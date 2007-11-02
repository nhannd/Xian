namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class LookupField
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
            this._findButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._inputField = new ClearCanvas.Desktop.View.WinForms.SuggestComboField();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _findButton
            // 
            this._findButton.Location = new System.Drawing.Point(0, 20);
            this._findButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this._findButton.Name = "_findButton";
            this._findButton.Size = new System.Drawing.Size(32, 23);
            this._findButton.TabIndex = 1;
            this._findButton.Text = "...";
            this._findButton.UseVisualStyleBackColor = true;
            this._findButton.Click += new System.EventHandler(this._findButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel1.Controls.Add(this._inputField, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(230, 54);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // _inputField
            // 
            this._inputField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._inputField.LabelText = "label";
            this._inputField.Location = new System.Drawing.Point(3, 2);
            this._inputField.Margin = new System.Windows.Forms.Padding(3, 2, 1, 2);
            this._inputField.Name = "_inputField";
            this._inputField.Size = new System.Drawing.Size(192, 50);
            this._inputField.TabIndex = 0;
            this._inputField.Value = null;
            this._inputField.Format += new System.Windows.Forms.ListControlConvertEventHandler(this._inputField_Format);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._findButton);
            this.panel1.Location = new System.Drawing.Point(197, 3);
            this.panel1.Margin = new System.Windows.Forms.Padding(1, 3, 0, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(33, 48);
            this.panel1.TabIndex = 3;
            // 
            // LookupField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LookupField";
            this.Size = new System.Drawing.Size(233, 59);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _findButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private ClearCanvas.Desktop.View.WinForms.SuggestComboField _inputField;
    }
}
