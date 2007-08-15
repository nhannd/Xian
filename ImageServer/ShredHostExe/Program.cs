using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageServer.ShredHostExe
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "Main thread";

            ShredHost.Start();
            Console.WriteLine("Press <Enter> to terminate the ShredHost.");
            Console.WriteLine();
            Console.ReadLine();
            ShredHost.Stop();
        }
    }
}
