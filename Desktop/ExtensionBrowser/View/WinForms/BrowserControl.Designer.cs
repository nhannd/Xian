namespace ClearCanvas.Workstation.ExtensionBrowser.View.WinForms
{
    partial class BrowserControl
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
            this._pluginTree = new System.Windows.Forms.TreeView();
            this._tabView = new System.Windows.Forms.TabControl();
            this._pluginViewTabPage = new System.Windows.Forms.TabPage();
            this._extPointViewTabPage = new System.Windows.Forms.TabPage();
            this._extPointTree = new System.Windows.Forms.TreeView();
            this._tabView.SuspendLayout();
            this._pluginViewTabPage.SuspendLayout();
            this._extPointViewTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // _pluginTree
            // 
            this._pluginTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pluginTree.Location = new System.Drawing.Point(3, 3);
            this._pluginTree.Name = "_pluginTree";
            this._pluginTree.Size = new System.Drawing.Size(424, 277);
            this._pluginTree.TabIndex = 0;
            // 
            // _tabView
            // 
            this._tabView.Controls.Add(this._extPointViewTabPage);
            this._tabView.Controls.Add(this._pluginViewTabPage);
            this._tabView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabView.Location = new System.Drawing.Point(0, 0);
            this._tabView.Name = "_tabView";
            this._tabView.SelectedIndex = 0;
            this._tabView.Size = new System.Drawing.Size(438, 309);
            this._tabView.TabIndex = 1;
            // 
            // _pluginViewTabPage
            // 
            this._pluginViewTabPage.Controls.Add(this._pluginTree);
            this._pluginViewTabPage.Location = new System.Drawing.Point(4, 22);
            this._pluginViewTabPage.Name = "_pluginViewTabPage";
            this._pluginViewTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._pluginViewTabPage.Size = new System.Drawing.Size(430, 283);
            this._pluginViewTabPage.TabIndex = 0;
            this._pluginViewTabPage.Text = "Plugins";
            this._pluginViewTabPage.UseVisualStyleBackColor = true;
            // 
            // _extPointViewTabPage
            // 
            this._extPointViewTabPage.Controls.Add(this._extPointTree);
            this._extPointViewTabPage.Location = new System.Drawing.Point(4, 22);
            this._extPointViewTabPage.Name = "_extPointViewTabPage";
            this._extPointViewTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._extPointViewTabPage.Size = new System.Drawing.Size(430, 283);
            this._extPointViewTabPage.TabIndex = 1;
            this._extPointViewTabPage.Text = "Extension Points";
            this._extPointViewTabPage.UseVisualStyleBackColor = true;
            // 
            // _extPointTree
            // 
            this._extPointTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._extPointTree.Location = new System.Drawing.Point(3, 3);
            this._extPointTree.Name = "_extPointTree";
            this._extPointTree.Size = new System.Drawing.Size(424, 277);
            this._extPointTree.TabIndex = 0;
            // 
            // BrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tabView);
            this.Name = "BrowserControl";
            this.Size = new System.Drawing.Size(438, 309);
            this._tabView.ResumeLayout(false);
            this._pluginViewTabPage.ResumeLayout(false);
            this._extPointViewTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView _pluginTree;
        private System.Windows.Forms.TabControl _tabView;
        private System.Windows.Forms.TabPage _pluginViewTabPage;
        private System.Windows.Forms.TabPage _extPointViewTabPage;
        private System.Windows.Forms.TreeView _extPointTree;
    }
}
