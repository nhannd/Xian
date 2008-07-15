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
            StreamingResult result = null;
            
            StreamingClient client = new StreamingClient(BaseUri.Text);
                
            try
            {
                if (RetrieveFrame.Checked)
                {
                    result = client.RetrieveFrame( StudyUid.Text, SeriesUid.Text, ObjectUid.Text, int.Parse(Frame.Text), (ContentTypes) ContentTypes.SelectedItem);
                }
                else
                {
                    result = client.RetrieveImage(StudyUid.Text, SeriesUid.Text, ObjectUid.Text, (ContentTypes)ContentTypes.SelectedItem);
                }

                String msg = String.Format("Status:\t{0}\nType:\t{1}\nSize:\t{2}\nTime:\t{3}\nSpeed:\t{4}",
                                           result.Status, result.MimeType,
                                           ByteCountFormatter.Format((ulong) result.ContentStream.Length),
                                           client.ElapsedTime.FormattedValue, client.Speed.FormattedValue);

                if (result != null)
                {
                    Uri.Text = result.Uri.ToString();
                } 
                
                MessageBox.Show(msg);

                

            
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