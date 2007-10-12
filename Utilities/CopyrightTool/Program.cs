using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CopyrightTool
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				Console.Write("Usage: COPYRIGHTTOOL path newDate\n");
				return;
			}

			string path = args[0];
			string newDate = args[1];

			bool isFile = File.Exists(path);
			bool isDirectory = Directory.Exists(path);

			if (!isFile && !isDirectory)
			{
				string msg = string.Format("The path {0} does not exist.\n", path);
				Console.Write(msg);
				return;
			}

			if (isFile)
				ProcessFile(path, newDate);

			if (isDirectory)
			{
				string[] csfiles = Directory.GetFiles(path, "*.cs");

				foreach (string file in csfiles)
				{
					ProcessFile(file, newDate);
				}
			}
		}

		private static void ProcessFile(string file, string newDate)
		{
			Console.Write("{0}\n", file);
			string cscode = File.ReadAllText(file);

			// Insert the license if the license isn't already there
			if (!cscode.Contains(Copyright.LicenseRegion))
			{
				StringBuilder builder = new StringBuilder(Copyright.FullLicenseText);
				builder.Append(cscode);
				cscode = builder.ToString();
			}

			cscode = UpdateLicense(cscode, newDate);

			File.WriteAllText(file, cscode);
		}

		private static string UpdateLicense(string text, string newDate)
		{
			// Find the beginning of the copyright line
			int start = text.IndexOf("/");
			// Find the end of the copyright line
			int end = text.IndexOf(".", start);

			string oldCopyright = text.Substring(start, end - start + 1);
			string newCopyright = string.Format("// Copyright (c) {0}, ClearCanvas Inc.", newDate);

			if (oldCopyright != newCopyright)
				text = text.Replace(oldCopyright, newCopyright);

			return text;
		}
	}
}