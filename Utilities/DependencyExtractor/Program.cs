#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

//#define DEVBUILD

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;

namespace ClearCanvas.Utilities.DependencyExtractor
{
	class Program
	{
		private static readonly List<string> _alreadyAnalyzed = new List<string>();
		private static readonly List<string> _dependencies = new List<string>();
		private static readonly List<string> _applicationFiles = new List<string>();

		private static readonly List<Version> _targetVersions = new List<Version>();
		private static readonly List<string> _probingFolders = new List<string>();
		private static string _outputDirectory = null;

		static void Main(string[] args)
		{
			if (args.Length <= 0)
			{
				PrintUsage();
			}
			else
			{
				string applicationPath = args[0];

				if (Directory.Exists(applicationPath))
				{
					for (int i = 1; i < args.Length; ++i)
					{
						Match match = Regex.Match(args[i], "/p:(.*)");
						if (match != null && match.Groups.Count > 1 && match.Groups[0].Captures.Count > 0)
						{
							string probing = match.Groups[1].Captures[0].Value;
							_probingFolders.AddRange((probing ?? "").Split(new char[] {';'}));
						}

						match = Regex.Match(args[i], "/v:(.*)");
						if (match != null && match.Groups.Count > 1 && match.Groups[0].Captures.Count > 0)
						{
							string versions = match.Groups[1].Captures[0].Value;
							string[] targetVersions = (versions ?? "").Split(new char[] { ';' });

							foreach (string targetVersion in targetVersions)
							{
								try
								{
									_targetVersions.Add(new Version(targetVersion));
								}
								catch(Exception e)
								{
									Console.WriteLine("Invalid target version: " + targetVersion);
								}
							}
						}

						match = Regex.Match(args[i], "/o:(.*)");
						if (match != null && match.Groups.Count > 1 && match.Groups[0].Captures.Count > 0)
						{
							_outputDirectory = match.Groups[1].Captures[0].Value;
						}
					}

					Console.WriteLine("Application path: " + applicationPath);

					bool probingFoldersValid = _probingFolders.Count > 0;
					if (probingFoldersValid)
					{
						StringBuilder builder = new StringBuilder();
						foreach (string probingFolder in _probingFolders)
						{
							if (builder.Length > 0)
								builder.Append(", ");

							builder.Append(probingFolder);
						}

						Console.WriteLine("Probing folders: " + builder.ToString());
					}
					else
					{
						Console.WriteLine("Probing folders: not specified.");
					}

					bool outputDirectoryValid = _outputDirectory != null;
					if (outputDirectoryValid)
						Console.WriteLine("Output path: " + _outputDirectory);
					else
						Console.WriteLine("Output path: not specified.");

					Console.WriteLine();

					if (probingFoldersValid)
						LoadProbingAssemblies(applicationPath);

					Analyze(applicationPath);

					if (outputDirectoryValid)
					{
						Deploy();
					}
					else
					{
						Console.WriteLine("Listing dependencies found: ");

						foreach (string dependency in _dependencies)
							Console.WriteLine(dependency);
					}
				}
				else
				{
					Console.WriteLine("Application path does not exist: " + applicationPath);
				}
			}

#if DEVBUILD
			Console.WriteLine();
			Console.WriteLine("Press a key");
			Console.ReadKey();
#endif
		}

		//pre-load the probing assemblies.
		private static void PrintUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("ClearCanvas.Utilities.DependencyExtractor.exe <application path> [/p:<probing folders>] [/o:<output path>]");
			Console.WriteLine();
			Console.WriteLine("Example:");
			Console.WriteLine(@"ClearCanvas.Utilities.DependencyExtractor.exe c:\MyAppFolder /p:common;plugins /o:c:\MyOutputFolder");
		}

		private static void LoadProbingAssemblies(string applicationPath)
		{
			Console.WriteLine("Loading probing assemblies ...");
			Console.WriteLine();

			foreach (string probingFolder in _probingFolders)
			{
				string probingFolderPath = Path.Combine(applicationPath, probingFolder);
				if (Directory.Exists(probingFolderPath))
				{
					string[] assemblyFiles = Directory.GetFiles(probingFolderPath);
					foreach (string assemblyFile in assemblyFiles)
					{
						try
						{
							//load all assemblies in 'probing' directories
							Assembly.ReflectionOnlyLoadFrom(assemblyFile);
						}
						catch (BadImageFormatException e)
						{
							Console.WriteLine("Failed to load: " + assemblyFile);
						}
						catch (Exception e)
						{
							Console.WriteLine(e.Message);
						}
					}
				}
				else
				{
					Console.WriteLine("Probing folder does not exist: " + probingFolderPath);
				}
			}
		}

		private static void Analyze(string applicationPath)
		{
			Console.WriteLine("Analyzing dependencies ...");
			Console.WriteLine();

			string[] applicationFiles = Directory.GetFiles(applicationPath, "*.*", SearchOption.AllDirectories);
			List<Assembly> assemblies = new List<Assembly>();
			
			foreach (string applicationFile in applicationFiles)
			{
				try
				{
					Assembly assembly = Assembly.LoadFile(applicationFile);
					
					Uri uri = new Uri(assembly.GetName().CodeBase);
					string localFilename = uri.LocalPath;
					if (!_applicationFiles.Contains(localFilename))
					{
						_applicationFiles.Add(localFilename);
						assemblies.Add(assembly);
					}
				}
				catch (BadImageFormatException e)
				{
					Console.WriteLine("Failed to load: " + applicationFile);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}

			foreach (Assembly assembly in assemblies)
				AddDependencies(assembly);
		}

		private static void AddDependencies(Assembly assembly)
		{
			string assemblyFile = assembly.CodeBase;
			if (_alreadyAnalyzed.Contains(assemblyFile))
				return;

			_alreadyAnalyzed.Add(assemblyFile);

			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();

			foreach (AssemblyName referencedAssemblyName in referencedAssemblies)
			{
				try
				{
					Assembly referencedAssembly = Assembly.ReflectionOnlyLoad(referencedAssemblyName.FullName);
					Uri uri = new Uri(referencedAssembly.GetName().CodeBase);
					string localFilename = uri.LocalPath;

					if (!_applicationFiles.Contains(localFilename) && !_dependencies.Contains(localFilename) && IsTargetedVersion(referencedAssembly))
						_dependencies.Add(localFilename);

					AddDependencies(referencedAssembly);
				}
				catch (BadImageFormatException e)
				{
					Console.WriteLine("Failed to load: " + referencedAssemblyName.FullName);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}

		private static bool IsTargetedVersion(Assembly assembly)
		{
			if (_targetVersions.Count == 0)
				return true;

			foreach (Version targetVersion in _targetVersions)
			{
				if (assembly.GetName().Version.Equals(targetVersion))
					return true;
			}

			return false;
		}

		private static void Deploy()
		{
			Console.WriteLine("Deploying dependencies ...");
			Console.WriteLine();

			if (!Directory.Exists(_outputDirectory))
				Directory.CreateDirectory(_outputDirectory);

			foreach (string dependency in _dependencies)
			{
				string fileName = Path.GetFileName(dependency);
				File.Copy(dependency, Path.Combine(_outputDirectory, fileName), true);
			}
		}
	}
}
