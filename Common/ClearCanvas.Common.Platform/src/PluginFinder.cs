using System;
using System.Collections;
using System.Reflection;
using System.IO;

namespace ClearCanvas.Common.Platform
{
	/// <summary>
	/// Summary description for PluginFinder.
	/// </summary>
	internal class PluginFinder : MarshalByRefObject
	{
		// Private attributes
		private ArrayList m_PluginFileList = new ArrayList();

		// Constructor
		public PluginFinder() { }

		// Properties
		public ArrayList PluginFileList { get {	return m_PluginFileList; } }

		// Public methods
		public void FindPlugin(string path)
		{
			try
			{
				Assembly asm = LoadAssembly(path);

				if (IsPlugin(asm))
					m_PluginFileList.Add(path);
			}
			catch (BadImageFormatException e)
			{
				// Encountered an unmanaged DLL in the plugin directory; this is okay
				// but we'll log it anyway
				string str = String.Format("Found unmanaged DLL: {0}", e.FileName);
				Platform.Log(str);
			}
			catch (Exception e)
			{
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
