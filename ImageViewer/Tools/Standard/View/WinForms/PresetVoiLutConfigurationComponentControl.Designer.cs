namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    partial class PresetVoiLutConfigurationComponentControl
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
			this._comboAddPreset = new ClearCanvas.Controls.WinForms.ComboBoxField();
			this._addPresetButton = new System.Windows.Forms.Button();
			this._tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _presetVoiLuts
			// 
			this._tableLayoutPanel.SetColumnSpan(this._presetVoiLuts, 2);
			this._presetVoiLuts.Dock = System.Windows.Forms.DockStyle.Fill;
			this._presetVoiLuts.Location = new System.Drawing.Point(3, 63);
			this._presetVoiLuts.MenuModel = null;
			this._presetVoiLuts.MultiSelect = false;
			this._presetVoiLuts.Name = "_presetVoiLuts";
			this._presetVoiLuts.ReadOnly = false;
			this._presetVoiLuts.Selection = selection1;
			this._presetVoiLuts.Size = new System.Drawing.Size(335, 190);
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
			this._tableLayoutPanel.ColumnCount = 2;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.Controls.Add(this._comboModality, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._presetVoiLuts, 0, 1);
			this._tableLayoutPanel.Controls.Add(this._comboAddPreset, 0, 2);
			this._tableLayoutPanel.Controls.Add(this._addPresetButton, 1, 2);
			this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 3;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.Size = new System.Drawing.Size(341, 300);
			this._tableLayoutPanel.TabIndex = 2;
			// 
			// _comboAddPreset
			// 
			this._comboAddPreset.DataSource = null;
			this._comboAddPreset.DisplayMember = "";
			this._comboAddPreset.Dock = System.Windows.Forms.DockStyle.Fill;
			this._comboAddPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._comboAddPreset.LabelText = "Add Preset Type";
			this._comboAddPreset.Location = new System.Drawing.Point(2, 258);
			this._comboAddPreset.Margin = new System.Windows.Forms.Padding(2);
			this._comboAddPreset.Name = "_comboAddPreset";
			this._comboAddPreset.Size = new System.Drawing.Size(256, 41);
			this._comboAddPreset.TabIndex = 2;
			this._comboAddPreset.Value = null;
			// 
			// _addPresetButton
			// 
			this._addPresetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._addPresetButton.Location = new System.Drawing.Point(263, 275);
			this._addPresetButton.Name = "_addPresetButton";
			this._addPresetButton.Size = new System.Drawing.Size(75, 23);
			this._addPresetButton.TabIndex = 3;
			this._addPresetButton.Text = "Add";
			this._addPresetButton.UseVisualStyleBackColor = true;
			// 
			// PresetVoiLutConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._tableLayoutPanel);
			this.Name = "PresetVoiLutConfigurationComponentControl";
			this.Size = new System.Drawing.Size(341, 300);
			this._tableLayoutPanel.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _presetVoiLuts;
		private ClearCanvas.Controls.WinForms.ComboBoxField _comboModality;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
		private ClearCanvas.Controls.WinForms.ComboBoxField _comboAddPreset;
		private System.Windows.Forms.Button _addPresetButton;
    }
}
