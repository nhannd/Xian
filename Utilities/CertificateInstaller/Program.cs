#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.IO;

namespace ClearCanvas.Utilities.CertificateInstaller
{
	internal class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				PrintUsage();
			}
			else
			{
				string certificateFilePath = args[0] ?? "";
				string password = null;
				StoreName storeName = StoreName.My;
				StoreLocation storeLocation = StoreLocation.CurrentUser;

				if (!File.Exists(certificateFilePath))
				{
					Console.WriteLine("The specified file does not exist.");
				}
				else
				{
					string storeArg = null;
					string locationArg = null;

					for (int i = 1; i < args.Length; ++i)
					{
						Match match = Regex.Match(args[i], "/p:(.*)");
						if (match != null && match.Groups.Count > 1 && match.Groups[0].Captures.Count > 0)
							password = match.Groups[1].Captures[0].Value;

						match = Regex.Match(args[i], "/s:(.*)");
						if (match != null && match.Groups.Count > 1 && match.Groups[0].Captures.Count > 0)
							storeArg = match.Groups[1].Captures[0].Value;

						match = Regex.Match(args[i], "/l:(.*)");
						if (match != null && match.Groups.Count > 1 && match.Groups[0].Captures.Count > 0)
							locationArg = match.Groups[1].Captures[0].Value;
					}

					if (!string.IsNullOrEmpty(storeArg))
					{
						try
						{
							EnumConverter converter = new EnumConverter(typeof (StoreName));
							storeName = (StoreName) converter.ConvertFromInvariantString(storeArg);
						}
						catch(Exception)
						{
							Console.WriteLine("The specified store name could not be found.");
							return;
						}
					}

					if (!string.IsNullOrEmpty(locationArg))
					{
						try
						{
							EnumConverter converter = new EnumConverter(typeof(StoreLocation));
							storeLocation = (StoreLocation)converter.ConvertFromInvariantString(locationArg);
						}
						catch (Exception)
						{
							Console.WriteLine("The specified store location could not be found.");
							return;
						}
					}

					Console.WriteLine("Installing Certificate ...");
					Console.WriteLine(String.Format("Certificate: {0}", certificateFilePath));
					Console.WriteLine(String.Format("Store: {0}", storeName.ToString()));
					Console.WriteLine(String.Format("Location: {0}", storeLocation.ToString()));

					try
					{
						InstallCertificate(certificateFilePath, password, storeName, storeLocation);
						Console.WriteLine("Certificate installed successfully.");
					}
					catch(Exception e)
					{
						Console.WriteLine("Failed to install certificate:");
						Console.WriteLine(e.ToString());
					}
				}
			}
		}

		private static void InstallCertificate(string filename, string password, StoreName storeName, StoreLocation storeLocation)
		{
			X509Store certificateStore = new X509Store(storeName, storeLocation);
			certificateStore.Open(OpenFlags.ReadWrite);
			
			X509Certificate2 certificate = new X509Certificate2();
			X509KeyStorageFlags flags = X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable;
			if (storeLocation == StoreLocation.CurrentUser)
				flags |= X509KeyStorageFlags.UserKeySet;
			else
				flags |= X509KeyStorageFlags.MachineKeySet;

			certificate.Import(filename, password, flags);
			certificateStore.Add(certificate);
			certificateStore.Close();
		}

		private static void PrintUsage()
		{
			EnumConverter converter = new EnumConverter(typeof(StoreName));

			Console.WriteLine("Usage:");
			Console.WriteLine("ClearCanvas.Utilities.CertificateInstaller.exe <certificate file name> [/p:password] [/s:<store name>] [/l:<store location>]");
			Console.WriteLine();
			Console.WriteLine("Store names:");
			Console.WriteLine("My (default)");

			foreach (StoreName storeName in converter.GetStandardValues())
			{
				if (storeName != StoreName.My)
					Console.WriteLine(storeName);
			}

			Console.WriteLine();
			Console.WriteLine("Store locations:");
			Console.WriteLine("CurrentUser (default)");
			Console.WriteLine("LocalMachine");
		}
	}
}
