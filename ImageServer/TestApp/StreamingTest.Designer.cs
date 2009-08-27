namespace ClearCanvas.ImageServer.TestApp
{
    partial class StreamingTest
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
            this.StudyUID = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.WadoUrl = new System.Windows.Forms.TextBox();
            this.StudyPath = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SimReadDelay = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.KeepAlive = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // StudyUID
            // 
            this.StudyUID.Location = new System.Drawing.Point(85, 62);
            this.StudyUID.Name = "StudyUID";
            this.StudyUID.Size = new System.Drawing.Size(472, 20);
            this.StudyUID.TabIndex = 0;
            this.StudyUID.Text = "1.2.804.114118.13.1501930874";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(72, 194);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 39);
            this.button1.TabIndex = 2;
            this.button1.Text = "Start Streaming";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(82, 120);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            // 
            // WadoUrl
            // 
            this.WadoUrl.Location = new System.Drawing.Point(85, 24);
            this.WadoUrl.Name = "WadoUrl";
            this.WadoUrl.Size = new System.Drawing.Size(472, 20);
            this.WadoUrl.TabIndex = 4;
            this.WadoUrl.Text = "http://NORTHSTAR:1000/wado/NORTHSTAR";
            // 
            // StudyPath
            // 
            this.StudyPath.Location = new System.Drawing.Point(85, 88);
            this.StudyPath.Name = "StudyPath";
            this.StudyPath.Size = new System.Drawing.Size(472, 20);
            this.StudyPath.TabIndex = 5;
            this.StudyPath.Text = "\\\\kirkwood\\FS_NORTHSTAR\\Primary\\20060621\\1.2.804.114118.13.1501930874";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(209, 194);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(176, 39);
            this.button2.TabIndex = 6;
            this.button2.Text = "Speed Test Uncompressed";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(404, 194);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(138, 39);
            this.button3.TabIndex = 7;
            this.button3.Text = "Speed Test (Compressed)";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // SimReadDelay
            // 
            this.SimReadDelay.Location = new System.Drawing.Point(285, 243);
            this.SimReadDelay.Name = "SimReadDelay";
            this.SimReadDelay.Size = new System.Drawing.Size(100, 20);
            this.SimReadDelay.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(196, 246);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Sim Read Delay";
            // 
            // KeepAlive
            // 
            this.KeepAlive.AutoSize = true;
            this.KeepAlive.Location = new System.Drawing.Point(431, 245);
            this.KeepAlive.Name = "KeepAlive";
            this.KeepAlive.Size = new System.Drawing.Size(77, 17);
            this.KeepAlive.TabIndex = 10;
            this.KeepAlive.Text = "Keep Alive";
            this.KeepAlive.UseVisualStyleBackColor = true;
            this.KeepAlive.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // StreamingTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 277);
            this.Controls.Add(this.KeepAlive);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SimReadDelay);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.StudyPath);
            this.Controls.Add(this.WadoUrl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.StudyUID);
            this.Name = "StreamingTest";
            this.Text = "StreamingTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox StudyUID;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox WadoUrl;
        private System.Windows.Forms.TextBox StudyPath;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox SimReadDelay;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox KeepAlive;
    }
}