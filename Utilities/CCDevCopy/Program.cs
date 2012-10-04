#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace CCDevCopy
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.Write("Usage: ccdevcopy.exe installPath");
				return;
			}

			string dstPath = args[0];

			if (!Directory.Exists(dstPath))
			{
				Console.Write("The path {0} does not exist.", dstPath);
				return;
			}

			string sdkPath = GetSdkPath();

			if (String.IsNullOrEmpty(sdkPath))
			{
				Console.Write("Unable to locate ClearCanvas SDK installation path.");
				return;
			}

			string srcPath = Path.Combine(sdkPath, "bin\\redistributable");
			srcPath = QuoteThePath(srcPath);
			dstPath = QuoteThePath(dstPath);

			// This set of xcopy parameters ensures that only files not in the
			// destination path will be copied.
			string parameters = string.Format("{0} {1} /d /s /y /c", srcPath, dstPath);

			try
			{
				Process xcopy = Process.Start("xcopy", parameters);
				// Wait until the xcopy is done before exiting
				xcopy.WaitForExit();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private static string QuoteThePath(string path)
		{
			return string.Format("\"{0}\"", path);
		}

		private static string GetSdkPath()
		{
			object value = Registry.GetValue(
				@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\ClearCanvas SDK",
				"UninstallString",
				null);

			if (value == null)
				return String.Empty;

			String sdkPath = Path.GetDirectoryName(value.ToString());

			if (!Directory.Exists(sdkPath))
				return String.Empty;

			return sdkPath;
		}
	}
}
