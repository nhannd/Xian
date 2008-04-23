namespace ClearCanvas.ImageServer.TestApp
{
    partial class TestEditStudyForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestEditStudyForm));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.imageServerDataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.imageServerDataSet = new ClearCanvas.ImageServer.TestApp.ImageServerDataSet();
            this.studyTableAdapter = new ClearCanvas.ImageServer.TestApp.ImageServerDataSetTableAdapters.StudyTableAdapter();
            this.studyStorageTableAdapter = new ClearCanvas.ImageServer.TestApp.ImageServerDataSetTableAdapters.StudyStorageTableAdapter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageServerDataSetBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageServerDataSet)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(914, 210);
            this.dataGridView1.TabIndex = 0;
            // 
            // imageServerDataSetBindingSource
            // 
            this.imageServerDataSetBindingSource.DataSource = this.imageServerDataSet;
            this.imageServerDataSetBindingSource.Position = 0;
            // 
            // imageServerDataSet
            // 
            this.imageServerDataSet.DataSetName = "ImageServerDataSet";
            this.imageServerDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // studyTableAdapter
            // 
            this.studyTableAdapter.ClearBeforeFill = true;
            // 
            // studyStorageTableAdapter
            // 
            this.studyStorageTableAdapter.ClearBeforeFill = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 210);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(914, 258);
            this.panel1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(691, 90);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(134, 46);
            this.button1.TabIndex = 1;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(626, 258);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // TestEditStudyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 469);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "TestEditStudyForm";
            this.Text = "TestEditStudy";
            this.Load += new System.EventHandler(this.TestEditStudyForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageServerDataSetBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageServerDataSet)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource imageServerDataSetBindingSource;
        private ImageServerDataSet imageServerDataSet;
        private ClearCanvas.ImageServer.TestApp.ImageServerDataSetTableAdapters.StudyTableAdapter studyTableAdapter;
        private ClearCanvas.ImageServer.TestApp.ImageServerDataSetTableAdapters.StudyStorageTableAdapter studyStorageTableAdapter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
    }
}