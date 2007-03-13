namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    partial class WindowLevelConfigurationApplicationComponentControl
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
			this._modalityCombo = new ClearCanvas.Controls.WinForms.ComboBoxField();
			this._presetTable = new ClearCanvas.Desktop.View.WinForms.CrudTableView();
			this.SuspendLayout();
			// 
			// _modalityCombo
			// 
			this._modalityCombo.DataSource = null;
			this._modalityCombo.DisplayMember = "";
			this._modalityCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._modalityCombo.LabelText = "Modality";
			this._modalityCombo.Location = new System.Drawing.Point(15, 11);
			this._modalityCombo.Margin = new System.Windows.Forms.Padding(2);
			this._modalityCombo.Name = "_modalityCombo";
			this._modalityCombo.Size = new System.Drawing.Size(83, 41);
			this._modalityCombo.TabIndex = 0;
			this._modalityCombo.Value = null;
			// 
			// _presetTable
			// 
			this._presetTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._presetTable.Location = new System.Drawing.Point(15, 57);
			this._presetTable.MenuModel = null;
			this._presetTable.Name = "_presetTable";
			this._presetTable.ReadOnly = false;
			this._presetTable.Selection = selection1;
			this._presetTable.Size = new System.Drawing.Size(236, 140);
			this._presetTable.TabIndex = 1;
			this._presetTable.Table = null;
			this._presetTable.ToolbarModel = null;
			this._presetTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._presetTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// WindowLevelConfigurationApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._presetTable);
			this.Controls.Add(this._modalityCombo);
			this.Name = "WindowLevelConfigurationApplicationComponentControl";
			this.Size = new System.Drawing.Size(267, 212);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Controls.WinForms.ComboBoxField _modalityCombo;
		private ClearCanvas.Desktop.View.WinForms.CrudTableView _presetTable;

	}
}
