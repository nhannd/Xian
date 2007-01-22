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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SampleShred1InterfaceClient proxy = new SampleShred1InterfaceClient();
            int primeNumber = proxy.GetLastPrimeFound();
            textBox1.Text = primeNumber.ToString();
            proxy.Close();
        }

        private SampleShred1InterfaceClient _proxy;
        private SampleShred2InterfaceClient _piProxy;

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SampleShred2InterfaceClient piProxy = new SampleShred2InterfaceClient();
            string pi = piProxy.GetLastPiFound();
            textBox2.Text = pi;
            piProxy.Close();
        }
    }
}