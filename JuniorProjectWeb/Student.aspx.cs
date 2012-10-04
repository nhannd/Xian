using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.ImageViewer.Services.Auditing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Services.Automation;
using System.Xml;
using System.IO;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;
using System.Diagnostics;

namespace JuniorProjectWeb
{
    public partial class Student : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ddlPatientID.Items.Count == 0)
            {
                XmlDocument queryXML = new XmlDocument();

                queryXML.Load("C:\\Code\\Trunk\\JuniorProjectWeb\\XMLFile1.xml");

                InstanceXml instanceXML = new InstanceXml(queryXML.DocumentElement, null);
                DicomAttributeCollection queryMessage = instanceXML.Collection;

                StudyRootFindScu findScu = new StudyRootFindScu();
                IList<DicomAttributeCollection> resultsList;

                resultsList = findScu.Find("SERVERAE", "SERVERAE", "SERVERAE", 104, queryMessage);

                findScu.Dispose();

                ddlPatientID.Items.Add("Select your patient ID");

                foreach (DicomAttributeCollection msg in resultsList)
                {
                    ddlPatientID.Items.Add(msg.GetAttribute(1048608));
                }
            }
        }

        protected void BtnTeacher_Click(object sender, EventArgs e)
        {
            Server.Transfer("Teacher.aspx");
        }

        protected void BtnAdmin_Click(object sender, EventArgs e)
        {
            Server.Transfer("Admin.aspx");
        }

        protected void ddlPatientID_SelectedIndexChanged(object sender, EventArgs e)
        {
            String patientID = ddlPatientID.SelectedValue;

            XmlDocument queryXML = new XmlDocument();

            queryXML.Load("C:\\Code\\Trunk\\JuniorProjectWeb\\XMLFile1.xml");

            InstanceXml instanceXML = new InstanceXml(queryXML.DocumentElement, null);

            instanceXML.Collection.ElementAt(8).Values = patientID;

            DicomAttributeCollection queryMessage = instanceXML.Collection;

            StudyRootFindScu findScu = new StudyRootFindScu();
            IList<DicomAttributeCollection> resultsList;

            resultsList = findScu.Find("SERVERAE", "SERVERAE", "SERVERAE", 104, queryMessage);

            findScu.Dispose();

            foreach (DicomAttributeCollection msg in resultsList)
            {
                String studyName = msg.GetAttribute(528432);

                if (studyName.Length == 0)
                    studyName = "None";

                String patientName = msg.GetAttribute(1048592);

                String split = "^";

                String first = patientName.Replace( split, " " );
                
                String format = String.Format("Study Name: {0} Patient Name: {1}", studyName, first);

                lResult.Text = format;
                lResult.Visible = true;
            }
        }

        protected void btnBurn_Click(object sender, EventArgs e)
        {
            String patientID = ddlPatientID.SelectedValue;
            Directory.SetCurrentDirectory( "C:\\FS\\Primary" );
            System.Diagnostics.Process.Start(@"C:\FS\Primary\Query.bat", patientID);
        
            
        }
    }
}
