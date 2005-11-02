using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace ClearCanvas.Common
{
	public class PluginManager 
	{
		// Private attributes
		private PluginList m_PluginList = new PluginList();
		private bool m_PluginsLoaded = false;
		private string m_PluginDir;
		private event EventHandler m_PluginProgressEvent;

		// Constructor
		public PluginManager(string pluginDir)
		{
			Platform.CheckForNullReference(pluginDir, "pluginDir");
			Platform.CheckForEmptyString(pluginDir, "pluginDir");

			m_PluginDir = pluginDir;
		}

		// Properties
		public bool PluginsLoaded {	get { return m_PluginsLoaded; }	}

		public int NumberOfPlugins
		{
			get
			{
				return m_PluginList.NumberOfPlugins;
			}
		}

		// Event accessor
		public event EventHandler PluginProgress
		{
			add
			{
				m_PluginProgressEvent += value;
			}
			remove
			{
				m_PluginProgressEvent -= value;
			}
		}

		// Public methods
		public void LoadPlugins()
		{
			if (this.PluginsLoaded)
				return;

			if (!RootPluginDirectoryExists())
				throw new PluginException(SR.ExceptionPluginDirectoryNotFound);

			StringCollection pluginFileList;
			FindPlugins(m_PluginDir, out pluginFileList);
			LoadFoundPlugins(pluginFileList);
			Validate();
		}

		public void LoadPlugins(StringCollection pluginFileList)
		{
			Platform.CheckForNullReference(pluginFileList, "pluginFileList");

			if (!RootPluginDirectoryExists())
				throw new PluginException(SR.ExceptionPluginDirectoryNotFound);

			LoadFoundPlugins(pluginFileList);
			Validate();
		}

		public Plugin GetPlugin(string name)
		{
			Platform.CheckForNullReference(name, "name");
			Platform.CheckForEmptyString(name, "name");

			foreach (Plugin plugin in m_PluginList)
			{
				if (name == plugin.Name)
					return plugin;
			}

			return null;
		}

		public void StartPlugin(string name)
		{
			Platform.CheckForNullReference(name, "name");
			Platform.CheckForEmptyString(name, "name");

			LoadPlugins();
			Plugin plugin = GetPlugin(name);

			if (plugin == null)
				throw new PluginException();

			StartPlugin(plugin);
		}

		public void StartModelPlugin()
		{
			StartPlugin(Plugin.PluginType.Model);
		}

		public void StartViewPlugin()
		{
			StartPlugin(Plugin.PluginType.View);
		}

		public Object[] CreatePluginExtensions(Type type)
		{
			Platform.CheckForNullReference(type, "type");

			ArrayList extensionList = new ArrayList();

			foreach (Plugin plugin in m_PluginList)
				extensionList.AddRange(plugin.CreateExtensions(type));

			return extensionList.ToArray();
		}

		// Private methods
		private bool RootPluginDirectoryExists()
		{
			return Directory.Exists(m_PluginDir);
		}

		private void FindPlugins(string path, out StringCollection pluginFileList)
		{
			Platform.CheckForNullReference(path, "path");
			Platform.CheckForEmptyString(path, "path");

			AppDomain domain = null;
			pluginFileList = null;

			try
			{
				EventsHelper.Fire(m_PluginProgressEvent, this, new PluginProgressEventArgs("Finding plugins..."));

				// Create a secondary AppDomain where we can load all the DLLs in the plugin directory
				domain = AppDomain.CreateDomain("Secondary");

				Assembly asm = Assembly.GetExecutingAssembly();

				// Instantiate the finder in the secondary domain
				PluginFinder finder = domain.CreateInstanceAndUnwrap(asm.FullName, "ClearCanvas.Common.PluginFinder") as PluginFinder;

				// Assign the FileProcessor's delegate to the finder
				FileProcessor.ProcessFile del = new FileProcessor.ProcessFile(finder.FindPlugin);

				// Process the plugin directory
				FileProcessor.Process(path, "*.dll", del, true);

				// Get the list of legitimate plugin DLLs
				pluginFileList = finder.PluginFileList;
			}
			catch (Exception e)
			{
				bool rethrow = Platform.HandleException(e, "LogExceptionPolicy");

				if (rethrow)
					throw;
			}
			finally
			{
				// Unload the domain so that we free up memory used on loading non-plugin DLLs
				if (domain != null)
					AppDomain.Unload(domain);

				if (pluginFileList == null || pluginFileList.Count == 0)
					throw new PluginException(SR.ExceptionNoPluginsFound);
			}
		}

		private void LoadFoundPlugins(StringCollection pluginFileList)
		{
			Platform.CheckForNullReference(pluginFileList, "pluginFileList");

			PluginLoader loader = new PluginLoader();

			// Load the legitimate plugins into the primary AppDomain
			foreach (string pluginFile in pluginFileList)
			{
				loader.LoadPlugin(pluginFile);
				string pluginName = Path.GetFileName(pluginFile);
				EventsHelper.Fire(m_PluginProgressEvent, this, new PluginProgressEventArgs(SR.LoadingPlugin(pluginName)));
			}

			m_PluginList = loader.PluginList;
		}

		private void Validate()
		{
			if (m_PluginList.NumberOfPlugins > 0)
				m_PluginsLoaded = true;

			// If no plugins could be loaded, throw a fatal exception
			if (!m_PluginsLoaded)
				throw new PluginException(SR.ExceptionUnableToLoadPlugins);
		}

		private Plugin GetPlugin(Plugin.PluginType type)
		{
			Platform.CheckForNullReference(type, "type");

			foreach (Plugin plugin in m_PluginList)
			{
				if (type == plugin.Type)
					return plugin;
			}

			return null;
		}

		private void StartPlugin(Plugin.PluginType type)
		{
			Platform.CheckForNullReference(type, "type");

			LoadPlugins();
			Plugin plugin = GetPlugin(type);

			if (plugin == null)
				throw new PluginException();

			StartPlugin(plugin);
		}

		private void StartPlugin(Plugin plugin)
		{
			Platform.CheckForNullReference(plugin, "plugin");

			if (!plugin.Started)
				plugin.Start();
		}
	}
}