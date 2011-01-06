#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS
#pragma warning disable 1591

using System;
using NUnit.Framework;
using System.Configuration;
using System.Reflection;
using System.IO;

namespace ClearCanvas.Common.Configuration.Tests
{
	[TestFixture]
	public class ApplicationSettingsExtensionsTests : SettingsTestBase
	{
		private static readonly Type _settingsClass = typeof(LocalMixedScopeSettings);

		private static System.Configuration.Configuration GetExeConfiguration()
		{
			return SystemConfigurationHelper.GetExeConfiguration();
		}

		private static void RemoveSettings()
		{
			SystemConfigurationHelper.RemoveSettingsValues(GetExeConfiguration(), _settingsClass);
		}

		[TestFixtureSetUp]
		public void Setup()
		{
			ResetAllSettingsClasses();
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			RemoveSettings();
		}

		[Test]
		public void TestGetSharedSettings_NoneExist()
		{
			RemoveSettings();

			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(_settingsClass);
			settings.Reload();

			foreach (SettingsProperty property in settings.Properties)
			{
				var shared = ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
				Assert.AreEqual(property.DefaultValue, shared);

				if (SettingsPropertyExtensions.IsAppScoped(property))
					Assert.AreEqual(property.DefaultValue, settings[property.Name]);
			}
		}

		[Test]
		public void TestGetSharedSettings_Exists()
		{
			RemoveSettings();

			SystemConfigurationHelperTests.WriteSharedValuesToConfig(_settingsClass, SettingValue.Current);
			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(_settingsClass);
			settings.Reload();

			foreach (SettingsProperty property in settings.Properties)
			{
				var shared = ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
				string expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
				Assert.AreEqual(expected, shared);

				if (SettingsPropertyExtensions.IsAppScoped(property))
					Assert.AreEqual(expected, settings[property.Name]);
			}
		}

		[Test]
		public void TestGetPreviousSharedValues_NoneExist()
		{
			ResetAllSettingsClasses();

			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(_settingsClass);
			settings.Reload();
			foreach (SettingsProperty property in settings.Properties)
			{
				var previous = ApplicationSettingsExtensions.GetPreviousSharedPropertyValue(settings, property.Name, null);
				Assert.IsNull(previous);
			}
		}

		[Test]
		public void TestGetPreviousSharedValues_Exists()
		{
			ResetAllSettingsClasses();

			string path = Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath);
			string fileName = String.Format("{0}{1}TestPrevious.exe.config", path, System.IO.Path.DirectorySeparatorChar);
			TestConfigResourceToFile(fileName);

			try
			{
				var settings = ApplicationSettingsHelper.GetSettingsClassInstance(_settingsClass);
				settings.Reload();
				foreach (SettingsProperty property in settings.Properties)
				{
					var actual = ApplicationSettingsExtensions.GetPreviousSharedPropertyValue(settings, property.Name, fileName);
					var expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Previous);
					Assert.AreEqual(expected, actual);
				}
			}
			finally
			{
				File.Delete(fileName);
			}
		}
	}
}

#endif