using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;

namespace ClearCanvas.Common.Platform
{
	/// <summary>
	/// Summary description for PluginLoader.
	/// </summary>
	internal class PluginLoader 
	{
		// Private attributes
		private ArrayList m_PluginList = new ArrayList();
		private bool m_FoundModelPlugin = false;
		private bool m_Warning = false;

		// Constructor
		public PluginLoader() {	}

		// Properties
		public ArrayList PluginList { get {	return m_PluginList; } }
		public bool Warning { get { return m_Warning; } }

		// Public methods
		public void LoadPlugin(string path)
		{
			try
			{
				Assembly asm = Assembly.LoadFrom(path);
				Type pluginType = GetPluginType(asm);

				if (pluginType == null)
					throw new PluginException(SR.ExceptionNotAPlugin(path));

				Plugin plugin = (Plugin) Activator.CreateInstance(pluginType);

				ValidatePlugin(plugin);
				m_PluginList.Add(plugin);

				string str = String.Format("Loaded plugin: {0}", path);
				Platform.Log(str);
			}
			catch (Exception e)
			{
				m_Warning = true;

				bool rethrow = Platform.HandleException(e, "LogExceptionPolicy");

				if (rethrow)
					throw;
			}
		}

		private Type GetPluginType(Assembly asm)
		{
			foreach (Type type in asm.GetExportedTypes())
			{
				if (typeof(Plugin).IsAssignableFrom(type))
					return type;
			}

			return null;
		}

		private void ValidatePlugin(Plugin plugin)
		{
			bool exists = m_PluginList.Contains(plugin);

			if (exists)
				throw new PluginException(SR.ExceptionDuplicatePluginFound(plugin.Name));

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
