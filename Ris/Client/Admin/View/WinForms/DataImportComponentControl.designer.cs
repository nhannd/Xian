namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class DataImportComponentControl
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
            this._browseButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this._startButton = new System.Windows.Forms.Button();
            this._importer = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._dataFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _browseButton
            // 
            this._browseButton.Location = new System.Drawing.Point(412, 106);
            this._browseButton.Margin = new System.Windows.Forms.Padding(4);
            this._browseButton.Name = "_browseButton";
            this._browseButton.Size = new System.Drawing.Size(76, 28);
            this._browseButton.TabIndex = 1;
            this._browseButton.Text = "Browse";
            this._browseButton.UseVisualStyleBackColor = true;
            this._browseButton.Click += new System.EventHandler(this._browseButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // _startButton
            // 
            this._startButton.Location = new System.Drawing.Point(38, 182);
            this._startButton.Margin = new System.Windows.Forms.Padding(4);
            this._startButton.Name = "_startButton";
            this._startButton.Size = new System.Drawing.Size(100, 28);
            this._startButton.TabIndex = 3;
            this._startButton.Text = "Import";
            this._startButton.UseVisualStyleBackColor = true;
            this._startButton.Click += new System.EventHandler(this._startButton_Click);
            // 
            // _importer
            // 
            this._importer.DataSource = null;
            this._importer.DisplayMember = "";
            this._importer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._importer.LabelText = "Import Type";
            this._importer.Location = new System.Drawing.Point(38, 18);
            this._importer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._importer.Name = "_importer";
            this._importer.Size = new System.Drawing.Size(367, 62);
            this._importer.TabIndex = 4;
            this._importer.Value = null;
            // 
            // _dataFile
            // 
            this._dataFile.Location = new System.Drawing.Point(38, 109);
            this._dataFile.Name = "_dataFile";
            this._dataFile.Size = new System.Drawing.Size(367, 22);
            this._dataFile.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Data File";
            // 
            // DataImportComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this._dataFile);
            this.Controls.Add(this._importer);
            this.Controls.Add(this._startButton);
            this.Controls.Add(this._browseButton);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DataImportComponentControl";
            this.Size = new System.Drawing.Size(585, 250);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _browseButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button _startButton;
        private ClearCanvas.Controls.WinForms.ComboBoxField _importer;
        private System.Windows.Forms.TextBox _dataFile;
        private System.Windows.Forms.Label label1;
    }
}
