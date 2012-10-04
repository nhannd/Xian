using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.ImageViewer.Services.Auditing;
using System.Net;
using System.Net.Sockets;
//using System;
using System.ServiceModel;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Services.Automation;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;

namespace JuniorProject
{
    public partial class EditQuery : Form
    {
        public EditQuery()
        {
            InitializeComponent();

            XmlDocument queryDoc = new XmlDocument();
            
            queryDoc.Load("XMLFile1.xml");

            StringWriter sw = new StringWriter();

            XmlWriterSettings xmlSettings = new XmlWriterSettings();

            xmlSettings.Encoding = Encoding.UTF8;
            xmlSettings.ConformanceLevel = ConformanceLevel.Fragment;
            xmlSettings.Indent = true;
            xmlSettings.NewLineOnAttributes = false;
            xmlSettings.CheckCharacters = true;
            xmlSettings.IndentChars = "  ";

            XmlWriter tw = XmlWriter.Create(sw, xmlSettings);
            if (tw != null)
            {
                queryDoc.WriteTo(tw);
                tw.Close();
            }
            textBox1.Text = sw.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            XmlDocument edited = new XmlDocument();
            edited.LoadXml(textBox1.Text);

            edited.Save("XMLFile1.xml");

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
