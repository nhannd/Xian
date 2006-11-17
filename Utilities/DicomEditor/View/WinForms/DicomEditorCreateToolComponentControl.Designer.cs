namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
    partial class DicomEditorCreateToolComponentControl
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
            this._tagName = new System.Windows.Forms.TextBox();
            this._tagNameLabel = new System.Windows.Forms.Label();
            this._accept = new System.Windows.Forms.Button();
            this._cancel = new System.Windows.Forms.Button();
            this._groupLabel = new System.Windows.Forms.Label();
            this._group = new System.Windows.Forms.TextBox();
            this._elementLabel = new System.Windows.Forms.Label();
            this._element = new System.Windows.Forms.TextBox();
            this._vr = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._value = new ClearCanvas.Controls.WinForms.TextField();
            this.SuspendLayout();
            // 
            // _tagName
            // 
            this._tagName.Location = new System.Drawing.Point(132, 32);
            this._tagName.Name = "_tagName";
            this._tagName.ReadOnly = true;
            this._tagName.Size = new System.Drawing.Size(299, 20);
            this._tagName.TabIndex = 3;
            // 
            // _tagNameLabel
            // 
            this._tagNameLabel.AutoSize = true;
            this._tagNameLabel.Location = new System.Drawing.Point(129, 12);
            this._tagNameLabel.Name = "_tagNameLabel";
            this._tagNameLabel.Size = new System.Drawing.Size(57, 13);
            this._tagNameLabel.TabIndex = 4;
            this._tagNameLabel.Text = "Tag Name";
            // 
            // _accept
            // 
            this._accept.Location = new System.Drawing.Point(310, 111);
            this._accept.Name = "_accept";
            this._accept.Size = new System.Drawing.Size(83, 31);
            this._accept.TabIndex = 9;
            this._accept.Text = "Accept";
            this._accept.UseVisualStyleBackColor = true;
            this._accept.Click += new System.EventHandler(this._accept_Click);
            // 
            // _cancel
            // 
            this._cancel.Location = new System.Drawing.Point(405, 111);
            this._cancel.Name = "_cancel";
            this._cancel.Size = new System.Drawing.Size(83, 31);
            this._cancel.TabIndex = 10;
            this._cancel.Text = "Cancel";
            this._cancel.UseVisualStyleBackColor = true;
            this._cancel.Click += new System.EventHandler(this._cancel_Click);
            // 
            // _groupLabel
            // 
            this._groupLabel.AutoSize = true;
            this._groupLabel.Location = new System.Drawing.Point(29, 12);
            this._groupLabel.Name = "_groupLabel";
            this._groupLabel.Size = new System.Drawing.Size(36, 13);
            this._groupLabel.TabIndex = 12;
            this._groupLabel.Text = "Group";
            // 
            // _group
            // 
            this._group.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this._group.Location = new System.Drawing.Point(32, 32);
            this._group.MaxLength = 4;
            this._group.Name = "_group";
            this._group.Size = new System.Drawing.Size(31, 20);
            this._group.TabIndex = 11;
            this._group.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // _elementLabel
            // 
            this._elementLabel.AutoSize = true;
            this._elementLabel.Location = new System.Drawing.Point(75, 12);
            this._elementLabel.Name = "_elementLabel";
            this._elementLabel.Size = new System.Drawing.Size(45, 13);
            this._elementLabel.TabIndex = 14;
            this._elementLabel.Text = "Element";
            // 
            // _element
            // 
            this._element.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this._element.Location = new System.Drawing.Point(78, 32);
            this._element.MaxLength = 4;
            this._element.Name = "_element";
            this._element.Size = new System.Drawing.Size(31, 20);
            this._element.TabIndex = 13;
            this._element.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // _vr
            // 
            this._vr.DataSource = null;
            this._vr.Enabled = false;
            this._vr.LabelText = "VR";
            this._vr.Location = new System.Drawing.Point(436, 12);
            this._vr.Margin = new System.Windows.Forms.Padding(2);
            this._vr.Name = "_vr";
            this._vr.Size = new System.Drawing.Size(52, 41);
            this._vr.TabIndex = 15;
            this._vr.Value = null;
            // 
            // _value
            // 
            this._value.LabelText = "Value";
            this._value.Location = new System.Drawing.Point(32, 65);
            this._value.Margin = new System.Windows.Forms.Padding(2);
            this._value.Name = "_value";
            this._value.Size = new System.Drawing.Size(456, 41);
            this._value.TabIndex = 16;
            this._value.Value = null;
            // 
            // DicomEditorCreateToolComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._value);
            this.Controls.Add(this._vr);
            this.Controls.Add(this._elementLabel);
            this.Controls.Add(this._element);
            this.Controls.Add(this._groupLabel);
            this.Controls.Add(this._group);
            this.Controls.Add(this._cancel);
            this.Controls.Add(this._accept);
            this.Controls.Add(this._tagNameLabel);
            this.Controls.Add(this._tagName);
            this.Name = "DicomEditorCreateToolComponentControl";
            this.Size = new System.Drawing.Size(521, 152);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _tagName;
        private System.Windows.Forms.Label _tagNameLabel;
        private System.Windows.Forms.Button _accept;
        private System.Windows.Forms.Button _cancel;
        private System.Windows.Forms.Label _groupLabel;
        private System.Windows.Forms.TextBox _group;
        private System.Windows.Forms.Label _elementLabel;
        private System.Windows.Forms.TextBox _element;
        private ClearCanvas.Controls.WinForms.ComboBoxField _vr;
        private ClearCanvas.Controls.WinForms.TextField _value;
    }
}
