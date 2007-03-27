namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class PatientOverviewComponentControl
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
            this._name = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _name
            // 
            this._name.AutoSize = true;
            this._name.Location = new System.Drawing.Point(3, 11);
            this._name.Name = "_name";
            this._name.Size = new System.Drawing.Size(35, 13);
            this._name.TabIndex = 1;
            this._name.Text = "label1";
            // 
            // PatientOverviewComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._name);
            this.Name = "PatientOverviewComponentControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _name;

    }
}
