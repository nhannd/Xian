#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using ClickOnceAppLauncher.Properties;

//

namespace ClickOnceAppLauncher
{
	class Program
	{
		static void Main(string[] args)
		{
			if (String.IsNullOrEmpty(Settings.Default.ExeFile))
			{
				Console.WriteLine("Specify the executable to launch in the .config file.");
				return;
			}

			Process.Start(Settings.Default.ExeFile);
		}
	}
}
