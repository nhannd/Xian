using System;
using System.Collections.Generic;
using System.Text;
//using System.Net.Sockets;
using ClearCanvas.Dicom.Network;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            // start up winsock
            Socket s = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("Hello world\n");

            T_ASC_Network nw = new T_ASC_Network(T_ASC_NetworkRole.NET_ACCEPTOR, 104, 100);

            
            Console.WriteLine("me like durian!\n");
            */
            ApplicationEntity me = new ApplicationEntity(new HostName("localhost"), new AETitle("TEST_CLIENT"), new ListeningPort(3000));
            DicomClient dc = new DicomClient(me);

            ApplicationEntity server = new ApplicationEntity(new HostName("192.168.0.103"), new AETitle("CLINTONLAPTOP"), new ListeningPort(4006));
            dc.Verify(server);
        }
    }
}
