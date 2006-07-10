namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class PatientAdminControl
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
            this._patientTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _patientTableView
            // 
            this._patientTableView.Location = new System.Drawing.Point(22, 39);
            this._patientTableView.Margin = new System.Windows.Forms.Padding(4);
            this._patientTableView.Name = "_patientTableView";
            this._patientTableView.Size = new System.Drawing.Size(720, 281);
            this._patientTableView.TabIndex = 0;
            // 
            // PatientAdminControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._patientTableView);
            this.Name = "PatientAdminControl";
            this.Size = new System.Drawing.Size(765, 374);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _patientTableView;




    }
}
