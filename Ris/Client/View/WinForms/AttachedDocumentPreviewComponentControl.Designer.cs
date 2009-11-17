namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class AttachedDocumentPreviewComponentControl
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
			this._splitContainer = new System.Windows.Forms.SplitContainer();
			this._attachments = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._splitContainer.Panel1.SuspendLayout();
			this._splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// _splitContainer
			// 
			this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._splitContainer.Location = new System.Drawing.Point(4, 2);
			this._splitContainer.Name = "_splitContainer";
			this._splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// _splitContainer.Panel1
			// 
			this._splitContainer.Panel1.Controls.Add(this._attachments);
			this._splitContainer.Panel1MinSize = 100;
			// 
			// _splitContainer.Panel2
			// 
			this._splitContainer.Panel2.BackColor = System.Drawing.SystemColors.ControlDark;
			this._splitContainer.Panel2.Padding = new System.Windows.Forms.Padding(1);
			this._splitContainer.Size = new System.Drawing.Size(409, 431);
			this._splitContainer.SplitterDistance = 100;
			this._splitContainer.TabIndex = 1;
			// 
			// _attachments
			// 
			this._attachments.Dock = System.Windows.Forms.DockStyle.Fill;
			this._attachments.Location = new System.Drawing.Point(0, 0);
			this._attachments.MinimumSize = new System.Drawing.Size(0, 100);
			this._attachments.MultiSelect = false;
			this._attachments.Name = "_attachments";
			this._attachments.ReadOnly = false;
			this._attachments.Size = new System.Drawing.Size(409, 100);
			this._attachments.TabIndex = 0;
			// 
			// AttachedDocumentPreviewComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this._splitContainer);
			this.Name = "AttachedDocumentPreviewComponentControl";
			this.Padding = new System.Windows.Forms.Padding(4, 2, 2, 7);
			this.Size = new System.Drawing.Size(415, 440);
			this.Load += new System.EventHandler(this.AttachedDocumentPreviewComponentControl_Load);
			this._splitContainer.Panel1.ResumeLayout(false);
			this._splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer _splitContainer;
        private ClearCanvas.Desktop.View.WinForms.TableView _attachments;
    }
}
