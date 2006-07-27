using System;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    partial class DesktopForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DesktopForm));
			this.mainMenu = new System.Windows.Forms.MenuStrip();
			this.toolbar = new System.Windows.Forms.ToolStrip();
			this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
			this.tabControl = new Crownwood.DotNetMagic.Controls.TabControl();
			this.toolStripContainer.ContentPanel.SuspendLayout();
			this.toolStripContainer.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainMenu
			// 
			this.mainMenu.Dock = System.Windows.Forms.DockStyle.None;
			this.mainMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
			this.mainMenu.ImageScalingSize = new System.Drawing.Size(32, 32);
			this.mainMenu.Location = new System.Drawing.Point(0, 0);
			this.mainMenu.Name = "mainMenu";
			this.mainMenu.Size = new System.Drawing.Size(792, 24);
			this.mainMenu.TabIndex = 4;
			this.mainMenu.Text = "menuStrip1";
			// 
			// toolbar
			// 
			this.toolbar.AllowItemReorder = true;
			this.toolbar.Dock = System.Windows.Forms.DockStyle.None;
			this.toolbar.ImageScalingSize = new System.Drawing.Size(32, 32);
			this.toolbar.Location = new System.Drawing.Point(3, 24);
			this.toolbar.Name = "toolbar";
			this.toolbar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.toolbar.Size = new System.Drawing.Size(111, 25);
			this.toolbar.TabIndex = 4;
			this.toolbar.Text = "toolStrip1";
			// 
			// toolStripContainer
			// 
			// 
			// toolStripContainer.ContentPanel
			// 
			this.toolStripContainer.ContentPanel.BackColor = System.Drawing.SystemColors.ControlDark;
			this.toolStripContainer.ContentPanel.Controls.Add(this.tabControl);
			this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(792, 512);
			this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer.Name = "toolStripContainer";
			this.toolStripContainer.Size = new System.Drawing.Size(792, 561);
			this.toolStripContainer.TabIndex = 5;
			this.toolStripContainer.Text = "toolStripContainer1";
			// 
			// toolStripContainer.TopToolStripPanel
			// 
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.mainMenu);
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.toolbar);
			// 
			// tabControl
			// 
			this.tabControl.Appearance = Crownwood.DotNetMagic.Controls.VisualAppearance.MultiDocument;
			this.tabControl.BackColor = System.Drawing.SystemColors.ControlDark;
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabControl.HideTabsMode = Crownwood.DotNetMagic.Controls.HideTabsModes.HideAlways;
			this.tabControl.IDE2005PixelBorder = false;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.OfficeDockSides = false;
			this.tabControl.ShowDropSelect = false;
			this.tabControl.Size = new System.Drawing.Size(792, 512);
			this.tabControl.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
			this.tabControl.TabIndex = 0;
			this.tabControl.TextTips = true;
			// 
			// DesktopForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 561);
			this.Controls.Add(this.toolStripContainer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.MainMenuStrip = this.mainMenu;
			this.Name = "DesktopForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.toolStripContainer.ContentPanel.ResumeLayout(false);
			this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer.TopToolStripPanel.PerformLayout();
			this.toolStripContainer.ResumeLayout(false);
			this.toolStripContainer.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private MenuStrip mainMenu;
		private ToolStrip toolbar;
		private ToolStripContainer toolStripContainer;
		private Crownwood.DotNetMagic.Controls.TabControl tabControl;
	}
}