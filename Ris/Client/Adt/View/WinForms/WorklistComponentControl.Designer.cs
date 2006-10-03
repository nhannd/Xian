namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class WorklistComponentControl
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
            this._patientProfileTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _patientProfileTable
            // 
            this._patientProfileTable.DataSource = null;
            this._patientProfileTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._patientProfileTable.Location = new System.Drawing.Point(0, 0);
            this._patientProfileTable.Margin = new System.Windows.Forms.Padding(4);
            this._patientProfileTable.MenuModel = null;
            this._patientProfileTable.Name = "_patientProfileTable";
            this._patientProfileTable.ReadOnly = true;
            this._patientProfileTable.Size = new System.Drawing.Size(771, 429);
            this._patientProfileTable.TabIndex = 0;
            this._patientProfileTable.ToolbarModel = null;
            this._patientProfileTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._patientProfileTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._patientProfileTable.ItemDoubleClicked += new System.EventHandler(this._patientProfileTable_ItemDoubleClicked);
            this._patientProfileTable.SelectionChanged += new System.EventHandler(this._patientProfileTable_SelectionChanged);
            // 
            // WorklistComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._patientProfileTable);
            this.Name = "WorklistComponentControl";
            this.Size = new System.Drawing.Size(771, 429);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _patientProfileTable;
    }
}
