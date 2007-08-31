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
            this.buttonSelectDirectory = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
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
            // buttonSelectDirectory
            // 
            this.buttonSelectDirectory.Location = new System.Drawing.Point(13, 36);
            this.buttonSelectDirectory.Name = "buttonSelectDirectory";
            this.buttonSelectDirectory.Size = new System.Drawing.Size(95, 23);
            this.buttonSelectDirectory.TabIndex = 1;
            this.buttonSelectDirectory.Text = "Scan Directory";
            this.buttonSelectDirectory.UseVisualStyleBackColor = true;
            this.buttonSelectDirectory.Click += new System.EventHandler(this.buttonSelectDirectory_Click);
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Select Folder for Scanning";
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // TestAppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.buttonSelectDirectory);
            this.Controls.Add(this.checkBoxLoadTest);
            this.Name = "TestAppForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxLoadTest;
        private System.Windows.Forms.Button buttonSelectDirectory;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}

