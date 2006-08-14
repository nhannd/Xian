using System;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    partial class DesktopForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DesktopForm));
			this._mainMenu = new System.Windows.Forms.MenuStrip();
			this._toolbar = new System.Windows.Forms.ToolStrip();
			this._toolStripContainer = new System.Windows.Forms.ToolStripContainer();
			this._tabControl = new Crownwood.DotNetMagic.Controls.TabControl();
			this._toolStripContainer.ContentPanel.SuspendLayout();
			this._toolStripContainer.TopToolStripPanel.SuspendLayout();
			this._toolStripContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainMenu
			// 
			this._mainMenu.Dock = System.Windows.Forms.DockStyle.None;
			this._mainMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
			this._mainMenu.ImageScalingSize = new System.Drawing.Size(32, 32);
			this._mainMenu.Location = new System.Drawing.Point(0, 0);
			this._mainMenu.Name = "mainMenu";
			this._mainMenu.Size = new System.Drawing.Size(792, 24);
			this._mainMenu.TabIndex = 4;
			this._mainMenu.Text = "menuStrip1";
			// 
			// toolbar
			// 
			this._toolbar.AllowItemReorder = true;
			this._toolbar.Dock = System.Windows.Forms.DockStyle.None;
			this._toolbar.ImageScalingSize = new System.Drawing.Size(32, 32);
			this._toolbar.Location = new System.Drawing.Point(3, 24);
			this._toolbar.Name = "toolbar";
			this._toolbar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this._toolbar.Size = new System.Drawing.Size(111, 25);
			this._toolbar.TabIndex = 4;
			this._toolbar.Text = "toolStrip1";
			// 
			// toolStripContainer
			// 
			// 
			// toolStripContainer.ContentPanel
			// 
			this._toolStripContainer.ContentPanel.BackColor = System.Drawing.SystemColors.ControlDark;
			this._toolStripContainer.ContentPanel.Controls.Add(this._tabControl);
			this._toolStripContainer.ContentPanel.Size = new System.Drawing.Size(792, 512);
			this._toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._toolStripContainer.Location = new System.Drawing.Point(0, 0);
			this._toolStripContainer.Name = "toolStripContainer";
			this._toolStripContainer.Size = new System.Drawing.Size(792, 561);
			this._toolStripContainer.TabIndex = 5;
			this._toolStripContainer.Text = "toolStripContainer1";
			// 
			// toolStripContainer.TopToolStripPanel
			// 
			this._toolStripContainer.TopToolStripPanel.Controls.Add(this._mainMenu);
			this._toolStripContainer.TopToolStripPanel.Controls.Add(this._toolbar);
			// 
			// tabControl
			// 
			this._tabControl.Appearance = Crownwood.DotNetMagic.Controls.VisualAppearance.MultiDocument;
			this._tabControl.BackColor = System.Drawing.SystemColors.ControlDark;
			this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tabControl.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._tabControl.HideTabsMode = Crownwood.DotNetMagic.Controls.HideTabsModes.HideAlways;
			this._tabControl.IDE2005PixelBorder = false;
			this._tabControl.Location = new System.Drawing.Point(0, 0);
			this._tabControl.Name = "tabControl";
			this._tabControl.OfficeDockSides = false;
			this._tabControl.ShowDropSelect = false;
			this._tabControl.Size = new System.Drawing.Size(792, 512);
			this._tabControl.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
			this._tabControl.TabIndex = 0;
			this._tabControl.TextTips = true;
			// 
			// DesktopForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 561);
			this.Controls.Add(this._toolStripContainer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.MainMenuStrip = this._mainMenu;
			this.Name = "DesktopForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this._toolStripContainer.ContentPanel.ResumeLayout(false);
			this._toolStripContainer.TopToolStripPanel.ResumeLayout(false);
			this._toolStripContainer.TopToolStripPanel.PerformLayout();
			this._toolStripContainer.ResumeLayout(false);
			this._toolStripContainer.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private MenuStrip _mainMenu;
		private ToolStrip _toolbar;
		private ToolStripContainer _toolStripContainer;
		private Crownwood.DotNetMagic.Controls.TabControl _tabControl;
	}
}