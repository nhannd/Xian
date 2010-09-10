#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace ClearCanvas.Common.Configuration.Tests
{
	[TestFixture]
	public class ConfigurationFileReaderTests : SettingsTestBase
	{
		[Test]
		public void TestReadNoSettings()
		{
			Type settingsClass = typeof(LocalMixedScopeSettings);
			SystemConfigurationHelper.RemoveSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass);

			var reader = new ConfigurationFileReader(SystemConfigurationHelper.GetExeConfiguration().FilePath);
			var path = new ConfigurationSectionPath(typeof(LocalMixedScopeSettings), SettingScope.Application);
			var values = reader.GetSettingsValues(path);
			Assert.AreEqual(0, values.Count);

			path = new ConfigurationSectionPath(typeof(LocalMixedScopeSettings), SettingScope.User);
			values = reader.GetSettingsValues(path);
			Assert.AreEqual(0, values.Count);
		}

		[Test]
		public void TestReadStringSettings()
		{
			Type settingsClass = typeof (LocalMixedScopeSettings);
			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);

			ApplicationSettingsExtensions.SetSharedVersion(settings, LocalMixedScopeSettings.PropertyApp1, "TestApp1");
			ApplicationSettingsExtensions.SetSharedVersion(settings, LocalMixedScopeSettings.PropertyApp2, "TestApp2");
			ApplicationSettingsExtensions.SetSharedVersion(settings, LocalMixedScopeSettings.PropertyUser1, "TestUser1");
			ApplicationSettingsExtensions.SetSharedVersion(settings, LocalMixedScopeSettings.PropertyUser2, "TestUser2");

			var reader = new ConfigurationFileReader(SystemConfigurationHelper.GetExeConfiguration().FilePath);
			var path = new ConfigurationSectionPath(typeof(LocalMixedScopeSettings), SettingScope.Application);
			var values = reader.GetSettingsValues(path);
			Assert.AreEqual(2, values.Count);
			Assert.AreEqual("TestApp1", values[LocalMixedScopeSettings.PropertyApp1]);
			Assert.AreEqual("TestApp2", values[LocalMixedScopeSettings.PropertyApp2]);

			path = new ConfigurationSectionPath(typeof(LocalMixedScopeSettings), SettingScope.User);
			values = reader.GetSettingsValues(path);
			Assert.AreEqual(2, values.Count);
			Assert.AreEqual("TestUser1", values[LocalMixedScopeSettings.PropertyUser1]);
			Assert.AreEqual("TestUser2", values[LocalMixedScopeSettings.PropertyUser2]);
			
			SystemConfigurationHelper.RemoveSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass);
		}

		[Test]
		public void TestReadXmlSettings()
		{
			Type settingsClass = typeof(LocalXmlSettings);
			var settings = ApplicationSettingsHelper.GetSettingsClassInstance(settingsClass);

			var appValue = @"<test><app/></test>";
			XmlDocument appDocument = new XmlDocument();
			appDocument.LoadXml(appValue);
			ApplicationSettingsExtensions.SetSharedVersion(settings, LocalXmlSettings.PropertyApp, appDocument);

			var userValue = @"<test><user/></test>";
			XmlDocument userDocument= new XmlDocument();
			userDocument.LoadXml(userValue);
			ApplicationSettingsExtensions.SetSharedVersion(settings, LocalXmlSettings.PropertyUser, userDocument);

			var reader = new ConfigurationFileReader(SystemConfigurationHelper.GetExeConfiguration().FilePath);
			var path = new ConfigurationSectionPath(typeof(LocalXmlSettings), SettingScope.Application);
			var values = reader.GetSettingsValues(path);
			Assert.AreEqual(1, values.Count);

			XmlDocument testDocument = new XmlDocument();
			testDocument.LoadXml(values[LocalXmlSettings.PropertyApp]);
			Assert.AreEqual(appDocument.InnerXml, testDocument.InnerXml);

			path = new ConfigurationSectionPath(typeof(LocalXmlSettings), SettingScope.User);
			values = reader.GetSettingsValues(path);
			Assert.AreEqual(1, values.Count);

			testDocument = new XmlDocument();
			testDocument.LoadXml(values[LocalXmlSettings.PropertyUser]);
			Assert.AreEqual(userDocument.InnerXml, testDocument.InnerXml);

			SystemConfigurationHelper.RemoveSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass);
		}
	}
}

#endif