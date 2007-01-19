namespace ClearCanvas.Server.ShredHostClientUI.View.WinForms
{
    partial class ShredHostClientComponentControl
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
            ClearCanvas.Desktop.Selection selection3 = new ClearCanvas.Desktop.Selection();
            this._shredCollectionTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._toggleButton = new System.Windows.Forms.Button();
            this._runningStateLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _shredCollectionTable
            // 
            this._shredCollectionTable.Location = new System.Drawing.Point(4, 117);
            this._shredCollectionTable.Margin = new System.Windows.Forms.Padding(4);
            this._shredCollectionTable.MenuModel = null;
            this._shredCollectionTable.Name = "_shredCollectionTable";
            this._shredCollectionTable.ReadOnly = false;
            this._shredCollectionTable.Selection = selection3;
            this._shredCollectionTable.Size = new System.Drawing.Size(314, 250);
            this._shredCollectionTable.TabIndex = 2;
            this._shredCollectionTable.Table = null;
            this._shredCollectionTable.ToolbarModel = null;
            this._shredCollectionTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._shredCollectionTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _toggleButton
            // 
            this._toggleButton.Location = new System.Drawing.Point(158, 30);
            this._toggleButton.Name = "_toggleButton";
            this._toggleButton.Size = new System.Drawing.Size(130, 23);
            this._toggleButton.TabIndex = 1;
            this._toggleButton.Text = "Start/Stop";
            this._toggleButton.UseVisualStyleBackColor = true;
            // 
            // _runningStateLabel
            // 
            this._runningStateLabel.AutoSize = true;
            this._runningStateLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._runningStateLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._runningStateLabel.Location = new System.Drawing.Point(21, 30);
            this._runningStateLabel.Name = "_runningStateLabel";
            this._runningStateLabel.Size = new System.Drawing.Size(99, 19);
            this._runningStateLabel.TabIndex = 0;
            this._runningStateLabel.Text = "_runningState";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._toggleButton);
            this.groupBox1.Controls.Add(this._runningStateLabel);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(311, 111);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ShredHost";
            // 
            // ShredHostClientComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._shredCollectionTable);
            this.Name = "ShredHostClientComponentControl";
            this.Size = new System.Drawing.Size(322, 371);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _shredCollectionTable;
        private System.Windows.Forms.Button _toggleButton;
        private System.Windows.Forms.Label _runningStateLabel;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
