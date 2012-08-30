namespace ClearCanvas.Enterprise.Desktop.View.WinForms
{
    partial class UserSessionsManagmentControl
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this._terminateSelectedButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this._userName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._displayName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._lastLogin = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this._sessionsLabel = new System.Windows.Forms.Label();
            this._sessionsTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(8, 5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(5, 283);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(718, 0);
            this.panel1.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel4, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(8, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(712, 275);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this._terminateSelectedButton);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(549, 241);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(5, 5, 3, 5);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(160, 29);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // _terminateSelectedButton
            // 
            this._terminateSelectedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._terminateSelectedButton.AutoSize = true;
            this._terminateSelectedButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._terminateSelectedButton.Location = new System.Drawing.Point(3, 3);
            this._terminateSelectedButton.Name = "_terminateSelectedButton";
            this._terminateSelectedButton.Size = new System.Drawing.Size(154, 23);
            this._terminateSelectedButton.TabIndex = 2;
            this._terminateSelectedButton.Text = "Terminate Selected Sessions";
            this._terminateSelectedButton.UseVisualStyleBackColor = true;
            this._terminateSelectedButton.Click += new System.EventHandler(this._terminateSelectedButton_Click);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel3.Controls.Add(this._userName);
            this.flowLayoutPanel3.Controls.Add(this._displayName);
            this.flowLayoutPanel3.Controls.Add(this._lastLogin);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Padding = new System.Windows.Forms.Padding(0, 10, 10, 0);
            this.flowLayoutPanel3.Size = new System.Drawing.Size(472, 55);
            this.flowLayoutPanel3.TabIndex = 2;
            // 
            // _userName
            // 
            this._userName.LabelText = "User ID";
            this._userName.Location = new System.Drawing.Point(2, 12);
            this._userName.Margin = new System.Windows.Forms.Padding(2);
            this._userName.Mask = "";
            this._userName.Name = "_userName";
            this._userName.PasswordChar = '\0';
            this._userName.ReadOnly = true;
            this._userName.Size = new System.Drawing.Size(150, 41);
            this._userName.TabIndex = 0;
            this._userName.TabStop = false;
            this._userName.ToolTip = null;
            this._userName.Value = null;
            // 
            // _displayName
            // 
            this._displayName.LabelText = "Display Name";
            this._displayName.Location = new System.Drawing.Point(156, 12);
            this._displayName.Margin = new System.Windows.Forms.Padding(2);
            this._displayName.Mask = "";
            this._displayName.Name = "_displayName";
            this._displayName.PasswordChar = '\0';
            this._displayName.ReadOnly = true;
            this._displayName.Size = new System.Drawing.Size(150, 41);
            this._displayName.TabIndex = 1;
            this._displayName.TabStop = false;
            this._displayName.ToolTip = null;
            this._displayName.Value = null;
            // 
            // _lastLogin
            // 
            this._lastLogin.LabelText = "Last Login";
            this._lastLogin.Location = new System.Drawing.Point(310, 12);
            this._lastLogin.Margin = new System.Windows.Forms.Padding(2);
            this._lastLogin.Mask = "";
            this._lastLogin.Name = "_lastLogin";
            this._lastLogin.PasswordChar = '\0';
            this._lastLogin.ReadOnly = true;
            this._lastLogin.Size = new System.Drawing.Size(150, 41);
            this._lastLogin.TabIndex = 2;
            this._lastLogin.TabStop = false;
            this._lastLogin.ToolTip = null;
            this._lastLogin.Value = null;
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.AutoSize = true;
            this.flowLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel4.Controls.Add(this._sessionsLabel);
            this.flowLayoutPanel4.Controls.Add(this._sessionsTable);
            this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel4.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(3, 64);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(706, 169);
            this.flowLayoutPanel4.TabIndex = 3;
            // 
            // _sessionsLabel
            // 
            this._sessionsLabel.AutoSize = true;
            this._sessionsLabel.Location = new System.Drawing.Point(3, 0);
            this._sessionsLabel.Name = "_sessionsLabel";
            this._sessionsLabel.Size = new System.Drawing.Size(49, 13);
            this._sessionsLabel.TabIndex = 2;
            this._sessionsLabel.Text = "Sessions";
            // 
            // _sessionsTable
            // 
            this._sessionsTable.AutoSize = true;
            this._sessionsTable.ColumnHeaderTooltip = null;
            this._sessionsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._sessionsTable.Location = new System.Drawing.Point(3, 16);
            this._sessionsTable.MinimumSize = new System.Drawing.Size(700, 150);
            this._sessionsTable.Name = "_sessionsTable";
            this._sessionsTable.ReadOnly = false;
            this._sessionsTable.ShowToolbar = false;
            this._sessionsTable.Size = new System.Drawing.Size(700, 150);
            this._sessionsTable.SortButtonTooltip = null;
            this._sessionsTable.TabIndex = 1;
            // 
            // UserSessionsManagmentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "UserSessionsManagmentControl";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(728, 288);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button _terminateSelectedButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private ClearCanvas.Desktop.View.WinForms.TextField _userName;
        private ClearCanvas.Desktop.View.WinForms.TextField _displayName;
        private ClearCanvas.Desktop.View.WinForms.TextField _lastLogin;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.Label _sessionsLabel;
        private ClearCanvas.Desktop.View.WinForms.TableView _sessionsTable;
    }
}
