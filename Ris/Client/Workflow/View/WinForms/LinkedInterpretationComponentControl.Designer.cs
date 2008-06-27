namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class LinkedInterpretationComponentControl
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
			this._worklistItemTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _worklistItemTableView
			// 
			this._worklistItemTableView.Location = new System.Drawing.Point(13, 30);
			this._worklistItemTableView.Name = "_worklistItemTableView";
			this._worklistItemTableView.ReadOnly = false;
			this._worklistItemTableView.ShowToolbar = false;
			this._worklistItemTableView.Size = new System.Drawing.Size(462, 271);
			this._worklistItemTableView.TabIndex = 1;
			this._worklistItemTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(319, 318);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 2;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(400, 318);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 3;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(324, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Check any additional procedures that should be linked to this report";
			// 
			// LinkedInterpretationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label1);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._worklistItemTableView);
			this.Name = "LinkedInterpretationComponentControl";
			this.Size = new System.Drawing.Size(491, 354);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _worklistItemTableView;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Label label1;
    }
}
