#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	/// <summary>
    /// Provides the user-interface for <see cref="LayoutComponentView"/>
	/// </summary>
	public class LayoutControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label _tileColumnsLabel;
		private System.Windows.Forms.Label _tileRowsLabel;
		private System.Windows.Forms.Label _imageBoxColumnsLabel;
		private System.Windows.Forms.Label _imageBoxRowsLabel;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _imageBoxRows;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _imageBoxColumns;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _tileColumns;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _tileRows;
		private Button _applyTiles;
		private Button _applyImageBoxes;
		private Panel imageBoxPanel;
		private Panel tilePanel;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


        private LayoutComponent _layoutComponent;
		private GroupBox groupBox1;
		private GroupBox groupBox2;
		private Button _buttonConfigure;
        private BindingSource _bindingSource;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="component">The component to look at</param>
		public LayoutControl(LayoutComponent component)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

            _layoutComponent = component;

            // rather than binding directly to the component, create a binding source
            // this is the only way that we can pull data from the component on demand
            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _layoutComponent;

			//these values are just constants, so we won't databind them, it's unnecessary.
			_imageBoxRows.Minimum = 1;
			_imageBoxColumns.Minimum = 1;
			_tileRows.Minimum = 1;
			_tileColumns.Minimum = 1;

			_imageBoxRows.Maximum = _layoutComponent.MaximumImageBoxRows;
			_imageBoxColumns.Maximum = _layoutComponent.MaximumImageBoxColumns;
			_tileRows.Maximum = _layoutComponent.MaximumTileRows;
			_tileColumns.Maximum = _layoutComponent.MaximumTileColumns;
			
			// bind control values
            _tileColumns.DataBindings.Add("Value", _bindingSource, "TileColumns", true, DataSourceUpdateMode.OnPropertyChanged);
            _tileRows.DataBindings.Add("Value", _bindingSource, "TileRows", true, DataSourceUpdateMode.OnPropertyChanged);
            _imageBoxColumns.DataBindings.Add("Value", _bindingSource, "ImageBoxColumns", true, DataSourceUpdateMode.OnPropertyChanged);
            _imageBoxRows.DataBindings.Add("Value", _bindingSource, "ImageBoxRows", true, DataSourceUpdateMode.OnPropertyChanged);

            // bind control enablement
			_imageBoxColumns.DataBindings.Add("Enabled", _bindingSource, "ImageBoxSectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_imageBoxRows.DataBindings.Add("Enabled", _bindingSource, "ImageBoxSectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_applyImageBoxes.DataBindings.Add("Enabled", _bindingSource, "ImageBoxSectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_tileColumns.DataBindings.Add("Enabled", _bindingSource, "TileSectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_tileRows.DataBindings.Add("Enabled", _bindingSource, "TileSectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_applyTiles.DataBindings.Add("Enabled", _bindingSource, "TileSectionEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        /// <summary>
        /// Event handler for the image boxes Apply button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _applyImageBoxes_Click(object sender, EventArgs e)
        {
            _layoutComponent.ApplyImageBoxLayout();
        }

        /// <summary>
        /// Event handler for the tiles Apply button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _applyTiles_Click(object sender, EventArgs e)
        {
            _layoutComponent.ApplyTileLayout();
        }

		private void OnButtonConfigureClick(object sender, EventArgs e)
		{
			_layoutComponent.Configure();
		}

        /// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._tileColumnsLabel = new System.Windows.Forms.Label();
			this._tileRowsLabel = new System.Windows.Forms.Label();
			this._imageBoxColumnsLabel = new System.Windows.Forms.Label();
			this._imageBoxRowsLabel = new System.Windows.Forms.Label();
			this._applyTiles = new System.Windows.Forms.Button();
			this._applyImageBoxes = new System.Windows.Forms.Button();
			this.imageBoxPanel = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._imageBoxColumns = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._imageBoxRows = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this.tilePanel = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._tileRows = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._tileColumns = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._buttonConfigure = new System.Windows.Forms.Button();
			this.imageBoxPanel.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxColumns)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxRows)).BeginInit();
			this.tilePanel.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._tileRows)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._tileColumns)).BeginInit();
			this.SuspendLayout();
			// 
			// _tileColumnsLabel
			// 
			this._tileColumnsLabel.Location = new System.Drawing.Point(112, 28);
			this._tileColumnsLabel.Name = "_tileColumnsLabel";
			this._tileColumnsLabel.Size = new System.Drawing.Size(70, 23);
			this._tileColumnsLabel.TabIndex = 11;
			this._tileColumnsLabel.Text = "Columns";
			// 
			// _tileRowsLabel
			// 
			this._tileRowsLabel.Location = new System.Drawing.Point(27, 28);
			this._tileRowsLabel.Name = "_tileRowsLabel";
			this._tileRowsLabel.Size = new System.Drawing.Size(48, 23);
			this._tileRowsLabel.TabIndex = 9;
			this._tileRowsLabel.Text = "Rows";
			// 
			// _imageBoxColumnsLabel
			// 
			this._imageBoxColumnsLabel.Location = new System.Drawing.Point(112, 32);
			this._imageBoxColumnsLabel.Name = "_imageBoxColumnsLabel";
			this._imageBoxColumnsLabel.Size = new System.Drawing.Size(67, 23);
			this._imageBoxColumnsLabel.TabIndex = 4;
			this._imageBoxColumnsLabel.Text = "Columns";
			// 
			// _imageBoxRowsLabel
			// 
			this._imageBoxRowsLabel.Location = new System.Drawing.Point(27, 31);
			this._imageBoxRowsLabel.Name = "_imageBoxRowsLabel";
			this._imageBoxRowsLabel.Size = new System.Drawing.Size(48, 23);
			this._imageBoxRowsLabel.TabIndex = 2;
			this._imageBoxRowsLabel.Text = "Rows";
			// 
			// _applyTiles
			// 
			this._applyTiles.Location = new System.Drawing.Point(59, 97);
			this._applyTiles.Name = "_applyTiles";
			this._applyTiles.Size = new System.Drawing.Size(75, 23);
			this._applyTiles.TabIndex = 13;
			this._applyTiles.Text = "Apply";
			this._applyTiles.Click += new System.EventHandler(this._applyTiles_Click);
			// 
			// _applyImageBoxes
			// 
			this._applyImageBoxes.Location = new System.Drawing.Point(59, 98);
			this._applyImageBoxes.Name = "_applyImageBoxes";
			this._applyImageBoxes.Size = new System.Drawing.Size(75, 23);
			this._applyImageBoxes.TabIndex = 6;
			this._applyImageBoxes.Text = "Apply";
			this._applyImageBoxes.Click += new System.EventHandler(this._applyImageBoxes_Click);
			// 
			// imageBoxPanel
			// 
			this.imageBoxPanel.Controls.Add(this.groupBox1);
			this.imageBoxPanel.Location = new System.Drawing.Point(0, 0);
			this.imageBoxPanel.Name = "imageBoxPanel";
			this.imageBoxPanel.Size = new System.Drawing.Size(225, 155);
			this.imageBoxPanel.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this._imageBoxColumnsLabel);
			this.groupBox1.Controls.Add(this._imageBoxColumns);
			this.groupBox1.Controls.Add(this._imageBoxRows);
			this.groupBox1.Controls.Add(this._imageBoxRowsLabel);
			this.groupBox1.Controls.Add(this._applyImageBoxes);
			this.groupBox1.Location = new System.Drawing.Point(15, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(193, 152);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Image Box";
			// 
			// _imageBoxColumns
			// 
			this._imageBoxColumns.Font = new System.Drawing.Font("Arial", 9.75F);
			this._imageBoxColumns.Location = new System.Drawing.Point(115, 58);
			this._imageBoxColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._imageBoxColumns.Name = "_imageBoxColumns";
			this._imageBoxColumns.Size = new System.Drawing.Size(48, 22);
			this._imageBoxColumns.TabIndex = 5;
			this._imageBoxColumns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _imageBoxRows
			// 
			this._imageBoxRows.Font = new System.Drawing.Font("Arial", 9.75F);
			this._imageBoxRows.Location = new System.Drawing.Point(30, 58);
			this._imageBoxRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._imageBoxRows.Name = "_imageBoxRows";
			this._imageBoxRows.Size = new System.Drawing.Size(48, 22);
			this._imageBoxRows.TabIndex = 3;
			this._imageBoxRows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// tilePanel
			// 
			this.tilePanel.Controls.Add(this.groupBox2);
			this.tilePanel.Location = new System.Drawing.Point(0, 155);
			this.tilePanel.Name = "tilePanel";
			this.tilePanel.Size = new System.Drawing.Size(225, 155);
			this.tilePanel.TabIndex = 7;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this._applyTiles);
			this.groupBox2.Controls.Add(this._tileRowsLabel);
			this.groupBox2.Controls.Add(this._tileColumnsLabel);
			this.groupBox2.Controls.Add(this._tileRows);
			this.groupBox2.Controls.Add(this._tileColumns);
			this.groupBox2.Location = new System.Drawing.Point(15, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(193, 146);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Tile";
			// 
			// _tileRows
			// 
			this._tileRows.Font = new System.Drawing.Font("Arial", 9.75F);
			this._tileRows.Location = new System.Drawing.Point(30, 54);
			this._tileRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._tileRows.Name = "_tileRows";
			this._tileRows.Size = new System.Drawing.Size(48, 22);
			this._tileRows.TabIndex = 10;
			this._tileRows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _tileColumns
			// 
			this._tileColumns.Font = new System.Drawing.Font("Arial", 9.75F);
			this._tileColumns.Location = new System.Drawing.Point(115, 54);
			this._tileColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._tileColumns.Name = "_tileColumns";
			this._tileColumns.Size = new System.Drawing.Size(48, 22);
			this._tileColumns.TabIndex = 12;
			this._tileColumns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _buttonConfigure
			// 
			this._buttonConfigure.Location = new System.Drawing.Point(60, 316);
			this._buttonConfigure.Name = "_buttonConfigure";
			this._buttonConfigure.Size = new System.Drawing.Size(104, 23);
			this._buttonConfigure.TabIndex = 14;
			this._buttonConfigure.Text = "Change Defaults";
			this._buttonConfigure.UseVisualStyleBackColor = true;
			this._buttonConfigure.Click += new System.EventHandler(this.OnButtonConfigureClick);
			// 
			// LayoutControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._buttonConfigure);
			this.Controls.Add(this.tilePanel);
			this.Controls.Add(this.imageBoxPanel);
			this.Name = "LayoutControl";
			this.Size = new System.Drawing.Size(225, 359);
			this.imageBoxPanel.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._imageBoxColumns)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxRows)).EndInit();
			this.tilePanel.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._tileRows)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._tileColumns)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
