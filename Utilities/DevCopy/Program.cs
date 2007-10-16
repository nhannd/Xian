using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace DevCopy
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.Write("Usage: DevInstaller installPath");
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

			string srcPath = Path.Combine(sdkPath, "bin");
			srcPath = QuoteThePath(srcPath);
			dstPath = QuoteThePath(dstPath);

			// This set of xcopy parameters ensures that only files not in the
			// destination path will be copied.
			string parameters = string.Format("{0} {1} /d /s /y /c", srcPath, dstPath);

			try
			{
				Process.Start("xcopy", parameters);
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
