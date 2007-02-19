using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using ClearCanvas.Common;
using System.ComponentModel;
using System.Reflection;

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
	public class ApplicationSettingsRegister
	{
		private static ApplicationSettingsRegister _instance;

		private static object _syncLock = new object();
		private List<ApplicationSettingsBase> _registeredSettingsInstances;

		private ApplicationSettingsRegister()
		{
			_registeredSettingsInstances = new List<ApplicationSettingsBase>();
		}
		
		/// <summary>
		/// The single instance of this class that is publicly accessible.
		/// </summary>
		public static ApplicationSettingsRegister Instance
		{
			get
			{
				lock (_syncLock)
				{
					if (_instance == null)
						_instance = new ApplicationSettingsRegister();
				}

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
		/// Synchronizes existing settings instances (in memory) of a given Type with the default profile.
		/// </summary>
		/// <param name="settingsType">the type of the settings class</param>
		/// <param name="changedValues">the previous values for the properties that were changed in the default profile</param>
		/// <param name="newValues">the new values that have already been stored in the default profile</param>
		public void SynchronizeExistingSettings(Type settingsType, IDictionary<string, string> changedValues, IDictionary<string, string> newValues)
		{
			lock (_syncLock)
			{
				List<ApplicationSettingsBase> settingsInstances = _registeredSettingsInstances.FindAll
					(
						delegate(ApplicationSettingsBase settingsInstance){ return settingsInstance.GetType() == settingsType; }
					);

				foreach (ApplicationSettingsBase settingsInstance in settingsInstances)
					SynchronizeExistingSetting(settingsInstance, changedValues, newValues);
			}
		}

		/// <summary>
		/// Synchronizes an instance of a settings class with the default profile.
		/// </summary>
		/// <param name="settingsInstance">the instance to be synchronized</param>
		/// <param name="changedValues">the previous values for the properties that were changed in the default profile</param>
		/// <param name="newValues">the new values that have already been stored in the default profile</param>
		private void SynchronizeExistingSetting(ApplicationSettingsBase settingsInstance, IDictionary<string, string> changedValues, IDictionary<string, string> newValues)
		{
			bool modified = false;
			
			foreach (string key in changedValues.Keys)
			{
				SettingsPropertyValue value = settingsInstance.PropertyValues[key];
				if (value == null)
					continue; //it will already be loaded from defaults on first access.
				
				PropertyInfo property = settingsInstance.GetType().GetProperty(key);
				if (property == null)
					continue;

				SettingsProperty settingsProperty = settingsInstance.Properties[key];

				if (SettingsClassMetaDataReader.IsAppScoped(property))
				{
					//this will cause the value to be reloaded from the default profile the next time it is accessed.
					settingsInstance.PropertyValues.Remove(key);
					modified = true;
					continue;
				}

				//for user-scoped properties, we need to do a bit more work to set existing values.
				if ((string)value.SerializedValue == changedValues[key])
				{
					modified = true;
					SettingsPropertyValue temp = new SettingsPropertyValue(settingsInstance.Properties[key]);
					temp.Deserialized = false;

					//if newValues doesn't contain the key, then the value should just be the default.
					if (newValues.ContainsKey(key))
						temp.SerializedValue = newValues[key];
					else
						temp.SerializedValue = SettingsClassMetaDataReader.TranslateDefaultValue(settingsInstance.GetType(), (string)settingsProperty.DefaultValue);
					
					//this forces the PropertyChanged event to fire.
					settingsInstance[key] = temp.PropertyValue;
				}
			}

			if (modified)
				settingsInstance.Save();
		}
	}
}
