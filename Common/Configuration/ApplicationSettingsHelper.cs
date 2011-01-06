#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using System.Reflection;

namespace ClearCanvas.Common.Configuration
{
	internal static class ApplicationSettingsHelper
	{
		public static bool IsUserSettingsMigrationEnabled(Type settingsClass)
		{
			CheckType(settingsClass);
			return !settingsClass.IsDefined(typeof(UserSettingsMigrationDisabledAttribute), false);
		}

		public static bool IsSharedSettingsMigrationEnabled(Type settingsClass)
		{
			CheckType(settingsClass);
			return !settingsClass.IsDefined(typeof(SharedSettingsMigrationDisabledAttribute), false);
		}

		public static void CheckType(Type settingsClass)
		{
			if (!settingsClass.IsSubclassOf(typeof(ApplicationSettingsBase)))
			{
				throw new ArgumentException(
					String.Format("Type does not derive from ApplicationSettingsBase: {0}", settingsClass.FullName));
			}
		}

		public static Type GetSettingsClass(SettingsGroupDescriptor group)
		{
			Type settingsClass = Type.GetType(group.AssemblyQualifiedTypeName, true);
			CheckType(settingsClass);
		    return settingsClass;
		}

		public static ApplicationSettingsBase GetSettingsClassInstance(Type settingsClass)
		{
			CheckType(settingsClass);

			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
			PropertyInfo defaultProperty = settingsClass.GetProperty("Default", bindingFlags);

			if (defaultProperty != null)
				return (ApplicationSettingsBase)defaultProperty.GetValue(null, null);

			//try to return an instance of the class
			return (ApplicationSettingsBase)Activator.CreateInstance(settingsClass);
		}
	}
}