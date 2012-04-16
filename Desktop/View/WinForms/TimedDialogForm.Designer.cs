namespace ClearCanvas.Desktop.View.WinForms
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimedDialogForm));
			this._hostPanel = new System.Windows.Forms.Panel();
			this._contextualLink = new System.Windows.Forms.LinkLabel();
			this._message = new System.Windows.Forms.Label();
			this._timer = new System.Windows.Forms.Timer(this.components);
			this._hostPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _hostPanel
			// 
			resources.ApplyResources(this._hostPanel, "_hostPanel");
			this._hostPanel.Controls.Add(this._contextualLink);
			this._hostPanel.Controls.Add(this._message);
			this._hostPanel.Name = "_hostPanel";
			// 
			// _contextualLink
			// 
			resources.ApplyResources(this._contextualLink, "_contextualLink");
			this._contextualLink.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
			this._contextualLink.Name = "_contextualLink";
			this._contextualLink.TabStop = true;
			this._contextualLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._contextualLink_LinkClicked);
			// 
			// _message
			// 
			resources.ApplyResources(this._message, "_message");
			this._message.AutoEllipsis = true;
			this._message.Name = "_message";
			// 
			// _timer
			// 
			this._timer.Tick += new System.EventHandler(this.OnTimerTick);
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
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TimedDialogForm_FormClosing);
			this._hostPanel.ResumeLayout(false);
			this._hostPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel _hostPanel;
		private System.Windows.Forms.LinkLabel _contextualLink;
		private System.Windows.Forms.Label _message;
		private System.Windows.Forms.Timer _timer;
    }
}