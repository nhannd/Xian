namespace ClearCanvas.Desktop.View.WinForms
{
    partial class TestComponentControl
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
            this._label = new System.Windows.Forms.Label();
            this._text = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._showMessageBox = new System.Windows.Forms.Button();
            this._showDialogBox = new System.Windows.Forms.Button();
            this._close = new System.Windows.Forms.Button();
            this._setTitle = new System.Windows.Forms.Button();
            this._modify = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _label
            // 
            this._label.AutoSize = true;
            this._label.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._label.Location = new System.Drawing.Point(39, 58);
            this._label.Name = "_label";
            this._label.Size = new System.Drawing.Size(77, 32);
            this._label.TabIndex = 0;
            this._label.Text = "label";
            // 
            // _text
            // 
            this._text.Location = new System.Drawing.Point(45, 150);
            this._text.Name = "_text";
            this._text.Size = new System.Drawing.Size(199, 22);
            this._text.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 130);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Text";
            // 
            // _showMessageBox
            // 
            this._showMessageBox.Location = new System.Drawing.Point(45, 260);
            this._showMessageBox.Name = "_showMessageBox";
            this._showMessageBox.Size = new System.Drawing.Size(132, 23);
            this._showMessageBox.TabIndex = 3;
            this._showMessageBox.Text = "Message Box";
            this._showMessageBox.UseVisualStyleBackColor = true;
            this._showMessageBox.Click += new System.EventHandler(this._showMessageBox_Click);
            // 
            // _showDialogBox
            // 
            this._showDialogBox.Location = new System.Drawing.Point(195, 260);
            this._showDialogBox.Name = "_showDialogBox";
            this._showDialogBox.Size = new System.Drawing.Size(132, 23);
            this._showDialogBox.TabIndex = 4;
            this._showDialogBox.Text = "Dialog Box";
            this._showDialogBox.UseVisualStyleBackColor = true;
            this._showDialogBox.Click += new System.EventHandler(this._showDialogBox_Click);
            // 
            // _close
            // 
            this._close.Location = new System.Drawing.Point(45, 306);
            this._close.Name = "_close";
            this._close.Size = new System.Drawing.Size(74, 23);
            this._close.TabIndex = 5;
            this._close.Text = "Close";
            this._close.UseVisualStyleBackColor = true;
            this._close.Click += new System.EventHandler(this._close_Click);
            // 
            // _setTitle
            // 
            this._setTitle.Location = new System.Drawing.Point(253, 149);
            this._setTitle.Name = "_setTitle";
            this._setTitle.Size = new System.Drawing.Size(102, 23);
            this._setTitle.TabIndex = 6;
            this._setTitle.Text = "Set Title";
            this._setTitle.UseVisualStyleBackColor = true;
            this._setTitle.Click += new System.EventHandler(this._setTitle_Click);
            // 
            // _modify
            // 
            this._modify.Location = new System.Drawing.Point(45, 206);
            this._modify.Name = "_modify";
            this._modify.Size = new System.Drawing.Size(74, 23);
            this._modify.TabIndex = 7;
            this._modify.Text = "Modify";
            this._modify.UseVisualStyleBackColor = true;
            this._modify.Click += new System.EventHandler(this._modify_Click);
            // 
            // TestComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._modify);
            this.Controls.Add(this._setTitle);
            this.Controls.Add(this._close);
            this.Controls.Add(this._showDialogBox);
            this.Controls.Add(this._showMessageBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._text);
            this.Controls.Add(this._label);
            this.Name = "TestComponentControl";
            this.Size = new System.Drawing.Size(422, 353);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _label;
        private System.Windows.Forms.TextBox _text;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _showMessageBox;
        private System.Windows.Forms.Button _showDialogBox;
        private System.Windows.Forms.Button _close;
        private System.Windows.Forms.Button _setTitle;
        private System.Windows.Forms.Button _modify;

    }
}
