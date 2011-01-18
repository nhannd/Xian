#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

// Note: This utility is meant to execute without use any CC stuff.
// Do not reference any CC stuff here
using System;

namespace ClearCanvas.Enterprise.VersionExecutable
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                VersionCheckerCommandLine commandLine = new VersionCheckerCommandLine(args);
                VersionChecker checker = new VersionChecker(commandLine.Host, commandLine.Port, commandLine.ServiceIdentity);
                return checker.Run();
            }
            catch (UsageException ex)
            {
                Console.WriteLine(ex.Message);
                VersionChecker.PrintSyntax();
            }

            return -2;
        }
    }
}