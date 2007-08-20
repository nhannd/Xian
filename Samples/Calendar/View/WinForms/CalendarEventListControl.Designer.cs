namespace ClearCanvas.Ris.Client.Calendar.View.WinForms
{
    partial class CalendarEventListControl
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
            this._eventsTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _eventsTable
            // 
            this._eventsTable.DataSource = null;
            this._eventsTable.Location = new System.Drawing.Point(48, 39);
            this._eventsTable.Margin = new System.Windows.Forms.Padding(4);
            this._eventsTable.MenuModel = null;
            this._eventsTable.Name = "_eventsTable";
            this._eventsTable.Size = new System.Drawing.Size(559, 283);
            this._eventsTable.TabIndex = 0;
            this._eventsTable.ToolbarModel = null;
            this._eventsTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // CalendarEventListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._eventsTable);
            this.Name = "CalendarEventListControl";
            this.Size = new System.Drawing.Size(685, 428);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _eventsTable;

    }
}
