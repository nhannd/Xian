#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
	internal class PluginLoader 
	{
		// Private attributes
        private List<Assembly> _pluginList = new List<Assembly>();

		// Constructor
		public PluginLoader() {	}

		// Properties
        public Assembly[] PluginAssemblies
        {
            get { return _pluginList.ToArray(); }
        }

		// Public methods
		public Assembly LoadPlugin(string path)
		{
			Platform.CheckForNullReference(path, "path");
			Platform.CheckForEmptyString(path, "path");

			try
			{
				Assembly asm = Assembly.LoadFrom(path);
                _pluginList.Add(asm);

				Platform.Log(LogLevel.Debug, SR.LogPluginLoaded, path);

				return asm;
			}
			catch (FileNotFoundException e)
			{
				Platform.Log(LogLevel.Error, e);
				throw;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				throw;
			}
		}

		private Type GetPluginType(Assembly asm)
		{
			Platform.CheckForNullReference(asm, "asm");

			foreach (Type type in asm.GetExportedTypes())
			{
				if (typeof(PluginInfo).IsAssignableFrom(type))
					return type;
			}

			return null;
		}
	}
}
