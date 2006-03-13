using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Summary description for PluginLoader.
	/// </summary>
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
		public void LoadPlugin(string path)
		{
			Platform.CheckForNullReference(path, "path");
			Platform.CheckForEmptyString(path, "path");

			try
			{
				Assembly asm = Assembly.LoadFrom(path);
                _pluginList.Add(asm);

				Platform.Log(String.Format(SR.LogPluginLoaded, path));
			}
			catch (FileNotFoundException e)
			{
				bool rethrow = Platform.HandleException(e);

				if (rethrow)
					throw;
			}
			catch (Exception e)
			{
				bool rethrow = Platform.HandleException(e);

				if (rethrow)
					throw;
			}
		}

		private Type GetPluginType(Assembly asm)
		{
			Platform.CheckForNullReference(asm, "asm");

			foreach (Type type in asm.GetExportedTypes())
			{
				if (typeof(Plugin).IsAssignableFrom(type))
					return type;
			}

			return null;
		}
	}
}
