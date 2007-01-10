using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PrimeClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _proxy = new SampleShred1InterfaceClient();
            _piProxy = new SampleShred2InterfaceClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int primeNumber = _proxy.GetLastPrimeFound();
            textBox1.Text = primeNumber.ToString();
        }

        private SampleShred1InterfaceClient _proxy;
        private SampleShred2InterfaceClient _piProxy;

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _proxy.Close();
            _piProxy.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string pi = _piProxy.GetLastPiFound();
            textBox2.Text = pi;
        }
    }
}