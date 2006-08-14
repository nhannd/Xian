using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	/// <summary>
	/// Summary description for LayoutControl.
	/// </summary>
	public class LayoutControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label _label3;
		private System.Windows.Forms.Label _label4;
		private System.Windows.Forms.NumericUpDown _tileColumns;
		private System.Windows.Forms.NumericUpDown _tileRows;
		private System.Windows.Forms.Label _label2;
		private System.Windows.Forms.Label _label1;
		private System.Windows.Forms.NumericUpDown _imageBoxColumns;
		private System.Windows.Forms.NumericUpDown _imageBoxRows;
		private Button _applyTiles;
		private Button _applyImageBoxes;
		private Panel _imageBoxPanel;
		private Panel _tilePanel;
		private ClearCanvas.Controls.WinForms.HeaderStrip _tileHeaderStrip;
		private ClearCanvas.Controls.WinForms.HeaderStrip _imageBoxHeaderStrip;
		private ToolStripLabel _imageBoxHeaderStripLabel;
		private ToolStripLabel _tileHeaderStripLabel;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container _components = null;

		public LayoutControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			this._imageBoxHeaderStripLabel.Text = "Image Box Layout";
			this._tileHeaderStripLabel.Text = "Tile Layout";
		}

        public int TileColumns
        {
            get { return (int)_tileColumns.Value; }
            set { _tileColumns.Value = value; }
        }

        public int TileRows
        {
            get { return (int)_tileRows.Value; }
            set { _tileRows.Value = value; }
        }

        public int ImageBoxColumns
        {
            get { return (int)_imageBoxColumns.Value; }
            set { _imageBoxColumns.Value = value; }
        }

        public int ImageBoxRows
        {
            get { return (int)_imageBoxRows.Value; }
            set { _imageBoxRows.Value = value; }
        }

        public Button ApplyTilesButton
        {
            get { return _applyTiles; }
        }

        public Button ApplyImageBoxesButton
        {
            get { return _applyImageBoxes; }
        }

        /// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(_components != null)
				{
					_components.Dispose();
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
			this._label3 = new System.Windows.Forms.Label();
			this._label4 = new System.Windows.Forms.Label();
			this._tileColumns = new System.Windows.Forms.NumericUpDown();
			this._tileRows = new System.Windows.Forms.NumericUpDown();
			this._label2 = new System.Windows.Forms.Label();
			this._label1 = new System.Windows.Forms.Label();
			this._imageBoxColumns = new System.Windows.Forms.NumericUpDown();
			this._imageBoxRows = new System.Windows.Forms.NumericUpDown();
			this._applyTiles = new System.Windows.Forms.Button();
			this._applyImageBoxes = new System.Windows.Forms.Button();
			this._imageBoxPanel = new System.Windows.Forms.Panel();
			this._imageBoxHeaderStrip = new ClearCanvas.Controls.WinForms.HeaderStrip();
			this._imageBoxHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
			this._tileHeaderStrip = new ClearCanvas.Controls.WinForms.HeaderStrip();
			this._tileHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
			this._tilePanel = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this._tileColumns)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._tileRows)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxColumns)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxRows)).BeginInit();
			this._imageBoxPanel.SuspendLayout();
			this._imageBoxHeaderStrip.SuspendLayout();
			this._tileHeaderStrip.SuspendLayout();
			this._tilePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// label3
			// 
			this._label3.Location = new System.Drawing.Point(112, 46);
			this._label3.Name = "label3";
			this._label3.Size = new System.Drawing.Size(64, 23);
			this._label3.TabIndex = 18;
			this._label3.Text = "Columns";
			// 
			// label4
			// 
			this._label4.Location = new System.Drawing.Point(27, 46);
			this._label4.Name = "label4";
			this._label4.Size = new System.Drawing.Size(48, 23);
			this._label4.TabIndex = 17;
			this._label4.Text = "Rows";
			// 
			// tileColumns
			// 
			this._tileColumns.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._tileColumns.Location = new System.Drawing.Point(115, 74);
			this._tileColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._tileColumns.Name = "tileColumns";
			this._tileColumns.Size = new System.Drawing.Size(48, 22);
			this._tileColumns.TabIndex = 16;
			this._tileColumns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// tileRows
			// 
			this._tileRows.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._tileRows.Location = new System.Drawing.Point(30, 72);
			this._tileRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._tileRows.Name = "tileRows";
			this._tileRows.Size = new System.Drawing.Size(48, 22);
			this._tileRows.TabIndex = 15;
			this._tileRows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label2
			// 
			this._label2.Location = new System.Drawing.Point(112, 48);
			this._label2.Name = "label2";
			this._label2.Size = new System.Drawing.Size(64, 23);
			this._label2.TabIndex = 13;
			this._label2.Text = "Columns";
			// 
			// label1
			// 
			this._label1.Location = new System.Drawing.Point(27, 46);
			this._label1.Name = "label1";
			this._label1.Size = new System.Drawing.Size(48, 23);
			this._label1.TabIndex = 12;
			this._label1.Text = "Rows";
			// 
			// imageBoxColumns
			// 
			this._imageBoxColumns.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._imageBoxColumns.Location = new System.Drawing.Point(115, 72);
			this._imageBoxColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._imageBoxColumns.Name = "imageBoxColumns";
			this._imageBoxColumns.Size = new System.Drawing.Size(48, 22);
			this._imageBoxColumns.TabIndex = 11;
			this._imageBoxColumns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// imageBoxRows
			// 
			this._imageBoxRows.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._imageBoxRows.Location = new System.Drawing.Point(30, 72);
			this._imageBoxRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._imageBoxRows.Name = "imageBoxRows";
			this._imageBoxRows.Size = new System.Drawing.Size(48, 22);
			this._imageBoxRows.TabIndex = 10;
			this._imageBoxRows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// applyTiles
			// 
			this._applyTiles.Location = new System.Drawing.Point(59, 113);
			this._applyTiles.Name = "applyTiles";
			this._applyTiles.Size = new System.Drawing.Size(75, 23);
			this._applyTiles.TabIndex = 11;
			this._applyTiles.Text = "Apply";
			// 
			// applyImageBoxes
			// 
			this._applyImageBoxes.Location = new System.Drawing.Point(59, 114);
			this._applyImageBoxes.Name = "applyImageBoxes";
			this._applyImageBoxes.Size = new System.Drawing.Size(75, 23);
			this._applyImageBoxes.TabIndex = 10;
			this._applyImageBoxes.Text = "Apply";
			// 
			// imageBoxPanel
			// 
			this._imageBoxPanel.Controls.Add(this._imageBoxHeaderStrip);
			this._imageBoxPanel.Controls.Add(this._label1);
			this._imageBoxPanel.Controls.Add(this._label2);
			this._imageBoxPanel.Controls.Add(this._imageBoxColumns);
			this._imageBoxPanel.Controls.Add(this._imageBoxRows);
			this._imageBoxPanel.Controls.Add(this._applyImageBoxes);
			this._imageBoxPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this._imageBoxPanel.Location = new System.Drawing.Point(0, 0);
			this._imageBoxPanel.Name = "imageBoxPanel";
			this._imageBoxPanel.Size = new System.Drawing.Size(193, 155);
			this._imageBoxPanel.TabIndex = 19;
			// 
			// imageBoxHeaderStrip
			// 
			this._imageBoxHeaderStrip.AutoSize = false;
			this._imageBoxHeaderStrip.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Bold);
			this._imageBoxHeaderStrip.ForeColor = System.Drawing.Color.White;
			this._imageBoxHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._imageBoxHeaderStrip.HeaderStyle = ClearCanvas.Controls.WinForms.AreaHeaderStyle.Small;
			this._imageBoxHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._imageBoxHeaderStripLabel});
			this._imageBoxHeaderStrip.Location = new System.Drawing.Point(0, 0);
			this._imageBoxHeaderStrip.Name = "imageBoxHeaderStrip";
			this._imageBoxHeaderStrip.Size = new System.Drawing.Size(193, 22);
			this._imageBoxHeaderStrip.TabIndex = 19;
			this._imageBoxHeaderStrip.Text = "Image Box";
			// 
			// imageBoxHeaderStripLabel
			// 
			this._imageBoxHeaderStripLabel.Name = "imageBoxHeaderStripLabel";
			this._imageBoxHeaderStripLabel.Size = new System.Drawing.Size(81, 19);
			this._imageBoxHeaderStripLabel.Text = "Image Box";
			// 
			// tileHeaderStrip
			// 
			this._tileHeaderStrip.AutoSize = false;
			this._tileHeaderStrip.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Bold);
			this._tileHeaderStrip.ForeColor = System.Drawing.Color.White;
			this._tileHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._tileHeaderStrip.HeaderStyle = ClearCanvas.Controls.WinForms.AreaHeaderStyle.Small;
			this._tileHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._tileHeaderStripLabel});
			this._tileHeaderStrip.Location = new System.Drawing.Point(0, 0);
			this._tileHeaderStrip.Name = "tileHeaderStrip";
			this._tileHeaderStrip.Size = new System.Drawing.Size(193, 22);
			this._tileHeaderStrip.TabIndex = 14;
			this._tileHeaderStrip.Text = "Tile Layout";
			// 
			// tileHeaderStripLabel
			// 
			this._tileHeaderStripLabel.Name = "tileHeaderStripLabel";
			this._tileHeaderStripLabel.Size = new System.Drawing.Size(33, 19);
			this._tileHeaderStripLabel.Text = "Tile";
			// 
			// tilePanel
			// 
			this._tilePanel.Controls.Add(this._tileHeaderStrip);
			this._tilePanel.Controls.Add(this._label3);
			this._tilePanel.Controls.Add(this._label4);
			this._tilePanel.Controls.Add(this._tileRows);
			this._tilePanel.Controls.Add(this._tileColumns);
			this._tilePanel.Controls.Add(this._applyTiles);
			this._tilePanel.Dock = System.Windows.Forms.DockStyle.Top;
			this._tilePanel.Location = new System.Drawing.Point(0, 155);
			this._tilePanel.Name = "tilePanel";
			this._tilePanel.Size = new System.Drawing.Size(193, 155);
			this._tilePanel.TabIndex = 20;
			// 
			// LayoutControl
			// 
			this.Controls.Add(this._tilePanel);
			this.Controls.Add(this._imageBoxPanel);
			this.Name = "LayoutControl";
			this.Size = new System.Drawing.Size(193, 315);
			((System.ComponentModel.ISupportInitialize)(this._tileColumns)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._tileRows)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxColumns)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxRows)).EndInit();
			this._imageBoxPanel.ResumeLayout(false);
			this._imageBoxHeaderStrip.ResumeLayout(false);
			this._imageBoxHeaderStrip.PerformLayout();
			this._tileHeaderStrip.ResumeLayout(false);
			this._tileHeaderStrip.PerformLayout();
			this._tilePanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
