///////////////////////////////////////////////////////////
//
//  PluginManager.cs
//
//  Created on:      08-Feb-2005 10:35:54 PM
//
//  Original author: Norman Young
//
///////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace ClearCanvas.Common.Platform
{
	public class PluginManager 
	{
		// Private attributes
		private ArrayList m_PluginList = new ArrayList();
		private bool m_PluginsLoaded = false;
		private string m_PluginDir;

		// Constructor
		public PluginManager(string pluginDir)
		{
			m_PluginDir = pluginDir;
		}

		// Properties
		public bool PluginsLoaded {	get { return m_PluginsLoaded; }	}

		public int Count
		{
			get
			{
				return m_PluginList.Count;
			}
		}

		// Public methods
		public void LoadPlugins()
		{
			if (PluginsLoaded)
				return;

			if (!RootPluginDirectoryExists())
				throw new PluginErrorException(SR.ExceptionPluginDirectoryNotFound);

			ArrayList pluginFileList;
			FindPlugins(m_PluginDir, out pluginFileList);
			LoadFoundPlugins(pluginFileList);
			Validate();
		}

		public void LoadPlugins(ArrayList pluginFileList)
		{
			if (!RootPluginDirectoryExists())
				throw new PluginErrorException(SR.ExceptionPluginDirectoryNotFound);

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

		private void FindPlugins(string path, out ArrayList pluginFileList)
		{
			AppDomain domain = null;
			pluginFileList = null;

			try
			{
				// Create a secondary AppDomain where we can load all the DLLs in the plugin directory
				domain = AppDomain.CreateDomain("Secondary");

				Assembly asm = Assembly.GetExecutingAssembly();

				// Instantiate the finder in the secondary domain
				PluginFinder finder = (PluginFinder) domain.CreateInstanceAndUnwrap(asm.FullName, "ClearCanvas.Common.Platform.PluginFinder");

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
					throw new PluginWarningException(SR.ExceptionNoPluginsFound);
			}
		}

		private void LoadFoundPlugins(ArrayList pluginFileList)
		{
			PluginLoader loader = new PluginLoader();

			// Load the legitimate plugins into the primary AppDomain
			foreach (string pluginFile in pluginFileList)
				loader.LoadPlugin(pluginFile);				

			m_PluginList = loader.PluginList;
		}

		private void Validate()
		{
			if (m_PluginList.Count > 0)
				m_PluginsLoaded = true;

			// If no plugins could be loaded, throw a fatal exception
			if (!m_PluginsLoaded)
				throw new PluginErrorException(SR.ExceptionUnableToLoadPlugins);
		}

		private Plugin GetPlugin(Plugin.PluginType type)
		{
			foreach (Plugin plugin in m_PluginList)
			{
				if (type == plugin.Type)
					return plugin;
			}

			return null;
		}

		private void StartPlugin(Plugin.PluginType type)
		{
			LoadPlugins();
			Plugin plugin = GetPlugin(type);
			StartPlugin(plugin);
		}

		private void StartPlugin(Plugin plugin)
		{
			if (plugin == null)
				throw new PluginErrorException(SR.ExceptionPluginCouldNotBeFound(plugin.Name));

			if (!plugin.Started)
				plugin.Start();
		}
	}
}