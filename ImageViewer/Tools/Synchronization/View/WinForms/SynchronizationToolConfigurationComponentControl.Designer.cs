namespace ClearCanvas.ImageViewer.Tools.Synchronization.View.WinForms {
	partial class SynchronizationToolConfigurationComponentControl {
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.FlowLayoutPanel _flowSynchroTools;
			System.Windows.Forms.Label _lblToleranceAngle;
			System.Windows.Forms.Label _lblToleranceUnits;
			this._pnlToleranceAngleControl = new System.Windows.Forms.Panel();
			this._txtToleranceAngle = new System.Windows.Forms.TextBox();
			this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			_flowSynchroTools = new System.Windows.Forms.FlowLayoutPanel();
			_lblToleranceAngle = new System.Windows.Forms.Label();
			_lblToleranceUnits = new System.Windows.Forms.Label();
			_flowSynchroTools.SuspendLayout();
			this._pnlToleranceAngleControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// _flowSynchroTools
			// 
			_flowSynchroTools.Controls.Add(_lblToleranceAngle);
			_flowSynchroTools.Controls.Add(this._pnlToleranceAngleControl);
			_flowSynchroTools.Dock = System.Windows.Forms.DockStyle.Fill;
			_flowSynchroTools.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			_flowSynchroTools.Location = new System.Drawing.Point(0, 0);
			_flowSynchroTools.Name = "_flowSynchroTools";
			_flowSynchroTools.Padding = new System.Windows.Forms.Padding(15);
			_flowSynchroTools.Size = new System.Drawing.Size(449, 250);
			_flowSynchroTools.TabIndex = 2;
			// 
			// _lblToleranceAngle
			// 
			_lblToleranceAngle.AutoSize = true;
			_lblToleranceAngle.Location = new System.Drawing.Point(18, 15);
			_lblToleranceAngle.Name = "_lblToleranceAngle";
			_lblToleranceAngle.Size = new System.Drawing.Size(254, 13);
			_lblToleranceAngle.TabIndex = 0;
			_lblToleranceAngle.Text = "Tolerance angle for planes to be considered parallel:";
			// 
			// _pnlToleranceAngleControl
			// 
			this._pnlToleranceAngleControl.Controls.Add(this._txtToleranceAngle);
			this._pnlToleranceAngleControl.Controls.Add(_lblToleranceUnits);
			this._pnlToleranceAngleControl.Location = new System.Drawing.Point(18, 31);
			this._pnlToleranceAngleControl.Name = "_pnlToleranceAngleControl";
			this._pnlToleranceAngleControl.Size = new System.Drawing.Size(127, 25);
			this._pnlToleranceAngleControl.TabIndex = 2;
			// 
			// _txtToleranceAngle
			// 
			this._txtToleranceAngle.Dock = System.Windows.Forms.DockStyle.Fill;
			this._txtToleranceAngle.Location = new System.Drawing.Point(0, 0);
			this._txtToleranceAngle.MaxLength = 16;
			this._txtToleranceAngle.Name = "_txtToleranceAngle";
			this._txtToleranceAngle.Size = new System.Drawing.Size(74, 20);
			this._txtToleranceAngle.TabIndex = 1;
			// 
			// _lblToleranceUnits
			// 
			_lblToleranceUnits.Dock = System.Windows.Forms.DockStyle.Right;
			_lblToleranceUnits.Location = new System.Drawing.Point(74, 0);
			_lblToleranceUnits.Name = "_lblToleranceUnits";
			_lblToleranceUnits.Size = new System.Drawing.Size(53, 25);
			_lblToleranceUnits.TabIndex = 2;
			_lblToleranceUnits.Text = "degrees";
			_lblToleranceUnits.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _errorProvider
			// 
			this._errorProvider.ContainerControl = this;
			// 
			// SynchronizationToolConfigComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(_flowSynchroTools);
			this.Name = "SynchronizationToolConfigComponentControl";
			this.Size = new System.Drawing.Size(449, 250);
			_flowSynchroTools.ResumeLayout(false);
			_flowSynchroTools.PerformLayout();
			this._pnlToleranceAngleControl.ResumeLayout(false);
			this._pnlToleranceAngleControl.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox _txtToleranceAngle;
		private System.Windows.Forms.ErrorProvider _errorProvider;
		private System.Windows.Forms.Panel _pnlToleranceAngleControl;
	}
}
