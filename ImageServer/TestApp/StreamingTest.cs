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
using ClearCanvas.Dicom.ServiceModel.Streaming;

namespace ClearCanvas.ImageServer.TestApp
{

    public partial class StreamingTest : Form
    {
        private List<Series> _series;
        public StreamingTest()
        {
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
           LoadSops();
            StartNormalStreaming(_series);
        }

        private void StartNormalStreaming(List<Series> seriesList)
        {
            foreach (Series series in seriesList)
            {
                
                foreach(string sop in series.SopInstanceUids)
                {
                    StringBuilder url = new StringBuilder();
                    url.Append(WadoUrl.Text);
                    url.AppendFormat("?requesttype=WADO&studyUID={0}&seriesUID={1}&objectUID={2}",
                        StudyUID.Text, series.SeriesInstanceUid, sop);
                    url.AppendFormat("&frameNumber={0}", 0);
                    url.AppendFormat("&contentType={0}", HttpUtility.HtmlEncode("application/clearcanvas"));
                    
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url.ToString());
                    request.Accept = "application/dicom,application/clearcanvas,image/jpeg";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    BinaryReader reader = new BinaryReader(responseStream);
                    reader.ReadBytes((int)response.ContentLength);
                    reader.Close();
                    responseStream.Close();
                    response.Close();
                    
                }
                
            }
            

            
        }

        private void StartQuickTestStreaming(List<Series> seriesList, bool compressed)
        {
            List<HttpRequest> requests = new List<HttpRequest>();
            foreach (Series series in seriesList)
            {

                foreach (string sop in series.SopInstanceUids)
                {
                    StringBuilder url = new StringBuilder();
                    url.Append(WadoUrl.Text);
                    url.AppendFormat("?requesttype=WADO&studyUID={0}&seriesUID={1}&objectUID={2}",
                        StudyUID.Text, series.SeriesInstanceUid, sop);
                    url.AppendFormat("&frameNumber={0}", 0);
                    url.AppendFormat("&contentType={0}", HttpUtility.HtmlEncode("application/clearcanvas"));
                    url.Append(compressed ? "&testcompressed=true" : "&testuncompressed=true");

                    if (SimReadDelay.Text!="")
                    {
                        url.AppendFormat("&simreaddelay={0}", SimReadDelay.Text);
                    }

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url.ToString());
                    request.Accept = "application/dicom,application/clearcanvas,image/jpeg";
                    if (KeepAlive.Checked)
                        request.KeepAlive = true;
                    
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    Stream responseStream = response.GetResponseStream();
                    BinaryReader reader = new BinaryReader(responseStream);
                    reader.ReadBytes((int)response.ContentLength);
                    reader.Close();
                    responseStream.Close();
                    response.Close();

                }

            }



        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadSops();

            StartQuickTestStreaming(_series, false);
        }

        private void LoadSops()
        {
            int imageCount = 0;
            _series = new List<Series>();
            DirectoryInfo directory = new DirectoryInfo(StudyPath.Text);
            foreach (DirectoryInfo seriesDir in directory.GetDirectories())
            {
                Series series = new Series();
                series.SeriesInstanceUid = seriesDir.Name;
                _series.Add(series);
                series.SopInstanceUids = new List<string>();
                foreach (FileInfo file in seriesDir.GetFiles("*.dcm"))
                {
                    series.SopInstanceUids.Add(file.Name.Replace(".dcm", ""));
                    imageCount++;
                }
            }
            MessageBox.Show("Ready to stream");
            label1.Text = String.Format("{0} Series, {1} Images", _series.Count, imageCount);
        }

        private void button3_Click(object sender, EventArgs e)
        {

            LoadSops();

            StartQuickTestStreaming(_series, true);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }


    class Series
    {
        public string SeriesInstanceUid;
        public List<string> SopInstanceUids;
    }
}