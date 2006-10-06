namespace ClearCanvas.Dicom.Services.View.WinForms
{
    partial class QueueDisplayControl
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
            this._removeButton = new System.Windows.Forms.Button();
            this._pauseButton = new System.Windows.Forms.Button();
            this._abortButton = new System.Windows.Forms.Button();
            this._parcelTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._refreshButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _removeButton
            // 
            this._removeButton.Location = new System.Drawing.Point(640, 446);
            this._removeButton.Name = "_removeButton";
            this._removeButton.Size = new System.Drawing.Size(75, 23);
            this._removeButton.TabIndex = 1;
            this._removeButton.Text = "Remove";
            this._removeButton.UseVisualStyleBackColor = true;
            // 
            // _pauseButton
            // 
            this._pauseButton.Location = new System.Drawing.Point(721, 446);
            this._pauseButton.Name = "_pauseButton";
            this._pauseButton.Size = new System.Drawing.Size(75, 23);
            this._pauseButton.TabIndex = 2;
            this._pauseButton.Text = "Pause";
            this._pauseButton.UseVisualStyleBackColor = true;
            // 
            // _abortButton
            // 
            this._abortButton.Location = new System.Drawing.Point(802, 446);
            this._abortButton.Name = "_abortButton";
            this._abortButton.Size = new System.Drawing.Size(75, 23);
            this._abortButton.TabIndex = 3;
            this._abortButton.Text = "Abort";
            this._abortButton.UseVisualStyleBackColor = true;
            // 
            // _parcelTableView
            // 
            this._parcelTableView.Table = null;
            this._parcelTableView.Location = new System.Drawing.Point(16, 14);
            this._parcelTableView.Margin = new System.Windows.Forms.Padding(4);
            this._parcelTableView.MenuModel = null;
            this._parcelTableView.Name = "_parcelTableView";
            this._parcelTableView.Size = new System.Drawing.Size(861, 416);
            this._parcelTableView.TabIndex = 4;
            this._parcelTableView.ToolbarModel = null;
            this._parcelTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._parcelTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _refreshButton
            // 
            this._refreshButton.Location = new System.Drawing.Point(559, 446);
            this._refreshButton.Name = "_refreshButton";
            this._refreshButton.Size = new System.Drawing.Size(75, 23);
            this._refreshButton.TabIndex = 5;
            this._refreshButton.Text = "Refresh";
            this._refreshButton.UseVisualStyleBackColor = true;
            // 
            // QueueDisplayControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._refreshButton);
            this.Controls.Add(this._parcelTableView);
            this.Controls.Add(this._abortButton);
            this.Controls.Add(this._pauseButton);
            this.Controls.Add(this._removeButton);
            this.Name = "QueueDisplayControl";
            this.Size = new System.Drawing.Size(893, 498);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _removeButton;
        private System.Windows.Forms.Button _pauseButton;
        private System.Windows.Forms.Button _abortButton;
        private ClearCanvas.Desktop.View.WinForms.TableView _parcelTableView;
        private System.Windows.Forms.Button _refreshButton;
    }
}
