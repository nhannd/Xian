namespace ClearCanvas.Utilities.RebuildDatabase
{
    public partial class RebuildDatabaseForm
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
            this.label1 = new System.Windows.Forms.Label();
            this._connectionStringText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._imageFolderText = new System.Windows.Forms.TextBox();
            this._findFilesRecursivelyCheckbox = new System.Windows.Forms.CheckBox();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._completedRebuildFile = new System.Windows.Forms.Label();
            this._startButton = new System.Windows.Forms.Button();
            this._stopButton = new System.Windows.Forms.Button();
            this._exitButton = new System.Windows.Forms.Button();
            this._browseForFolderButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "DB Connection String";
            // 
            // _connectionStringText
            // 
            this._connectionStringText.Location = new System.Drawing.Point(15, 30);
            this._connectionStringText.Name = "_connectionStringText";
            this._connectionStringText.Size = new System.Drawing.Size(560, 22);
            this._connectionStringText.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "DICOM Image Folder";
            // 
            // _imageFolderText
            // 
            this._imageFolderText.Location = new System.Drawing.Point(15, 80);
            this._imageFolderText.Name = "_imageFolderText";
            this._imageFolderText.Size = new System.Drawing.Size(524, 22);
            this._imageFolderText.TabIndex = 3;
            // 
            // _findFilesRecursivelyCheckbox
            // 
            this._findFilesRecursivelyCheckbox.AutoSize = true;
            this._findFilesRecursivelyCheckbox.Location = new System.Drawing.Point(15, 109);
            this._findFilesRecursivelyCheckbox.Name = "_findFilesRecursivelyCheckbox";
            this._findFilesRecursivelyCheckbox.Size = new System.Drawing.Size(203, 21);
            this._findFilesRecursivelyCheckbox.TabIndex = 4;
            this._findFilesRecursivelyCheckbox.Text = "Find DICOM files recursively";
            this._findFilesRecursivelyCheckbox.UseVisualStyleBackColor = true;
            // 
            // _progressBar
            // 
            this._progressBar.Location = new System.Drawing.Point(15, 220);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(560, 23);
            this._progressBar.TabIndex = 5;
            // 
            // _completedRebuildFile
            // 
            this._completedRebuildFile.AutoSize = true;
            this._completedRebuildFile.Location = new System.Drawing.Point(15, 197);
            this._completedRebuildFile.Name = "_completedRebuildFile";
            this._completedRebuildFile.Size = new System.Drawing.Size(0, 17);
            this._completedRebuildFile.TabIndex = 6;
            // 
            // _startButton
            // 
            this._startButton.Location = new System.Drawing.Point(184, 149);
            this._startButton.Name = "_startButton";
            this._startButton.Size = new System.Drawing.Size(75, 23);
            this._startButton.TabIndex = 7;
            this._startButton.Text = "Start";
            this._startButton.UseVisualStyleBackColor = true;
            this._startButton.Click += new System.EventHandler(this._startButton_Click);
            // 
            // _stopButton
            // 
            this._stopButton.Location = new System.Drawing.Point(266, 149);
            this._stopButton.Name = "_stopButton";
            this._stopButton.Size = new System.Drawing.Size(75, 23);
            this._stopButton.TabIndex = 8;
            this._stopButton.Text = "Stop";
            this._stopButton.UseVisualStyleBackColor = true;
            this._stopButton.Click += new System.EventHandler(this._stopButton_Click);
            // 
            // _exitButton
            // 
            this._exitButton.Location = new System.Drawing.Point(348, 149);
            this._exitButton.Name = "_exitButton";
            this._exitButton.Size = new System.Drawing.Size(75, 23);
            this._exitButton.TabIndex = 9;
            this._exitButton.Text = "Exit";
            this._exitButton.UseVisualStyleBackColor = true;
            this._exitButton.Click += new System.EventHandler(this._exitButton_Click);
            // 
            // _browseForFolderButton
            // 
            this._browseForFolderButton.Location = new System.Drawing.Point(546, 78);
            this._browseForFolderButton.Name = "_browseForFolderButton";
            this._browseForFolderButton.Size = new System.Drawing.Size(29, 23);
            this._browseForFolderButton.TabIndex = 10;
            this._browseForFolderButton.Text = "...";
            this._browseForFolderButton.UseVisualStyleBackColor = true;
            this._browseForFolderButton.Click += new System.EventHandler(this.Browse_Click);
            // 
            // RebuildDatabaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 255);
            this.Controls.Add(this._browseForFolderButton);
            this.Controls.Add(this._exitButton);
            this.Controls.Add(this._stopButton);
            this.Controls.Add(this._startButton);
            this.Controls.Add(this._completedRebuildFile);
            this.Controls.Add(this._progressBar);
            this.Controls.Add(this._findFilesRecursivelyCheckbox);
            this.Controls.Add(this._imageFolderText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._connectionStringText);
            this.Controls.Add(this.label1);
            this.Name = "RebuildDatabaseForm";
            this.Text = "Rebuild Database";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RebuildDatabase_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _connectionStringText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _imageFolderText;
        private System.Windows.Forms.CheckBox _findFilesRecursivelyCheckbox;
        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.Label _completedRebuildFile;
        private System.Windows.Forms.Button _startButton;
        private System.Windows.Forms.Button _stopButton;
        private System.Windows.Forms.Button _exitButton;
        private System.Windows.Forms.Button _browseForFolderButton;
    }
}

