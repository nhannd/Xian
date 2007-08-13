using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageServer.ShredHostExe
{
    class Program
    {
        static void Main(string[] args)
        {
            ShredHost.Start();
            Console.WriteLine("Press <Enter> to terminate the ShredHost.");
            Console.WriteLine();
            Console.ReadLine();
            ShredHost.Stop();
        }
    }
}
