namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class PatientProfileAdditionalInfoEditorComponentControl
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
            this._primaryLanguage = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._religion = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this._primaryLanguage, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._religion, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(252, 121);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _primaryLanguage
            // 
            this._primaryLanguage.DataSource = null;
            this._primaryLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this._primaryLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._primaryLanguage.LabelText = "Primary Language";
            this._primaryLanguage.Location = new System.Drawing.Point(2, 2);
            this._primaryLanguage.Margin = new System.Windows.Forms.Padding(2);
            this._primaryLanguage.Name = "_primaryLanguage";
            this._primaryLanguage.Size = new System.Drawing.Size(122, 41);
            this._primaryLanguage.TabIndex = 0;
            this._primaryLanguage.Value = null;
            // 
            // _religion
            // 
            this._religion.DataSource = null;
            this._religion.Dock = System.Windows.Forms.DockStyle.Fill;
            this._religion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._religion.LabelText = "Religion";
            this._religion.Location = new System.Drawing.Point(128, 2);
            this._religion.Margin = new System.Windows.Forms.Padding(2);
            this._religion.Name = "_religion";
            this._religion.Size = new System.Drawing.Size(122, 41);
            this._religion.TabIndex = 1;
            this._religion.Value = null;
            // 
            // PatientProfileAdditionalInfoEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PatientProfileAdditionalInfoEditorComponentControl";
            this.Size = new System.Drawing.Size(252, 121);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Controls.WinForms.ComboBoxField _primaryLanguage;
        private ClearCanvas.Controls.WinForms.ComboBoxField _religion;
    }
}
