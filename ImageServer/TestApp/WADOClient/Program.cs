using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;

namespace WADOClient
{
    class Program
    {
        static private string serverHost;
        static private int serverPort;

        static void Main(string[] args)
        {
            serverHost = args[0];
            serverPort = int.Parse(args[1]);

            string fsDir = args[2];
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
                    // pick one
                    DirectoryInfo study = studies[r.Next(studies.Length)];

                    string path = study.FullName;
                    RetrieveImages(path);

                    Thread.Sleep(r.Next(10000));

                } while (true);
 
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            

           
        }

        static private int OptimizeBufferSize(long contentSize)
        {
            int readBufferSize;
            
            const int KILOBYTES = 1024;
            const int MEGABYTES = 1024 * KILOBYTES;


            // This is very simple optimization algorithm: the buffer size is set according to the content size
            // Other factors may be considered in the future: disk access speed, available physical memory, network speed, cpu usage
            if (contentSize > 3 * MEGABYTES)
                readBufferSize = 3 * MEGABYTES;
            else if (contentSize > 1 * MEGABYTES)
                readBufferSize = 1 * MEGABYTES;
            else if (contentSize > 500 * KILOBYTES)
                readBufferSize = 256 * KILOBYTES;
            else if (contentSize > 100 * KILOBYTES)
                readBufferSize = 128 * KILOBYTES;
            else
                readBufferSize = 64 * KILOBYTES;

            return readBufferSize;
        }

        private static void RetrieveImages(string studypath)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(studypath);
            string studyuid = dirinfo.Name;
            long studysize = 0;
            int sopcount = 0;
            TimeSpanStatistics total = new TimeSpanStatistics();
            total.Start();
            AverageTimeSpanStatistics average = new AverageTimeSpanStatistics();
            AverageRateStatistics averageSpeed = new AverageRateStatistics(RateType.BYTES);
            AverageByteCountStatistics averageBuffer = new AverageByteCountStatistics();
            string[] seriesDirs = Directory.GetDirectories(studypath);
            int readCount = 0;
                
            foreach(string seriesPath in seriesDirs)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(seriesPath);
                string seriesuid = dirInfo.Name;
                string[] objectuidPath = Directory.GetFiles(seriesPath, "*.dcm");
                        
                foreach (string uidPath in objectuidPath)
                {
                    FileInfo fileinfo = new FileInfo(uidPath);
                    string uid = fileinfo.Name.Replace(".dcm", "");
                    string url = "http://{3}:{4}/wado?requesttype=WADO&studyUID={0}&seriesUID={1}&objectUID={2}";

                    url = String.Format(url, studyuid, seriesuid, uid, serverHost, serverPort);
                    RateStatistics speed = new RateStatistics("Speed", RateType.BYTES);
                    TimeSpanStatistics elapse = new TimeSpanStatistics();
                    speed.Start();
                    elapse.Start();
                
                    HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                    //request.Connection = "Close";
                    //request.Accept = "application/jpeg";
                    
                    HttpWebResponse response = (HttpWebResponse) request.GetResponse();

                    int count = 0;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream stream = response.GetResponseStream();
                        int bufferSize = 30*1024*1024;// OptimizeBufferSize(response.ContentLength);
                        byte[] buffer = new byte[bufferSize];
                        long size = 0;

                            do
                            {
                                count = stream.Read(buffer, 0, buffer.Length);
                                if (count > 0)
                                {
                                    readCount++;
                                    size += count;
                                    speed.SetData(size);
                                    //Console.Write(message, uid, size / 1024.0f / 1024.0f, speed.FormattedValue, elapse.FormattedValue);
                                    
                                }
                            } while (count > 0);

                        
                        stream.Close();
                        studysize += size;
                        averageBuffer.AddSample(bufferSize);

                        speed.End();
                        elapse.End();

                        averageSpeed.AddSample(speed);

                        average.AddSample(elapse);
                        sopcount++;
                    }
                    else
                    {
                        Console.WriteLine("{0} : {1}", response.StatusCode, response.StatusDescription);
                    }

                    response.Close();

                }
            }

            total.End();

            if (sopcount>0)
            {
                Console.WriteLine("{0,-64}... {1,3} x {2,4:0} KB  @ {3,10} in {4,10}... {5,5:0.0}fps", 
                                    studyuid, sopcount,
                                    (float)studysize/sopcount/1024.0,
                                    averageSpeed.FormattedValue,
                                    total.FormattedValue,
                                    sopcount/total.Value.TotalSeconds
                                    );

                Console.WriteLine("Buffer: {0}, Read={1}",averageBuffer.FormattedValue, readCount / sopcount);
                Console.Out.Flush();
            }
                
                                        
        }
    }
}
