#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using ClearCanvas.ImageServer.Common;

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
                int numSop = series.SopInstanceUids.Count;
                for (int i = 0; i < numSop; i++)
                {
                    
                    string sopUid = series.SopInstanceUids[i];
                    StringBuilder url = new StringBuilder();
                    url.Append(WadoUrl.Text);
                    url.AppendFormat("?requesttype=WADO&studyUID={0}&seriesUID={1}&objectUID={2}",
                        StudyUID.Text, series.SeriesInstanceUid, sopUid);
                    url.AppendFormat("&frameNumber={0}", 0);
                    url.AppendFormat("&contentType={0}", HttpUtility.HtmlEncode("application/clearcanvas"));
                    
                    if (ServerPrefetch.Checked && i < numSop - 1)
                    {
                        url.AppendFormat("&nextSeriesUid={0}", series.SeriesInstanceUid);
                        url.AppendFormat("&nextObjectUid={0}", series.SopInstanceUids[i+1]);
                    }
                    
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
                    series.SopInstanceUids.Add(file.Name.Replace(ServerPlatform.DicomFileExtension, ""));
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