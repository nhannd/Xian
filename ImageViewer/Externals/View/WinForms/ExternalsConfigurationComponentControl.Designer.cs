namespace ClearCanvas.ImageViewer.Externals.View.WinForms
{
	partial class ExternalsConfigurationComponentControl {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.Windows.Forms.Panel _pnlContent;
			System.Windows.Forms.FlowLayoutPanel _pnlButtons;
			this._listExternals = new System.Windows.Forms.ListView();
			this.colLabel = new System.Windows.Forms.ColumnHeader();
			this.colDescription = new System.Windows.Forms.ColumnHeader();
			this._btnAdd = new System.Windows.Forms.Button();
			this._btnRemove = new System.Windows.Forms.Button();
			this._btnEdit = new System.Windows.Forms.Button();
			_pnlContent = new System.Windows.Forms.Panel();
			_pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
			_pnlContent.SuspendLayout();
			_pnlButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// _pnlContent
			// 
			_pnlContent.Controls.Add(this._listExternals);
			_pnlContent.Controls.Add(_pnlButtons);
			_pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			_pnlContent.Location = new System.Drawing.Point(0, 0);
			_pnlContent.Name = "_pnlContent";
			_pnlContent.Size = new System.Drawing.Size(464, 274);
			_pnlContent.TabIndex = 3;
			// 
			// _listExternals
			// 
			this._listExternals.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colLabel,
            this.colDescription});
			this._listExternals.Dock = System.Windows.Forms.DockStyle.Fill;
			this._listExternals.FullRowSelect = true;
			this._listExternals.GridLines = true;
			this._listExternals.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this._listExternals.LabelEdit = true;
			this._listExternals.Location = new System.Drawing.Point(0, 0);
			this._listExternals.MultiSelect = false;
			this._listExternals.Name = "_listExternals";
			this._listExternals.Size = new System.Drawing.Size(464, 232);
			this._listExternals.TabIndex = 0;
			this._listExternals.UseCompatibleStateImageBehavior = false;
			this._listExternals.View = System.Windows.Forms.View.Details;
			this._listExternals.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this._listExternals_AfterLabelEdit);
			this._listExternals.SelectedIndexChanged += new System.EventHandler(this._listExternals_SelectedIndexChanged);
			this._listExternals.DoubleClick += new System.EventHandler(this._listExternals_DoubleClick);
			// 
			// colLabel
			// 
			this.colLabel.Text = "External";
			this.colLabel.Width = 351;
			// 
			// colDescription
			// 
			this.colDescription.Text = "Description";
			this.colDescription.Width = 271;
			// 
			// _pnlButtons
			// 
			_pnlButtons.Controls.Add(this._btnAdd);
			_pnlButtons.Controls.Add(this._btnRemove);
			_pnlButtons.Controls.Add(this._btnEdit);
			_pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			_pnlButtons.Location = new System.Drawing.Point(0, 232);
			_pnlButtons.Name = "_pnlButtons";
			_pnlButtons.Padding = new System.Windows.Forms.Padding(3);
			_pnlButtons.Size = new System.Drawing.Size(464, 42);
			_pnlButtons.TabIndex = 3;
			// 
			// _btnAdd
			// 
			this._btnAdd.Location = new System.Drawing.Point(6, 6);
			this._btnAdd.Name = "_btnAdd";
			this._btnAdd.Size = new System.Drawing.Size(132, 29);
			this._btnAdd.TabIndex = 1;
			this._btnAdd.Text = "Add New External";
			this._btnAdd.UseVisualStyleBackColor = true;
			this._btnAdd.Click += new System.EventHandler(this._btnAdd_Click);
			// 
			// _btnRemove
			// 
			this._btnRemove.Location = new System.Drawing.Point(144, 6);
			this._btnRemove.Name = "_btnRemove";
			this._btnRemove.Size = new System.Drawing.Size(132, 29);
			this._btnRemove.TabIndex = 2;
			this._btnRemove.Text = "Remove External";
			this._btnRemove.UseVisualStyleBackColor = true;
			this._btnRemove.Click += new System.EventHandler(this._btnRemove_Click);
			// 
			// _btnEdit
			// 
			this._btnEdit.Enabled = false;
			this._btnEdit.Location = new System.Drawing.Point(282, 6);
			this._btnEdit.Name = "_btnEdit";
			this._btnEdit.Size = new System.Drawing.Size(132, 29);
			this._btnEdit.TabIndex = 3;
			this._btnEdit.Text = "Edit External";
			this._btnEdit.UseVisualStyleBackColor = true;
			this._btnEdit.Click += new System.EventHandler(this._btnEdit_Click);
			// 
			// ExternalsConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(_pnlContent);
			this.Name = "ExternalsConfigurationComponentControl";
			this.Size = new System.Drawing.Size(464, 274);
			_pnlContent.ResumeLayout(false);
			_pnlButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView _listExternals;
		private System.Windows.Forms.ColumnHeader colLabel;
		private System.Windows.Forms.Button _btnAdd;
		private System.Windows.Forms.Button _btnRemove;
		private System.Windows.Forms.ColumnHeader colDescription;
		private System.Windows.Forms.Button _btnEdit;
	}
}