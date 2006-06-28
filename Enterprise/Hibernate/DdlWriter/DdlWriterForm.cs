using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
    public partial class DdlWriterForm : Form
    {
        #region Fields
        DdlWriter _writer;
        #endregion

        #region Constructors
        public DdlWriterForm()
        {
            InitializeComponent();
            _writer = new DdlWriter();
            _writer.Changed += WriterStatusChanged;

            _txtCreate.Text = _writer.CreateSchemaFileName;
            _txtDrop.Text = _writer.DropSchemaFileName;
        }
        #endregion

        #region Event Callbacks

        #region WinForm Events

        private void _btnStart_Click(object sender, EventArgs e)
        {
            _txtStatus.Text = "";
 
            _writer.CreateSchemaFileName = _txtCreate.Text;
            _writer.DropSchemaFileName = _txtDrop.Text;

            Thread t = new Thread( new ThreadStart( _writer.Execute ));
            t.Start();
        }

        private void _btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void _btnCreateFile_Click(object sender, EventArgs e)
        {
            FindFile(_txtCreate);
        }

        private void _btnDropFile_Click(object sender, EventArgs e)
        {
            FindFile(_txtDrop);
        }

        #endregion

        #region Custom Events

        delegate void SetTextCallback(String description);
        private void WriterStatusChanged(String description)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this._txtStatus.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(WriterStatusChanged);
                this.Invoke(d, new object[] { description });
            }
            else
            {
                this._txtStatus.Text += description + "\r\n";
            }
        }

        #endregion

        #endregion

        #region Methods

        private void FindFile(TextBox textBox)
        {
            OpenFileDialog selectFile = new OpenFileDialog();
            selectFile.Filter = "SQL files (*.sql, *.ddl)|*.sql;*.ddl";
            selectFile.FileName = textBox.Text;

            if (selectFile.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = selectFile.FileName;
            }
        }

        #endregion
    }
}