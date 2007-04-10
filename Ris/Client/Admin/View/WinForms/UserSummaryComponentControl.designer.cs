namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class UserSummaryComponentControl
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
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            this._users = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _users
            // 
            this._users.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._users.AutoSize = true;
            this._users.Location = new System.Drawing.Point(0, 0);
            this._users.MenuModel = null;
            this._users.Name = "_users";
            this._users.ReadOnly = false;
            this._users.Selection = selection1;
            this._users.Size = new System.Drawing.Size(150, 150);
            this._users.TabIndex = 0;
            this._users.Table = null;
            this._users.ToolbarModel = null;
            this._users.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._users.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._users.Load += new System.EventHandler(this._users_Load);
            this._users.ItemDoubleClicked += new System.EventHandler(this._users_ItemDoubleClicked);
            // 
            // UserSummaryComponentControl
            // 
            this.AutoSize = true;
            this.Controls.Add(this._users);
            this.Name = "UserSummaryComponentControl";
            this.Size = new System.Drawing.Size(153, 153);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _users;
    }
}
