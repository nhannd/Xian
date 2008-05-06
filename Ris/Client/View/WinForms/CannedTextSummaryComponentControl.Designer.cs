namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class CannedTextSummaryComponentControl
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
			this._cannedTexts = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._text = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _cannedTexts
			// 
			this._cannedTexts.Dock = System.Windows.Forms.DockStyle.Fill;
			this._cannedTexts.Location = new System.Drawing.Point(0, 0);
			this._cannedTexts.Name = "_cannedTexts";
			this._cannedTexts.ReadOnly = false;
			this._cannedTexts.Size = new System.Drawing.Size(301, 179);
			this._cannedTexts.TabIndex = 0;
			this._cannedTexts.TabStop = false;
			this._cannedTexts.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
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
			this.splitContainer1.Panel1.Controls.Add(this._cannedTexts);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._text);
			this.splitContainer1.Size = new System.Drawing.Size(301, 372);
			this.splitContainer1.SplitterDistance = 179;
			this.splitContainer1.TabIndex = 0;
			this.splitContainer1.TabStop = false;
			// 
			// _text
			// 
			this._text.Dock = System.Windows.Forms.DockStyle.Fill;
			this._text.LabelText = "Text";
			this._text.Location = new System.Drawing.Point(0, 0);
			this._text.Margin = new System.Windows.Forms.Padding(2);
			this._text.Name = "_text";
			this._text.ReadOnly = true;
			this._text.Size = new System.Drawing.Size(301, 189);
			this._text.TabIndex = 3;
			this._text.Value = null;
			// 
			// CannedTextSummaryComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "CannedTextSummaryComponentControl";
			this.Size = new System.Drawing.Size(301, 372);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _cannedTexts;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private ClearCanvas.Desktop.View.WinForms.TextAreaField _text;
    }
}
