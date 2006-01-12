using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Loads and keeps track of all plugins.
	/// </summary>
	/// <remarks>
	/// The PluginManager class is at the heart of the ClearCanvas framework.
	/// It provides <I>all</I> the functionality required for loading binary components known
	/// as <i>plugins</i> at runtime.  A plugin is the basic building block for extending the
	/// functionality of the application.  Two special plugins are noteworthy: 1) the Model 
	/// plugin and 2) the View plugin. The Model plugin is typically the domain and business 
	/// logic layer, whereas the View plugin is typically the presentation or UI layer.  
	/// A ClearCanvas application will <i>not</i> run unless the Model plugin has been 
	/// installed in the plugin directory.  To start a UI application,
	/// both the Model and View plugins need to be started.  The bootstrapping executable
	/// can do this by calling <see cref="PluginManager.StartModelPlugin"/> and 
	/// <see cref="PluginManager.StartViewPlugin"/> directly, or it can call the convenience
	/// function <see cref="Platform.StartApp"/>.
	///	</remarks>
	public class PluginManager 
	{
		private PluginList m_PluginList = new PluginList();
		private string m_PluginDir;
		private event EventHandler<PluginProgressEventArgs> m_PluginProgressEvent;

		// Constructor is internal, since we only ever one instance of it and that
		// one instance is created through the Platform class.
		internal PluginManager(string pluginDir)
		{
			Platform.CheckForNullReference(pluginDir, "pluginDir");
			Platform.CheckForEmptyString(pluginDir, "pluginDir");

			m_PluginDir = pluginDir;
		}

		/// <summary>
		/// Gets a value indicating whether any plugins have been loaded.
		/// </summary>
		/// <value><b>true</b> if at least one plugin has been loaded. <b>false</b>
		/// if no plugins have been loaded.</value>
		public bool PluginsLoaded 
		{	
			get 
			{ 
				return m_PluginList.NumberOfPlugins > 0; 
			}	
		}

		/// <summary>
		/// Gets the number of plugins currently loaded.
		/// </summary>
		/// <value>The number of plugins currently loaded.</value>
		public int NumberOfPlugins
		{
			get
			{
				return m_PluginList.NumberOfPlugins;
			}
		}

		/// <summary>
		/// Occurs when a plugin is loaded.
		/// </summary>
		public event EventHandler<PluginProgressEventArgs> PluginProgress
		{
			add { m_PluginProgressEvent += value; }
			remove { m_PluginProgressEvent -= value; }
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
			if (this.PluginsLoaded)
				return;

			if (!RootPluginDirectoryExists())
				throw new PluginException(SR.ExceptionPluginDirectoryNotFound);

			StringCollection pluginFileList;
			FindPlugins(m_PluginDir, out pluginFileList);
			LoadFoundPlugins(pluginFileList);
			Validate();
		}

		/// <summary>
		/// Loads a list of plugins.
		/// </summary>
		/// <param name="pluginFileList">A list of plugin filenames.</param>
		/// <seealso cref="LoadPlugins"/>
		/// <exception cref="PluginException">Specified plugin directory does not exist or 
		/// a problem with the loading of a plugin.</exception>
		public void LoadPlugins(StringCollection pluginFileList)
		{
			Platform.CheckForNullReference(pluginFileList, "pluginFileList");

			if (!RootPluginDirectoryExists())
				throw new PluginException(SR.ExceptionPluginDirectoryNotFound);

			LoadFoundPlugins(pluginFileList);
			Validate();
		}

		/// <summary>
		/// Gets a plugin by name.
		/// </summary>
		/// <param name="name">The name of the plugin.  By convention, the name of the
		/// plugin is the fully qualified name of the <see cref="Plugin"/> derived class.
		/// </param>
		/// <returns>The <see cref="Plugin"/> object. <b>null</b> if the specified
		/// plugin cannot be found.</returns>
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

		/// <summary>
		/// Starts a plugin by name.
		/// </summary>
		/// <param name="name">The name of the plugin.  By convention, the name of the
		/// plugin is the fully qualified name of the <see cref="Plugin"/> derived class.
		/// </param>
		/// <remarks>When this method is called, plugins will be automatically loaded 
		/// if they have not already been loaded earlier.  That is, there is no need 
		/// to call <see cref="LoadPlugins"/> first.
		/// </remarks>
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

		/// <summary>
		/// Starts the Model plugin.
		/// </summary>
		/// <remarks>When this method is called, plugins will be automatically loaded 
		/// if they have not already been loaded earlier.  That is, there is no need 
		/// to call <see cref="LoadPlugins"/> first.
		/// </remarks>
		public void StartModelPlugin()
		{
			StartPlugin(Plugin.PluginType.Model);
		}

		/// <summary>
		/// Starts the View plugin.
		/// </summary>
		/// <remarks>When this method is called, plugins will be automatically loaded 
		/// if they have not already been loaded earlier.  That is, there is no need 
		/// to call <see cref="LoadPlugins"/> first.
		/// </remarks>
		public void StartViewPlugin()
		{
			StartPlugin(Plugin.PluginType.View);
		}

		/// <summary>
		/// Instantiates all <see cref="IExtensionPoint"/> based objects of a specified
		/// type across all loaded plugins.
		/// </summary>
		/// <param name="type">The type of object to instantiate. Typically an interface
		/// or base class defined in the host plugin.</param>
		/// <returns>An array of objects of the specified type.</returns>
		/// <remarks>
		/// This method allows objects across all plugins to be instantiated without
		/// knowing the names of any concrete classes.  This is key to the idea of
		/// <i>extension points</i>.  For a host plugin to be extended by an
		/// extension plugin(s), the host plugin must be able to instantiate objects
		/// in the extension plugin(s).  Because there is no way for the host plugin
		/// to know the name of the object in the extension plugin(s) at compile time, 
		/// the objects in the extension plugin(s) must implement an interface or subclass
		/// a base class defined in the host plugin.  It is that interface or base class
		/// that is passed in as the <paramref name="type"/>.
		/// </remarks>
		/// <seealso cref="IExtensionPoint"/>
		public IExtensionPoint[] CreatePluginExtensions(Type type)
		{
			Platform.CheckForNullReference(type, "type");
			
			if (m_PluginList.NumberOfPlugins == 0)
				throw new InvalidOperationException(SR.ExceptionNoPluginsLoaded);

			List<IExtensionPoint> extensionList = new List<IExtensionPoint>();

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
				EventsHelper.Fire(m_PluginProgressEvent, this, new PluginProgressEventArgs(String.Format(SR.LoadingPlugin, pluginName)));
			}

			m_PluginList = loader.PluginList;
		}

		private void Validate()
		{
			// If no plugins could be loaded, throw a fatal exception
			if (m_PluginList.NumberOfPlugins == 0)
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