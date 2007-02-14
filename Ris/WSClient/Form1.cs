using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Ris.Services.Client;
using System.ServiceModel;

namespace WSClient
{
    public partial class Form1 : Form
    {
        TestServiceClient _testService;
        public Form1()
        {
            InitializeComponent();

            EndpointAddress endpoint = new EndpointAddress("http://localhost:8000/ClearCanvas.Ris.Services.ITestService/");
            BasicHttpBinding binding = new BasicHttpBinding();

            _testService = new TestServiceClient(binding, endpoint);
        }

        private void _sendButton_Click(object sender, EventArgs e)
        {
            try
            {
                _result.Text = _testService.GetPatientDetails(_number.Text).Mrn;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}