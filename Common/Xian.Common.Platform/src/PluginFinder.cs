using System;
using System.Collections;
using System.Reflection;
using System.IO;

namespace Xian.Common.Platform
{
	/// <summary>
	/// Summary description for PluginFinder.
	/// </summary>
	internal class PluginFinder : MarshalByRefObject
	{
		// Private attributes
		private ArrayList m_PluginFileList = new ArrayList();
		private bool m_Warning = false;

		// Constructor
		public PluginFinder() { }

		// Properties
		public ArrayList PluginFileList { get {	return m_PluginFileList; } }
		public bool Warning { get { return m_Warning; } }

		// Public methods
		public void FindPlugin(string path)
		{
			try
			{
				Assembly asm = LoadAssembly(path);

				if (IsPlugin(asm))
					m_PluginFileList.Add(path);
			}
			catch (Exception e)
			{
				m_Warning = true;

				bool rethrow = Platform.HandleException(e, "LogExceptionPolicy");

				if (rethrow)
					throw;
			}
		}

		// Private methods
		private Assembly LoadAssembly(string path)
		{
			AppDomain domain = AppDomain.CurrentDomain;

			// Set the AppDomain's relative search path
			string baseDir = domain.BaseDirectory;
			string pathDir = Path.GetDirectoryName(path);
			string relDir = pathDir.Replace(baseDir, "");
			domain.AppendPrivatePath(relDir);

			// Assembly name = filename without extension
			string assemblyName = Path.GetFileNameWithoutExtension(path);
			return domain.Load(assemblyName);
		}

		bool IsPlugin(Assembly asm)
		{
			foreach (Type type in asm.GetExportedTypes())
			{
				if (typeof(Plugin).IsAssignableFrom(type))
					return true;
			}

			return false;
		}
	}
}
