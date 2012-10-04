namespace ClearCanvas.Utilities.ProductSettingsGenerator
{
	partial class GeneratorForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneratorForm));
			this._btnGenerateXml = new System.Windows.Forms.Button();
			this._versionSuffix = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this._license = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this._version = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this._component = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this._copyright = new System.Windows.Forms.TextBox();
			this._btnGenerateConfig = new System.Windows.Forms.Button();
			this._product = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._btnReset = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this._edition = new System.Windows.Forms.TextBox();
			this._dlgSaveXml = new System.Windows.Forms.SaveFileDialog();
			this._dlgSaveConfig = new System.Windows.Forms.SaveFileDialog();
			this.label7 = new System.Windows.Forms.Label();
			this._release = new System.Windows.Forms.ComboBox();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _btnGenerateXml
			// 
			resources.ApplyResources(this._btnGenerateXml, "_btnGenerateXml");
			this._btnGenerateXml.Name = "_btnGenerateXml";
			this._btnGenerateXml.UseVisualStyleBackColor = true;
			this._btnGenerateXml.Click += new System.EventHandler(this.btnGenerateXml_Click);
			// 
			// _versionSuffix
			// 
			resources.ApplyResources(this._versionSuffix, "_versionSuffix");
			this._versionSuffix.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this._versionSuffix.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this._versionSuffix.Name = "_versionSuffix";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// _license
			// 
			resources.ApplyResources(this._license, "_license");
			this._license.Name = "_license";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// _version
			// 
			resources.ApplyResources(this._version, "_version");
			this._version.Name = "_version";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// _component
			// 
			resources.ApplyResources(this._component, "_component");
			this._component.Name = "_component";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// _copyright
			// 
			resources.ApplyResources(this._copyright, "_copyright");
			this._copyright.Name = "_copyright";
			// 
			// _btnGenerateConfig
			// 
			resources.ApplyResources(this._btnGenerateConfig, "_btnGenerateConfig");
			this._btnGenerateConfig.Name = "_btnGenerateConfig";
			this._btnGenerateConfig.UseVisualStyleBackColor = true;
			this._btnGenerateConfig.Click += new System.EventHandler(this.btnGenerateConfig_Click);
			// 
			// _product
			// 
			resources.ApplyResources(this._product, "_product");
			this._product.Name = "_product";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this._btnGenerateXml);
			this.flowLayoutPanel1.Controls.Add(this._btnGenerateConfig);
			this.flowLayoutPanel1.Controls.Add(this._btnReset);
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// _btnReset
			// 
			resources.ApplyResources(this._btnReset, "_btnReset");
			this._btnReset.Name = "_btnReset";
			this._btnReset.UseVisualStyleBackColor = true;
			this._btnReset.Click += new System.EventHandler(this._btnReset_Click);
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.Name = "label8";
			// 
			// _edition
			// 
			resources.ApplyResources(this._edition, "_edition");
			this._edition.Name = "_edition";
			// 
			// _dlgSaveXml
			// 
			this._dlgSaveXml.DefaultExt = "xml";
			this._dlgSaveXml.FileName = "ProductSettings.xml";
			resources.ApplyResources(this._dlgSaveXml, "_dlgSaveXml");
			// 
			// _dlgSaveConfig
			// 
			this._dlgSaveConfig.DefaultExt = "xml";
			this._dlgSaveConfig.FileName = "ClearCanvas.Common.dll.config";
			resources.ApplyResources(this._dlgSaveConfig, "_dlgSaveConfig");
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// _release
			// 
			resources.ApplyResources(this._release, "_release");
			this._release.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
			this._release.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this._release.FormattingEnabled = true;
			this._release.Name = "_release";
			// 
			// GeneratorForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._release);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label8);
			this.Controls.Add(this._edition);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this._product);
			this.Controls.Add(this.label5);
			this.Controls.Add(this._copyright);
			this.Controls.Add(this.label4);
			this.Controls.Add(this._component);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._version);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._license);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._versionSuffix);
			this.Name = "GeneratorForm";
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _btnGenerateXml;
		private System.Windows.Forms.TextBox _versionSuffix;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _license;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _version;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox _component;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox _copyright;
		private System.Windows.Forms.Button _btnGenerateConfig;
        private System.Windows.Forms.TextBox _product;
        private System.Windows.Forms.Label label6;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox _edition;
		private System.Windows.Forms.Button _btnReset;
		private System.Windows.Forms.SaveFileDialog _dlgSaveXml;
		private System.Windows.Forms.SaveFileDialog _dlgSaveConfig;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox _release;
	}
}