namespace ClearCanvas.ImageViewer.Externals.View.WinForms.General
{
	partial class CommandLineExternalConfigurationControl {
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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this._txtCommand = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this._txtWorkingDir = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this._txtArguments = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoScroll = true;
			this.flowLayoutPanel1.Controls.Add(this.label1);
			this.flowLayoutPanel1.Controls.Add(this._txtCommand);
			this.flowLayoutPanel1.Controls.Add(this.label2);
			this.flowLayoutPanel1.Controls.Add(this.label3);
			this.flowLayoutPanel1.Controls.Add(this._txtWorkingDir);
			this.flowLayoutPanel1.Controls.Add(this.label4);
			this.flowLayoutPanel1.Controls.Add(this.label5);
			this.flowLayoutPanel1.Controls.Add(this._txtArguments);
			this.flowLayoutPanel1.Controls.Add(this.label6);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
			this.flowLayoutPanel1.Size = new System.Drawing.Size(396, 291);
			this.flowLayoutPanel1.TabIndex = 0;
			this.flowLayoutPanel1.WrapContents = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Command";
			// 
			// _txtCommand
			// 
			this._txtCommand.Location = new System.Drawing.Point(13, 26);
			this._txtCommand.Name = "_txtCommand";
			this._txtCommand.Size = new System.Drawing.Size(364, 20);
			this._txtCommand.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(0, 13);
			this.label2.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 62);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(92, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Working Directory";
			// 
			// _txtWorkingDir
			// 
			this._txtWorkingDir.Location = new System.Drawing.Point(13, 78);
			this._txtWorkingDir.Name = "_txtWorkingDir";
			this._txtWorkingDir.Size = new System.Drawing.Size(364, 20);
			this._txtWorkingDir.TabIndex = 4;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(13, 101);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(0, 13);
			this.label4.TabIndex = 5;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(13, 114);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(121, 13);
			this.label5.TabIndex = 6;
			this.label5.Text = "Arguments (one per line)";
			// 
			// _txtArguments
			// 
			this._txtArguments.Location = new System.Drawing.Point(13, 130);
			this._txtArguments.Multiline = true;
			this._txtArguments.Name = "_txtArguments";
			this._txtArguments.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this._txtArguments.Size = new System.Drawing.Size(364, 113);
			this._txtArguments.TabIndex = 7;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(13, 246);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(350, 26);
			this.label6.TabIndex = 8;
			this.label6.Text = "Valid fields include: $FILENAME$, $DIRECTORY$. Use $$ to indicate a literal $ cha" +
				"racter.";
			// 
			// CommandLineExternalConfigurationControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "CommandLineExternalConfigurationControl";
			this.Size = new System.Drawing.Size(396, 291);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _txtCommand;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _txtWorkingDir;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox _txtArguments;
		private System.Windows.Forms.Label label6;
	}
}