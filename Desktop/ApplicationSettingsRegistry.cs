#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A Singleton class that provides a way for <see cref="ApplicationSettingsBase"/>-derived objects
	/// to be updated when a setting value was modified externally.
	/// </summary>
	/// <remarks>
    /// This class provides a way to update existing instances of settings objects derived from
    /// <see cref="ApplicationSettingsBase"/>.  The individual instances must register themselves
    /// with this class in order to receive updates.
    /// </remarks>
	public class ApplicationSettingsRegistry
	{
		private static readonly ApplicationSettingsRegistry _instance = new ApplicationSettingsRegistry();

		private readonly object _syncLock = new object();
		private readonly List<ApplicationSettingsBase> _registeredSettingsInstances;

		private ApplicationSettingsRegistry()
		{
			_registeredSettingsInstances = new List<ApplicationSettingsBase>();
		}
		
		/// <summary>
		/// Gets the single instance of this class.
		/// </summary>
		public static ApplicationSettingsRegistry Instance
		{
			get { return _instance; }
		}

		/// <summary>
		/// Registers an instance of a settings class.
		/// </summary>
		public void RegisterInstance(ApplicationSettingsBase settingsInstance)
		{ 
			lock(_syncLock)
			{
				if (!_registeredSettingsInstances.Contains(settingsInstance))
					_registeredSettingsInstances.Add(settingsInstance);
			}
		}

		/// <summary>
		/// Unregisters an instance of a settings class.
		/// </summary>
		public void UnregisterInstance(ApplicationSettingsBase settingsInstance)
		{
			lock (_syncLock)
			{
				if (_registeredSettingsInstances.Contains(settingsInstance))
					_registeredSettingsInstances.Remove(settingsInstance);
			}
		}

		/// <summary>
		/// Calls <see cref="ApplicationSettingsBase.Reload"/> on all registered 
		/// settings instances that match the specified group.
		/// </summary>
		public void Reload(SettingsGroupDescriptor group)
		{
			Type settingsClass = Type.GetType(group.AssemblyQualifiedTypeName, false);
			if (settingsClass != null)
			{
				lock (_syncLock)
				{
					_registeredSettingsInstances
						.FindAll(delegate(ApplicationSettingsBase instance) { return instance.GetType().Equals(settingsClass); })
						.ForEach(delegate(ApplicationSettingsBase instance) { instance.Reload(); });
				}
			}
		}
	}
}