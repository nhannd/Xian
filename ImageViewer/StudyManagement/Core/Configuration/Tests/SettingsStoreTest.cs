#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Configuration;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Configuration.Tests
{
    [TestFixture]
    internal class SettingsStoreTest
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetConfigurationDocumentBroker();
                broker.DeleteAllDocuments();
            }
        }

        [Test]
        public void SettingsStorePutGet()
        {
            const string testVal = "A man, a plan, a canal, panama";

            var store = new SystemConfigurationSettingsStore();

            var settings = new AppSettings();

            var group = new SettingsGroupDescriptor(typeof (AppSettings));

            settings.App = testVal;
            var dic = new Dictionary<string, string>();
            dic.Add(AppSettings.PropertyApp, settings.App);

            store.PutSettingsValues(group, null, null, dic);

            var resultDic = store.GetSettingsValues(group, null, null);

            string val;

            Assert.IsTrue(resultDic.TryGetValue(AppSettings.PropertyApp, out val));

            Assert.AreEqual(val, testVal);
        }

        [Test]
        public void SettingsPrior()
        {
            const string testVal = "A man, a plan, a canal, panama 2";
            const string testValOld = "A man, a plan, a canal, panama 3";

            var store = new SystemConfigurationSettingsStore();

            var settings = new AppSettings();

            var group = new SettingsGroupDescriptor(typeof(AppSettings));

            settings.App = testVal;
            var dic = new Dictionary<string, string>();
            dic.Add(AppSettings.PropertyApp, settings.App);

            store.PutSettingsValues(group, null, null, dic);

            var oldGroup = new SettingsGroupDescriptor(group.Name, new Version(group.Version.Major -1,group.Version.Minor, group.Version.Build,group.Version.Revision), group.Description,
                                                       group.AssemblyQualifiedTypeName, group.HasUserScopedSettings);
            dic[AppSettings.PropertyApp] = testValOld;

            store.PutSettingsValues(oldGroup, null, null, dic);

            var resultDic = store.GetSettingsValues(group, null, null);

            string val;

            Assert.IsTrue(resultDic.TryGetValue(AppSettings.PropertyApp, out val));

            Assert.AreEqual(val, testVal);


            resultDic = store.GetSettingsValues(oldGroup, null, null);

            Assert.IsTrue(resultDic.TryGetValue(AppSettings.PropertyApp, out val));

            Assert.AreEqual(val, testValOld);

            var returnedOldGroup = store.GetPreviousSettingsGroup(group);
            Assert.IsNotNull(returnedOldGroup);
            Assert.AreEqual(returnedOldGroup.Version.Major, oldGroup.Version.Major);
            Assert.AreEqual(returnedOldGroup.Version.Minor, oldGroup.Version.Minor);
            Assert.AreEqual(returnedOldGroup.Description, oldGroup.Description);
            Assert.AreEqual(returnedOldGroup.Name, oldGroup.Name);
            Assert.AreEqual(returnedOldGroup.AssemblyQualifiedTypeName, oldGroup.AssemblyQualifiedTypeName);


            returnedOldGroup = store.GetPreviousSettingsGroup(oldGroup);
            Assert.IsNull(returnedOldGroup);

        }
    }
}

#endif