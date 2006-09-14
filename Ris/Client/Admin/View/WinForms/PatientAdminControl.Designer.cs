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
            this._patientTableView.DataSource = null;
            this._patientTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._patientTableView.Location = new System.Drawing.Point(0, 0);
            this._patientTableView.Margin = new System.Windows.Forms.Padding(4);
            this._patientTableView.MenuModel = null;
            this._patientTableView.Name = "_patientTableView";
            this._patientTableView.ReadOnly = true;
            this._patientTableView.Size = new System.Drawing.Size(765, 374);
            this._patientTableView.TabIndex = 0;
            this._patientTableView.ToolbarModel = null;
            this._patientTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._patientTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._patientTableView.ItemDoubleClicked += new System.EventHandler(this._patientTableView_ItemDoubleClicked);
            this._patientTableView.SelectionChanged += new System.EventHandler(this._patientTableView_SelectionChanged);
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
