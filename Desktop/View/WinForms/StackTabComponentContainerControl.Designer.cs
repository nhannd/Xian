namespace ClearCanvas.Desktop.View.WinForms
{
    partial class StackTabComponentContainerControl
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
            this._stackTabControl = new Crownwood.DotNetMagic.Controls.TabbedGroups();
            ((System.ComponentModel.ISupportInitialize)(this._stackTabControl)).BeginInit();
            this.SuspendLayout();
            // 
            // _stackTabControl
            // 
            this._stackTabControl.AllowDrop = true;
            this._stackTabControl.AtLeastOneLeaf = false;
            this._stackTabControl.DisplayTabMode = Crownwood.DotNetMagic.Controls.DisplayTabModes.HideAll;
            this._stackTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._stackTabControl.Location = new System.Drawing.Point(0, 0);
            this._stackTabControl.Name = "_stackTabControl";
            this._stackTabControl.ProminentLeaf = null;
            this._stackTabControl.ResizeBarColor = System.Drawing.SystemColors.Control;
            this._stackTabControl.Size = new System.Drawing.Size(222, 258);
            this._stackTabControl.TabIndex = 0;
            // 
            // StackTabComponentContainerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._stackTabControl);
            this.Name = "StackTabComponentContainerControl";
            this.Size = new System.Drawing.Size(222, 258);
            ((System.ComponentModel.ISupportInitialize)(this._stackTabControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Crownwood.DotNetMagic.Controls.TabbedGroups _stackTabControl;
    }
}
