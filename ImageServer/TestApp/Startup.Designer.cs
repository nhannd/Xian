namespace ClearCanvas.ImageServer.TestApp
{
    partial class Startup
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
            this.TestRule = new System.Windows.Forms.Button();
            this.TestHeaderStreamButton = new System.Windows.Forms.Button();
            this.buttonCompression = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TestRule
            // 
            this.TestRule.Location = new System.Drawing.Point(28, 37);
            this.TestRule.Name = "TestRule";
            this.TestRule.Size = new System.Drawing.Size(183, 59);
            this.TestRule.TabIndex = 0;
            this.TestRule.Text = "Rules";
            this.TestRule.UseVisualStyleBackColor = true;
            this.TestRule.Click += new System.EventHandler(this.TestRule_Click);
            // 
            // TestHeaderStreamButton
            // 
            this.TestHeaderStreamButton.Location = new System.Drawing.Point(258, 37);
            this.TestHeaderStreamButton.Name = "TestHeaderStreamButton";
            this.TestHeaderStreamButton.Size = new System.Drawing.Size(176, 59);
            this.TestHeaderStreamButton.TabIndex = 1;
            this.TestHeaderStreamButton.Text = "Header Retrieval Client";
            this.TestHeaderStreamButton.UseVisualStyleBackColor = true;
            this.TestHeaderStreamButton.Click += new System.EventHandler(this.TestHeaderStreamButton_Click);
            // 
            // buttonCompression
            // 
            this.buttonCompression.Location = new System.Drawing.Point(28, 135);
            this.buttonCompression.Name = "buttonCompression";
            this.buttonCompression.Size = new System.Drawing.Size(183, 59);
            this.buttonCompression.TabIndex = 2;
            this.buttonCompression.Text = "Compression";
            this.buttonCompression.UseVisualStyleBackColor = true;
            this.buttonCompression.Click += new System.EventHandler(this.buttonCompression_Click);
            // 
            // Startup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 226);
            this.Controls.Add(this.buttonCompression);
            this.Controls.Add(this.TestHeaderStreamButton);
            this.Controls.Add(this.TestRule);
            this.Name = "Startup";
            this.Text = "Startup";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button TestRule;
        private System.Windows.Forms.Button TestHeaderStreamButton;
        private System.Windows.Forms.Button buttonCompression;
    }
}