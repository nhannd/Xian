namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class DiagnosticServiceTreeComponentControl
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
			this._diagnosticServiceTree = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._procedures = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _diagnosticServiceTree
			// 
			this._diagnosticServiceTree.AllowDrop = true;
			this._diagnosticServiceTree.IconColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this._diagnosticServiceTree.IconSize = new System.Drawing.Size(16, 16);
			this._diagnosticServiceTree.Location = new System.Drawing.Point(2, 2);
			this._diagnosticServiceTree.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this._diagnosticServiceTree.Name = "_diagnosticServiceTree";
			this._diagnosticServiceTree.ShowToolbar = false;
			this._diagnosticServiceTree.Size = new System.Drawing.Size(385, 261);
			this._diagnosticServiceTree.TabIndex = 0;
			this._diagnosticServiceTree.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._diagnosticServiceTree.TreeBackColor = System.Drawing.SystemColors.Window;
			this._diagnosticServiceTree.TreeForeColor = System.Drawing.SystemColors.WindowText;
			this._diagnosticServiceTree.TreeLineColor = System.Drawing.Color.Black;
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(262, 370);
			this._okButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(56, 19);
			this._okButton.TabIndex = 3;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(322, 370);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(56, 19);
			this._cancelButton.TabIndex = 4;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _procedures
			// 
			this._procedures.Location = new System.Drawing.Point(3, 289);
			this._procedures.Name = "_procedures";
			this._procedures.ReadOnly = false;
			this._procedures.ShowToolbar = false;
			this._procedures.Size = new System.Drawing.Size(383, 77);
			this._procedures.TabIndex = 2;
			this._procedures.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(2, 272);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(61, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Procedures";
			// 
			// DiagnosticServiceTreeComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label1);
			this.Controls.Add(this._procedures);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._diagnosticServiceTree);
			this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.Name = "DiagnosticServiceTreeComponentControl";
			this.Size = new System.Drawing.Size(389, 396);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.BindingTreeView _diagnosticServiceTree;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private ClearCanvas.Desktop.View.WinForms.TableView _procedures;
        private System.Windows.Forms.Label label1;
    }
}
