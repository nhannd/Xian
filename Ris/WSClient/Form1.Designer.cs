namespace WSClient
{
    partial class Form1
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
            this._sendButton = new System.Windows.Forms.Button();
            this._number = new System.Windows.Forms.TextBox();
            this._result = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _sendButton
            // 
            this._sendButton.Location = new System.Drawing.Point(208, 47);
            this._sendButton.Name = "_sendButton";
            this._sendButton.Size = new System.Drawing.Size(75, 23);
            this._sendButton.TabIndex = 0;
            this._sendButton.Text = "Send";
            this._sendButton.UseVisualStyleBackColor = true;
            this._sendButton.Click += new System.EventHandler(this._sendButton_Click);
            // 
            // _number
            // 
            this._number.Location = new System.Drawing.Point(46, 47);
            this._number.Name = "_number";
            this._number.Size = new System.Drawing.Size(100, 22);
            this._number.TabIndex = 1;
            // 
            // _result
            // 
            this._result.Location = new System.Drawing.Point(46, 103);
            this._result.Name = "_result";
            this._result.Size = new System.Drawing.Size(237, 22);
            this._result.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 336);
            this.Controls.Add(this._result);
            this.Controls.Add(this._number);
            this.Controls.Add(this._sendButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _sendButton;
        private System.Windows.Forms.TextBox _number;
        private System.Windows.Forms.TextBox _result;
    }
}

