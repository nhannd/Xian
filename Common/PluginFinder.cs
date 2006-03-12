using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Summary description for PluginFinder.
	/// </summary>
	internal class PluginFinder : MarshalByRefObject
	{
		// Private attributes
		private StringCollection m_PluginFileList = new StringCollection();

		// Constructor
		public PluginFinder() { }

		// Properties
		public StringCollection PluginFileList { get {	return m_PluginFileList; } }

		// Public methods
		public void FindPlugin(string path)
		{
			Platform.CheckForNullReference(path, "path");
			Platform.CheckForEmptyString(path, "path");

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
				Platform.Log(String.Format(SR.LogFoundUnmanagedDLL, e.FileName));
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

		private bool IsPlugin(Assembly asm)
		{
			Platform.CheckForNullReference(asm, "asm");

			foreach (Type type in asm.GetExportedTypes())
			{
				if (typeof(Plugin).IsAssignableFrom(type))
					return true;
			}

			return false;
		}
	}
}
