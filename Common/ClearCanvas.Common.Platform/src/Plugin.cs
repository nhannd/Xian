using System;
using System.Resources;
using System.Reflection;
using System.Collections.Generic;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Abstract base class from which all plugin entry point classes are derived.
	/// </summary>
	/// <remarks>
	/// Every valid plugin assembly has a single <b>Plugin</b> derived class that acts
	/// as the plugin assembly's "entry point".  The existence of such a class tells
	/// the <see cref="PluginManager"/> that the assembly is a valid plugin when
	/// it attempts to load assemblies from the plugin directory.
	/// </remarks>
	/// <seealso cref="PluginManager"/>
	public abstract class Plugin 
	{
		// Private attributes
		private bool m_Started = false;
		private List<Type> m_ExtensionTypeList = new List<Type>();
		private Plugin m_PluginSubclass;

		/// <summary>
		/// The type of plugin.
		/// </summary>
		public enum PluginType
		{
			/// <summary>
			/// A Model plugin.
			/// </summary>
			Model,
			/// <summary>
			/// A View plugin.
			/// </summary>
			View,
			/// <summary>
			/// A regular plugin.
			/// </summary>
			Other
		}

		protected Plugin()
		{
		}

		/// <summary>
		/// Gets the name of the plugin.
		/// </summary>
		/// <value>The plugin name.</value>
		/// <remarks>
		/// By convention, the name of the plugin is the fully qualified
		/// name of the <see cref="Plugin"/> derived class.
		/// </remarks>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the plugin type.
		/// </summary>
		/// <value>The plugin type.</value>
		public abstract Plugin.PluginType Type { get; }

		/// <summary>
		/// Gets a value indicating whether the plugin has been started.
		/// </summary>
		/// <value><b>true</b> if the plugin has been started; <b>false</b> otherwise.</value>
		public bool Started { get { return m_Started; }	}

		/// <summary>
		/// Starts the plugin.
		/// </summary>
		/// <remarks>
		/// This method can be overridden to perform any initialization of the plugin
		/// that may be necessary.  The base class implementation should be called at the end
		/// so that the <see cref="Started"/> flag can be set and the starting of the
		/// plugin logged.
		/// </remarks>
		public virtual void Start()
		{
			if (!m_Started)
			{
				m_Started = true;
				string str = String.Format("Started plugin: {0}", Name);
				Platform.Log(str);
			}
		}

		/// <summary>
		/// Stops the plugin.
		/// </summary>
		/// <remarks>
		/// At present, this method is never used.  Eventually, however, when the application
		/// shuts down, Stop() will be called for all plugins.
		/// </remarks>
		public virtual void Stop()
		{
			if (m_Started)
			{
				m_Started = false;

				string str = String.Format("Stopped plugin: {0}", Name);
				Platform.Log(str);
			}
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || this.GetType() != obj.GetType()) 
				return false;

			return this.Name == ((Plugin)obj).Name;
		}

		/// <summary>
		/// Instantiates all <see cref="IExtensionPoint"/> based objects of a specified
		/// type in the executing plugin assembly.
		/// </summary>
		/// <param name="type">The type of object to instantiate. Typically an interface
		/// or base class defined in the host plugin.</param>
		/// <returns>
		/// An array of <see cref="IExtensionPoint"/> objects of the specified type.
		/// </returns>
		/// <remarks>
		/// <see cref="PluginManager.CreatePluginExtensions"/> calls this method across
		/// all plugins.
		/// </remarks>
		/// <seealso cref="PluginManager.CreatePluginExtensions"/>
		public IExtensionPoint[] CreateExtensions(Type type)
		{
			Platform.CheckForNullReference(type, "type");

			List<IExtensionPoint> extensionList = new List<IExtensionPoint>();

			foreach (Type extensionType in m_ExtensionTypeList)
			{
				if (type.IsAssignableFrom(extensionType))
				{
					if (!Started)
						Start();

					try
					{
						IExtensionPoint extension = (IExtensionPoint) Activator.CreateInstance(extensionType);
						extensionList.Add(extension);
					}
					catch (Exception e)
					{
						Platform.HandleException(e, "LogExceptionPolicy");
					}
				}
			}

			return extensionList.ToArray();
		}

		/// <summary>
		/// Initializes the plugin.
		/// </summary>
		/// <param name="pluginSubclass">The class that has subclassed <b>Plugin</b>.</param>
		/// <remarks>
		/// This method <i>must</i> be called from the constructor of the <b>Plugin</b> subclass,
		/// passing itself as the parameter.  The initialization process involves finding all
		/// types in the assembly containing the <b>Plugin</b> subclass that have been marked with
		/// the <see cref="IExtensionPoint"/> interface.
		/// </remarks>
		/// <example>
		/// <code>
		/// public class ModelPlugin : Plugin
		/// {
		///    public ModelPlugin()
		///    {
		///       Initialize(this);
		///    }
		/// }
		/// </code>
		/// </example>
		protected void Initialize(Plugin pluginSubclass)
		{
			Platform.CheckForNullReference(pluginSubclass, "pluginSubclass");

			m_PluginSubclass = pluginSubclass;
			FindExtensionTypes(pluginSubclass.GetType().Assembly);
		}

		// Private methods

		// Called from constructor of subclass to determine extension points in this plugin
		private void FindExtensionTypes(Assembly asm)
		{
			foreach (Type type in asm.GetTypes())
			{
				// Check that type is a concrete class and implements IExtensionPoint
				if (IsExtensionPoint(type) && IsConcreteClass(type))
					m_ExtensionTypeList.Add(type);
			}
		}

		private bool IsExtensionPoint(Type type)
		{
			return typeof(IExtensionPoint).IsAssignableFrom(type);
		}

		private bool IsConcreteClass(Type type)
		{
			return !type.IsAbstract && type.IsClass;
		}
	}
}
