namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class OrderNoteSummaryComponentControl
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
			this._noteList = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.SuspendLayout();
			// 
			// _noteList
			// 
			this._noteList.Dock = System.Windows.Forms.DockStyle.Fill;
			this._noteList.Location = new System.Drawing.Point(0, 0);
			this._noteList.MultiSelect = false;
			this._noteList.Name = "_noteList";
			this._noteList.ReadOnly = false;
			this._noteList.Size = new System.Drawing.Size(150, 150);
			this._noteList.TabIndex = 1;
			this._noteList.ItemDoubleClicked += new System.EventHandler(this._noteList_ItemDoubleClicked);
			// 
			// OrderNoteSummaryComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._noteList);
			this.Name = "OrderNoteSummaryComponentControl";
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _noteList;
    }
}
