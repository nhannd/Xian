namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class PerformingDocumentationOrderDetailsComponentControl
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this._protocolsPanel = new System.Windows.Forms.Panel();
			this._orderNotesGroupBox = new System.Windows.Forms.GroupBox();
			this._rightHandPanel = new System.Windows.Forms.Panel();
			this._borderPanel = new System.Windows.Forms.Panel();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this._borderPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(4, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(0, 7, 0, 1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._rightHandPanel);
			this.splitContainer1.Size = new System.Drawing.Size(495, 361);
			this.splitContainer1.SplitterDistance = 237;
			this.splitContainer1.TabIndex = 0;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 7);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this._protocolsPanel);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this._orderNotesGroupBox);
			this.splitContainer2.Size = new System.Drawing.Size(237, 353);
			this.splitContainer2.SplitterDistance = 170;
			this.splitContainer2.TabIndex = 0;
			// 
			// _protocolsPanel
			// 
			this._protocolsPanel.BackColor = System.Drawing.SystemColors.ControlDark;
			this._protocolsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._protocolsPanel.Location = new System.Drawing.Point(0, 0);
			this._protocolsPanel.Name = "_protocolsPanel";
			this._protocolsPanel.Padding = new System.Windows.Forms.Padding(1);
			this._protocolsPanel.Size = new System.Drawing.Size(237, 170);
			this._protocolsPanel.TabIndex = 0;
			// 
			// _orderNotesGroupBox
			// 
			this._orderNotesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderNotesGroupBox.Location = new System.Drawing.Point(0, 0);
			this._orderNotesGroupBox.Name = "_orderNotesGroupBox";
			this._orderNotesGroupBox.Size = new System.Drawing.Size(237, 179);
			this._orderNotesGroupBox.TabIndex = 0;
			this._orderNotesGroupBox.TabStop = false;
			this._orderNotesGroupBox.Text = "Order Notes";
			// 
			// _rightHandPanel
			// 
			this._rightHandPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._rightHandPanel.Location = new System.Drawing.Point(0, 0);
			this._rightHandPanel.Name = "_rightHandPanel";
			this._rightHandPanel.Size = new System.Drawing.Size(254, 361);
			this._rightHandPanel.TabIndex = 0;
			// 
			// _borderPanel
			// 
			this._borderPanel.BackColor = System.Drawing.SystemColors.Control;
			this._borderPanel.Controls.Add(this.splitContainer1);
			this._borderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._borderPanel.Location = new System.Drawing.Point(1, 1);
			this._borderPanel.Name = "_borderPanel";
			this._borderPanel.Padding = new System.Windows.Forms.Padding(4, 0, 4, 4);
			this._borderPanel.Size = new System.Drawing.Size(503, 365);
			this._borderPanel.TabIndex = 1;
			// 
			// PerformingDocumentationOrderDetailsComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Controls.Add(this._borderPanel);
			this.Name = "PerformingDocumentationOrderDetailsComponentControl";
			this.Padding = new System.Windows.Forms.Padding(1);
			this.Size = new System.Drawing.Size(505, 367);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this._borderPanel.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Panel _protocolsPanel;
        private System.Windows.Forms.Panel _rightHandPanel;
		private System.Windows.Forms.GroupBox _orderNotesGroupBox;
		private System.Windows.Forms.Panel _borderPanel;

    }
}
