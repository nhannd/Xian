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

using System.Windows.Forms;
namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class PatientOverviewComponentControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatientOverviewComponentControl));
            this._alertList = new System.Windows.Forms.ListView();
            this._alertIcons = new System.Windows.Forms.ImageList(this.components);
            this._picture = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this._mrn = new System.Windows.Forms.Label();
            this._healthCard = new System.Windows.Forms.Label();
            this._dateOfBirth = new System.Windows.Forms.Label();
            this._ageSex = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._name = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._picture)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _alertList
            // 
            this._alertList.BackColor = System.Drawing.SystemColors.Control;
            this._alertList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._alertList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._alertList.HoverSelection = true;
            this._alertList.LargeImageList = this._alertIcons;
            this._alertList.Location = new System.Drawing.Point(500, 0);
            this._alertList.Margin = new System.Windows.Forms.Padding(0);
            this._alertList.MultiSelect = false;
            this._alertList.Name = "_alertList";
            this._alertList.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._alertList.RightToLeftLayout = true;
            this._alertList.Scrollable = false;
            this._alertList.ShowItemToolTips = true;
            this._alertList.Size = new System.Drawing.Size(500, 76);
            this._alertList.SmallImageList = this._alertIcons;
            this._alertList.TabIndex = 6;
            this._alertList.UseCompatibleStateImageBehavior = false;
            // 
            // _alertIcons
            // 
            this._alertIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_alertIcons.ImageStream")));
            this._alertIcons.TransparentColor = System.Drawing.Color.Transparent;
            this._alertIcons.Images.SetKeyName(0, "");
            this._alertIcons.Images.SetKeyName(1, "");
            this._alertIcons.Images.SetKeyName(2, "");
            this._alertIcons.Images.SetKeyName(3, "");
            this._alertIcons.Images.SetKeyName(4, "");
            this._alertIcons.Images.SetKeyName(5, "");
            // 
            // _picture
            // 
            this._picture.Dock = System.Windows.Forms.DockStyle.Fill;
            this._picture.Location = new System.Drawing.Point(2, 2);
            this._picture.Margin = new System.Windows.Forms.Padding(2);
            this._picture.Name = "_picture";
            this._picture.Size = new System.Drawing.Size(71, 72);
            this._picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._picture.TabIndex = 0;
            this._picture.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 500F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._picture, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._alertList, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1000, 76);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this._mrn, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this._healthCard, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this._dateOfBirth, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this._ageSex, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(75, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(425, 76);
            this.tableLayoutPanel2.TabIndex = 10;
            // 
            // _mrn
            // 
            this._mrn.AutoSize = true;
            this._mrn.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._mrn.Location = new System.Drawing.Point(2, 38);
            this._mrn.Margin = new System.Windows.Forms.Padding(2);
            this._mrn.Name = "_mrn";
            this._mrn.Size = new System.Drawing.Size(32, 16);
            this._mrn.TabIndex = 7;
            this._mrn.Text = "Mrn";
            // 
            // _healthCard
            // 
            this._healthCard.AutoSize = true;
            this._healthCard.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._healthCard.Location = new System.Drawing.Point(2, 58);
            this._healthCard.Margin = new System.Windows.Forms.Padding(2);
            this._healthCard.Name = "_healthCard";
            this._healthCard.Size = new System.Drawing.Size(79, 16);
            this._healthCard.TabIndex = 9;
            this._healthCard.Text = "Healthcard";
            // 
            // _dateOfBirth
            // 
            this._dateOfBirth.AutoSize = true;
            this._dateOfBirth.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._dateOfBirth.Location = new System.Drawing.Point(85, 58);
            this._dateOfBirth.Margin = new System.Windows.Forms.Padding(2);
            this._dateOfBirth.Name = "_dateOfBirth";
            this._dateOfBirth.Size = new System.Drawing.Size(35, 16);
            this._dateOfBirth.TabIndex = 10;
            this._dateOfBirth.Text = "DOB";
            // 
            // _ageSex
            // 
            this._ageSex.AutoSize = true;
            this._ageSex.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ageSex.Location = new System.Drawing.Point(85, 38);
            this._ageSex.Margin = new System.Windows.Forms.Padding(2);
            this._ageSex.Name = "_ageSex";
            this._ageSex.Size = new System.Drawing.Size(63, 16);
            this._ageSex.TabIndex = 8;
            this._ageSex.Text = "Age/Sex";
            // 
            // flowLayoutPanel1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this._name);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(425, 36);
            this.flowLayoutPanel1.TabIndex = 11;
            // 
            // _name
            // 
            this._name.AutoSize = true;
            this._name.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._name.Location = new System.Drawing.Point(2, 2);
            this._name.Margin = new System.Windows.Forms.Padding(2);
            this._name.Name = "_name";
            this._name.Size = new System.Drawing.Size(134, 23);
            this._name.TabIndex = 1;
            this._name.Text = "Patient name";
            // 
            // PatientOverviewComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(0, 76);
            this.MinimumSize = new System.Drawing.Size(1000, 76);
            this.Name = "PatientOverviewComponentControl";
            this.Size = new System.Drawing.Size(1000, 76);
            ((System.ComponentModel.ISupportInitialize)(this._picture)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ListView _alertList;
        private ImageList _alertIcons;
        private PictureBox _picture;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Label _mrn;
        private Label _ageSex;
        private Label _healthCard;
        private Label _dateOfBirth;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label _name;


    }
}
