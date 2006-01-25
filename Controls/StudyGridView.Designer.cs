namespace ClearCanvas.Controls
{
	partial class StudyGridView
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this._dataGridView = new System.Windows.Forms.DataGridView();
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this._dataGridView)).BeginInit();
			this._tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _dataGridView
			// 
			this._dataGridView.AllowUserToAddRows = false;
			this._dataGridView.AllowUserToDeleteRows = false;
			this._dataGridView.AllowUserToResizeRows = false;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.HotTrack;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			this._dataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
			this._dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
			this._dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this._dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.HotTrack;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this._dataGridView.DefaultCellStyle = dataGridViewCellStyle2;
			this._dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._dataGridView.Location = new System.Drawing.Point(0, 25);
			this._dataGridView.Margin = new System.Windows.Forms.Padding(0);
			this._dataGridView.Name = "_dataGridView";
			this._dataGridView.ReadOnly = true;
			this._dataGridView.RowHeadersVisible = false;
			this._dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this._dataGridView.Size = new System.Drawing.Size(540, 203);
			this._dataGridView.TabIndex = 0;
			// 
			// _toolStrip
			// 
			this._toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
			this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._toolStrip.Location = new System.Drawing.Point(0, 0);
			this._toolStrip.Name = "_toolStrip";
			this._toolStrip.Size = new System.Drawing.Size(540, 25);
			this._toolStrip.TabIndex = 1;
			this._toolStrip.Text = "toolStrip1";
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.AutoScroll = true;
			this._tableLayoutPanel.ColumnCount = 1;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel.Controls.Add(this._toolStrip, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._dataGridView, 0, 1);
			this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 2;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel.Size = new System.Drawing.Size(540, 228);
			this._tableLayoutPanel.TabIndex = 2;
			// 
			// StudyGridView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._tableLayoutPanel);
			this.Name = "StudyGridView";
			this.Size = new System.Drawing.Size(540, 228);
			((System.ComponentModel.ISupportInitialize)(this._dataGridView)).EndInit();
			this._tableLayoutPanel.ResumeLayout(false);
			this._tableLayoutPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView _dataGridView;
		private System.Windows.Forms.ToolStrip _toolStrip;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
	}
}
