#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    partial class LayoutConfigurationComponentControl
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._imageBoxLabelRows = new System.Windows.Forms.Label();
			this._imageBoxRows = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._imageBoxLabelColumns = new System.Windows.Forms.Label();
			this._imageBoxColumns = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._tileLabelRows = new System.Windows.Forms.Label();
			this._tileRows = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._tileLabelColumns = new System.Windows.Forms.Label();
			this._tileColumns = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this._modality = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxRows)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxColumns)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._tileRows)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._tileColumns)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._imageBoxLabelRows);
			this.groupBox1.Controls.Add(this._imageBoxRows);
			this.groupBox1.Controls.Add(this._imageBoxLabelColumns);
			this.groupBox1.Controls.Add(this._imageBoxColumns);
			this.groupBox1.Location = new System.Drawing.Point(15, 61);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(189, 77);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Image Box";
			// 
			// _imageBoxLabelRows
			// 
			this._imageBoxLabelRows.AutoSize = true;
			this._imageBoxLabelRows.Location = new System.Drawing.Point(15, 22);
			this._imageBoxLabelRows.Name = "_imageBoxLabelRows";
			this._imageBoxLabelRows.Size = new System.Drawing.Size(34, 13);
			this._imageBoxLabelRows.TabIndex = 2;
			this._imageBoxLabelRows.Text = "Rows";
			// 
			// _imageBoxRows
			// 
			this._imageBoxRows.Location = new System.Drawing.Point(18, 40);
			this._imageBoxRows.Name = "_imageBoxRows";
			this._imageBoxRows.Size = new System.Drawing.Size(60, 20);
			this._imageBoxRows.TabIndex = 3;
			// 
			// _imageBoxLabelColumns
			// 
			this._imageBoxLabelColumns.AutoSize = true;
			this._imageBoxLabelColumns.Location = new System.Drawing.Point(110, 22);
			this._imageBoxLabelColumns.Name = "_imageBoxLabelColumns";
			this._imageBoxLabelColumns.Size = new System.Drawing.Size(47, 13);
			this._imageBoxLabelColumns.TabIndex = 4;
			this._imageBoxLabelColumns.Text = "Columns";
			// 
			// _imageBoxColumns
			// 
			this._imageBoxColumns.Location = new System.Drawing.Point(113, 40);
			this._imageBoxColumns.Name = "_imageBoxColumns";
			this._imageBoxColumns.Size = new System.Drawing.Size(60, 20);
			this._imageBoxColumns.TabIndex = 5;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this._tileLabelRows);
			this.groupBox2.Controls.Add(this._tileRows);
			this.groupBox2.Controls.Add(this._tileLabelColumns);
			this.groupBox2.Controls.Add(this._tileColumns);
			this.groupBox2.Location = new System.Drawing.Point(15, 154);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(189, 77);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Tile";
			// 
			// _tileLabelRows
			// 
			this._tileLabelRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._tileLabelRows.AutoSize = true;
			this._tileLabelRows.Location = new System.Drawing.Point(15, 21);
			this._tileLabelRows.Name = "_tileLabelRows";
			this._tileLabelRows.Size = new System.Drawing.Size(34, 13);
			this._tileLabelRows.TabIndex = 7;
			this._tileLabelRows.Text = "Rows";
			// 
			// _tileRows
			// 
			this._tileRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._tileRows.Location = new System.Drawing.Point(18, 39);
			this._tileRows.Name = "_tileRows";
			this._tileRows.Size = new System.Drawing.Size(60, 20);
			this._tileRows.TabIndex = 8;
			// 
			// _tileLabelColumns
			// 
			this._tileLabelColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._tileLabelColumns.AutoSize = true;
			this._tileLabelColumns.Location = new System.Drawing.Point(110, 21);
			this._tileLabelColumns.Name = "_tileLabelColumns";
			this._tileLabelColumns.Size = new System.Drawing.Size(47, 13);
			this._tileLabelColumns.TabIndex = 9;
			this._tileLabelColumns.Text = "Columns";
			// 
			// _tileColumns
			// 
			this._tileColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._tileColumns.Location = new System.Drawing.Point(113, 39);
			this._tileColumns.Name = "_tileColumns";
			this._tileColumns.Size = new System.Drawing.Size(60, 20);
			this._tileColumns.TabIndex = 10;
			// 
			// _modality
			// 
			this._modality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._modality.FormattingEnabled = true;
			this._modality.Location = new System.Drawing.Point(15, 25);
			this._modality.MaxDropDownItems = 25;
			this._modality.Name = "_modality";
			this._modality.Size = new System.Drawing.Size(86, 21);
			this._modality.TabIndex = 11;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(46, 13);
			this.label1.TabIndex = 12;
			this.label1.Text = "Modality";
			// 
			// LayoutConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._modality);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "LayoutConfigurationComponentControl";
			this.Size = new System.Drawing.Size(224, 254);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxRows)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._imageBoxColumns)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._tileRows)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._tileColumns)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _imageBoxColumns;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label _imageBoxLabelColumns;
		private System.Windows.Forms.Label _imageBoxLabelRows;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _imageBoxRows;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label _tileLabelRows;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _tileRows;
		private System.Windows.Forms.Label _tileLabelColumns;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _tileColumns;
		private System.Windows.Forms.ComboBox _modality;
		private System.Windows.Forms.Label label1;
    }
}
