#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Thumbnails.View.WinForms
{
    partial class ThumbnailComponentControl
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
			this._splitContainer = new System.Windows.Forms.SplitContainer();
			this._imageSetTree = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
			this._galleryView = new ClearCanvas.Desktop.View.WinForms.GalleryView();
			this._splitContainer.Panel1.SuspendLayout();
			this._splitContainer.Panel2.SuspendLayout();
			this._splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// _splitContainer
			// 
			this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this._splitContainer.Location = new System.Drawing.Point(0, 0);
			this._splitContainer.Name = "_splitContainer";
			// 
			// _splitContainer.Panel1
			// 
			this._splitContainer.Panel1.Controls.Add(this._imageSetTree);
			// 
			// _splitContainer.Panel2
			// 
			this._splitContainer.Panel2.Controls.Add(this._galleryView);
			this._splitContainer.Size = new System.Drawing.Size(770, 166);
			this._splitContainer.SplitterDistance = 345;
			this._splitContainer.TabIndex = 2;
			// 
			// _imageSetTree
			// 
			this._imageSetTree.AllowDrop = true;
			this._imageSetTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this._imageSetTree.IconColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this._imageSetTree.IconSize = new System.Drawing.Size(16, 16);
			this._imageSetTree.Location = new System.Drawing.Point(0, 0);
			this._imageSetTree.Margin = new System.Windows.Forms.Padding(2);
			this._imageSetTree.Name = "_imageSetTree";
			this._imageSetTree.ShowToolbar = false;
			this._imageSetTree.Size = new System.Drawing.Size(345, 166);
			this._imageSetTree.TabIndex = 2;
			this._imageSetTree.TreeBackColor = System.Drawing.SystemColors.Window;
			this._imageSetTree.TreeForeColor = System.Drawing.SystemColors.WindowText;
			this._imageSetTree.TreeLineColor = System.Drawing.Color.Black;
			// 
			// _galleryView
			// 
			this._galleryView.DataSource = null;
			this._galleryView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._galleryView.DragOutside = false;
			this._galleryView.DragReorder = false;
			this._galleryView.Location = new System.Drawing.Point(0, 0);
			this._galleryView.MultiSelect = true;
			this._galleryView.Name = "_galleryView";
			this._galleryView.Size = new System.Drawing.Size(421, 166);
			this._galleryView.TabIndex = 1;
			// 
			// ThumbnailComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._splitContainer);
			this.Name = "ThumbnailComponentControl";
			this.Size = new System.Drawing.Size(770, 166);
			this._splitContainer.Panel1.ResumeLayout(false);
			this._splitContainer.Panel2.ResumeLayout(false);
			this._splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.SplitContainer _splitContainer;
		private ClearCanvas.Desktop.View.WinForms.BindingTreeView _imageSetTree;
		private ClearCanvas.Desktop.View.WinForms.GalleryView _galleryView;


	}
}
