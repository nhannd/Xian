namespace ImageStreaming
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
            this.BaseUri = new System.Windows.Forms.TextBox();
            this.StudyUid = new System.Windows.Forms.TextBox();
            this.SeriesUid = new System.Windows.Forms.TextBox();
            this.ObjectUid = new System.Windows.Forms.TextBox();
            this.Retrieve = new System.Windows.Forms.Button();
            this.Browse = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.RetrieveFrame = new System.Windows.Forms.CheckBox();
            this.Frame = new System.Windows.Forms.TextBox();
            this.ContentTypes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Uri = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // BaseUri
            // 
            this.BaseUri.Location = new System.Drawing.Point(161, 12);
            this.BaseUri.Name = "BaseUri";
            this.BaseUri.Size = new System.Drawing.Size(259, 20);
            this.BaseUri.TabIndex = 0;
            this.BaseUri.Text = "http://localhost:1000/wado";
            // 
            // StudyUid
            // 
            this.StudyUid.Location = new System.Drawing.Point(161, 53);
            this.StudyUid.Name = "StudyUid";
            this.StudyUid.Size = new System.Drawing.Size(259, 20);
            this.StudyUid.TabIndex = 1;
            // 
            // SeriesUid
            // 
            this.SeriesUid.Location = new System.Drawing.Point(161, 79);
            this.SeriesUid.Name = "SeriesUid";
            this.SeriesUid.Size = new System.Drawing.Size(259, 20);
            this.SeriesUid.TabIndex = 1;
            // 
            // ObjectUid
            // 
            this.ObjectUid.Location = new System.Drawing.Point(161, 105);
            this.ObjectUid.Name = "ObjectUid";
            this.ObjectUid.Size = new System.Drawing.Size(259, 20);
            this.ObjectUid.TabIndex = 1;
            // 
            // Retrieve
            // 
            this.Retrieve.Location = new System.Drawing.Point(161, 213);
            this.Retrieve.Name = "Retrieve";
            this.Retrieve.Size = new System.Drawing.Size(75, 23);
            this.Retrieve.TabIndex = 2;
            this.Retrieve.Text = "Retrieve";
            this.Retrieve.UseVisualStyleBackColor = true;
            this.Retrieve.Click += new System.EventHandler(this.Retrieve_Click);
            // 
            // Browse
            // 
            this.Browse.Location = new System.Drawing.Point(441, 50);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(75, 23);
            this.Browse.TabIndex = 3;
            this.Browse.Text = "Browse";
            this.Browse.UseVisualStyleBackColor = true;
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // RetrieveFrame
            // 
            this.RetrieveFrame.AutoSize = true;
            this.RetrieveFrame.Location = new System.Drawing.Point(161, 141);
            this.RetrieveFrame.Name = "RetrieveFrame";
            this.RetrieveFrame.Size = new System.Drawing.Size(55, 17);
            this.RetrieveFrame.TabIndex = 6;
            this.RetrieveFrame.Text = "Frame";
            this.RetrieveFrame.UseVisualStyleBackColor = true;
            this.RetrieveFrame.CheckedChanged += new System.EventHandler(this.RetrieveFrame_CheckedChanged);
            // 
            // Frame
            // 
            this.Frame.Enabled = false;
            this.Frame.Location = new System.Drawing.Point(235, 138);
            this.Frame.Name = "Frame";
            this.Frame.Size = new System.Drawing.Size(100, 20);
            this.Frame.TabIndex = 7;
            this.Frame.Text = "0";
            // 
            // ContentTypes
            // 
            this.ContentTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ContentTypes.FormattingEnabled = true;
            this.ContentTypes.Location = new System.Drawing.Point(161, 174);
            this.ContentTypes.Name = "ContentTypes";
            this.ContentTypes.Size = new System.Drawing.Size(259, 21);
            this.ContentTypes.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Base Uri";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Study Instance Uid";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Series Uid";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(48, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Object Uid";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(48, 182);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Request Type";
            // 
            // Uri
            // 
            this.Uri.Location = new System.Drawing.Point(161, 267);
            this.Uri.Multiline = true;
            this.Uri.Name = "Uri";
            this.Uri.Size = new System.Drawing.Size(633, 86);
            this.Uri.TabIndex = 16;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 448);
            this.Controls.Add(this.Uri);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ContentTypes);
            this.Controls.Add(this.Frame);
            this.Controls.Add(this.RetrieveFrame);
            this.Controls.Add(this.Browse);
            this.Controls.Add(this.Retrieve);
            this.Controls.Add(this.ObjectUid);
            this.Controls.Add(this.SeriesUid);
            this.Controls.Add(this.StudyUid);
            this.Controls.Add(this.BaseUri);
            this.Name = "Form1";
            this.Text = "Image Streaming Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox BaseUri;
        private System.Windows.Forms.TextBox StudyUid;
        private System.Windows.Forms.TextBox SeriesUid;
        private System.Windows.Forms.TextBox ObjectUid;
        private System.Windows.Forms.Button Retrieve;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox RetrieveFrame;
        private System.Windows.Forms.TextBox Frame;
        private System.Windows.Forms.ComboBox ContentTypes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox Uri;
    }
}

