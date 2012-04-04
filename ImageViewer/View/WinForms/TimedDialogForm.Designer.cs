namespace ClearCanvas.ImageViewer.View.WinForms
{
    partial class TimedDialogForm
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
                DisposeTimer();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimedDialogForm));
            this._hostPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // _hostPanel
            // 
            resources.ApplyResources(this._hostPanel, "_hostPanel");
            this._hostPanel.Name = "_hostPanel";
            // 
            // TimedDialogForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._hostPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TimedDialogForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel _hostPanel;
    }
}