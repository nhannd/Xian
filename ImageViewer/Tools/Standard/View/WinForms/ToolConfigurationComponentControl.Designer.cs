namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	partial class ToolConfigurationComponentControl
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolConfigurationComponentControl));
			this._grpToolModalityBehavior = new System.Windows.Forms.GroupBox();
			this._cboModality = new System.Windows.Forms.ComboBox();
			this._lblModality = new System.Windows.Forms.Label();
			this._lyoTable = new System.Windows.Forms.TableLayoutPanel();
			this._lblWindowLevel = new System.Windows.Forms.Label();
			this._lblReset = new System.Windows.Forms.Label();
			this._chkWindowLevel = new System.Windows.Forms.CheckBox();
			this._chkOrientation = new System.Windows.Forms.CheckBox();
			this._chkZoom = new System.Windows.Forms.CheckBox();
			this._chkPan = new System.Windows.Forms.CheckBox();
			this._chkReset = new System.Windows.Forms.CheckBox();
			this._lblSelectedImage = new System.Windows.Forms.Label();
			this._lblOrientation = new System.Windows.Forms.Label();
			this._lblPan = new System.Windows.Forms.Label();
			this._lblZoom = new System.Windows.Forms.Label();
			this._chkInvertZoomDirection = new System.Windows.Forms.CheckBox();
			this._grpGeneral = new System.Windows.Forms.GroupBox();
			this._tooltipProvider = new System.Windows.Forms.ToolTip(this.components);
			this._grpToolModalityBehavior.SuspendLayout();
			this._lyoTable.SuspendLayout();
			this._grpGeneral.SuspendLayout();
			this.SuspendLayout();
			// 
			// _grpToolModalityBehavior
			// 
			this._grpToolModalityBehavior.Controls.Add(this._cboModality);
			this._grpToolModalityBehavior.Controls.Add(this._lblModality);
			this._grpToolModalityBehavior.Controls.Add(this._lyoTable);
			resources.ApplyResources(this._grpToolModalityBehavior, "_grpToolModalityBehavior");
			this._grpToolModalityBehavior.Name = "_grpToolModalityBehavior";
			this._grpToolModalityBehavior.TabStop = false;
			// 
			// _cboModality
			// 
			this._cboModality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cboModality.FormattingEnabled = true;
			resources.ApplyResources(this._cboModality, "_cboModality");
			this._cboModality.Name = "_cboModality";
			// 
			// _lblModality
			// 
			resources.ApplyResources(this._lblModality, "_lblModality");
			this._lblModality.Name = "_lblModality";
			// 
			// _lyoTable
			// 
			resources.ApplyResources(this._lyoTable, "_lyoTable");
			this._lyoTable.Controls.Add(this._lblWindowLevel, 0, 1);
			this._lyoTable.Controls.Add(this._lblReset, 0, 5);
			this._lyoTable.Controls.Add(this._chkWindowLevel, 1, 1);
			this._lyoTable.Controls.Add(this._chkOrientation, 1, 2);
			this._lyoTable.Controls.Add(this._chkZoom, 1, 3);
			this._lyoTable.Controls.Add(this._chkPan, 1, 4);
			this._lyoTable.Controls.Add(this._chkReset, 1, 5);
			this._lyoTable.Controls.Add(this._lblSelectedImage, 1, 0);
			this._lyoTable.Controls.Add(this._lblOrientation, 0, 2);
			this._lyoTable.Controls.Add(this._lblPan, 0, 4);
			this._lyoTable.Controls.Add(this._lblZoom, 0, 3);
			this._lyoTable.Name = "_lyoTable";
			// 
			// _lblWindowLevel
			// 
			resources.ApplyResources(this._lblWindowLevel, "_lblWindowLevel");
			this._lblWindowLevel.Name = "_lblWindowLevel";
			// 
			// _lblReset
			// 
			resources.ApplyResources(this._lblReset, "_lblReset");
			this._lblReset.Name = "_lblReset";
			// 
			// _chkWindowLevel
			// 
			resources.ApplyResources(this._chkWindowLevel, "_chkWindowLevel");
			this._chkWindowLevel.Name = "_chkWindowLevel";
			this._tooltipProvider.SetToolTip(this._chkWindowLevel, resources.GetString("_chkWindowLevel.ToolTip"));
			this._chkWindowLevel.UseVisualStyleBackColor = true;
			// 
			// _chkOrientation
			// 
			resources.ApplyResources(this._chkOrientation, "_chkOrientation");
			this._chkOrientation.Name = "_chkOrientation";
			this._tooltipProvider.SetToolTip(this._chkOrientation, resources.GetString("_chkOrientation.ToolTip"));
			this._chkOrientation.UseVisualStyleBackColor = true;
			// 
			// _chkZoom
			// 
			resources.ApplyResources(this._chkZoom, "_chkZoom");
			this._chkZoom.Name = "_chkZoom";
			this._tooltipProvider.SetToolTip(this._chkZoom, resources.GetString("_chkZoom.ToolTip"));
			this._chkZoom.UseVisualStyleBackColor = true;
			// 
			// _chkPan
			// 
			resources.ApplyResources(this._chkPan, "_chkPan");
			this._chkPan.Name = "_chkPan";
			this._tooltipProvider.SetToolTip(this._chkPan, resources.GetString("_chkPan.ToolTip"));
			this._chkPan.UseVisualStyleBackColor = true;
			// 
			// _chkReset
			// 
			resources.ApplyResources(this._chkReset, "_chkReset");
			this._chkReset.Name = "_chkReset";
			this._tooltipProvider.SetToolTip(this._chkReset, resources.GetString("_chkReset.ToolTip"));
			this._chkReset.UseVisualStyleBackColor = true;
			// 
			// _lblSelectedImage
			// 
			resources.ApplyResources(this._lblSelectedImage, "_lblSelectedImage");
			this._lblSelectedImage.Name = "_lblSelectedImage";
			// 
			// _lblOrientation
			// 
			resources.ApplyResources(this._lblOrientation, "_lblOrientation");
			this._lblOrientation.Name = "_lblOrientation";
			// 
			// _lblPan
			// 
			resources.ApplyResources(this._lblPan, "_lblPan");
			this._lblPan.Name = "_lblPan";
			// 
			// _lblZoom
			// 
			resources.ApplyResources(this._lblZoom, "_lblZoom");
			this._lblZoom.Name = "_lblZoom";
			// 
			// _chkInvertZoomDirection
			// 
			resources.ApplyResources(this._chkInvertZoomDirection, "_chkInvertZoomDirection");
			this._chkInvertZoomDirection.Name = "_chkInvertZoomDirection";
			this._tooltipProvider.SetToolTip(this._chkInvertZoomDirection, resources.GetString("_chkInvertZoomDirection.ToolTip"));
			this._chkInvertZoomDirection.UseVisualStyleBackColor = true;
			// 
			// _grpGeneral
			// 
			this._grpGeneral.Controls.Add(this._chkInvertZoomDirection);
			resources.ApplyResources(this._grpGeneral, "_grpGeneral");
			this._grpGeneral.Name = "_grpGeneral";
			this._grpGeneral.TabStop = false;
			// 
			// ToolConfigurationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._grpGeneral);
			this.Controls.Add(this._grpToolModalityBehavior);
			this.Name = "ToolConfigurationComponentControl";
			this._grpToolModalityBehavior.ResumeLayout(false);
			this._grpToolModalityBehavior.PerformLayout();
			this._lyoTable.ResumeLayout(false);
			this._lyoTable.PerformLayout();
			this._grpGeneral.ResumeLayout(false);
			this._grpGeneral.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox _grpToolModalityBehavior;
		private System.Windows.Forms.TableLayoutPanel _lyoTable;
		private System.Windows.Forms.Label _lblWindowLevel;
		private System.Windows.Forms.Label _lblReset;
		private System.Windows.Forms.CheckBox _chkWindowLevel;
		private System.Windows.Forms.CheckBox _chkOrientation;
		private System.Windows.Forms.CheckBox _chkZoom;
		private System.Windows.Forms.CheckBox _chkPan;
		private System.Windows.Forms.CheckBox _chkReset;
		private System.Windows.Forms.Label _lblSelectedImage;
		private System.Windows.Forms.Label _lblOrientation;
		private System.Windows.Forms.Label _lblPan;
		private System.Windows.Forms.Label _lblZoom;
		private System.Windows.Forms.ComboBox _cboModality;
		private System.Windows.Forms.CheckBox _chkInvertZoomDirection;
		private System.Windows.Forms.GroupBox _grpGeneral;
		private System.Windows.Forms.Label _lblModality;
		private System.Windows.Forms.ToolTip _tooltipProvider;
	}
}
