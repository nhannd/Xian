using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Dashboard.Remote
{
    public partial class RetrieveProgressForm : Form
    {
        public RetrieveProgressForm(uint totalNumberOfImages)
        {
            InitializeComponent();

            // set up progress bar
            _progressRetrieve.Visible = true;
            _progressRetrieve.Minimum = 1;
            _progressRetrieve.Maximum = (int) totalNumberOfImages;
            _progressRetrieve.Value = 1;
            _progressRetrieve.Step = 1;
        }

        public void StepProgress()
        {
            _progressRetrieve.PerformStep();
        }

        public String DisplayText
        {
            get { return _labelDescription.Text; }
            set { _labelDescription.Text = value; }
        }
    }
}