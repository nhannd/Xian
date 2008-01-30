namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class ValidationEditorComponentControl
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
            this._validationXml = new System.Windows.Forms.RichTextBox();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._propertiesTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._testButton = new System.Windows.Forms.Button();
            this._macroButton = new System.Windows.Forms.Button();
            this._propertiesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // _validationXml
            // 
            this._validationXml.AcceptsTab = true;
            this._validationXml.AutoWordSelection = true;
            this._validationXml.DetectUrls = false;
            this._validationXml.Location = new System.Drawing.Point(16, 195);
            this._validationXml.Name = "_validationXml";
            this._validationXml.Size = new System.Drawing.Size(479, 210);
            this._validationXml.TabIndex = 0;
            this._validationXml.Text = "";
            this._validationXml.WordWrap = false;
            // 
            // _okButton
            // 
            this._okButton.Location = new System.Drawing.Point(381, 411);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 1;
            this._okButton.Text = "Save";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(462, 411);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 2;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _propertiesTableView
            // 
            this._propertiesTableView.Location = new System.Drawing.Point(16, 12);
            this._propertiesTableView.Name = "_propertiesTableView";
            this._propertiesTableView.ReadOnly = false;
            this._propertiesTableView.Size = new System.Drawing.Size(521, 159);
            this._propertiesTableView.TabIndex = 3;
            this._propertiesTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            // 
            // _testButton
            // 
            this._testButton.Location = new System.Drawing.Point(300, 411);
            this._testButton.Name = "_testButton";
            this._testButton.Size = new System.Drawing.Size(75, 23);
            this._testButton.TabIndex = 4;
            this._testButton.Text = "Test";
            this._testButton.UseVisualStyleBackColor = true;
            this._testButton.Click += new System.EventHandler(this._testButton_Click);
            // 
            // _macroButton
            // 
            this._macroButton.Location = new System.Drawing.Point(501, 195);
            this._macroButton.Name = "_macroButton";
            this._macroButton.Size = new System.Drawing.Size(36, 23);
            this._macroButton.TabIndex = 5;
            this._macroButton.Text = "<<";
            this._macroButton.UseVisualStyleBackColor = true;
            this._macroButton.Click += new System.EventHandler(this._macroButton_Click);
            // 
            // _propertiesMenu
            // 
            this._propertiesMenu.Name = "_propertiesMenu";
            this._propertiesMenu.Size = new System.Drawing.Size(61, 4);
            this._propertiesMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._propertiesMenu_ItemClicked);
            // 
            // ValidationEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._macroButton);
            this.Controls.Add(this._testButton);
            this.Controls.Add(this._propertiesTableView);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._validationXml);
            this.Name = "ValidationEditorComponentControl";
            this.Size = new System.Drawing.Size(552, 442);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox _validationXml;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private ClearCanvas.Desktop.View.WinForms.TableView _propertiesTableView;
        private System.Windows.Forms.Button _testButton;
        private System.Windows.Forms.Button _macroButton;
        private System.Windows.Forms.ContextMenuStrip _propertiesMenu;
    }
}
