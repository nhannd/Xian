namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    partial class DictationEditorControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DictationEditorControl));
            this._editorTable = new System.Windows.Forms.TableLayoutPanel();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._documentEditMode = new System.Windows.Forms.ToolStripStatusLabel();
            this._appState = new System.Windows.Forms.ToolStripStatusLabel();
            this._recognizerMode = new System.Windows.Forms.ToolStripStatusLabel();
            this._audioState = new System.Windows.Forms.ToolStripStatusLabel();
            this._controlPanel = new System.Windows.Forms.Panel();
            this._recorderPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._recordButton = new System.Windows.Forms.Button();
            this._playButton = new System.Windows.Forms.Button();
            this._stopButton = new System.Windows.Forms.Button();
            this._rewindButton = new System.Windows.Forms.Button();
            this._forwardButton = new System.Windows.Forms.Button();
            this._recordedTime = new System.Windows.Forms.TextBox();
            this._trackBar = new System.Windows.Forms.TrackBar();
            this._volumeMeter = new System.Windows.Forms.ProgressBar();
            this._richTextBox = new System.Windows.Forms.RichTextBox();
            this._volumeTimer = new System.Windows.Forms.Timer(this.components);
            this._trackBarTimer = new System.Windows.Forms.Timer(this.components);
            this._previewStrip = new System.Windows.Forms.StatusStrip();
            this._preview = new System.Windows.Forms.ToolStripStatusLabel();
            this._editorTable.SuspendLayout();
            this._statusStrip.SuspendLayout();
            this._controlPanel.SuspendLayout();
            this._recorderPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._trackBar)).BeginInit();
            this._previewStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _editorTable
            // 
            this._editorTable.ColumnCount = 1;
            this._editorTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._editorTable.Controls.Add(this._statusStrip, 0, 3);
            this._editorTable.Controls.Add(this._controlPanel, 0, 0);
            this._editorTable.Controls.Add(this._richTextBox, 0, 1);
            this._editorTable.Controls.Add(this._previewStrip, 0, 2);
            this._editorTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._editorTable.Location = new System.Drawing.Point(0, 0);
            this._editorTable.Name = "_editorTable";
            this._editorTable.RowCount = 4;
            this._editorTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._editorTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._editorTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._editorTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._editorTable.Size = new System.Drawing.Size(515, 490);
            this._editorTable.TabIndex = 0;
            // 
            // _statusStrip
            // 
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._documentEditMode,
            this._appState,
            this._recognizerMode,
            this._audioState});
            this._statusStrip.Location = new System.Drawing.Point(0, 470);
            this._statusStrip.Name = "_statusStrip";
            this._statusStrip.Size = new System.Drawing.Size(515, 20);
            this._statusStrip.TabIndex = 1;
            this._statusStrip.Text = "Status";
            // 
            // _documentEditMode
            // 
            this._documentEditMode.Name = "_documentEditMode";
            this._documentEditMode.Size = new System.Drawing.Size(86, 15);
            this._documentEditMode.Text = "Edit Mode: None";
            // 
            // _appState
            // 
            this._appState.Name = "_appState";
            this._appState.Size = new System.Drawing.Size(118, 15);
            this._appState.Text = "App State: Uninitialized";
            // 
            // _recognizerMode
            // 
            this._recognizerMode.Name = "_recognizerMode";
            this._recognizerMode.Size = new System.Drawing.Size(115, 15);
            this._recognizerMode.Text = "Recognizer Mod: None";
            // 
            // _audioState
            // 
            this._audioState.Name = "_audioState";
            this._audioState.Size = new System.Drawing.Size(110, 15);
            this._audioState.Text = "Audio State: Stopped";
            // 
            // _controlPanel
            // 
            this._controlPanel.Controls.Add(this._recorderPanel);
            this._controlPanel.Controls.Add(this._recordedTime);
            this._controlPanel.Controls.Add(this._trackBar);
            this._controlPanel.Controls.Add(this._volumeMeter);
            this._controlPanel.Location = new System.Drawing.Point(3, 3);
            this._controlPanel.Name = "_controlPanel";
            this._controlPanel.Size = new System.Drawing.Size(509, 53);
            this._controlPanel.TabIndex = 4;
            // 
            // _recorderPanel
            // 
            this._recorderPanel.Controls.Add(this._recordButton);
            this._recorderPanel.Controls.Add(this._playButton);
            this._recorderPanel.Controls.Add(this._stopButton);
            this._recorderPanel.Controls.Add(this._rewindButton);
            this._recorderPanel.Controls.Add(this._forwardButton);
            this._recorderPanel.Location = new System.Drawing.Point(165, 22);
            this._recorderPanel.Name = "_recorderPanel";
            this._recorderPanel.Size = new System.Drawing.Size(127, 26);
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
            // _recordedTime
            // 
            this._recordedTime.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._recordedTime.Location = new System.Drawing.Point(299, 4);
            this._recordedTime.Name = "_recordedTime";
            this._recordedTime.ReadOnly = true;
            this._recordedTime.Size = new System.Drawing.Size(100, 20);
            this._recordedTime.TabIndex = 8;
            this._recordedTime.TabStop = false;
            // 
            // _trackBar
            // 
            this._trackBar.Location = new System.Drawing.Point(3, 3);
            this._trackBar.Name = "_trackBar";
            this._trackBar.Size = new System.Drawing.Size(157, 45);
            this._trackBar.TabIndex = 7;
            this._trackBar.TabStop = false;
            this._trackBar.Scroll += new System.EventHandler(this._trackBar_Scroll);
            // 
            // _volumeMeter
            // 
            this._volumeMeter.Location = new System.Drawing.Point(166, 3);
            this._volumeMeter.Name = "_volumeMeter";
            this._volumeMeter.Size = new System.Drawing.Size(126, 13);
            this._volumeMeter.Step = 1;
            this._volumeMeter.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this._volumeMeter.TabIndex = 6;
            // 
            // _richTextBox
            // 
            this._richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._richTextBox.Location = new System.Drawing.Point(3, 62);
            this._richTextBox.Name = "_richTextBox";
            this._richTextBox.Size = new System.Drawing.Size(509, 385);
            this._richTextBox.TabIndex = 0;
            this._richTextBox.Text = "";
            // 
            // _volumeTimer
            // 
            this._volumeTimer.Tick += new System.EventHandler(this._volumeTimer_Tick);
            // 
            // _trackBarTimer
            // 
            this._trackBarTimer.Tick += new System.EventHandler(this._trackBarTimer_Tick);
            // 
            // _previewStrip
            // 
            this._previewStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._preview});
            this._previewStrip.Location = new System.Drawing.Point(0, 450);
            this._previewStrip.Name = "_previewStrip";
            this._previewStrip.Size = new System.Drawing.Size(515, 20);
            this._previewStrip.TabIndex = 5;
            this._previewStrip.Text = "statusStrip1";
            // 
            // _preview
            // 
            this._preview.Name = "_preview";
            this._preview.Size = new System.Drawing.Size(70, 15);
            this._preview.Text = "Preview Text";
            // 
            // DictationEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._editorTable);
            this.Name = "DictationEditorControl";
            this.Size = new System.Drawing.Size(515, 490);
            this.Load += new System.EventHandler(this.SpeechMagicControl_Load);
            this._editorTable.ResumeLayout(false);
            this._editorTable.PerformLayout();
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this._controlPanel.ResumeLayout(false);
            this._controlPanel.PerformLayout();
            this._recorderPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._trackBar)).EndInit();
            this._previewStrip.ResumeLayout(false);
            this._previewStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _editorTable;
        private System.Windows.Forms.Timer _volumeTimer;
        private System.Windows.Forms.Panel _controlPanel;
        private System.Windows.Forms.ProgressBar _volumeMeter;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _documentEditMode;
        private System.Windows.Forms.ToolStripStatusLabel _appState;
        private System.Windows.Forms.ToolStripStatusLabel _recognizerMode;
        private System.Windows.Forms.ToolStripStatusLabel _audioState;
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
        private System.Windows.Forms.StatusStrip _previewStrip;
        private System.Windows.Forms.ToolStripStatusLabel _preview;
    }
}
