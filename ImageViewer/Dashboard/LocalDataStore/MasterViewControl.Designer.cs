namespace ClearCanvas.ImageViewer.Dashboard.LocalDataStore
{
    partial class MasterViewControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._textConnectionString = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this._textPassword = new System.Windows.Forms.MaskedTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._textUser = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._textDB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._textName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._textHost = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._textConnectionString);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this._textPassword);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this._textUser);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this._textDB);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this._textName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this._textHost);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(205, 419);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SQL Server";
            // 
            // _textConnectionString
            // 
            this._textConnectionString.Location = new System.Drawing.Point(14, 258);
            this._textConnectionString.Multiline = true;
            this._textConnectionString.Name = "_textConnectionString";
            this._textConnectionString.ReadOnly = true;
            this._textConnectionString.Size = new System.Drawing.Size(181, 140);
            this._textConnectionString.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 238);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 17);
            this.label6.TabIndex = 10;
            this.label6.Text = "Connection String";
            // 
            // _textPassword
            // 
            this._textPassword.Location = new System.Drawing.Point(78, 199);
            this._textPassword.Name = "_textPassword";
            this._textPassword.PasswordChar = '*';
            this._textPassword.Size = new System.Drawing.Size(117, 22);
            this._textPassword.TabIndex = 9;
            this._textPassword.Leave += new System.EventHandler(this._textPassword_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 202);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 17);
            this.label5.TabIndex = 8;
            this.label5.Text = "Password";
            // 
            // _textUser
            // 
            this._textUser.Location = new System.Drawing.Point(78, 161);
            this._textUser.Name = "_textUser";
            this._textUser.Size = new System.Drawing.Size(117, 22);
            this._textUser.TabIndex = 7;
            this._textUser.Leave += new System.EventHandler(this._textUser_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(34, 164);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "User";
            // 
            // _textDB
            // 
            this._textDB.Location = new System.Drawing.Point(78, 123);
            this._textDB.Name = "_textDB";
            this._textDB.Size = new System.Drawing.Size(117, 22);
            this._textDB.TabIndex = 5;
            this._textDB.Leave += new System.EventHandler(this._textDB_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(45, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "DB";
            // 
            // _textName
            // 
            this._textName.Location = new System.Drawing.Point(78, 82);
            this._textName.Name = "_textName";
            this._textName.Size = new System.Drawing.Size(117, 22);
            this._textName.TabIndex = 3;
            this._textName.Leave += new System.EventHandler(this._textName_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Name";
            // 
            // _textHost
            // 
            this._textHost.Location = new System.Drawing.Point(78, 42);
            this._textHost.Name = "_textHost";
            this._textHost.Size = new System.Drawing.Size(117, 22);
            this._textHost.TabIndex = 1;
            this._textHost.Leave += new System.EventHandler(this._textHost_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Host";
            // 
            // MasterViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MasterViewControl";
            this.Size = new System.Drawing.Size(212, 446);
            this.Leave += new System.EventHandler(this.MasterViewControl_Leave);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox _textName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _textHost;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _textUser;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _textDB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _textConnectionString;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.MaskedTextBox _textPassword;

    }
}
