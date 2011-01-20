#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceProcess;
using System.Threading;
using ClearCanvas.Common.UsageTracking;
using ClearCanvas.Utilities.Manifest;

namespace ClearCanvas.ImageServer.ShredHostService
{
    static class Program
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 0 && String.Compare(args[0], "-service", true) == 0)
            {
                ServiceBase[] ServicesToRun;

                // More than one user Service may run within the same process. To add
                // another service to this process, change the following line to
                // create a second service object. For example,
                //
                //   ServicesToRun = new ServiceBase[] {new Service1(), new MySecondUserService()};
                //
                ServicesToRun = new ServiceBase[] {new ShredHostService()};

                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                Thread.CurrentThread.Name = "Main thread";
                if (!ManifestVerification.Valid)
                    Console.WriteLine("The manifest detected an invalid installation.");
                ShredHostService.InternalStart();
                Console.WriteLine("Press <Enter> to terminate the ShredHost.");
                Console.WriteLine();
                Console.ReadLine();
                ShredHostService.InternalStop();
            }

        }
    }
}