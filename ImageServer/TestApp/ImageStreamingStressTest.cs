using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ClearCanvas.Dicom.ServiceModel.Streaming;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class ImageStreamingStressTest : Form
    {
        public ImageStreamingStressTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for(int i=0;i<100;i++)
            {
                Thread t = new Thread(delegate(object parm)
                                      {
                                          StreamingClient client =
                                              new StreamingClient(new Uri("http://localhost:1000/wado"));
                                          do
                                          {
                                              Stream image = client.RetrieveImage("ImageServer",
                                                                                  "1.2.840.113619.2.5.1762583153.215519.978957063.78",
                                                                                  "1.2.840.113619.2.5.1762583153.215519.978957063.79",
                                                                                  "1.2.840.113619.2.5.1762583153.215519.978957063.89"
                                                  );
                                          } while (true);
                                      });

                t.Start();
            
            }
            
                            
        }
    }
}