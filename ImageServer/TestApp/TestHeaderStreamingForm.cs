#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.TestApp.services;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class TestHeaderStreamingForm : Form
    {
        private WSHttpBinding wsHttpBinding = new WSHttpBinding();

        
        public TestHeaderStreamingForm()
        {
            InitializeComponent();

            ClientCertSubjectName.Text = System.Environment.MachineName; // because the certificate generated by SetUpClientCert.bat use the machine name for the subject
            EndPointDNS.Text = "ImageServer"; // this is what the server will use for its endpoint 
            
            Random rand = new Random();
            timer1.Interval = 5000 + (int) (rand.NextDouble()*15000);
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();
        }

        private void Log(string msg)
        {
            LogTextPanel.Text += msg;
            LogTextPanel.ScrollToCaret();
            Console.WriteLine(msg);
        }

        delegate void UpdateStudyTreeCallback(DicomAttributeCollection ds);

        private void UpdateStudyTree(DicomAttributeCollection ds)
        {
            string acession = ds[DicomTags.AccessionNumber].GetString(0, "");
            string patientid = ds[DicomTags.PatientId].GetString(0, "");
            string name= ds[DicomTags.PatientsName].GetString(0, "");
            string modality = ds[DicomTags.ModalitiesInStudy].GetString(0, "");
            string description = ds[DicomTags.StudyDescription].GetString(0, "");
            string studyinstanceuid = ds[DicomTags.StudyInstanceUid].GetString(0, "");

            String text = String.Format("{0}-{1}-{2}-{3}", patientid, name, acession, description);
            
            TreeNode studyNode = new TreeNode();
            studyNode.Text = text;
            studyNode.Tag = studyinstanceuid;

            StudyTree.Nodes.Add(studyNode);
        }

        private void Query_Click(object sender, EventArgs e)
        {
            LogTextPanel.Text = "";

            StudyTree.Nodes.Clear();

            CFindSCU client = new CFindSCU();
            client.AETitle = AETitle.Text;

            client.Query(ServerAE.Text, IP.Text, Int32.Parse(Port.Text), delegate (DicomAttributeCollection ds)
                                                                             {
                                                                                 UpdateStudyTreeCallback callback =
                                                                                     new UpdateStudyTreeCallback(
                                                                                         UpdateStudyTree);

                                                                                 BeginInvoke(callback, ds);
                                                                                            
                                                                             });

            
        }


        private void PopulateSeries(string studyInstanceUid)
        {
            LogTextPanel.Text = "";
            StatisticsLog.Text = "";
            HeaderStreamingServiceClient proxy = null;
           
            try
            {
                proxy = new HeaderStreamingServiceClient();
                HeaderStreamingParameters parms = new HeaderStreamingParameters();
                parms.StudyInstanceUID = studyInstanceUid;
                parms.ServerAETitle = ServerAE.Text;
                parms.ReferenceID = Guid.NewGuid().ToString();

                TimeSpanStatistics servicecall = new TimeSpanStatistics();
                servicecall.Start();
                Stream stream = proxy.GetStudyHeader(AETitle.Text, parms);
                
                servicecall.End();


                TimeSpanStatistics decompression = new TimeSpanStatistics();
                decompression.Start();
                
                //GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress);
                XmlDocument doc = new XmlDocument();
                StudyXmlIo.ReadGzip(doc, stream);
                //doc.Load(gzipStream);

                decompression.End();
                

                XmlWriterSettings settings = new XmlWriterSettings();
                //settings.Indent = true;
                settings.NewLineOnAttributes = false;
                settings.OmitXmlDeclaration = true;
                settings.Encoding = Encoding.UTF8;
                StringWriter sw = new StringWriter();
                XmlWriter writer = XmlWriter.Create(sw, settings);
                doc.WriteTo(writer);
                writer.Flush();
                Log(sw.ToString());

                TimeSpanStatistics loading = new TimeSpanStatistics();
                loading.Start();
                
                StudyXml xml = new StudyXml();
                xml.SetMemento(doc);
                loading.End();

                int sopCounter = 0;
                SeriesTree.Nodes.Clear();
                foreach(SeriesXml series in xml)
                {
                    TreeNode seriesNode = new TreeNode(series.SeriesInstanceUid);
                    SeriesTree.Nodes.Add(seriesNode);
                    foreach(InstanceXml instance in series)
                    {
                        TreeNode instanceNode = new TreeNode(instance.SopInstanceUid);
                        seriesNode.Nodes.Add(instanceNode);
                        sopCounter++;
                    }
                }
                
               

                StatisticsLog.Text="";
                StatisticsLog.Text += String.Format("\r\nHeader Size (Decompressed): {0} KB", sw.ToString().Length / 1024);
                
                StatisticsLog.Text += String.Format("\r\nWCF Service call  : {0} ms", servicecall.Value.TotalMilliseconds);
                StatisticsLog.Text += String.Format("\r\nDecompression    : {0} ms", decompression.Value.TotalMilliseconds);
                StatisticsLog.Text += String.Format("\r\nLoading StudyXml : {0} ms", loading.Value.TotalMilliseconds);
                

                SeriesLabel.Text = String.Format("Series : {0} \tInstances: {1}", SeriesTree.Nodes.Count, sopCounter);

                stream.Close();

            }
            catch(FaultException<StudyIsInUseFault> ex)
            {
                timer1.Stop();
                MessageBox.Show(String.Format("StudyIsInUseFault received:{0}\n\nState={1}" ,
                            ex.Message, ex.Detail.StudyState));
            }
            catch (FaultException<StudyIsNearlineFault> ex)
            {
                timer1.Stop();
                MessageBox.Show("StudyIsNearlineFault received:\n" + ex.Message);
                
            }
            catch (FaultException<StudyNotFoundFault> ex)
            {
                timer1.Stop();
                MessageBox.Show("StudyNotFoundFault received:\n" + ex.Message);
                
            }
            catch (Exception ex)
            {
                timer1.Stop();
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (proxy.State == CommunicationState.Opened)
                    proxy.Close();
            }

        }

        private void StudyTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string studyinstnaceuid = (string) e.Node.Tag;

            PopulateSeries(studyinstnaceuid);
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            if (StudyTree.SelectedNode!=null)
            {
                string studyinstnaceuid = (string)StudyTree.SelectedNode.Tag;
                PopulateSeries(studyinstnaceuid);
            }
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

    }
}