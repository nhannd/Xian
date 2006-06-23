using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ClearCanvas.Workstation.Layout.Basic
{
	/// <summary>
	/// Summary description for LayoutControl.
	/// </summary>
	public class LayoutControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown tileColumns;
		private System.Windows.Forms.NumericUpDown tileRows;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown imageBoxColumns;
		private System.Windows.Forms.NumericUpDown imageBoxRows;
		private Button applyTiles;
		private Button applyImageBoxes;
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

		public LayoutControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
			this.imageBoxHeaderStripLabel.Text = "Image Box Layout";
			this.tileHeaderStripLabel.Text = "Tile Layout";
		}

        public int TileColumns
        {
            get { return (int)tileColumns.Value; }
            set { tileColumns.Value = value; }
        }

        public int TileRows
        {
            get { return (int)tileRows.Value; }
            set { tileRows.Value = value; }
        }

        public int ImageBoxColumns
        {
            get { return (int)imageBoxColumns.Value; }
            set { imageBoxColumns.Value = value; }
        }

        public int ImageBoxRows
        {
            get { return (int)imageBoxRows.Value; }
            set { imageBoxRows.Value = value; }
        }

        public Button ApplyTilesButton
        {
            get { return applyTiles; }
        }

        public Button ApplyImageBoxesButton
        {
            get { return applyImageBoxes; }
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
			this.tileColumns = new System.Windows.Forms.NumericUpDown();
			this.tileRows = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.imageBoxColumns = new System.Windows.Forms.NumericUpDown();
			this.imageBoxRows = new System.Windows.Forms.NumericUpDown();
			this.applyTiles = new System.Windows.Forms.Button();
			this.applyImageBoxes = new System.Windows.Forms.Button();
			this.imageBoxPanel = new System.Windows.Forms.Panel();
			this.imageBoxHeaderStrip = new ClearCanvas.Controls.WinForms.HeaderStrip();
			this.imageBoxHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
			this.tileHeaderStrip = new ClearCanvas.Controls.WinForms.HeaderStrip();
			this.tileHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
			this.tilePanel = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.tileColumns)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tileRows)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageBoxColumns)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageBoxRows)).BeginInit();
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
			// tileColumns
			// 
			this.tileColumns.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tileColumns.Location = new System.Drawing.Point(115, 74);
			this.tileColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.tileColumns.Name = "tileColumns";
			this.tileColumns.Size = new System.Drawing.Size(48, 22);
			this.tileColumns.TabIndex = 16;
			this.tileColumns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// tileRows
			// 
			this.tileRows.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tileRows.Location = new System.Drawing.Point(30, 72);
			this.tileRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.tileRows.Name = "tileRows";
			this.tileRows.Size = new System.Drawing.Size(48, 22);
			this.tileRows.TabIndex = 15;
			this.tileRows.Value = new decimal(new int[] {
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
			// imageBoxColumns
			// 
			this.imageBoxColumns.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.imageBoxColumns.Location = new System.Drawing.Point(115, 72);
			this.imageBoxColumns.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.imageBoxColumns.Name = "imageBoxColumns";
			this.imageBoxColumns.Size = new System.Drawing.Size(48, 22);
			this.imageBoxColumns.TabIndex = 11;
			this.imageBoxColumns.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// imageBoxRows
			// 
			this.imageBoxRows.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.imageBoxRows.Location = new System.Drawing.Point(30, 72);
			this.imageBoxRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.imageBoxRows.Name = "imageBoxRows";
			this.imageBoxRows.Size = new System.Drawing.Size(48, 22);
			this.imageBoxRows.TabIndex = 10;
			this.imageBoxRows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// applyTiles
			// 
			this.applyTiles.Location = new System.Drawing.Point(59, 113);
			this.applyTiles.Name = "applyTiles";
			this.applyTiles.Size = new System.Drawing.Size(75, 23);
			this.applyTiles.TabIndex = 11;
			this.applyTiles.Text = "Apply";
			// 
			// applyImageBoxes
			// 
			this.applyImageBoxes.Location = new System.Drawing.Point(59, 114);
			this.applyImageBoxes.Name = "applyImageBoxes";
			this.applyImageBoxes.Size = new System.Drawing.Size(75, 23);
			this.applyImageBoxes.TabIndex = 10;
			this.applyImageBoxes.Text = "Apply";
			// 
			// imageBoxPanel
			// 
			this.imageBoxPanel.Controls.Add(this.imageBoxHeaderStrip);
			this.imageBoxPanel.Controls.Add(this.label1);
			this.imageBoxPanel.Controls.Add(this.label2);
			this.imageBoxPanel.Controls.Add(this.imageBoxColumns);
			this.imageBoxPanel.Controls.Add(this.imageBoxRows);
			this.imageBoxPanel.Controls.Add(this.applyImageBoxes);
			this.imageBoxPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.imageBoxPanel.Location = new System.Drawing.Point(0, 0);
			this.imageBoxPanel.Name = "imageBoxPanel";
			this.imageBoxPanel.Size = new System.Drawing.Size(193, 155);
			this.imageBoxPanel.TabIndex = 19;
			// 
			// imageBoxHeaderStrip
			// 
			this.imageBoxHeaderStrip.AutoSize = false;
			this.imageBoxHeaderStrip.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Bold);
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
			this.imageBoxHeaderStripLabel.Size = new System.Drawing.Size(81, 19);
			this.imageBoxHeaderStripLabel.Text = "Image Box";
			// 
			// tileHeaderStrip
			// 
			this.tileHeaderStrip.AutoSize = false;
			this.tileHeaderStrip.Font = new System.Drawing.Font("Arial", 10.5F, System.Drawing.FontStyle.Bold);
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
			this.tileHeaderStripLabel.Size = new System.Drawing.Size(33, 19);
			this.tileHeaderStripLabel.Text = "Tile";
			// 
			// tilePanel
			// 
			this.tilePanel.Controls.Add(this.tileHeaderStrip);
			this.tilePanel.Controls.Add(this.label3);
			this.tilePanel.Controls.Add(this.label4);
			this.tilePanel.Controls.Add(this.tileRows);
			this.tilePanel.Controls.Add(this.tileColumns);
			this.tilePanel.Controls.Add(this.applyTiles);
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
			((System.ComponentModel.ISupportInitialize)(this.tileColumns)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tileRows)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageBoxColumns)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageBoxRows)).EndInit();
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
