using System;
using System.Collections.Generic;
using System.Configuration;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// This class provides a way to update existing instances of settings objects derived from
	/// <see cref="ApplicationSettingsBase"/>.  The individual instances must register themselves
	/// with this class in order to receive updates.  When a setting value is changed in the default
	/// profile, the individual settings in memory are inspected to see if their values corresponded
	/// to the values that were just changed.  If they do, then those values are changed to correspond
	/// to the new values.  This class implements the Singleton design pattern.
	/// </summary>
	public class ApplicationSettingsRegistry
	{
		private static ApplicationSettingsRegistry _instance = new ApplicationSettingsRegistry();

		private static object _syncLock = new object();
		private List<ApplicationSettingsBase> _registeredSettingsInstances;

		private ApplicationSettingsRegistry()
		{
			_registeredSettingsInstances = new List<ApplicationSettingsBase>();
		}
		
		/// <summary>
		/// The single instance of this class that is publicly accessible.
		/// </summary>
		public static ApplicationSettingsRegistry Instance
		{
			get
			{
				return _instance;
			}
		}

		/// <summary>
		/// Registers an instance of a settings class.
		/// </summary>
		/// <param name="settingsInstance">the instance</param>
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
		/// <param name="settingsInstance">the instance</param>
		public void UnregisterInstance(ApplicationSettingsBase settingsInstance)
		{
			lock (_syncLock)
			{
				if (_registeredSettingsInstances.Contains(settingsInstance))
					_registeredSettingsInstances.Remove(settingsInstance);
			}
		}

        /// <summary>
        /// Synchronizes all registered settings instances that match the specified group.
        /// </summary>
        /// <param name="group"></param>
        public void Synchronize(SettingsGroupDescriptor group)
        {
            Type settingsClass = Type.GetType(group.AssemblyQualifiedTypeName, false);
            if (settingsClass != null)
            {
                _registeredSettingsInstances
                    .FindAll(delegate(ApplicationSettingsBase instance) { return instance.GetType().Equals(settingsClass); })
                    .ForEach(delegate(ApplicationSettingsBase instance) { instance.Reload(); });
            }
        }
    }
}
