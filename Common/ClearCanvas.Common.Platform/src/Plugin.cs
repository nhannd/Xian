///////////////////////////////////////////////////////////
//
//  Plugin.cs
//
//  Created on:      08-Feb-2005 10:35:55 PM
//
//  Original author: Norman Young
//
///////////////////////////////////////////////////////////

using System;
using System.Resources;
using System.Reflection;
using System.Collections;

namespace ClearCanvas.Common.Platform
{
	public abstract class Plugin 
	{
		// Private attributes
		private bool m_Started = false;
		private ArrayList m_ExtensionTypeList = new ArrayList();
		private Plugin m_PluginSubclass;

		public enum PluginType
		{
			Model,
			View,
			Other
		}

		// Constructor
		protected Plugin()
		{
		}

		// Properties
		public abstract string Name { get; }
		public abstract Plugin.PluginType Type { get; }
		public bool Started { get { return m_Started; }	}

		// Public methods		
		public virtual void Start()
		{
			if (!m_Started)
			{
				m_Started = true;
				string str = String.Format("Started plugin: {0}", Name);
				Platform.Log(str);
			}
		}

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

		public Object[] CreateExtensions(Type type)
		{
			Platform.CheckForNullReference(type, "type");

			ArrayList extensionList = new ArrayList();

			foreach (Object obj in m_ExtensionTypeList)
			{
				Type extensionType = (Type) obj;

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

		// Protected methods
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

/* Plugin Template

	using ClearCanvas.Common.Platform;

	public class HelpPlugin : Plugin
	{
		private static HelpPlugin m_Me;

		public HelpPlugin()
		{
			m_Me = this;
			Initialize(this);
		}

		// Properties
		public static HelpPlugin Me { get { return m_Me; } }
		public override string Name	{ get {	return this.GetType().Namespace; } }
		public override Plugin.PluginType Type { get { return Plugin.PluginType.Other; } }

		// Public methods
		public override void Start()
		{
			base.Start();
		}

		public override void Stop()
		{
			base.Stop();
		}
	}

*/