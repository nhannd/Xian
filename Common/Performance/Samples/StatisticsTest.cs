using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using ClearCanvas.Common.Performance;
using ClearCanvas.Common.Performance.Network;
using ClearCanvas.Common.Performance.WorkQueue;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;

namespace ClearCanvas.Common.Performance.Samples
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class StatisticsTest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stat"></param>
        public void WriteStat(StatisticsSet stat)
        {
            XmlDocument doc = new XmlDocument();
            
            doc.AppendChild(stat.GetXmlElement(doc));
            Write(doc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        public void Write(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;
            xmlSettings.IndentChars = "\t";
            xmlSettings.Encoding = Encoding.UTF8;

            XmlWriter writer = XmlWriter.Create(sb, xmlSettings);
            doc.WriteTo(writer);
            writer.Flush();
            Console.WriteLine(sb.ToString());
            Console.Out.Flush();
        }
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void Test()
        {
            Random ran = new Random();


            WorkQueueAverageStatistics averageWQQueue = new WorkQueueAverageStatistics("WorkQueueStatistics");
            averageWQQueue.StudyInstanceUID.Value = "1.2.3.12.12.3.3";

            int n = ran.Next(5) + 1;
            // process instance
            for( int i=0; i<n; i++)
            {
                InstanceProcessingStatistics instanceStat = averageWQQueue.NewStatistics();
                instanceStat.DBAccessTime.Start();

                Thread.Sleep(ran.Next(300));

                instanceStat.DBAccessTime.End();
                instanceStat.InstanceUid = "123.12312.123.123 " + ran.Next();

                instanceStat.FileAccessTime.Start();
                Thread.Sleep(ran.Next(500));
                instanceStat.FileAccessTime.End();

                instanceStat.InstanceSize = ran.Next();

            }

            averageWQQueue.LogAll = true;
            WriteStat(averageWQQueue);
                    
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void TestTransmission()
        {
            Random ran = new Random();


            TransmissionStatistics stat = new TransmissionStatistics("Association from....");

            stat.Begin();

            int n = ran.Next(5) + 1;
            for (int i = 0; i < n; i++)
            {

                Thread.Sleep(ran.Next(500));

                stat.IncomingBytes += ran.Next();

                stat.OutgoingBytes += ran.Next();

                stat.IncomingMessages += ran.Next(5) + 1;
                stat.OutgoingMessages += ran.Next(5) + 1;


            }


            stat.End();



            WriteStat(stat);

        }

    }
}
