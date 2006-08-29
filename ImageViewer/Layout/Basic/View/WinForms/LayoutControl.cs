using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	/// <summary>
    /// Provides the user-interface for <see cref="LayoutComponentView"/>
	/// </summary>
	public class LayoutControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown _tileColumns;
		private System.Windows.Forms.NumericUpDown _tileRows;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown _imageBoxColumns;
		private System.Windows.Forms.NumericUpDown _imageBoxRows;
		private Button _applyTiles;
		private Button _applyImageBoxes;
		private Panel imageBoxPanel;
		private Panel tilePanel;
		private ClearCanvas.Controls.WinForms.HeaderStrip tileHeaderStrip;
		private ClearCanvas.Controls.WinForms.HeaderStrip imageBoxHeaderStrip;
		private ToolStripLabel imageBoxHeaderStripLabel;
		private ToolStripLabel tileHeaderStripLabel;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


        private LayoutComponent _layoutComponent;
        private BindingSource _bindingSource;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="component">The component to look at</param>
		public LayoutControl(LayoutComponent component)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			this.imageBoxHeaderStripLabel.Text = "Image Box Layout";
			this.tileHeaderStripLabel.Text = "Tile Layout";

            _layoutComponent = component;

            // rather than binding directly to the component, create a binding source
            // this is the only way that we can pull data from the component on demand
            _bindingSource = new BindingSource();
            _bindingSource.DataSource = _layoutComponent;

            // bind control values
            _tileColumns.DataBindings.Add("Value", _bindingSource, "TileColumns", true, DataSourceUpdateMode.OnPropertyChanged);
            _tileRows.DataBindings.Add("Value", _bindingSource, "TileRows", true, DataSourceUpdateMode.OnPropertyChanged);
            _imageBoxColumns.DataBindings.Add("Value", _bindingSource, "ImageBoxColumns", true, DataSourceUpdateMode.OnPropertyChanged);
            _imageBoxRows.DataBindings.Add("Value", _bindingSource, "ImageBoxRows", true, DataSourceUpdateMode.OnPropertyChanged);

            // bind control enablement
            _imageBoxColumns.DataBindings.Add("Enabled", _bindingSource, "ImageBoxSectionEnabled");
            _imageBoxRows.DataBindings.Add("Enabled", _bindingSource, "ImageBoxSectionEnabled");
            _applyImageBoxes.DataBindings.Add("Enabled", _bindingSource, "ImageBoxSectionEnabled");

            _tileColumns.DataBindings.Add("Enabled", _bindingSource, "TileSectionEnabled");
            _tileRows.DataBindings.Add("Enabled", _bindingSource, "TileSectionEnabled");
            _applyTiles.DataBindings.Add("Enabled", _bindingSource, "TileSectionEnabled");


            // listen for changes to the layout subject
            _layoutComponent.LayoutSubjectChanged += new EventHandler(LayoutSubjectChangedEventHandler);
     
        }

        /// <summary>
        /// Event handler for the <see cref="LayoutComponent.LayoutSubjectChanged"/> event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutSubjectChangedEventHandler(object sender, EventArgs e)
        {
            // the subject changed, so all the data needs to be refreshed from the component
            _bindingSource.ResetBindings(false);
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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this._tileColumns = new System.Windows.Forms.NumericUpDown();
            this._tileRows = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._imageBoxColumns = new System.Windows.Forms.NumericUpDown();
            this._imageBoxRows = new System.Windows.Forms.NumericUpDown();
            this._applyTiles = new System.Windows.Forms.Button();
            this._applyImageBoxes = new System.Windows.Forms.Button();
            this.imageBoxPanel = new System.Windows.Forms.Panel();
            this.imageBoxHeaderStrip = new ClearCanvas.Controls.WinForms.HeaderStrip();
            this.imageBoxHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.tileHeaderStrip = new ClearCanvas.Controls.WinForms.HeaderStrip();
            this.tileHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.tilePanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this._tileColumns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._tileRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._imageBoxColumns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._imageBoxRows)).BeginInit();
            this.imageBoxPanel.SuspendLayout();
            this.imageBoxHeaderStrip.SuspendLayout();
            this.tileHeaderStrip.SuspendLayout();
            this.tilePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(112, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 23);
            this.label3.TabIndex = 18;
            this.label3.Text = "Columns";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(27, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 23);
            this.label4.TabIndex = 17;
            this.label4.Text = "Rows";
            // 
            // _tileColumns
            // 
            this._tileColumns.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._tileColumns.Location = new System.Drawing.Point(115, 74);
            this._tileColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._tileColumns.Name = "_tileColumns";
            this._tileColumns.Size = new System.Drawing.Size(48, 26);
            this._tileColumns.TabIndex = 16;
            this._tileColumns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // _tileRows
            // 
            this._tileRows.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._tileRows.Location = new System.Drawing.Point(30, 72);
            this._tileRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._tileRows.Name = "_tileRows";
            this._tileRows.Size = new System.Drawing.Size(48, 26);
            this._tileRows.TabIndex = 15;
            this._tileRows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(112, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 23);
            this.label2.TabIndex = 13;
            this.label2.Text = "Columns";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(27, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 23);
            this.label1.TabIndex = 12;
            this.label1.Text = "Rows";
            // 
            // _imageBoxColumns
            // 
            this._imageBoxColumns.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._imageBoxColumns.Location = new System.Drawing.Point(115, 72);
            this._imageBoxColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._imageBoxColumns.Name = "_imageBoxColumns";
            this._imageBoxColumns.Size = new System.Drawing.Size(48, 26);
            this._imageBoxColumns.TabIndex = 11;
            this._imageBoxColumns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // _imageBoxRows
            // 
            this._imageBoxRows.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._imageBoxRows.Location = new System.Drawing.Point(30, 72);
            this._imageBoxRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._imageBoxRows.Name = "_imageBoxRows";
            this._imageBoxRows.Size = new System.Drawing.Size(48, 26);
            this._imageBoxRows.TabIndex = 10;
            this._imageBoxRows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // _applyTiles
            // 
            this._applyTiles.Location = new System.Drawing.Point(59, 113);
            this._applyTiles.Name = "_applyTiles";
            this._applyTiles.Size = new System.Drawing.Size(75, 23);
            this._applyTiles.TabIndex = 11;
            this._applyTiles.Text = "Apply";
            this._applyTiles.Click += new System.EventHandler(this._applyTiles_Click);
            // 
            // _applyImageBoxes
            // 
            this._applyImageBoxes.Location = new System.Drawing.Point(59, 114);
            this._applyImageBoxes.Name = "_applyImageBoxes";
            this._applyImageBoxes.Size = new System.Drawing.Size(75, 23);
            this._applyImageBoxes.TabIndex = 10;
            this._applyImageBoxes.Text = "Apply";
            this._applyImageBoxes.Click += new System.EventHandler(this._applyImageBoxes_Click);
            // 
            // imageBoxPanel
            // 
            this.imageBoxPanel.Controls.Add(this.imageBoxHeaderStrip);
            this.imageBoxPanel.Controls.Add(this.label1);
            this.imageBoxPanel.Controls.Add(this.label2);
            this.imageBoxPanel.Controls.Add(this._imageBoxColumns);
            this.imageBoxPanel.Controls.Add(this._imageBoxRows);
            this.imageBoxPanel.Controls.Add(this._applyImageBoxes);
            this.imageBoxPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.imageBoxPanel.Location = new System.Drawing.Point(0, 0);
            this.imageBoxPanel.Name = "imageBoxPanel";
            this.imageBoxPanel.Size = new System.Drawing.Size(193, 155);
            this.imageBoxPanel.TabIndex = 19;
            // 
            // imageBoxHeaderStrip
            // 
            this.imageBoxHeaderStrip.AutoSize = false;
            this.imageBoxHeaderStrip.Font = new System.Drawing.Font("Arial", 9.150001F, System.Drawing.FontStyle.Bold);
            this.imageBoxHeaderStrip.ForeColor = System.Drawing.Color.White;
            this.imageBoxHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.imageBoxHeaderStrip.HeaderStyle = ClearCanvas.Controls.WinForms.AreaHeaderStyle.Small;
            this.imageBoxHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imageBoxHeaderStripLabel});
            this.imageBoxHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.imageBoxHeaderStrip.Name = "imageBoxHeaderStrip";
            this.imageBoxHeaderStrip.Size = new System.Drawing.Size(193, 22);
            this.imageBoxHeaderStrip.TabIndex = 19;
            this.imageBoxHeaderStrip.Text = "Image Box";
            // 
            // imageBoxHeaderStripLabel
            // 
            this.imageBoxHeaderStripLabel.Name = "imageBoxHeaderStripLabel";
            this.imageBoxHeaderStripLabel.Size = new System.Drawing.Size(90, 19);
            this.imageBoxHeaderStripLabel.Text = "Image Box";
            // 
            // tileHeaderStrip
            // 
            this.tileHeaderStrip.AutoSize = false;
            this.tileHeaderStrip.Font = new System.Drawing.Font("Arial", 9.150001F, System.Drawing.FontStyle.Bold);
            this.tileHeaderStrip.ForeColor = System.Drawing.Color.White;
            this.tileHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tileHeaderStrip.HeaderStyle = ClearCanvas.Controls.WinForms.AreaHeaderStyle.Small;
            this.tileHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tileHeaderStripLabel});
            this.tileHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.tileHeaderStrip.Name = "tileHeaderStrip";
            this.tileHeaderStrip.Size = new System.Drawing.Size(193, 22);
            this.tileHeaderStrip.TabIndex = 14;
            this.tileHeaderStrip.Text = "Tile Layout";
            // 
            // tileHeaderStripLabel
            // 
            this.tileHeaderStripLabel.Name = "tileHeaderStripLabel";
            this.tileHeaderStripLabel.Size = new System.Drawing.Size(36, 19);
            this.tileHeaderStripLabel.Text = "Tile";
            // 
            // tilePanel
            // 
            this.tilePanel.Controls.Add(this.tileHeaderStrip);
            this.tilePanel.Controls.Add(this.label3);
            this.tilePanel.Controls.Add(this.label4);
            this.tilePanel.Controls.Add(this._tileRows);
            this.tilePanel.Controls.Add(this._tileColumns);
            this.tilePanel.Controls.Add(this._applyTiles);
            this.tilePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.tilePanel.Location = new System.Drawing.Point(0, 155);
            this.tilePanel.Name = "tilePanel";
            this.tilePanel.Size = new System.Drawing.Size(193, 155);
            this.tilePanel.TabIndex = 20;
            // 
            // LayoutControl
            // 
            this.Controls.Add(this.tilePanel);
            this.Controls.Add(this.imageBoxPanel);
            this.Name = "LayoutControl";
            this.Size = new System.Drawing.Size(193, 315);
            ((System.ComponentModel.ISupportInitialize)(this._tileColumns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._tileRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._imageBoxColumns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._imageBoxRows)).EndInit();
            this.imageBoxPanel.ResumeLayout(false);
            this.imageBoxHeaderStrip.ResumeLayout(false);
            this.imageBoxHeaderStrip.PerformLayout();
            this.tileHeaderStrip.ResumeLayout(false);
            this.tileHeaderStrip.PerformLayout();
            this.tilePanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

	}
}
