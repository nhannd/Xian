namespace ClearCanvas.ImageServer.TestApp
{
    partial class TestAppForm
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
            this.checkBoxLoadTest = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // checkBoxLoadTest
            // 
            this.checkBoxLoadTest.AutoSize = true;
            this.checkBoxLoadTest.Location = new System.Drawing.Point(12, 12);
            this.checkBoxLoadTest.Name = "checkBoxLoadTest";
            this.checkBoxLoadTest.Size = new System.Drawing.Size(71, 17);
            this.checkBoxLoadTest.TabIndex = 0;
            this.checkBoxLoadTest.Text = "LoadTest";
            this.checkBoxLoadTest.UseVisualStyleBackColor = true;
            this.checkBoxLoadTest.CheckedChanged += new System.EventHandler(this.checkBoxLoadTest_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.checkBoxLoadTest);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxLoadTest;
    }
}

