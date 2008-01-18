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
            this._editorTable = new System.Windows.Forms.TableLayoutPanel();
            this._initializeButton = new System.Windows.Forms.Button();
            this._uninitializeButon = new System.Windows.Forms.Button();
            this._controlPanel = new System.Windows.Forms.Panel();
            this._volumeMeter = new System.Windows.Forms.ProgressBar();
            this._recorder = new ClearCanvas.Ris.Client.SpeechMagic.View.WinForms.RecorderControl();
            this._volumeTimer = new System.Windows.Forms.Timer(this.components);
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._documentEditMode = new System.Windows.Forms.ToolStripStatusLabel();
            this._appState = new System.Windows.Forms.ToolStripStatusLabel();
            this._recognizerMode = new System.Windows.Forms.ToolStripStatusLabel();
            this._audioState = new System.Windows.Forms.ToolStripStatusLabel();
            this._richTextBox = new System.Windows.Forms.RichTextBox();
            this._editorTable.SuspendLayout();
            this._controlPanel.SuspendLayout();
            this._statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _editorTable
            // 
            this._editorTable.ColumnCount = 2;
            this._editorTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._editorTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._editorTable.Controls.Add(this._statusStrip, 0, 2);
            this._editorTable.Controls.Add(this._richTextBox, 0, 1);
            this._editorTable.Controls.Add(this._controlPanel, 0, 0);
            this._editorTable.Controls.Add(this._volumeMeter, 1, 1);
            this._editorTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._editorTable.Location = new System.Drawing.Point(0, 0);
            this._editorTable.Name = "_editorTable";
            this._editorTable.RowCount = 3;
            this._editorTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._editorTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._editorTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._editorTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._editorTable.Size = new System.Drawing.Size(515, 490);
            this._editorTable.TabIndex = 0;
            // 
            // _initializeButton
            // 
            this._initializeButton.Location = new System.Drawing.Point(135, 3);
            this._initializeButton.Name = "_initializeButton";
            this._initializeButton.Size = new System.Drawing.Size(75, 23);
            this._initializeButton.TabIndex = 1;
            this._initializeButton.Text = "Initialize";
            this._initializeButton.UseVisualStyleBackColor = true;
            this._initializeButton.Click += new System.EventHandler(this._initializeButton_Click);
            // 
            // _uninitializeButon
            // 
            this._uninitializeButon.Location = new System.Drawing.Point(216, 3);
            this._uninitializeButon.Name = "_uninitializeButon";
            this._uninitializeButon.Size = new System.Drawing.Size(75, 23);
            this._uninitializeButon.TabIndex = 2;
            this._uninitializeButon.Text = "Uninitialize";
            this._uninitializeButon.UseVisualStyleBackColor = true;
            this._uninitializeButon.Click += new System.EventHandler(this._uninitializeButon_Click);
            // 
            // _controlPanel
            // 
            this._editorTable.SetColumnSpan(this._controlPanel, 2);
            this._controlPanel.Controls.Add(this._uninitializeButon);
            this._controlPanel.Controls.Add(this._initializeButton);
            this._controlPanel.Controls.Add(this._recorder);
            this._controlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._controlPanel.Location = new System.Drawing.Point(3, 3);
            this._controlPanel.Name = "_controlPanel";
            this._controlPanel.Size = new System.Drawing.Size(509, 31);
            this._controlPanel.TabIndex = 4;
            // 
            // _volumeMeter
            // 
            this._volumeMeter.Dock = System.Windows.Forms.DockStyle.Fill;
            this._volumeMeter.Location = new System.Drawing.Point(498, 40);
            this._volumeMeter.Name = "_volumeMeter";
            this._volumeMeter.Size = new System.Drawing.Size(14, 427);
            this._volumeMeter.Step = 1;
            this._volumeMeter.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this._volumeMeter.TabIndex = 6;
            // 
            // _recorder
            // 
            this._recorder.Location = new System.Drawing.Point(3, 3);
            this._recorder.Name = "_recorder";
            this._recorder.Size = new System.Drawing.Size(126, 26);
            this._recorder.TabIndex = 5;
            // 
            // _volumeTimer
            // 
            this._volumeTimer.Tick += new System.EventHandler(this._volumeTimer_Tick);
            // 
            // _statusStrip
            // 
            this._editorTable.SetColumnSpan(this._statusStrip, 2);
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
            this._documentEditMode.Size = new System.Drawing.Size(86, 17);
            this._documentEditMode.Text = "Edit Mode: None";
            // 
            // _appState
            // 
            this._appState.Name = "_appState";
            this._appState.Size = new System.Drawing.Size(118, 17);
            this._appState.Text = "App State: Uninitialized";
            // 
            // _recognizerMode
            // 
            this._recognizerMode.Name = "_recognizerMode";
            this._recognizerMode.Size = new System.Drawing.Size(115, 17);
            this._recognizerMode.Text = "Recognizer Mod: None";
            // 
            // _audioState
            // 
            this._audioState.Name = "_audioState";
            this._audioState.Size = new System.Drawing.Size(110, 17);
            this._audioState.Text = "Audio State: Stopped";
            // 
            // _richTextBox
            // 
            this._richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._richTextBox.Location = new System.Drawing.Point(3, 40);
            this._richTextBox.Name = "_richTextBox";
            this._richTextBox.Size = new System.Drawing.Size(489, 427);
            this._richTextBox.TabIndex = 0;
            this._richTextBox.Text = "";
            // 
            // SpeechMagicControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._editorTable);
            this.Name = "SpeechMagicControl";
            this.Size = new System.Drawing.Size(515, 490);
            this.Load += new System.EventHandler(this.SpeechMagicControl_Load);
            this._editorTable.ResumeLayout(false);
            this._editorTable.PerformLayout();
            this._controlPanel.ResumeLayout(false);
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _editorTable;
        private System.Windows.Forms.Button _initializeButton;
        private System.Windows.Forms.Button _uninitializeButon;
        private System.Windows.Forms.Timer _volumeTimer;
        private RecorderControl _recorder;
        private System.Windows.Forms.Panel _controlPanel;
        private System.Windows.Forms.ProgressBar _volumeMeter;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _documentEditMode;
        private System.Windows.Forms.ToolStripStatusLabel _appState;
        private System.Windows.Forms.ToolStripStatusLabel _recognizerMode;
        private System.Windows.Forms.ToolStripStatusLabel _audioState;
        private System.Windows.Forms.RichTextBox _richTextBox;
    }
}
