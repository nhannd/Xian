namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    partial class RecorderControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecorderControl));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._recordButton = new System.Windows.Forms.Button();
            this._playButton = new System.Windows.Forms.Button();
            this._stopButton = new System.Windows.Forms.Button();
            this._rewindButton = new System.Windows.Forms.Button();
            this._forwardButton = new System.Windows.Forms.Button();
            this._toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this._recordButton);
            this.flowLayoutPanel1.Controls.Add(this._playButton);
            this.flowLayoutPanel1.Controls.Add(this._stopButton);
            this.flowLayoutPanel1.Controls.Add(this._rewindButton);
            this.flowLayoutPanel1.Controls.Add(this._forwardButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(126, 25);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // _recordButton
            // 
            this._recordButton.Image = ((System.Drawing.Image)(resources.GetObject("_recordButton.Image")));
            this._recordButton.Location = new System.Drawing.Point(0, 0);
            this._recordButton.Margin = new System.Windows.Forms.Padding(0);
            this._recordButton.Name = "_recordButton";
            this._recordButton.Size = new System.Drawing.Size(25, 25);
            this._recordButton.TabIndex = 3;
            this._toolTip.SetToolTip(this._recordButton, "Start Recording");
            this._recordButton.UseVisualStyleBackColor = true;
            this._recordButton.Click += new System.EventHandler(this._recordButton_Click);
            // 
            // _playButton
            // 
            this._playButton.Enabled = false;
            this._playButton.Image = ((System.Drawing.Image)(resources.GetObject("_playButton.Image")));
            this._playButton.Location = new System.Drawing.Point(25, 0);
            this._playButton.Margin = new System.Windows.Forms.Padding(0);
            this._playButton.Name = "_playButton";
            this._playButton.Size = new System.Drawing.Size(25, 25);
            this._playButton.TabIndex = 1;
            this._toolTip.SetToolTip(this._playButton, "Start Playing");
            this._playButton.UseVisualStyleBackColor = true;
            this._playButton.Click += new System.EventHandler(this._playButton_Click);
            // 
            // _stopButton
            // 
            this._stopButton.Image = ((System.Drawing.Image)(resources.GetObject("_stopButton.Image")));
            this._stopButton.Location = new System.Drawing.Point(50, 0);
            this._stopButton.Margin = new System.Windows.Forms.Padding(0);
            this._stopButton.Name = "_stopButton";
            this._stopButton.Size = new System.Drawing.Size(25, 25);
            this._stopButton.TabIndex = 2;
            this._toolTip.SetToolTip(this._stopButton, "Stop");
            this._stopButton.UseVisualStyleBackColor = true;
            this._stopButton.Click += new System.EventHandler(this._stopButton_Click);
            // 
            // _rewindButton
            // 
            this._rewindButton.Enabled = false;
            this._rewindButton.Image = ((System.Drawing.Image)(resources.GetObject("_rewindButton.Image")));
            this._rewindButton.Location = new System.Drawing.Point(75, 0);
            this._rewindButton.Margin = new System.Windows.Forms.Padding(0);
            this._rewindButton.Name = "_rewindButton";
            this._rewindButton.Size = new System.Drawing.Size(25, 25);
            this._rewindButton.TabIndex = 4;
            this._toolTip.SetToolTip(this._rewindButton, "Fast Rewind");
            this._rewindButton.UseVisualStyleBackColor = true;
            this._rewindButton.Click += new System.EventHandler(this._rewindButton_Click);
            // 
            // _forwardButton
            // 
            this._forwardButton.Enabled = false;
            this._forwardButton.Image = ((System.Drawing.Image)(resources.GetObject("_forwardButton.Image")));
            this._forwardButton.Location = new System.Drawing.Point(100, 0);
            this._forwardButton.Margin = new System.Windows.Forms.Padding(0);
            this._forwardButton.Name = "_forwardButton";
            this._forwardButton.Size = new System.Drawing.Size(25, 25);
            this._forwardButton.TabIndex = 5;
            this._toolTip.SetToolTip(this._forwardButton, "Fast Forward");
            this._forwardButton.UseVisualStyleBackColor = true;
            this._forwardButton.Click += new System.EventHandler(this._forwardButton_Click);
            // 
            // _toolTip
            // 
            this._toolTip.ShowAlways = true;
            // 
            // RecorderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "RecorderControl";
            this.Size = new System.Drawing.Size(126, 25);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _playButton;
        private System.Windows.Forms.Button _stopButton;
        private System.Windows.Forms.Button _recordButton;
        private System.Windows.Forms.Button _rewindButton;
        private System.Windows.Forms.Button _forwardButton;
        private System.Windows.Forms.ToolTip _toolTip;
    }
}
