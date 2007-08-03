using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace ClearCanvas.Common
{
	public class PluginFinder : MarshalByRefObject
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
                Platform.Log(LogLevel.Info, SR.LogFoundUnmanagedDLL, e.FileName);
			}
			catch (FileNotFoundException e)
			{
				Platform.Log(LogLevel.Error,e);
				throw e;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error,e);
				throw e;
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
