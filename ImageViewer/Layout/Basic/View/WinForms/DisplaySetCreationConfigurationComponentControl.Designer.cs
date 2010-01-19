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

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    partial class DisplaySetCreationConfigurationComponentControl
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
			this._createSingleImageDisplaySets = new System.Windows.Forms.CheckBox();
			this._splitEchos = new System.Windows.Forms.CheckBox();
			this._showOriginalMultiEchoSeries = new System.Windows.Forms.CheckBox();
			this._splitMixedMultiframeSeries = new System.Windows.Forms.CheckBox();
			this._showOriginalMixedMultiframeSeries = new System.Windows.Forms.CheckBox();
			this._modality = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this._creationGroup = new System.Windows.Forms.GroupBox();
			this._presentationGroupBox = new System.Windows.Forms.GroupBox();
			this._invertImages = new System.Windows.Forms.CheckBox();
			this._creationGroup.SuspendLayout();
			this._presentationGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// _createSingleImageDisplaySets
			// 
			this._createSingleImageDisplaySets.AutoSize = true;
			this._createSingleImageDisplaySets.Location = new System.Drawing.Point(15, 24);
			this._createSingleImageDisplaySets.Name = "_createSingleImageDisplaySets";
			this._createSingleImageDisplaySets.Size = new System.Drawing.Size(175, 17);
			this._createSingleImageDisplaySets.TabIndex = 2;
			this._createSingleImageDisplaySets.Text = "Create single image display sets";
			this._createSingleImageDisplaySets.UseVisualStyleBackColor = true;
			// 
			// _splitEchos
			// 
			this._splitEchos.AutoSize = true;
			this._splitEchos.Location = new System.Drawing.Point(15, 47);
			this._splitEchos.Name = "_splitEchos";
			this._splitEchos.Size = new System.Drawing.Size(127, 17);
			this._splitEchos.TabIndex = 3;
			this._splitEchos.Text = "Split multi-echo series";
			this._splitEchos.UseVisualStyleBackColor = true;
			// 
			// _showOriginalMultiEchoSeries
			// 
			this._showOriginalMultiEchoSeries.AutoSize = true;
			this._showOriginalMultiEchoSeries.Location = new System.Drawing.Point(34, 70);
			this._showOriginalMultiEchoSeries.Name = "_showOriginalMultiEchoSeries";
			this._showOriginalMultiEchoSeries.Size = new System.Drawing.Size(119, 17);
			this._showOriginalMultiEchoSeries.TabIndex = 4;
			this._showOriginalMultiEchoSeries.Text = "Show original series";
			this._showOriginalMultiEchoSeries.UseVisualStyleBackColor = true;
			// 
			// _splitMixedMultiframeSeries
			// 
			this._splitMixedMultiframeSeries.AutoSize = true;
			this._splitMixedMultiframeSeries.Location = new System.Drawing.Point(15, 93);
			this._splitMixedMultiframeSeries.Name = "_splitMixedMultiframeSeries";
			this._splitMixedMultiframeSeries.Size = new System.Drawing.Size(159, 17);
			this._splitMixedMultiframeSeries.TabIndex = 5;
			this._splitMixedMultiframeSeries.Text = "Split mixed multi-frame series";
			this._splitMixedMultiframeSeries.UseVisualStyleBackColor = true;
			// 
			// _showOriginalMixedMultiframeSeries
			// 
			this._showOriginalMixedMultiframeSeries.AutoSize = true;
			this._showOriginalMixedMultiframeSeries.Location = new System.Drawing.Point(34, 116);
			this._showOriginalMixedMultiframeSeries.Name = "_showOriginalMixedMultiframeSeries";
			this._showOriginalMixedMultiframeSeries.Size = new System.Drawing.Size(119, 17);
			this._showOriginalMixedMultiframeSeries.TabIndex = 6;
			this._showOriginalMixedMultiframeSeries.Text = "Show original series";
			this._showOriginalMixedMultiframeSeries.UseVisualStyleBackColor = true;
			// 
			// _modality
			// 
			this._modality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._modality.FormattingEnabled = true;
			this._modality.Location = new System.Drawing.Point(15, 25);
			this._modality.MaxDropDownItems = 25;
			this._modality.Name = "_modality";
			this._modality.Size = new System.Drawing.Size(86, 21);
			this._modality.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(46, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Modality";
			// 
			// _creationGroup
			// 
			this._creationGroup.Controls.Add(this._showOriginalMultiEchoSeries);
			this._creationGroup.Controls.Add(this._createSingleImageDisplaySets);
			this._creationGroup.Controls.Add(this._splitEchos);
			this._creationGroup.Controls.Add(this._showOriginalMixedMultiframeSeries);
			this._creationGroup.Controls.Add(this._splitMixedMultiframeSeries);
			this._creationGroup.Location = new System.Drawing.Point(15, 52);
			this._creationGroup.Name = "_creationGroup";
			this._creationGroup.Size = new System.Drawing.Size(203, 149);
			this._creationGroup.TabIndex = 1;
			this._creationGroup.TabStop = false;
			this._creationGroup.Text = "Creation";
			// 
			// _presentationGroupBox
			// 
			this._presentationGroupBox.Controls.Add(this._invertImages);
			this._presentationGroupBox.Location = new System.Drawing.Point(15, 207);
			this._presentationGroupBox.Name = "_presentationGroupBox";
			this._presentationGroupBox.Size = new System.Drawing.Size(203, 45);
			this._presentationGroupBox.TabIndex = 7;
			this._presentationGroupBox.TabStop = false;
			this._presentationGroupBox.Text = "Presentation";
			// 
			// _invertImages
			// 
			this._invertImages.AutoSize = true;
			this._invertImages.Location = new System.Drawing.Point(15, 19);
			this._invertImages.Name = "_invertImages";
			this._invertImages.Size = new System.Drawing.Size(137, 17);
			this._invertImages.TabIndex = 8;
			this._invertImages.Text = "Invert grayscale images";
			this._invertImages.UseVisualStyleBackColor = true;
			// 
			// DisplaySetCreationConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._presentationGroupBox);
			this.Controls.Add(this._creationGroup);
			this.Controls.Add(this._modality);
			this.Controls.Add(this.label1);
			this.Name = "DisplaySetCreationConfigurationComponentControl";
			this.Size = new System.Drawing.Size(235, 271);
			this._creationGroup.ResumeLayout(false);
			this._creationGroup.PerformLayout();
			this._presentationGroupBox.ResumeLayout(false);
			this._presentationGroupBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.CheckBox _createSingleImageDisplaySets;
		private System.Windows.Forms.CheckBox _splitEchos;
		private System.Windows.Forms.CheckBox _showOriginalMultiEchoSeries;
		private System.Windows.Forms.CheckBox _splitMixedMultiframeSeries;
		private System.Windows.Forms.CheckBox _showOriginalMixedMultiframeSeries;
		private System.Windows.Forms.ComboBox _modality;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox _creationGroup;
		private System.Windows.Forms.GroupBox _presentationGroupBox;
		private System.Windows.Forms.CheckBox _invertImages;
    }
}
