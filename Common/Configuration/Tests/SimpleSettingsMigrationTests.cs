#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS
#pragma warning disable 1591

using NUnit.Framework;
using System;
using System.Configuration;

namespace ClearCanvas.Common.Configuration.Tests
{
	[TestFixture]
	public class SimpleSettingsMigrationTests : SettingsTestBase
	{
		[Test]
		public void TestSimpleUserSettingsMigration()
		{
			ResetUpgradeSettings();

			Type settingsClass = typeof (SimpleUserSettings);
			PopulateSimpleStore(settingsClass);
			Assert.IsTrue(SettingsMigrator.MigrateUserSettings(settingsClass));

			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);

			SettingsProperty property = settings.Properties[SimpleUserSettings.PropertyUser];
			string expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Previous);
			string actual = (string)settings[property.Name];
			Assert.AreEqual(expected, actual);

			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);

			expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Previous);
			Assert.IsFalse(SettingsMigrator.MigrateUserSettings(settingsClass));
			actual = (string)settings[property.Name]; 
			Assert.AreEqual(expected, actual);

			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			actual = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestNonMigratableSharedSettings()
		{
			ResetUpgradeSettings();

			Type settingsClass = typeof(NonMigratableSharedSettings);
			PopulateSimpleStore(settingsClass);

			Assert.IsFalse(SettingsMigrator.MigrateSharedSettings(settingsClass, null));
			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);

			SettingsProperty property = settings.Properties[NonMigratableSharedSettings.PropertyApp];
			string expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Current);
			Assert.AreEqual(expected, settings[property.Name]);

			string current = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, current);
		}

		[Test]
		public void TestUserDefaultSettingsMigrated()
		{
			ResetUpgradeSettings(); 
			
			Type settingsClass = typeof(SimpleUserSettings);
			PopulateSimpleStore(settingsClass);
			
			Assert.IsTrue(SettingsMigrator.MigrateSharedSettings(settingsClass, null));

			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);

			SettingsProperty property = settings.Properties[SimpleUserSettings.PropertyUser];
			string expected = CreateSettingValue(property, MigrationScope.User, SettingValue.Current);
			Assert.AreEqual(expected, settings[property.Name]);

			expected = CreateSettingValue(property, MigrationScope.Shared, SettingValue.Previous);
			string current = (string)ApplicationSettingsExtensions.GetSharedPropertyValue(settings, property.Name);
			Assert.AreEqual(expected, current);
		}

		[Test]
		public void TestNeverMigrateUpgradeSettings()
		{
			ResetUpgradeSettings();
			ResetAllSettingsClasses();
			Type settingsClass = typeof(UpgradeSettings);

			Assert.IsFalse(SettingsMigrator.MigrateUserSettings(settingsClass));
			Assert.IsFalse(SettingsMigrator.MigrateSharedSettings(settingsClass, null));
		}

		[Test]
		public void TestNeverMigrateProductSettings()
		{
			ResetUpgradeSettings();
			ResetAllSettingsClasses();
			Type settingsClass = typeof(ProductSettings);

			Assert.IsFalse(SettingsMigrator.MigrateUserSettings(settingsClass));
			Assert.IsFalse(SettingsMigrator.MigrateSharedSettings(settingsClass, null));
		}
	}
}

#endif