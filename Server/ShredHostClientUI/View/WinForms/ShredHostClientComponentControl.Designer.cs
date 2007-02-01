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
            this._shredHostGroupBox = new System.Windows.Forms.GroupBox();
            this._shredHostGroupBox.SuspendLayout();
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
            this._toggleButton.Location = new System.Drawing.Point(6, 35);
            this._toggleButton.Name = "_toggleButton";
            this._toggleButton.Size = new System.Drawing.Size(299, 57);
            this._toggleButton.TabIndex = 1;
            this._toggleButton.Text = "Start/Stop";
            this._toggleButton.UseVisualStyleBackColor = true;
            // 
            // _shredHostGroupBox
            // 
            this._shredHostGroupBox.Controls.Add(this._toggleButton);
            this._shredHostGroupBox.Location = new System.Drawing.Point(6, 6);
            this._shredHostGroupBox.Name = "_shredHostGroupBox";
            this._shredHostGroupBox.Size = new System.Drawing.Size(311, 111);
            this._shredHostGroupBox.TabIndex = 3;
            this._shredHostGroupBox.TabStop = false;
            this._shredHostGroupBox.Text = "ShredHost";
            // 
            // ShredHostClientComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._shredHostGroupBox);
            this.Controls.Add(this._shredCollectionTable);
            this.Name = "ShredHostClientComponentControl";
            this.Size = new System.Drawing.Size(322, 371);
            this._shredHostGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _shredCollectionTable;
        private System.Windows.Forms.Button _toggleButton;
        private System.Windows.Forms.GroupBox _shredHostGroupBox;
    }
}
