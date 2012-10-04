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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            //Logger.RegisterLogHandler(textBox1);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            XmlDocument queryXML = new XmlDocument();

            queryXML.Load("XMLFile1.xml");

            InstanceXml instanceXML = new InstanceXml(queryXML.DocumentElement, null);
            DicomAttributeCollection queryMessage = instanceXML.Collection;

            StudyRootFindScu findScu = new StudyRootFindScu();
            IList<DicomAttributeCollection> resultsList;

            resultsList = findScu.Find("SERVERAE", "SERVERAE", "localhost", 104, queryMessage);

            findScu.Dispose();

            string resultString = String.Format("Query returned {0} studies!\n\n", resultsList.Count());
            textBox1.AppendText(resultString);

            foreach (DicomAttributeCollection msg in resultsList)
            {
                textBox1.AppendText(msg.DumpString);
                textBox1.AppendText("\n");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EditQuery edit = new EditQuery();



            edit.Show();
        }
    }
}

