namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class ConfirmReconciliationComponentControl
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
            this._sourceTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._targetTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._cancelButton = new System.Windows.Forms.Button();
            this._continueButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _sourceTable
            // 
            this._sourceTable.DataSource = null;
            this._sourceTable.Location = new System.Drawing.Point(17, 44);
            this._sourceTable.Margin = new System.Windows.Forms.Padding(4);
            this._sourceTable.MenuModel = null;
            this._sourceTable.Name = "_sourceTable";
            this._sourceTable.ReadOnly = true;
            this._sourceTable.Size = new System.Drawing.Size(720, 209);
            this._sourceTable.TabIndex = 0;
            this._sourceTable.ToolbarModel = null;
            this._sourceTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._sourceTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _targetTable
            // 
            this._targetTable.DataSource = null;
            this._targetTable.Location = new System.Drawing.Point(17, 299);
            this._targetTable.Margin = new System.Windows.Forms.Padding(4);
            this._targetTable.MenuModel = null;
            this._targetTable.Name = "_targetTable";
            this._targetTable.ReadOnly = true;
            this._targetTable.Size = new System.Drawing.Size(720, 214);
            this._targetTable.TabIndex = 1;
            this._targetTable.ToolbarModel = null;
            this._targetTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._targetTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(662, 529);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 2;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _continueButton
            // 
            this._continueButton.Location = new System.Drawing.Point(581, 529);
            this._continueButton.Name = "_continueButton";
            this._continueButton.Size = new System.Drawing.Size(75, 23);
            this._continueButton.TabIndex = 3;
            this._continueButton.Text = "Continue";
            this._continueButton.UseVisualStyleBackColor = true;
            this._continueButton.Click += new System.EventHandler(this._continueButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "The following profiles";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 278);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(204, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "will be linked with these profiles";
            // 
            // ConfirmReconciliationComponentControl
            // 
            this.AcceptButton = this._continueButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._continueButton);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._targetTable);
            this.Controls.Add(this._sourceTable);
            this.Name = "ConfirmReconciliationComponentControl";
            this.Size = new System.Drawing.Size(761, 565);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _sourceTable;
        private ClearCanvas.Desktop.View.WinForms.TableView _targetTable;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _continueButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
