#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
				Console.Write("Usage:   COPYRIGHTTOOL path newDate\n");
				Console.Write("Example: COPYRIGHTTOOL C:\\Trunk 2006-2007\n");
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
				Console.Write("Enumerating files...\n");
				string[] csfiles = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

				foreach (string file in csfiles)
				{
					ProcessFile(file, newDate);
				}
			}
		}

		private static void ProcessFile(string file, string newDate)
		{
			// Don't process designer generated files.
			string lowerCaseFile = file.ToLower();
			if (lowerCaseFile.Contains(".designer.cs") )
				return;

			Encoding encoding;
			string cscode;
			using (StreamReader reader = new StreamReader(file, true))
			{
				encoding = reader.CurrentEncoding;
				cscode = reader.ReadToEnd();
			}

			// Insert the license if the license isn't already there
			if (!cscode.Contains(Copyright.LicenseRegion))
			{
				StringBuilder builder = new StringBuilder(Copyright.FullLicenseText);
				builder.Append(cscode);
				cscode = builder.ToString();
			}

			// Update the copyright in the license text
			bool licensedChanged;
			cscode = UpdateLicense(cscode, newDate, out licensedChanged);

			// Update the copyright in the assembly level attribute
			bool assemblyCopyrightChanged = false;

			if (Path.GetFileName(file) == "AssemblyInfo.cs")
				cscode = UpdateAssemblyCopyright(cscode, newDate, out assemblyCopyrightChanged);

			if (licensedChanged || assemblyCopyrightChanged)
			{
				using (StreamWriter writer = new StreamWriter(file, false, encoding))
				{
					writer.Write(cscode);
				}
				Console.Write("UPDATED: {0}\n", file);
			}
			else 
				Console.Write("UNCHANGED: {0}\n", file);
		}

		private static string UpdateLicense(string text, string newDate, out bool licensedChanged)
		{
			// Find the beginning of the copyright line
			int start = text.IndexOf("/");
			// Find the end of the copyright line
			int end = text.IndexOf(".", start);

			string oldCopyright = text.Substring(start, end - start + 1);
			string newCopyright = string.Format("// Copyright (c) {0}, ClearCanvas Inc.", newDate);

			text = ReplaceCopyright(text, oldCopyright, newCopyright, out licensedChanged);

			return text;
		}

		private static string UpdateAssemblyCopyright(string text, string newDate, out bool assemblyCopyrightChanged)
		{
			int start = text.IndexOf("[assembly: AssemblyCopyright");

			if (start == -1)
				start = text.IndexOf("[assembly : AssemblyCopyright");

			int end = text.IndexOf("]", start);

			string oldCopyright = text.Substring(start, end - start + 1);
			string newCopyright = string.Format("[assembly: AssemblyCopyright(\"Copyright (c) {0}\")]", newDate);

			text = ReplaceCopyright(text, oldCopyright, newCopyright, out assemblyCopyrightChanged);

			return text;
		}

		private static string ReplaceCopyright(string text, string oldCopyright, string newCopyright, out bool copyrightChanged)
		{
			if (oldCopyright != newCopyright)
			{
				text = text.Replace(oldCopyright, newCopyright);
				copyrightChanged = true;
			}
			else
				copyrightChanged = false;

			return text;
		}
	}
}