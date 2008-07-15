using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.ServiceModel.Streaming;

namespace WADOClient
{
    class Program
    {
        static private string serverHost;
        static private int serverPort;
        private static ContentTypes type;
        private static string studyFolder;

        static void Main(string[] args)
        {
            serverHost = args[0];
            serverPort = int.Parse(args[1]);

            string fsDir = args[2];

            if (args.Length>=4)
            {
                if (args[3] == "dicom") type = ContentTypes.Dicom;
                else if (args[3] == "pixel") type = ContentTypes.RawPixel;
                else
                    type = ContentTypes.NotSpecified;

            }

            if (args.Length>=5)
            {
                studyFolder = args[4];
            }

            if (studyFolder==null)
            {
                Console.WriteLine("Retrieve image in {0}", fsDir);

                DirectoryInfo dirInfo = new DirectoryInfo(fsDir);

                DirectoryInfo[] partitions = dirInfo.GetDirectories();

                try
                {
                    do
                    {
                        Random r = new Random();
                        DirectoryInfo partition = partitions[r.Next(partitions.Length)];

                        DirectoryInfo[] studydates = partition.GetDirectories();
                        // pick one
                        DirectoryInfo studyate = studydates[r.Next(studydates.Length)];

                        DirectoryInfo[] studies = studyate.GetDirectories();

                        if (studies.Length > 0)
                        {
                            // pick one
                            DirectoryInfo study = studies[r.Next(studies.Length)];

                            string path = study.FullName;
                            RetrieveImages(path);
                        }


                        Thread.Sleep(r.Next(10000));

                    } while (true);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                Console.WriteLine("Retrieve image in {0}", studyFolder);
                Random r = new Random();
                        
                do
                {
                    RetrieveImages(studyFolder);
                    //Thread.Sleep(r.Next(5000));
                } while (true);
                
                       
            }
        }

        
        private static void RetrieveImages(string studypath)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(studypath);
            string studyuid = dirinfo.Name;

            RateStatistics framerate = new RateStatistics("Speed", "frame");
            RateStatistics speed = new RateStatistics("Speed", RateType.BYTES);


            framerate.Start();
            speed.Start();

            AverageTimeSpanStatistics average = new AverageTimeSpanStatistics();
            AverageRateStatistics averageSpeed = new AverageRateStatistics(RateType.BYTES);
            ByteCountStatistics totalsize = new ByteCountStatistics("Size");
            
            Console.WriteLine("----------------------------------------------------------");

            string[] seriesDirs = Directory.GetDirectories(studypath);
            int frameCount = 0;
            foreach(string seriesPath in seriesDirs)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(seriesPath);
                string seriesuid = dirInfo.Name;
                string[] objectuidPath = Directory.GetFiles(seriesPath, "*.dcm");
                
                foreach (string uidPath in objectuidPath)
                {
                    FileInfo fileinfo = new FileInfo(uidPath);
                    string uid = fileinfo.Name.Replace(".dcm", "");
                    Console.Write("{0,-64}...", uid);
                                    
                    try
                    {
                        string baseUri = String.Format("http://{0}:{1}/wado", serverHost, serverPort);
                        StreamingClient client = new StreamingClient(baseUri);

                        StreamingResult result = null;
                                
                        int frameIndex = 0;
                        
                        switch(type)
                        {
                            case ContentTypes.Dicom:
                                result = client.RetrieveImage(studyuid, seriesuid, uid, type);
                                frameCount++;
                                averageSpeed.AddSample(client.Speed);
                                totalsize.Value += (ulong)result.ContentStream.Length;
                                break;

                            case ContentTypes.RawPixel:
                                TimeSpanStatistics ts = new TimeSpanStatistics();
                                ulong instanceSize = 0;
                                bool lastFrame = false;
                                
                                do
                                {
                                    result = client.RetrieveFrame(studyuid, seriesuid, uid, frameIndex, type);
                                    frameCount++;
                                    frameIndex++;
                                    ts.Add(client.ElapsedTime);
                                    averageSpeed.AddSample(client.Speed);
                                    totalsize.Value += (ulong)result.ContentStream.Length;
                                    instanceSize += (ulong)result.ContentStream.Length;
                                    lastFrame = !(result as FrameStreamingResult).HasMore;
                                    

                                } while (!lastFrame);

                                Console.WriteLine("{0} [{1}] in {2}", frameIndex, ByteCountFormatter.Format(instanceSize), ts.FormattedValue);
                                break;

                            default:
                                result = client.RetrieveImage(studyuid, seriesuid, uid, type);
                                break;
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
                            Console.WriteLine(msg);
                        }
                    }
                }
            }
            framerate.SetData(frameCount);
            framerate.End();
            speed.SetData(totalsize.Value);
            speed.End();


            Console.WriteLine("Total {0,3} images [{1,10}] in {2,12}   ==>  [ Speed: {3,12} / {4,12}]", 
                    frameCount, totalsize.FormattedValue,
                    TimeSpanFormatter.Format(framerate.ElapsedTime),
                    framerate.FormattedValue,
                    speed.FormattedValue
                    );

                                        
        }
    }
}
