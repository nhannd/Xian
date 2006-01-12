using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Summary description for PluginLoader.
	/// </summary>
	internal class PluginLoader 
	{
		// Private attributes
		private PluginList m_PluginList = new PluginList();
		private bool m_FoundModelPlugin = false;

		// Constructor
		public PluginLoader() {	}

		// Properties
		public PluginList PluginList { get { return m_PluginList; } }

		// Public methods
		public void LoadPlugin(string path)
		{
			Platform.CheckForNullReference(path, "path");
			Platform.CheckForEmptyString(path, "path");

			try
			{
				Assembly asm = Assembly.LoadFrom(path);
				Type pluginType = GetPluginType(asm);

				if (pluginType == null)
					throw new PluginException(String.Format(SR.ExceptionNotAPlugin, path));

				Plugin plugin = Activator.CreateInstance(pluginType) as Plugin;

				ValidatePlugin(plugin);
				m_PluginList.AddPlugin(plugin);

				Platform.Log(String.Format(SR.LogPluginLoaded, path));
			}
			catch (Exception e)
			{
				bool rethrow = Platform.HandleException(e, "LogExceptionPolicy");

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

		private void ValidatePlugin(Plugin plugin)
		{
			Platform.CheckForNullReference(plugin, "path");

			bool exists = m_PluginList.Contains(plugin);

			if (exists)
				throw new PluginException(String.Format(SR.ExceptionDuplicatePluginFound, plugin.Name));

			if (plugin.Type == Plugin.PluginType.Model)
			{
				if (m_FoundModelPlugin)
					throw new PluginException(SR.ExceptionMoreThanOneModelPluginFound);
				else
					m_FoundModelPlugin = true;
			}
		}
	}
}
