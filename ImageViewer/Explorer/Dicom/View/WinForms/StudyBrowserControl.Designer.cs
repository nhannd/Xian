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

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	partial class StudyBrowserControl
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
			this._resultsTitleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
			this._studyTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.SuspendLayout();
			// 
			// _resultsTitleBar
			// 
			this._resultsTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
			this._resultsTitleBar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._resultsTitleBar.GradientColoring = Crownwood.DotNetMagic.Controls.GradientColoring.LightBackToDarkBack;
			this._resultsTitleBar.Location = new System.Drawing.Point(0, 0);
			this._resultsTitleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._resultsTitleBar.Name = "_resultsTitleBar";
			this._resultsTitleBar.Size = new System.Drawing.Size(731, 30);
			this._resultsTitleBar.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
			this._resultsTitleBar.TabIndex = 3;
			this._resultsTitleBar.Text = "10 results found on server";
			// 
			// _studyTableView
			// 
			this._studyTableView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._studyTableView.Location = new System.Drawing.Point(0, 30);
			this._studyTableView.Name = "_studyTableView";
			this._studyTableView.ReadOnly = false;
			this._studyTableView.Size = new System.Drawing.Size(731, 325);
			this._studyTableView.SmartColumnSizing = true;
			this._studyTableView.TabIndex = 0;
			// 
			// StudyBrowserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._studyTableView);
			this.Controls.Add(this._resultsTitleBar);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "StudyBrowserControl";
			this.Size = new System.Drawing.Size(731, 355);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _studyTableView;
		private Crownwood.DotNetMagic.Controls.TitleBar _resultsTitleBar;


	}
}
