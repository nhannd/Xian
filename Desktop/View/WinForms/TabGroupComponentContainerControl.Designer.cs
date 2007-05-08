namespace ClearCanvas.Desktop.View.WinForms
{
    partial class TabGroupComponentContainerControl
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
            this._tabbedGroupsControl = new Crownwood.DotNetMagic.Controls.TabbedGroups();
            ((System.ComponentModel.ISupportInitialize)(this._tabbedGroupsControl)).BeginInit();
            this.SuspendLayout();
            // 
            // _tabbedGroupsControl
            // 
            this._tabbedGroupsControl.AllowDrop = true;
            this._tabbedGroupsControl.AtLeastOneLeaf = false;
            this._tabbedGroupsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabbedGroupsControl.Location = new System.Drawing.Point(0, 0);
            this._tabbedGroupsControl.Name = "_tabbedGroupsControl";
            this._tabbedGroupsControl.ProminentLeaf = null;
            this._tabbedGroupsControl.ResizeBarColor = System.Drawing.SystemColors.Control;
            this._tabbedGroupsControl.Size = new System.Drawing.Size(434, 348);
            this._tabbedGroupsControl.Style = Crownwood.DotNetMagic.Common.VisualStyle.Office2003;
            this._tabbedGroupsControl.TabIndex = 0;
            this._tabbedGroupsControl.TabControlCreated += new Crownwood.DotNetMagic.Controls.TabbedGroups.TabControlCreatedHandler(this.OnTabControlCreated);
            // 
            // TabbedGroupsComponentContainerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tabbedGroupsControl);
            this.Name = "TabbedGroupsComponentContainerControl";
            this.Size = new System.Drawing.Size(434, 348);
            ((System.ComponentModel.ISupportInitialize)(this._tabbedGroupsControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Crownwood.DotNetMagic.Controls.TabbedGroups _tabbedGroupsControl;
    }
}
