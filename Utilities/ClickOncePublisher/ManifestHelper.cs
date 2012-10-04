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
using System.IO;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;

namespace ClickOncePublisher
{
	public static class ManifestHelper
	{
		/// <summary>
		/// Sign the manifest with provided publisher certificate
		/// </summary>
		/// <param name="manifest">manifest to sign</param>
		/// <param name="certFilePath">Path to cert file</param>
		/// <param name="password">Password for cert file</param>
		public static void SignManifest(Manifest manifest, string certFilePath, string password)
		{
			// Make sure the entered cert file exists
			if (File.Exists(certFilePath))
			{
				// Construct cert object for cert
				X509Certificate2 cert;
				if (string.IsNullOrEmpty(password))
				{
					cert = new X509Certificate2(certFilePath);
				}
				else
				{
					cert = new X509Certificate2(certFilePath, password);
				}
				SecurityUtilities.SignFile(cert, null, manifest.SourcePath);
			}
			else
			{
				throw new ArgumentException("Invalid certificate file path");
			}
		}

		

		/// <summary>
		/// Copies files to be added to a target folder, adds .deploy extension
		/// if needed, and adds them to the manifest 
		/// </summary>
		/// <param name="targetFolder">Destination folder</param>
		/// <param name="appManifest">The app manifest the file is associated with</param>
		public static void AddFilesToTargetFolder(string sourceFolder, string targetFolder, 
		                                          ApplicationManifest appManifest)
		{
			string[] files = Directory.GetFiles(sourceFolder, "*.*", SearchOption.AllDirectories);

			foreach (string file in files)
			{
				string relativePath = GetRelativeFolderPath(file, sourceFolder);

				string newFolder = Path.Combine(targetFolder, relativePath);
				Directory.CreateDirectory(newFolder);
				string newName = Path.Combine(newFolder, Path.GetFileName(file));
				if (Path.GetExtension(newName).ToLower() != ".deploy")
				{
					newName += ".deploy";
				}
				File.Copy(file, newName, true);
				//AddFile(appManifest, newName, targetFolder);
			}
		}

		/// <summary>
		/// Adds an individual file to the manifest 
		/// </summary>
		/// <param name="appManifest">The app manifest the file is associated with</param>
		public static void AddFilesToAppManifest(string rootFolder, ApplicationManifest appManifest)
		{
			string[] files = Directory.GetFiles(rootFolder, "*.*", SearchOption.AllDirectories);

			int i = 0;
			foreach (string fileName in files)
			{
				// Check to see if it is an assembly
				AssemblyIdentity assemId = null;

				try
				{
					assemId = AssemblyIdentity.FromFile(fileName);

					if (assemId != null && assemId.ProcessorArchitecture == "msil" 
						&& Path.GetFileName(fileName) != "ClearCanvas.Desktop.Executable.rsm") // valid assembly
					{
						// Add as assembly
						AssemblyReference assemblyRef = new AssemblyReference(fileName);
						string relativePath = GetRelativeFolderPath(fileName, rootFolder);
						assemblyRef.TargetPath = Path.Combine(relativePath, Path.GetFileName(fileName));
						assemblyRef.AssemblyIdentity = assemId;

						appManifest.AssemblyReferences.Add(assemblyRef);
					}
					else
					{
						// Add as a file 
						FileReference fileRef = new FileReference(fileName);
						fileRef.TargetPath = Path.Combine(GetRelativeFolderPath(fileName, rootFolder), Path.GetFileName(fileName));
						appManifest.FileReferences.Add(fileRef);
					}
				}
				catch
				{
					// Add as a file 
					FileReference fileRef = new FileReference(fileName);
					fileRef.TargetPath = Path.Combine(GetRelativeFolderPath(fileName, rootFolder), Path.GetFileName(fileName));
					appManifest.FileReferences.Add(fileRef);
				}

				i++;
			}
		}


		/// <summary>
		/// Calculates a relative folder path between a file and a reference folder
		/// </summary>
		/// <param name="fileName">The full path to the file</param>
		/// <param name="folder">The folder to calculate the relative path from</param>
		/// <returns>The relative path</returns>
		public static string GetRelativeFolderPath(string fileName, string folder)
		{
			folder = folder.Trim();
			// Special case, drive letter only
			if (Path.GetPathRoot(folder) == folder) // Drive letter, root folder
			{
				if (!folder.EndsWith("\\"))
					folder += "\\"; // Add slash for drive letter comparison
			}
			// Different drive, must specify full path
			if (Path.GetPathRoot(fileName).ToLower() != Path.GetPathRoot(folder).ToLower())
			{
				return Path.GetDirectoryName(fileName);
			}

			// Strip trailing slash
			if (folder.EndsWith("\\"))
				folder = folder.Remove(folder.Length - 1);

			// Chop the paths into parts
			string[] fileParts = Path.GetDirectoryName(fileName).Split('\\');
			string[] folderParts = folder.Split('\\');
			// Same folder, empty relative folder path
			if (Path.GetDirectoryName(fileName).ToLower() == folder.ToLower())
			{
				return string.Empty;
			}

			// Find where the paths no longer match
			int mismatchCount = folderParts.Length; // assume the full folder path matches
			for (int i = 1; i < folderParts.Length; i++)
			{
				if ((i >= fileParts.Length - 1) ||
				    (fileParts[i].ToLower() != folderParts[i].ToLower()))
				{
					mismatchCount = i;
					break;
				}
			}

			// Build relative path
			string relPath = string.Empty;
			for (int i = mismatchCount; i < folderParts.Length; i++)
			{
				relPath += "..\\";
			}
			for (int i = mismatchCount; i < fileParts.Length; i++)
			{
				relPath += fileParts[i] + "\\";
			}
			if (relPath.EndsWith("\\"))
				relPath = relPath.Remove(relPath.Length - 1);
			return relPath;
		}
	}
}