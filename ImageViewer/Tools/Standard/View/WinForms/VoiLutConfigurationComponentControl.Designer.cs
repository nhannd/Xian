namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    partial class VoiLutConfigurationComponentControl
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
			this._presetVoiLuts = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._comboModality = new ClearCanvas.Controls.WinForms.ComboBoxField();
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _presetVoiLuts
			// 
			this._presetVoiLuts.Dock = System.Windows.Forms.DockStyle.Fill;
			this._presetVoiLuts.Location = new System.Drawing.Point(3, 63);
			this._presetVoiLuts.MenuModel = null;
			this._presetVoiLuts.MultiSelect = false;
			this._presetVoiLuts.Name = "_presetVoiLuts";
			this._presetVoiLuts.ReadOnly = false;
			this._presetVoiLuts.Selection = selection1;
			this._presetVoiLuts.Size = new System.Drawing.Size(332, 190);
			this._presetVoiLuts.TabIndex = 0;
			this._presetVoiLuts.Table = null;
			this._presetVoiLuts.ToolbarModel = null;
			this._presetVoiLuts.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._presetVoiLuts.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// _comboModality
			// 
			this._comboModality.DataSource = null;
			this._comboModality.DisplayMember = "";
			this._comboModality.Dock = System.Windows.Forms.DockStyle.Left;
			this._comboModality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._comboModality.LabelText = "Modality";
			this._comboModality.Location = new System.Drawing.Point(6, 6);
			this._comboModality.Margin = new System.Windows.Forms.Padding(6);
			this._comboModality.Name = "_comboModality";
			this._comboModality.Size = new System.Drawing.Size(110, 48);
			this._comboModality.TabIndex = 1;
			this._comboModality.Value = null;
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.ColumnCount = 1;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel.Controls.Add(this._comboModality, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._presetVoiLuts, 0, 1);
			this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 2;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.Size = new System.Drawing.Size(338, 256);
			this._tableLayoutPanel.TabIndex = 2;
			// 
			// VoiLutConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._tableLayoutPanel);
			this.Name = "VoiLutConfigurationComponentControl";
			this.Size = new System.Drawing.Size(338, 256);
			this._tableLayoutPanel.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _presetVoiLuts;
		private ClearCanvas.Controls.WinForms.ComboBoxField _comboModality;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
    }
}
