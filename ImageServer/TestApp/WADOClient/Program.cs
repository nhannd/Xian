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

        
        private static void RetrieveImages(string studyPath)
        {
            StreamingClient client = new StreamingClient();
            int totalFrameCount = 0;
            
            DirectoryInfo directoryInfo = new DirectoryInfo(studyPath);
            string studyUid = directoryInfo.Name;

            RateStatistics frameRate = new RateStatistics("Speed", "frame");
            RateStatistics speed = new RateStatistics("Speed", RateType.BYTES);
            AverageRateStatistics averageSpeed = new AverageRateStatistics(RateType.BYTES);
            ByteCountStatistics totalSize = new ByteCountStatistics("Size");
            
            frameRate.Start();
            speed.Start();

            Console.WriteLine("--------------------------------------------------------------------------------------------------------");

            string[] seriesDirs = Directory.GetDirectories(studyPath);
            foreach(string seriesPath in seriesDirs)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(seriesPath);
                string seriesUid = dirInfo.Name;
                string[] objectUidPath = Directory.GetFiles(seriesPath, "*.dcm");
                
                foreach (string uidPath in objectUidPath)
                {
                    FileInfo fileInfo = new FileInfo(uidPath);
                    string uid = fileInfo.Name.Replace(".dcm", "");
                    Console.Write("{0,-64}... ", uid);
                                    
                    try
                    {
                        string baseUri = String.Format("http://{0}:{1}/wado", serverHost, serverPort);

                        int frameIndex = 0;
                        
                        switch(type)
                        {
                            case ContentTypes.Dicom:
                                StreamingResultMetaData sopResultMetaData;
                                client.RetrieveImage(baseUri, studyUid, seriesUid, uid, ContentTypes.Dicom, out sopResultMetaData);
                                totalFrameCount++;
                                averageSpeed.AddSample(sopResultMetaData.Speed);
                                totalSize.Value += (ulong)sopResultMetaData.ContentLength;
                                break;

                            case ContentTypes.RawPixel:
                                TimeSpanStatistics elapsedTime = new TimeSpanStatistics();
                                ulong instanceSize = 0;
                                FrameStreamingResultMetaData frameResultMetaData;
                                do
                                {
                                    client.RetrieveFrame(baseUri, studyUid, seriesUid, uid, frameIndex, ContentTypes.Dicom, out frameResultMetaData);
                                    totalFrameCount++;
                                    frameIndex++;
                                    averageSpeed.AddSample(frameResultMetaData.Speed);
                                    totalSize.Value += (ulong)frameResultMetaData.ContentLength;
                                    instanceSize += (ulong)frameResultMetaData.ContentLength;

                                } while (!frameResultMetaData.IsLast);

                                elapsedTime.End();
                                Console.WriteLine("{0,3} frames [{1}] in {2}", frameIndex, ByteCountFormatter.Format(instanceSize), elapsedTime.FormattedValue);
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
            frameRate.SetData(totalFrameCount);
            frameRate.End();
            speed.SetData(totalSize.Value);
            speed.End();


            Console.WriteLine("\nTotal {0,3} image/frames [{1,10}] in {2,12}   ==>  [ Speed: {3,12} / {4,12}]",
                    totalFrameCount, totalSize.FormattedValue,
                    TimeSpanFormatter.Format(frameRate.ElapsedTime),
                    frameRate.FormattedValue,
                    speed.FormattedValue
                    );

                                        
        }
    }
}
