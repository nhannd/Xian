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

using System;
using NUnit.Framework;
using System.Collections.Generic;
using SystemConfiguration = System.Configuration.Configuration;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace ClearCanvas.Common.Configuration.Tests
{
	[TestFixture]
	public class SystemConfigurationHelperTests : SettingsTestBase
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

		[TestFixtureTearDown]
		public void TearDown()
		{
			RemoveSettings();
		}

		[Test]
		public void TestReadSettingsValues_NoneExist()
		{
			RemoveSettings();
			Assert.AreEqual(0, SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), _settingsClass).Count);
		}

		[Test]
		public void TestWriteSettingsDefaultValues_NoOp()
		{
			RemoveSettings();

			WriteSharedValuesToConfig(_settingsClass, SettingValue.Default);
			ValidateValuesInConfig(MigrationScope.Shared, SettingValue.Default);
		}

		[Test]
		public void TestReadWriteDefaultValueXml_Noop()
		{
			Type settingsClass = typeof(LocalXmlSettings);
			SystemConfigurationHelper.RemoveSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass);

			try
			{
				var values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), settingsClass);
				Assert.AreEqual(0, values.Count);

				values = new Dictionary<string, string>();
				values[LocalXmlSettings.PropertyUser] = LocalXmlSettings.DefaultValueUser;
				values[LocalXmlSettings.PropertyApp] = LocalXmlSettings.DefaultValueApp;

				SystemConfigurationHelper.PutSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass, values);
				values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), settingsClass);
				//Assert.AreEqual(0, values.Count); 

				//NOTE: not truly a no-op because we purposely store the default value, and it's not always exactly the same on reading it back in.
	
				LocalXmlSettings settings = (LocalXmlSettings)ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
				Assert.IsNull(ApplicationSettingsExtensions.GetSharedVersion(settings, LocalXmlSettings.PropertyUser));

				XmlDocument defaultDoc = new XmlDocument();
				defaultDoc.LoadXml(LocalXmlSettings.DefaultValueApp);
				Assert.AreEqual(defaultDoc.DocumentElement.OuterXml, settings.App.DocumentElement.OuterXml);
			}
			finally
			{
				SystemConfigurationHelper.RemoveSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass);
			}
		}

		[Test]
		public void TestReadWriteSettingsValues()
		{
			RemoveSettings();

			WriteSharedValuesToConfig(_settingsClass, SettingValue.Current);
			ValidateValuesInConfig(MigrationScope.Shared, SettingValue.Current);

			WriteSharedValuesToConfig(_settingsClass, SettingValue.Previous);
			ValidateValuesInConfig(MigrationScope.Shared, SettingValue.Previous);
		}

		[Test]
		public void TestReadWriteSettingsValues_SomeDefault()
		{
			RemoveSettings();
			var configuration = SystemConfigurationHelper.GetExeConfiguration();

			var values = CreateSettingsValues(_settingsClass, MigrationScope.Shared, SettingValue.Current);
			values[LocalMixedScopeSettings.PropertyApp1] = LocalMixedScopeSettings.PropertyApp1;
			values[LocalMixedScopeSettings.PropertyApp2] = LocalMixedScopeSettings.PropertyApp2;

			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);
			values.Remove(LocalMixedScopeSettings.PropertyApp1);
			values.Remove(LocalMixedScopeSettings.PropertyApp2);

			var readValues = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), _settingsClass);
			ValidateValues(values, readValues);
		}

		[Test]
		public void TestResetIndividualValues()
		{
			RemoveSettings();
			var configuration = SystemConfigurationHelper.GetExeConfiguration();

			var values = new Dictionary<string, string>();
			values[LocalMixedScopeSettings.PropertyApp1] = "Test1";
			values[LocalMixedScopeSettings.PropertyApp2] = "Test2";

			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);
			values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), _settingsClass);
			Assert.AreEqual(2, values.Count);
			Assert.AreEqual("Test1", values[LocalMixedScopeSettings.PropertyApp1]);
			Assert.AreEqual("Test2", values[LocalMixedScopeSettings.PropertyApp2]);

			values = new Dictionary<string, string>();
			values[LocalMixedScopeSettings.PropertyApp1] = null;
			values[LocalMixedScopeSettings.PropertyApp2] = null;

			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);
			values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), _settingsClass);
			Assert.AreEqual(0, values.Count);

			var settings = (LocalMixedScopeSettings)ApplicationSettingsHelper.GetSettingsClassInstance(_settingsClass);
			Assert.AreEqual(LocalMixedScopeSettings.PropertyApp1, settings.App1);
			Assert.AreEqual(LocalMixedScopeSettings.PropertyApp2, settings.App2);
		}

		[Test]
		public void TestWriteEmptyString()
		{
			RemoveSettings();
			var configuration = SystemConfigurationHelper.GetExeConfiguration();

			var values = new Dictionary<string, string>();
			values[LocalMixedScopeSettings.PropertyApp1] = "Test1";
			values[LocalMixedScopeSettings.PropertyApp2] = "Test2";

			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);
			values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), _settingsClass);
			Assert.AreEqual(2, values.Count);
			Assert.AreEqual("Test1", values[LocalMixedScopeSettings.PropertyApp1]);
			Assert.AreEqual("Test2", values[LocalMixedScopeSettings.PropertyApp2]);

			values = new Dictionary<string, string>();
			values[LocalMixedScopeSettings.PropertyApp1] = "";
			values[LocalMixedScopeSettings.PropertyApp2] = "";

			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);
			values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), _settingsClass);
			Assert.AreEqual(2, values.Count);
			Assert.AreEqual("", values[LocalMixedScopeSettings.PropertyApp1]);
			Assert.AreEqual("", values[LocalMixedScopeSettings.PropertyApp2]);

			var settings = (LocalMixedScopeSettings)ApplicationSettingsHelper.GetSettingsClassInstance(_settingsClass);
			Assert.AreEqual("", settings.App1);
			Assert.AreEqual("", settings.App2);
		}

		[Test]
		public void TestWriteDefaultValue_EntriesRemoved()
		{
			RemoveSettings();
			var configuration = SystemConfigurationHelper.GetExeConfiguration();

			var values = new Dictionary<string, string>();
			values[LocalMixedScopeSettings.PropertyApp1] = "Test1";
			values[LocalMixedScopeSettings.PropertyApp2] = "Test2";

			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);
			values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), _settingsClass);
			Assert.AreEqual(2, values.Count);
			Assert.AreEqual("Test1", values[LocalMixedScopeSettings.PropertyApp1]);
			Assert.AreEqual("Test2", values[LocalMixedScopeSettings.PropertyApp2]);

			values = new Dictionary<string, string>();
			values[LocalMixedScopeSettings.PropertyApp1] = LocalMixedScopeSettings.PropertyApp1;
			values[LocalMixedScopeSettings.PropertyApp2] = LocalMixedScopeSettings.PropertyApp2;

			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);
			values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), _settingsClass);
			Assert.AreEqual(0, values.Count);

			var settings = (LocalMixedScopeSettings)ApplicationSettingsHelper.GetSettingsClassInstance(_settingsClass);
			Assert.AreEqual(LocalMixedScopeSettings.PropertyApp1, settings.App1);
			Assert.AreEqual(LocalMixedScopeSettings.PropertyApp2, settings.App2);
		}

		[Test]
		public void TestStoreXmlSettingValueNull()
		{
			Type settingsClass = typeof (LocalXmlSettings);
			SystemConfigurationHelper.RemoveSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass);

			try
			{
				var values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), settingsClass);
				Assert.AreEqual(0, values.Count);

				values = new Dictionary<string, string>();
				values[LocalXmlSettings.PropertyApp] = "";

				SystemConfigurationHelper.PutSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass, values);
				values = SystemConfigurationHelper.GetSettingsValues(GetExeConfiguration(), settingsClass);
				Assert.AreEqual(1, values.Count);
				Assert.IsTrue(String.IsNullOrEmpty(values[LocalXmlSettings.PropertyApp]));

				LocalXmlSettings settings = (LocalXmlSettings)ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);
				XmlDocument defaultDoc = new XmlDocument();
				defaultDoc.LoadXml(LocalXmlSettings.DefaultValueApp);
				Assert.AreEqual(defaultDoc.DocumentElement.OuterXml, settings.App.DocumentElement.OuterXml);
			}
			finally
			{
				SystemConfigurationHelper.RemoveSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass);
			}
		}

		[Test]
		public void TestRepeatedWritesDifferentProperties()
		{
			RemoveSettings();
			var configuration = SystemConfigurationHelper.GetExeConfiguration();
			var expectedValue1 = CreateSettingValue(LocalMixedScopeSettings.PropertyApp1, SettingValue.Current);
			var expectedValue2 = CreateSettingValue(LocalMixedScopeSettings.PropertyApp2, SettingValue.Current);

			var values = new Dictionary<string, string>();
			values[LocalMixedScopeSettings.PropertyApp1] = expectedValue1;
			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);

			values = SystemConfigurationHelper.GetSettingsValues(configuration, _settingsClass);
			Assert.AreEqual(1, values.Count);
			Assert.AreEqual(expectedValue1, values[LocalMixedScopeSettings.PropertyApp1]);

			values.Clear();
			values[LocalMixedScopeSettings.PropertyApp2] = expectedValue2;
			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);

			values = SystemConfigurationHelper.GetSettingsValues(configuration, _settingsClass);
			Assert.AreEqual(2, values.Count);
			Assert.AreEqual(expectedValue1, values[LocalMixedScopeSettings.PropertyApp1]);
			Assert.AreEqual(expectedValue2, values[LocalMixedScopeSettings.PropertyApp2]);
		}

		internal static void WriteSharedValuesToConfig(Type type, SettingValue settingValue)
		{
			var configuration = SystemConfigurationHelper.GetExeConfiguration();
			var values = CreateSettingsValues(type, MigrationScope.Shared, settingValue);
			SystemConfigurationHelper.PutSettingsValues(configuration, _settingsClass, values);
		}

		private void ValidateValuesInConfig(MigrationScope migrationScope, SettingValue settingValue)
		{
			var configuration = GetExeConfiguration();
			var values = SystemConfigurationHelper.GetSettingsValues(configuration, _settingsClass);
			ValidateValues(values, migrationScope, settingValue);
		}

		private void ValidateValues(Dictionary<string, string> values, MigrationScope migrationScope, SettingValue settingValue)
		{
			if (settingValue == SettingValue.Default)
			{
				Assert.AreEqual(0, values.Count);
				return;
			}

			var expected = CreateSettingsValues(_settingsClass, migrationScope, settingValue);
			ValidateValues(values, expected);
		}

		private void ValidateValues(Dictionary<string, string> values, Dictionary<string, string> expected)
		{
			Assert.AreEqual(expected.Count, values.Count);

			foreach (var value in expected)
				Assert.AreEqual(value.Value, values[value.Key]);
		}
	}
}

#endif