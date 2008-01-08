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
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection2 = new ClearCanvas.Desktop.Selection();
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
            this._diagnosticServiceTree.Location = new System.Drawing.Point(3, 2);
            this._diagnosticServiceTree.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._diagnosticServiceTree.MenuModel = null;
            this._diagnosticServiceTree.Name = "_diagnosticServiceTree";
            this._diagnosticServiceTree.Selection = selection1;
            this._diagnosticServiceTree.ShowToolbar = false;
            this._diagnosticServiceTree.Size = new System.Drawing.Size(513, 321);
            this._diagnosticServiceTree.TabIndex = 14;
            this._diagnosticServiceTree.ToolbarModel = null;
            this._diagnosticServiceTree.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._diagnosticServiceTree.Tree = null;
            // 
            // _okButton
            // 
            this._okButton.Location = new System.Drawing.Point(349, 456);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 15;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(430, 456);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 16;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _procedures
            // 
            this._procedures.Location = new System.Drawing.Point(4, 356);
            this._procedures.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._procedures.MenuModel = null;
            this._procedures.Name = "_procedures";
            this._procedures.ReadOnly = false;
            this._procedures.Selection = selection2;
            this._procedures.ShowToolbar = false;
            this._procedures.Size = new System.Drawing.Size(511, 95);
            this._procedures.TabIndex = 17;
            this._procedures.Table = null;
            this._procedures.ToolbarModel = null;
            this._procedures.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 335);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 17);
            this.label1.TabIndex = 18;
            this.label1.Text = "Procedures";
            // 
            // DiagnosticServiceTreeComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this._procedures);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._diagnosticServiceTree);
            this.Name = "DiagnosticServiceTreeComponentControl";
            this.Size = new System.Drawing.Size(519, 488);
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
