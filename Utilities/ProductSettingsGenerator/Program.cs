#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Utilities.ProductSettingsGenerator
{
	public static class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				Application.Run(new GeneratorForm());
			}
			else if (args[0] == "-s")
			{
				var settings = new EncryptedProductSettings(
					ProductSettings.Default.Component,
					ProductSettings.Default.Product,
					ProductSettings.Default.Edition,
					ProductSettings.Default.Release,
					ProductSettings.Default.Version,
					ProductSettings.Default.VersionSuffix,
					ProductSettings.Default.Copyright,
					ProductSettings.Default.License);

				settings.Save();

				Console.WriteLine("Test");
			}
			else
			{
				var component = string.Empty;
				var product = string.Empty;
				var edition = string.Empty;
				var release = string.Empty;
				var version = string.Empty;
				var suffix = string.Empty;
				var copyright = string.Empty;
				var license = string.Empty;

				var outputFile = string.Empty;
				var outputFormatXml = false;
				var help = false;

				try
				{
					for (var n = 0; n < args.Length; n++)
					{
						switch (args[n].ToLowerInvariant())
						{
							case "-c":
							case "--component":
								component = args[++n];
								break;
							case "-p":
							case "--product":
								product = args[++n];
								break;
							case "-e":
							case "--edition":
								edition = args[++n];
								break;
							case "-r":
							case "--release":
								release = args[++n];
								break;
							case "-v":
							case "--version":
								version = args[++n];
								break;
							case "-vx":
							case "--suffix":
								suffix = args[++n];
								break;
							case "-ct":
							case "--copyright":
								copyright = args[++n];
								break;
							case "-lt":
							case "--license":
								license = args[++n];
								break;
							case "-cf":
							case "--copyrightfile":
								if (!TryReadFile(args[++n], out copyright))
									help = true;
								break;
							case "-lf":
							case "--licensefile":
								if (!TryReadFile(args[++n], out license))
									help = true;
								break;
							case "-o":
							case "-out":
							case "--output":
								outputFile = args[++n];
								break;
							case "-ox":
							case "--xml":
								outputFormatXml = true;
								break;
							case "-?":
							case "-h":
							case "--help":
							default:
								help = true;
								break;
						}
					}
				}
				catch (IndexOutOfRangeException)
				{
					Console.WriteLine("Missing argument.");
					Console.WriteLine();
					help = true;
				}

				if (help)
				{
					Console.WriteLine("Product Settings Generator");
					Console.WriteLine();

					return;
				}

				var settings = new EncryptedProductSettings(component, product, edition, release, version, suffix, copyright, license);
				var writer = new WriteFileDelegate(settings.Save);
				if (!outputFormatXml)
					writer = new WriteFileDelegate(fn => new ProductSettingsConfiguration(settings).Save(fn));

				if (string.IsNullOrEmpty(outputFile))
				{
					var tempFile = Path.GetTempFileName();
					try
					{
						File.Delete(tempFile);
						writer.Invoke(tempFile);
						foreach (var line in File.ReadAllLines(tempFile, Encoding.Default))
						{
							Console.WriteLine(line);
						}
					}
					finally
					{
						File.Delete(tempFile);
					}
				}
				else
				{
					writer.Invoke(outputFile);
					Console.WriteLine("Product settings written to {0}", outputFile);
				}
			}
		}

		private static bool TryReadFile(string filename, out string contents)
		{
			try
			{
				contents = File.ReadAllText(filename);
				return true;
			}
			catch (Exception)
			{
				contents = null;
				Console.WriteLine("There was an error reading the file {0}", filename);
				return false;
			}
		}

		private delegate void WriteFileDelegate(string filename);
	}
}