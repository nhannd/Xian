namespace ClearCanvas.Desktop.View.WinForms
{
    partial class DialogBoxForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer _components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogBoxForm));
			this._contentPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// _contentPanel
			// 
			this._contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._contentPanel.Location = new System.Drawing.Point(0, 0);
			this._contentPanel.Margin = new System.Windows.Forms.Padding(2);
			this._contentPanel.Name = "_contentPanel";
			this._contentPanel.Size = new System.Drawing.Size(296, 169);
			this._contentPanel.TabIndex = 0;
			// 
			// DialogBoxForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(296, 169);
			this.Controls.Add(this._contentPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(2);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DialogBoxForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Style = Crownwood.DotNetMagic.Common.VisualStyle.Office2007Black;
			this.Text = "ApplicationComponentHostDialogForm";
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _contentPanel;
    }
}