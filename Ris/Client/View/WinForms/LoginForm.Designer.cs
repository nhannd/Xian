namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class LoginForm
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
            this._userName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._cancelButton = new System.Windows.Forms.Button();
            this._loginButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _userName
            // 
            this._userName.Location = new System.Drawing.Point(27, 38);
            this._userName.Name = "_userName";
            this._userName.Size = new System.Drawing.Size(293, 22);
            this._userName.TabIndex = 0;
            this._userName.TextChanged += new System.EventHandler(this._userName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "User Name";
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(245, 166);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 2;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _loginButton
            // 
            this._loginButton.Enabled = false;
            this._loginButton.Location = new System.Drawing.Point(168, 166);
            this._loginButton.Name = "_loginButton";
            this._loginButton.Size = new System.Drawing.Size(75, 24);
            this._loginButton.TabIndex = 3;
            this._loginButton.Text = "Login";
            this._loginButton.UseVisualStyleBackColor = true;
            this._loginButton.Click += new System.EventHandler(this._loginButton_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 202);
            this.Controls.Add(this._loginButton);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._userName);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ClearCanvas Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _userName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _loginButton;
    }
}