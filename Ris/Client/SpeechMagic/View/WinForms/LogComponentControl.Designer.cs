namespace ClearCanvas.Ris.Client.SpeechMagic.View.WinForms
{
    partial class LogComponentControl
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._logGroupBox = new System.Windows.Forms.GroupBox();
            this._logs = new System.Windows.Forms.ListView();
            this._logContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._clearLogs = new System.Windows.Forms.ToolStripMenuItem();
            this._commandGroupBox = new System.Windows.Forms.GroupBox();
            this._commandLogs = new System.Windows.Forms.ListView();
            this._commandContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._clearCommands = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this._logGroupBox.SuspendLayout();
            this._logContextMenu.SuspendLayout();
            this._commandGroupBox.SuspendLayout();
            this._commandContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._logGroupBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._commandGroupBox);
            this.splitContainer1.Size = new System.Drawing.Size(453, 516);
            this.splitContainer1.SplitterDistance = 217;
            this.splitContainer1.TabIndex = 0;
            // 
            // _logGroupBox
            // 
            this._logGroupBox.Controls.Add(this._logs);
            this._logGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._logGroupBox.Location = new System.Drawing.Point(0, 0);
            this._logGroupBox.Name = "_logGroupBox";
            this._logGroupBox.Size = new System.Drawing.Size(453, 217);
            this._logGroupBox.TabIndex = 2;
            this._logGroupBox.TabStop = false;
            this._logGroupBox.Text = "Log";
            // 
            // _logs
            // 
            this._logs.ContextMenuStrip = this._logContextMenu;
            this._logs.Dock = System.Windows.Forms.DockStyle.Fill;
            this._logs.Location = new System.Drawing.Point(3, 16);
            this._logs.Name = "_logs";
            this._logs.ShowItemToolTips = true;
            this._logs.Size = new System.Drawing.Size(447, 198);
            this._logs.TabIndex = 0;
            this._logs.UseCompatibleStateImageBehavior = false;
            this._logs.View = System.Windows.Forms.View.Details;
            // 
            // _logContextMenu
            // 
            this._logContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._clearLogs});
            this._logContextMenu.Name = "_statusListContextMenu";
            this._logContextMenu.Size = new System.Drawing.Size(100, 26);
            // 
            // _clearLogs
            // 
            this._clearLogs.Name = "_clearLogs";
            this._clearLogs.Size = new System.Drawing.Size(99, 22);
            this._clearLogs.Text = "Clear";
            this._clearLogs.Click += new System.EventHandler(this._clearLog_Click);
            // 
            // _commandGroupBox
            // 
            this._commandGroupBox.Controls.Add(this._commandLogs);
            this._commandGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._commandGroupBox.Location = new System.Drawing.Point(0, 0);
            this._commandGroupBox.Name = "_commandGroupBox";
            this._commandGroupBox.Size = new System.Drawing.Size(453, 295);
            this._commandGroupBox.TabIndex = 3;
            this._commandGroupBox.TabStop = false;
            this._commandGroupBox.Text = "Commands Recognized";
            // 
            // _commandLogs
            // 
            this._commandLogs.ContextMenuStrip = this._commandContextMenu;
            this._commandLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this._commandLogs.Location = new System.Drawing.Point(3, 16);
            this._commandLogs.Name = "_commandLogs";
            this._commandLogs.ShowItemToolTips = true;
            this._commandLogs.Size = new System.Drawing.Size(447, 276);
            this._commandLogs.TabIndex = 0;
            this._commandLogs.UseCompatibleStateImageBehavior = false;
            this._commandLogs.View = System.Windows.Forms.View.Details;
            // 
            // _commandContextMenu
            // 
            this._commandContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._clearCommands});
            this._commandContextMenu.Name = "_commandListContextMenu";
            this._commandContextMenu.Size = new System.Drawing.Size(100, 26);
            // 
            // _clearCommands
            // 
            this._clearCommands.Name = "_clearCommands";
            this._clearCommands.Size = new System.Drawing.Size(99, 22);
            this._clearCommands.Text = "Clear";
            this._clearCommands.Click += new System.EventHandler(this._clearCommands_Click);
            // 
            // LogComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "LogComponentControl";
            this.Size = new System.Drawing.Size(453, 516);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this._logGroupBox.ResumeLayout(false);
            this._logContextMenu.ResumeLayout(false);
            this._commandGroupBox.ResumeLayout(false);
            this._commandContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox _logGroupBox;
        private System.Windows.Forms.GroupBox _commandGroupBox;
        private System.Windows.Forms.ContextMenuStrip _logContextMenu;
        private System.Windows.Forms.ToolStripMenuItem _clearLogs;
        private System.Windows.Forms.ContextMenuStrip _commandContextMenu;
        private System.Windows.Forms.ToolStripMenuItem _clearCommands;
        private System.Windows.Forms.ListView _logs;
        private System.Windows.Forms.ListView _commandLogs;
    }
}
