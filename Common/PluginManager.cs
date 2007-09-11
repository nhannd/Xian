using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Security.Policy;
using System.Security;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Loads and keeps track of all plugins.
	/// </summary>
	/// <remarks>
	/// The PluginManager class is at the heart of the ClearCanvas framework.
	/// It provides <I>all</I> the functionality required for loading binary components known
	/// as <i>plugins</i> at runtime.  A plugin is the basic building block for extending the
	/// functionality of the application.
	///	</remarks>
	public class PluginManager 
	{
        private PluginInfo[] _plugins;
        private ExtensionInfo[] _extensions;
        private ExtensionPointInfo[] _extensionPoints;
		private string _pluginDir;
		private event EventHandler<PluginLoadedEventArgs> _pluginProgressEvent;

		// Constructor is internal, since we only ever one instance of it and that
		// one instance is created through the Platform class.
		internal PluginManager(string pluginDir)
		{
			Platform.CheckForNullReference(pluginDir, "pluginDir");
			Platform.CheckForEmptyString(pluginDir, "pluginDir");

			_pluginDir = pluginDir;
		}

        /// <summary>
        /// The set of installed plugins.  If plugins have not yet been loaded into memory,
        /// querying this property will cause them to be loaded.
        /// </summary>
        public PluginInfo[] Plugins
        {
            get 
            {
                if (_plugins == null)
                {
                    LoadPlugins();
                }
                return _plugins;
            }
        }

        /// <summary>
        /// The set of extensions defined across all installed plugins.  If plugins have not yet been loaded
        /// into memory, querying this property will cause them to be loaded.
        /// </summary>
        public ExtensionInfo[] Extensions
        {
            get
            {
                if (_extensions == null)
                {
                    LoadPlugins();
                }
                return _extensions;
            }
        }

        /// <summary>
        /// The set of extension points defined across all installed plugins.  If plugins have not yet been loaded
        /// into memory, querying this property will cause them to be loaded.
        /// </summary>
        public ExtensionPointInfo[] ExtensionPoints
        {
            get
            {
                if (_extensionPoints == null)
                {
                    LoadPlugins();
                }
                return _extensionPoints;
            }
        }

		/// <summary>
		/// Occurs when a plugin is loaded.
		/// </summary>
		public event EventHandler<PluginLoadedEventArgs> PluginLoaded
		{
			add { _pluginProgressEvent += value; }
			remove { _pluginProgressEvent -= value; }
		}

		/// <summary>
		/// Loads all plugins in current plugin directory.
		/// </summary>
		/// <remarks>
		/// This method will traverse the plugin directory and all its subdirectories loading
		/// all valid plugin assemblies.  A valid plugin is an assembly that contains a class
		/// derived from <see cref="Plugin"/>.  Plugins are loaded only the first time this
		/// method is called; subsequent calls are ignored.
		/// </remarks>
		/// <exception cref="PluginException">Specified plugin directory does not exist or 
		/// a problem with the loading of a plugin.</exception>
		public void LoadPlugins()
		{
            if (!RootPluginDirectoryExists())
                throw new PluginException(SR.ExceptionPluginDirectoryNotFound);

            string[] pluginFiles = FindPlugins(_pluginDir);

            Assembly[] assemblies = LoadFoundPlugins(pluginFiles);
            _plugins = ProcessAssemblies(assemblies);

            // If no plugins could be loaded, throw a fatal exception
            if (_plugins.Length == 0)
                throw new PluginException(SR.ExceptionUnableToLoadPlugins);

            List<ExtensionInfo> extList = new List<ExtensionInfo>();
            foreach (PluginInfo plugin in _plugins)
            {
                extList.AddRange(plugin.Extensions);
            }

            // hack: add extensions from ClearCanvas.Common, which isn't technically a plugin
            extList.AddRange(PluginInfo.DiscoverExtensions(this.GetType().Assembly));
            _extensions = extList.ToArray();

            List<ExtensionPointInfo> epList = new List<ExtensionPointInfo>();
            foreach (PluginInfo plugin in _plugins)
            {
                epList.AddRange(plugin.ExtensionPoints);
            }
            // hack: add extension points from ClearCanvas.Common, which isn't technically a plugin
            epList.AddRange(PluginInfo.DiscoverExtensionPoints(this.GetType().Assembly));
            _extensionPoints = epList.ToArray();
        }

        private PluginInfo[] ProcessAssemblies(Assembly[] assemblies)
        {
            List<PluginInfo> plugins = new List<PluginInfo>();
            for(int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    object[] attrs = assemblies[i].GetCustomAttributes(typeof(PluginAttribute), false);
                    if (attrs.Length > 0)
                    {
                        PluginAttribute a = (PluginAttribute)attrs[0];
                        plugins.Add(new PluginInfo(assemblies[i], a.Name, a.Description));
                    }

                }
                catch (ReflectionTypeLoadException e)
                {
                    // this exception usually means one of the dependencies is missing
                    Platform.Log(LogLevel.Error, SR.LogFailedToProcessPluginAssembly, assemblies[i].FullName);
                    
                    // log a detail message for each missing dependency
                    foreach (Exception loaderException in e.LoaderExceptions)
                    {
                        // just log the message, don't need the full stack trace
                        Platform.Log(LogLevel.Error, loaderException.Message);
                    }
                }
                catch (Exception e)
                {
                    // there was a problem processing this assembly
                    Platform.Log(LogLevel.Error, e, SR.LogFailedToProcessPluginAssembly, assemblies[i].FullName);
                }
            }
            return plugins.ToArray();
        }

        private bool RootPluginDirectoryExists()
		{
			return Directory.Exists(_pluginDir);
		}

        private string[] FindPlugins(string path)
		{
			Platform.CheckForNullReference(path, "path");
			Platform.CheckForEmptyString(path, "path");

			AppDomain domain = null;
            string[] pluginFiles = null;

			try
			{
				EventsHelper.Fire(_pluginProgressEvent, this, new PluginLoadedEventArgs(SR.MessageFindingPlugins));

				// Create a secondary AppDomain where we can load all the DLLs in the plugin directory
#if MONO
				domain = AppDomain.CreateDomain("Secondary");
#else		
				Evidence evidence = new Evidence(
					new object[] { new Zone(SecurityZone.MyComputer) },
					new object[] { });

				PermissionSet permissions =
					SecurityManager.ResolvePolicy(evidence);

				AppDomainSetup setup = new AppDomainSetup();
				setup.ApplicationBase =	AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
				domain = AppDomain.CreateDomain(
					"Secondary", evidence, setup,
					permissions, new StrongName[] { }); 
#endif			
				Assembly asm = Assembly.GetExecutingAssembly();

				// Instantiate the finder in the secondary domain
				PluginFinder finder = domain.CreateInstanceAndUnwrap(asm.FullName, "ClearCanvas.Common.PluginFinder") as PluginFinder;

				// Assign the FileProcessor's delegate to the finder
				FileProcessor.ProcessFile del = new FileProcessor.ProcessFile(finder.FindPlugin);

				// Process the plugin directory
				FileProcessor.Process(path, "*.dll", del, true);

				// Get the list of legitimate plugin DLLs
                pluginFiles = finder.PluginFiles;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error,e);
				throw e;
			}
			finally
			{
				// Unload the domain so that we free up memory used on loading non-plugin DLLs
				if (domain != null)
					AppDomain.Unload(domain);

                if (pluginFiles == null || pluginFiles.Length == 0)
					throw new PluginException(SR.ExceptionNoPluginsFound);
			}
            return pluginFiles;
		}

		private Assembly[] LoadFoundPlugins(string[] pluginFileList)
		{
			Platform.CheckForNullReference(pluginFileList, "pluginFileList");

			PluginLoader loader = new PluginLoader();

			// Load the legitimate plugins into the primary AppDomain
			foreach (string pluginFile in pluginFileList)
			{
				loader.LoadPlugin(pluginFile);
				string pluginName = Path.GetFileName(pluginFile);
				EventsHelper.Fire(_pluginProgressEvent, this, new PluginLoadedEventArgs(String.Format(SR.FormatLoadingPlugin, pluginName)));
			}

			return loader.PluginAssemblies;
		}
	}
}