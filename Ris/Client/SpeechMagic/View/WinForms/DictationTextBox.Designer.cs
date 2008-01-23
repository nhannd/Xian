namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    partial class DictationTextBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DictationTextBox));
            this._editorTable = new System.Windows.Forms.TableLayoutPanel();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._volumeMeter = new System.Windows.Forms.ToolStripProgressBar();
            this._preview = new System.Windows.Forms.ToolStripStatusLabel();
            this._separator = new System.Windows.Forms.ToolStripSeparator();
            this._appState = new System.Windows.Forms.ToolStripLabel();
            this._recognizerMode = new System.Windows.Forms.ToolStripLabel();
            this._documentEditMode = new System.Windows.Forms.ToolStripLabel();
            this._audioState = new System.Windows.Forms.ToolStripLabel();
            this._recordedTime = new System.Windows.Forms.TextBox();
            this._trackBar = new System.Windows.Forms.TrackBar();
            this._richTextBox = new System.Windows.Forms.RichTextBox();
            this._recorderPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._recordButton = new System.Windows.Forms.Button();
            this._playButton = new System.Windows.Forms.Button();
            this._stopButton = new System.Windows.Forms.Button();
            this._rewindButton = new System.Windows.Forms.Button();
            this._forwardButton = new System.Windows.Forms.Button();
            this._volumeTimer = new System.Windows.Forms.Timer(this.components);
            this._trackBarTimer = new System.Windows.Forms.Timer(this.components);
            this._editorTable.SuspendLayout();
            this._statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._trackBar)).BeginInit();
            this._recorderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _editorTable
            // 
            this._editorTable.ColumnCount = 3;
            this._editorTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this._editorTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._editorTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this._editorTable.Controls.Add(this._statusStrip, 0, 2);
            this._editorTable.Controls.Add(this._recordedTime, 1, 0);
            this._editorTable.Controls.Add(this._trackBar, 1, 0);
            this._editorTable.Controls.Add(this._richTextBox, 0, 1);
            this._editorTable.Controls.Add(this._recorderPanel, 0, 0);
            this._editorTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._editorTable.Location = new System.Drawing.Point(0, 0);
            this._editorTable.Name = "_editorTable";
            this._editorTable.RowCount = 3;
            this._editorTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._editorTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._editorTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._editorTable.Size = new System.Drawing.Size(564, 490);
            this._editorTable.TabIndex = 0;
            // 
            // _statusStrip
            // 
            this._editorTable.SetColumnSpan(this._statusStrip, 3);
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._volumeMeter,
            this._preview,
            this._separator,
            this._appState,
            this._recognizerMode,
            this._documentEditMode,
            this._audioState});
            this._statusStrip.Location = new System.Drawing.Point(0, 467);
            this._statusStrip.Name = "_statusStrip";
            this._statusStrip.ShowItemToolTips = true;
            this._statusStrip.Size = new System.Drawing.Size(564, 23);
            this._statusStrip.TabIndex = 1;
            // 
            // _volumeMeter
            // 
            this._volumeMeter.Name = "_volumeMeter";
            this._volumeMeter.Size = new System.Drawing.Size(100, 17);
            this._volumeMeter.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this._volumeMeter.ToolTipText = "Volume";
            // 
            // _preview
            // 
            this._preview.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this._preview.Name = "_preview";
            this._preview.Size = new System.Drawing.Size(147, 18);
            this._preview.Spring = true;
            this._preview.Text = "Preview";
            this._preview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _separator
            // 
            this._separator.Name = "_separator";
            this._separator.Size = new System.Drawing.Size(6, 23);
            // 
            // _appState
            // 
            this._appState.ForeColor = System.Drawing.Color.SteelBlue;
            this._appState.Name = "_appState";
            this._appState.Size = new System.Drawing.Size(88, 21);
            this._appState.Text = "Application State";
            this._appState.ToolTipText = "Application State";
            // 
            // _recognizerMode
            // 
            this._recognizerMode.Name = "_recognizerMode";
            this._recognizerMode.Size = new System.Drawing.Size(89, 21);
            this._recognizerMode.Text = "Recognizer Mode";
            this._recognizerMode.ToolTipText = "Recognizer Mode";
            // 
            // _documentEditMode
            // 
            this._documentEditMode.Name = "_documentEditMode";
            this._documentEditMode.Size = new System.Drawing.Size(54, 21);
            this._documentEditMode.Text = "Edit Mode";
            this._documentEditMode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._documentEditMode.ToolTipText = "Edit Mode";
            // 
            // _audioState
            // 
            this._audioState.Name = "_audioState";
            this._audioState.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._audioState.Size = new System.Drawing.Size(63, 21);
            this._audioState.Text = "Audio State";
            this._audioState.ToolTipText = "Audio State";
            // 
            // _recordedTime
            // 
            this._recordedTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._recordedTime.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._recordedTime.Dock = System.Windows.Forms.DockStyle.Top;
            this._recordedTime.Location = new System.Drawing.Point(457, 3);
            this._recordedTime.Name = "_recordedTime";
            this._recordedTime.ReadOnly = true;
            this._recordedTime.Size = new System.Drawing.Size(104, 13);
            this._recordedTime.TabIndex = 8;
            this._recordedTime.TabStop = false;
            this._recordedTime.Text = "0 of 0 s";
            // 
            // _trackBar
            // 
            this._trackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this._trackBar.Location = new System.Drawing.Point(138, 3);
            this._trackBar.Name = "_trackBar";
            this._trackBar.Size = new System.Drawing.Size(313, 45);
            this._trackBar.TabIndex = 7;
            this._trackBar.TabStop = false;
            this._trackBar.Scroll += new System.EventHandler(this._trackBar_Scroll);
            // 
            // _richTextBox
            // 
            this._editorTable.SetColumnSpan(this._richTextBox, 3);
            this._richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._richTextBox.Location = new System.Drawing.Point(3, 54);
            this._richTextBox.Name = "_richTextBox";
            this._richTextBox.Size = new System.Drawing.Size(558, 410);
            this._richTextBox.TabIndex = 0;
            this._richTextBox.Text = "";
            // 
            // _recorderPanel
            // 
            this._recorderPanel.Controls.Add(this._recordButton);
            this._recorderPanel.Controls.Add(this._playButton);
            this._recorderPanel.Controls.Add(this._stopButton);
            this._recorderPanel.Controls.Add(this._rewindButton);
            this._recorderPanel.Controls.Add(this._forwardButton);
            this._recorderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._recorderPanel.Enabled = false;
            this._recorderPanel.Location = new System.Drawing.Point(3, 3);
            this._recorderPanel.Name = "_recorderPanel";
            this._recorderPanel.Size = new System.Drawing.Size(129, 26);
            this._recorderPanel.TabIndex = 14;
            // 
            // _recordButton
            // 
            this._recordButton.Enabled = false;
            this._recordButton.Image = ((System.Drawing.Image)(resources.GetObject("_recordButton.Image")));
            this._recordButton.Location = new System.Drawing.Point(0, 0);
            this._recordButton.Margin = new System.Windows.Forms.Padding(0);
            this._recordButton.Name = "_recordButton";
            this._recordButton.Size = new System.Drawing.Size(25, 25);
            this._recordButton.TabIndex = 1;
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
            this._playButton.TabIndex = 2;
            this._playButton.UseVisualStyleBackColor = true;
            this._playButton.Click += new System.EventHandler(this._playButton_Click);
            // 
            // _stopButton
            // 
            this._stopButton.Enabled = false;
            this._stopButton.Image = ((System.Drawing.Image)(resources.GetObject("_stopButton.Image")));
            this._stopButton.Location = new System.Drawing.Point(50, 0);
            this._stopButton.Margin = new System.Windows.Forms.Padding(0);
            this._stopButton.Name = "_stopButton";
            this._stopButton.Size = new System.Drawing.Size(25, 25);
            this._stopButton.TabIndex = 3;
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
            this._forwardButton.TabIndex = 13;
            this._forwardButton.UseVisualStyleBackColor = true;
            this._forwardButton.Click += new System.EventHandler(this._forwardButton_Click);
            // 
            // _volumeTimer
            // 
            this._volumeTimer.Tick += new System.EventHandler(this._volumeTimer_Tick);
            // 
            // _trackBarTimer
            // 
            this._trackBarTimer.Tick += new System.EventHandler(this._trackBarTimer_Tick);
            // 
            // DictationTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._editorTable);
            this.Name = "DictationTextBox";
            this.Size = new System.Drawing.Size(564, 490);
            this.Load += new System.EventHandler(this.SpeechMagicControl_Load);
            this._editorTable.ResumeLayout(false);
            this._editorTable.PerformLayout();
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._trackBar)).EndInit();
            this._recorderPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _editorTable;
        private System.Windows.Forms.Timer _volumeTimer;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripLabel _documentEditMode;
        private System.Windows.Forms.ToolStripLabel _appState;
        private System.Windows.Forms.ToolStripLabel _recognizerMode;
        private System.Windows.Forms.ToolStripLabel _audioState;
        private System.Windows.Forms.TrackBar _trackBar;
        private System.Windows.Forms.TextBox _recordedTime;
        private System.Windows.Forms.RichTextBox _richTextBox;
        private System.Windows.Forms.Button _recordButton;
        private System.Windows.Forms.Button _playButton;
        private System.Windows.Forms.Button _stopButton;
        private System.Windows.Forms.Button _rewindButton;
        private System.Windows.Forms.Button _forwardButton;
        private System.Windows.Forms.FlowLayoutPanel _recorderPanel;
        private System.Windows.Forms.Timer _trackBarTimer;
        private System.Windows.Forms.ToolStripProgressBar _volumeMeter;
        private System.Windows.Forms.ToolStripStatusLabel _preview;
        private System.Windows.Forms.ToolStripSeparator _separator;
    }
}
