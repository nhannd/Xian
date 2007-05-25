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
            this._procedureReport = new System.Windows.Forms.WebBrowser();
            this._saveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _procedureReport
            // 
            this._procedureReport.Location = new System.Drawing.Point(69, 116);
            this._procedureReport.MinimumSize = new System.Drawing.Size(20, 20);
            this._procedureReport.Name = "_procedureReport";
            this._procedureReport.Size = new System.Drawing.Size(745, 444);
            this._procedureReport.TabIndex = 0;
            // 
            // _saveButton
            // 
            this._saveButton.Location = new System.Drawing.Point(717, 587);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(75, 23);
            this._saveButton.TabIndex = 1;
            this._saveButton.Text = "Save";
            this._saveButton.UseVisualStyleBackColor = true;
            this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
            // 
            // PerformedProcedureComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._saveButton);
            this.Controls.Add(this._procedureReport);
            this.Name = "PerformedProcedureComponentControl";
            this.Size = new System.Drawing.Size(913, 627);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser _procedureReport;
        private System.Windows.Forms.Button _saveButton;
    }
}
