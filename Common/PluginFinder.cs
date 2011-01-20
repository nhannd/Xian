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
using System.IO;
using System.Reflection;

namespace ClearCanvas.Common
{
	/// <summary>
	/// A helper class for finding ClearCanvas plugins.
	/// </summary>
	internal class PluginFinder : MarshalByRefObject
	{
		// Private attributes
        private List<string> _pluginFiles = new List<string>();

		// Constructor
		public PluginFinder()
        {
        }

		// Properties
        public string[] PluginFiles
        {
            get { return _pluginFiles.ToArray(); }
        }

		// Public methods
		public void FindPlugin(string path)
		{
			Platform.CheckForNullReference(path, "path");
			Platform.CheckForEmptyString(path, "path");

			try
			{
				Assembly asm = LoadAssembly(path);
                Attribute[] attrs = Attribute.GetCustomAttributes(asm);
                foreach (Attribute attr in attrs)
                {
                    if (attr is PluginAttribute)
                    {
                        _pluginFiles.Add(path);

                        break;
                    }
                }
            }
			catch (BadImageFormatException e)
			{
				// Encountered an unmanaged DLL in the plugin directory; this is okay
				// but we'll log it anyway
                Platform.Log(LogLevel.Debug, SR.LogFoundUnmanagedDLL, e.FileName);
			}
			catch (FileNotFoundException e)
			{
				Platform.Log(LogLevel.Error, e, "File not found while loading plugin: {0}", path);
				throw;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Error loading plugin: {0}", path);
				throw;
			}
		}

		// Private methods
		private Assembly LoadAssembly(string path)
		{
			Platform.CheckForNullReference(path, "path");
			Platform.CheckForEmptyString(path, "path");

            // The following block of code did not work under Mono, presumably
            // because Mono expects a fully qualified assembly name

            /*
            AppDomain domain = AppDomain.CurrentDomain;

            // Set the AppDomain's relative search path
            string baseDir = domain.BaseDirectory;
            string pathDir = Path.GetDirectoryName(path);
            string relDir = pathDir.Replace(baseDir, "");
            domain.AppendPrivatePath(relDir);

            // Assembly name = filename without extension
            string assemblyName = Path.GetFileNameWithoutExtension(path);

            return domain.Load(assemblyName);
            */

            // However, the same effect can be accomplished with this single line of code
            return Assembly.LoadFrom(path);
		}
	}
}
