namespace ClearCanvas.ImageViewer.Dashboard.Remote.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NUnit.Framework;
    using ClearCanvas.Dicom;
    using ClearCanvas.Dicom.Network;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    [TestFixture]
    public class DashboardRemoteTests
    {


        [Test]
        public void XmlFileLoading()
        {

            Console.WriteLine("Serialize");

            List<Server> list = new List<Server>();

            list.Add(new Server("Clinton's Desktop Conquest", "Conquest PACS server",
                "207.219.39.207", "CONQUESTSRV1", 5678));

            list.Add(new Server("UHN Main Server", "UHN's main load-balanced server",
               "172.16.10.151", "FUSION_SRV01", 104));

            list.Add(new Server("UHN MG Server", "UHN MG Server",
                "172.16.10.140", "EFILM_NORMAN", 4006));

            // can't use simple XML serialization because it requires that all classes
            // have a parameterless constructor which we don't want, because we want
            // to enforce type-safe construction
            //XmlSerializer serializer = new XmlSerializer(typeof(List<Server>));
            //StreamWriter writer = new StreamWriter(@"C:\temp\appllist.xml");
            //serializer.Serialize(writer, list);
            //writer.Close();

            foreach (Server ae in list)
            {
                Console.WriteLine(ae);
            }

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(Directory.GetCurrentDirectory() + @"\..\..\..\..\..\UnitTestFiles\ClearCanvas.ImageViewer.Dashboard.Remote.Test.DashBoardRemoteTests\Server.bin", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, list);
                stream.Close();
            }

            Console.WriteLine("Deserialize");

            List<Server> list2;

            formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(Directory.GetCurrentDirectory() + @"\..\..\..\..\..\UnitTestFiles\ClearCanvas.ImageViewer.Dashboard.Remote.Test.DashBoardRemoteTests\Server.bin", FileMode.Open))
            {
                list2 = (List<Server>) formatter.Deserialize(stream);
                stream.Close();
            }

            foreach (Server ae in list2)
            {
                Console.WriteLine(ae);
            }
        }
    }
}
