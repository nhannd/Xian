#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.Executable
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string[] args1 = new string[args.Length - 1];
                Array.Copy(args, 1, args1, 0, args.Length - 1);
                Platform.StartApp(args[0], args1);
            }
            else
            {
				Console.WriteLine("ERROR: Application Root must be specified.");
            }
        }
    }
}
