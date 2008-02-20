using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.ImageServer.TestApp.HeaderStreamingService;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class HeaderStreamingTest : Form
    {
        private WSHttpBinding wsHttpBinding = new WSHttpBinding();
        private NetTcpBinding netTcpBinding = new NetTcpBinding();
        
        public HeaderStreamingTest()
        {
            InitializeComponent();


            Bindings.Items.Add(wsHttpBinding.Name);
            Bindings.Items.Add(netTcpBinding.Name);
            Bindings.SelectedIndex = 0;

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
            HeaderRetrievalServiceClient proxy = null;

            if (Bindings.SelectedItem.ToString()==wsHttpBinding.Name)
            {
                const int OneMegaByte = 1048576;
                bool authenticated = Secured.Checked;
                wsHttpBinding.MaxReceivedMessageSize = OneMegaByte;
                wsHttpBinding.ReaderQuotas.MaxStringContentLength = OneMegaByte;
                wsHttpBinding.ReaderQuotas.MaxArrayLength = OneMegaByte;
                wsHttpBinding.Security.Mode = authenticated ? SecurityMode.Message : SecurityMode.None;
                wsHttpBinding.Security.Message.ClientCredentialType = authenticated ?
                    MessageCredentialType.Windows : MessageCredentialType.None;

                string remotehost = IP.Text;
                int port = Int32.Parse(WcfPort.Text);

                Uri uri =
                    new UriBuilder(
                        String.Format("http://{0}:{1}/{2}/{3}", remotehost, port, ServiceName.Text, ServiceName.Text)).Uri;
                        
                EndpointAddress endpoint = new EndpointAddress(uri);

                proxy = new HeaderRetrievalServiceClient(wsHttpBinding, endpoint);
            }

            else if (Bindings.SelectedItem.ToString() == netTcpBinding.Name)
            {
                const int OneMegaByte = 1048576;
                bool authenticated = Secured.Checked;
                netTcpBinding.MaxReceivedMessageSize = OneMegaByte;
                netTcpBinding.ReaderQuotas.MaxStringContentLength = OneMegaByte;
                netTcpBinding.ReaderQuotas.MaxArrayLength = OneMegaByte;
                netTcpBinding.Security.Mode = authenticated ?  SecurityMode.Message:SecurityMode.None;
                netTcpBinding.Security.Message.ClientCredentialType = authenticated ?
                    MessageCredentialType.Windows : MessageCredentialType.None;

                string remotehost = IP.Text;
                int port = Int32.Parse(WcfPort.Text);


                Uri uri =
                    new UriBuilder(
                        String.Format("net.tcp://{0}:{1}/{2}/{3}", remotehost, port, ServiceName.Text, ServiceName.Text)).Uri;
                        
                EndpointAddress endpoint = new EndpointAddress(uri);

                proxy = new HeaderRetrievalServiceClient(netTcpBinding, endpoint);
            }

           
            try
            {
                HeaderRetrievalParameters parms = new HeaderRetrievalParameters();
                parms.StudyInstanceUID = studyInstanceUid;
                parms.ServerAETitle = ServerAE.Text;

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
            catch(Exception ex)
            {
                Log(ex+ ":" + ex.StackTrace);
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

        private void Secured_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Bindings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Bindings.SelectedItem.ToString()==wsHttpBinding.Name)
            {
                WcfPort.Text = "50221";
            }
            else if (Bindings.SelectedItem.ToString()==netTcpBinding.Name)
            {
                WcfPort.Text = "50222";
            }
        }
    }
}