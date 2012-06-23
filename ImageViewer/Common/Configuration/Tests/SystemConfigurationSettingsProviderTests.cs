#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS

using System.Configuration;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Common.Configuration.Tests
{
    [TestFixture]
    public class SystemConfigurationSettingsProviderTests
    {
        [TestFixtureSetUp]
        public void Initialize1()
        {
            TestSettingsStore.Instance.Reset();

            Platform.SetExtensionFactory(new UnitTestExtensionFactory
                                             {
                                                 { typeof(ServiceProviderExtensionPoint), typeof(TestSystemConfigurationServiceProvider) }
                                             });   
        }


        [Test]
        public void UserScopedSettingsTest()
        {
            try
            {
                MixedScopeSettings.Default.User1 = "a";
                MixedScopeSettings.Default.Save();
                Assert.Fail("Expected exception from mixed settings");
            }
            catch (ConfigurationErrorsException)
            {                
                Assert.Pass();
            }

            try
            {
                var val = MixedScopeSettings.Default.User1;
                Assert.Fail("Expected exception from mixed settings");
            }
            catch (ConfigurationErrorsException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void MultipleAppSettings()
        {
            TestSettingsStore.Instance.Reset();

            var settings = new AppSettings();
            var settings2 = new AppSettings();

            Assert.AreEqual(settings.App,AppSettings.DefaultValueApp);

            const string testVal = "Test Value, this really is a test value, I mean";
            const string testVal2 = "A man, a plan, a canal, panama";

            settings.App = testVal;
            Assert.AreEqual(settings.App, testVal);
            Assert.AreEqual(settings2.App, AppSettings.DefaultValueApp);
            var settings3 = new AppSettings();
            Assert.AreEqual(settings3.App, AppSettings.DefaultValueApp);

            settings.Save();

            Assert.AreEqual(settings.App, testVal);
            Assert.AreEqual(settings2.App, AppSettings.DefaultValueApp);
            settings3 = new AppSettings();
            Assert.AreEqual(settings3.App, testVal);

            settings2.App = testVal2;

            Assert.AreEqual(settings.App, testVal);
            Assert.AreEqual(settings2.App, testVal2);
            settings3 = new AppSettings();
            Assert.AreEqual(settings3.App, testVal);

            settings2.Save();

            Assert.AreEqual(settings.App, testVal);
            Assert.AreEqual(settings2.App, testVal2);
            settings3 = new AppSettings();
            Assert.AreEqual(settings3.App, testVal2);   
        }

        [Test]
        public void MigrateSettings()
        {
            TestSettingsStore.Instance.Reset();

            var settings = new MigrationAppSettings();
            
            const string testVal1 = "A man, a plan, a canal, panama";
            const string testVal2 = "Now is the time for all good men to come to the aide of their country";

            settings.App1 = testVal1;
            settings.App2 = testVal2;
            settings.Save();

            Assert.IsTrue(SettingsMigrator.MigrateSharedSettings(typeof (MigrationAppSettings), null));

            var settings1a = new MigrationAppSettings();
            Assert.AreEqual(settings1a.App1, testVal1);
            Assert.AreEqual(settings1a.App2, testVal2);
            Assert.AreEqual(settings1a.App3, MigrationAppSettings.DefaultValueApp);

            var group = new SettingsGroupDescriptor(typeof (MigrationAppSettings));
            TestSettingsStore.Instance.GetPreviousSettingsGroup(group);
            TestSettingsStore.Instance.RemoveSettingsGroup(group);

            var settings2 = new MigrationAppSettings();
            Assert.AreEqual(settings2.App1, MigrationAppSettings.DefaultValueApp);
            Assert.AreEqual(settings2.App2, MigrationAppSettings.DefaultValueApp);
            Assert.AreEqual(settings2.App3, MigrationAppSettings.DefaultValueApp);

            Assert.IsTrue(SettingsMigrator.MigrateSharedSettings(typeof (MigrationAppSettings), null));

            var settings3 = new MigrationAppSettings();
            Assert.AreEqual(settings3.App1, testVal1 + TestSettingsStore.TestString);
            Assert.AreEqual(settings3.App2, testVal2 + TestSettingsStore.TestString);
            Assert.AreEqual(settings3.App3, MigrationAppSettings.DefaultValueApp);

        }

        [Test]
        public void MigrateSettingsNoChanges()
        {
            TestSettingsStore.Instance.Reset();

            var settings = new MigrationAppSettings();


            SettingsMigrator.MigrateSharedSettings(typeof(MigrationAppSettings), null);

            var settings3 = new MigrationAppSettings();
            Assert.AreEqual(settings3.App1, MigrationAppSettings.DefaultValueApp);
            Assert.AreEqual(settings3.App2, MigrationAppSettings.DefaultValueApp);
            Assert.AreEqual(settings3.App3, MigrationAppSettings.DefaultValueApp);

        }

    }
}

#endif
