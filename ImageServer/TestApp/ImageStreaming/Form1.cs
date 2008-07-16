using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using ClearCanvas.Common.Statistics;
using ClearCanvas.DicomServices.ServiceModel.Streaming;

namespace ImageStreaming
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Array types = Enum.GetValues(typeof (ContentTypes));
            foreach(ContentTypes type in types)
            {
                ContentTypes.Items.Add(type);
            }

            ContentTypes.SelectedIndex = 0;
        }

        private void Retrieve_Click(object sender, EventArgs e)
        {
            StreamingClient client = new StreamingClient();
            try
            {
                if (RetrieveFrame.Checked)
                {
                    FrameStreamingResultMetaData metaData;
                    client.RetrieveFrame(BaseUri.Text, StudyUid.Text, SeriesUid.Text, ObjectUid.Text, int.Parse(Frame.Text), (ContentTypes)ContentTypes.SelectedItem, out metaData);

                    String msg = String.Format("Type:\t{0}\nSize:\t{1}\nSpeed:\t{2}", metaData.ResponseMimeType,
                            ByteCountFormatter.Format((ulong)metaData.ContentLength), metaData.Speed.FormattedValue);
                    MessageBox.Show(msg);
                }
                else
                {
                    StreamingResultMetaData metaData;
                    client.RetrieveImage(BaseUri.Text, StudyUid.Text, SeriesUid.Text, ObjectUid.Text, (ContentTypes)ContentTypes.SelectedItem, out metaData);

                    String msg = String.Format("Type:\t{0}\nSize:\t{1}\nSpeed:\t{2}", metaData.ResponseMimeType, ByteCountFormatter.Format((ulong)metaData.ContentLength), metaData.Speed.FormattedValue);
                    MessageBox.Show(msg);
                }
            
            }
            catch(WebException ex)
            {
                HttpWebResponse rsp = (ex.Response as HttpWebResponse);
                if (rsp != null)
                {
                    string msg = String.Format("Error: {0}\n{1}",
                                               rsp.StatusCode,
                                               HttpUtility.HtmlDecode(rsp.StatusDescription)
                        );
                    MessageBox.Show(msg);
                }
                
            }
        

           
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                FileInfo file = new FileInfo(openFileDialog1.FileName);
                
                ObjectUid.Text = file.Name.Replace(".dcm", "");

                SeriesUid.Text = file.Directory.Name;
                StudyUid.Text = file.Directory.Parent.Name;

               
            }
        }

        
        private void RetrieveFrame_CheckedChanged(object sender, EventArgs e)
        {
            Frame.Enabled = RetrieveFrame.Checked;
        }
    }
}