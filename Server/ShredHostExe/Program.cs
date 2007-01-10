using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Server.ShredHost
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
