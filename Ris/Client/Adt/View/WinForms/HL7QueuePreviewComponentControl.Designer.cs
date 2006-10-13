namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class HL7QueuePreviewComponentControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._showAll = new System.Windows.Forms.Button();
            this._showPending = new System.Windows.Forms.Button();
            this._resync = new System.Windows.Forms.Button();
            this._process = new System.Windows.Forms.Button();
            this._queue = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._message = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._queue, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._message, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(432, 386);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this._showAll);
            this.flowLayoutPanel1.Controls.Add(this._showPending);
            this.flowLayoutPanel1.Controls.Add(this._resync);
            this.flowLayoutPanel1.Controls.Add(this._process);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 351);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(426, 32);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // _showAll
            // 
            this._showAll.Location = new System.Drawing.Point(348, 3);
            this._showAll.Name = "_showAll";
            this._showAll.Size = new System.Drawing.Size(75, 23);
            this._showAll.TabIndex = 0;
            this._showAll.Text = "All";
            this._showAll.UseVisualStyleBackColor = true;
            this._showAll.Click += new System.EventHandler(this._showAll_Click);
            // 
            // _showPending
            // 
            this._showPending.Location = new System.Drawing.Point(267, 3);
            this._showPending.Name = "_showPending";
            this._showPending.Size = new System.Drawing.Size(75, 23);
            this._showPending.TabIndex = 2;
            this._showPending.Text = "Pending";
            this._showPending.UseVisualStyleBackColor = true;
            this._showPending.Click += new System.EventHandler(this._showPending_Click);
            // 
            // _resync
            // 
            this._resync.Location = new System.Drawing.Point(186, 3);
            this._resync.Name = "_resync";
            this._resync.Size = new System.Drawing.Size(75, 23);
            this._resync.TabIndex = 3;
            this._resync.Text = "Re-Synch";
            this._resync.UseVisualStyleBackColor = true;
            this._resync.Click += new System.EventHandler(this._resync_Click);
            // 
            // _process
            // 
            this._process.Location = new System.Drawing.Point(105, 3);
            this._process.Name = "_process";
            this._process.Size = new System.Drawing.Size(75, 23);
            this._process.TabIndex = 1;
            this._process.Text = "Process";
            this._process.UseVisualStyleBackColor = true;
            this._process.Click += new System.EventHandler(this._process_Click);
            // 
            // _queue
            // 
            this._queue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._queue.Location = new System.Drawing.Point(3, 3);
            this._queue.MenuModel = null;
            this._queue.Name = "_queue";
            this._queue.ReadOnly = false;
            this._queue.Size = new System.Drawing.Size(426, 168);
            this._queue.TabIndex = 0;
            this._queue.Table = null;
            this._queue.ToolbarModel = null;
            this._queue.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._queue.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._queue.SelectionChanged += new System.EventHandler(this._queue_SelectionChanged);
            // 
            // _message
            // 
            this._message.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._message.Location = new System.Drawing.Point(3, 177);
            this._message.Multiline = true;
            this._message.Name = "_message";
            this._message.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._message.Size = new System.Drawing.Size(426, 168);
            this._message.TabIndex = 2;
            // 
            // HL7QueuePreviewComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "HL7QueuePreviewComponentControl";
            this.Size = new System.Drawing.Size(438, 392);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.TableView _queue;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _showAll;
        private System.Windows.Forms.Button _process;
        private System.Windows.Forms.TextBox _message;
        private System.Windows.Forms.Button _showPending;
        private System.Windows.Forms.Button _resync;

    }
}
