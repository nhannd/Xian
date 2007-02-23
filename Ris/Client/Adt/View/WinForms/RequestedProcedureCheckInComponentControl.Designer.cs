namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class RequestedProcedureCheckInComponentControl
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
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            this._requestedProcedureTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _requestedProcedureTableView
            // 
            this._requestedProcedureTableView.Location = new System.Drawing.Point(7, 11);
            this._requestedProcedureTableView.MenuModel = null;
            this._requestedProcedureTableView.Name = "_requestedProcedureTableView";
            this._requestedProcedureTableView.ReadOnly = false;
            this._requestedProcedureTableView.Selection = selection1;
            this._requestedProcedureTableView.ShowToolbar = false;
            this._requestedProcedureTableView.Size = new System.Drawing.Size(469, 219);
            this._requestedProcedureTableView.TabIndex = 0;
            this._requestedProcedureTableView.Table = null;
            this._requestedProcedureTableView.ToolbarModel = null;
            this._requestedProcedureTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._requestedProcedureTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _okButton
            // 
            this._okButton.Location = new System.Drawing.Point(320, 241);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 2;
            this._okButton.Text = "Check-In";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(401, 241);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 3;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // RequestedProcedureCheckInComponentControl
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._requestedProcedureTableView);
            this.Name = "RequestedProcedureCheckInComponentControl";
            this.Size = new System.Drawing.Size(484, 272);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _requestedProcedureTableView;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
    }
}
