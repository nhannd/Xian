#if UNIT_TESTS

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.ServiceModel;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree.Tests
{
    //TODO (Marmot): Expand on these tests a bit. Write for legacy xml.
    [TestFixture]
    public class ServerTreeTests
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            var extensionFactory = new NullExtensionFactory();
            Platform.SetExtensionFactory(extensionFactory);
        }

        [Test]
        public void TestCreateExampleTree()
        {
            var tree = new ServerTree(null);
            TestHelper.AssertExampleTree(tree);

            tree = new ServerTree(null, null, false);
            TestHelper.AssertExampleTree(tree);
        }

        [Test]
        public void TestGetAllServers()
        {
            var tree = TestHelper.CreateTestTree1();
            var allServers = tree.RootServerGroup.GetAllServers();
            Assert.AreEqual(4, allServers.Count);
        }

        [Test]
        public void TestCreateStoredTreeFromDirectory()
        {
            List<ApplicationEntity> directoryServers;
            var tree = TestHelper.CreateTestTree1(out directoryServers);

            Assert.AreEqual(2, tree.RootServerGroup.ChildGroups.Count);
            Assert.AreEqual(2, tree.RootServerGroup.Servers.Count);
            Assert.AreEqual(2, tree.RootServerGroup.ChildGroups[1].Servers.Count);

            var server1 = (IServerTreeDicomServer)tree.RootServerGroup.ChildGroups.Skip(1).First().Servers.First();
            var treeServer1 = server1.ToDataContract();
            Assert.AreEqual(directoryServers[0], treeServer1);

            var server2 = (IServerTreeDicomServer)tree.RootServerGroup.ChildGroups.Skip(1).First().Servers.Skip(1).First();
            Assert.AreEqual(directoryServers[1], server2.ToDataContract());
            
            var server3 = (IServerTreeDicomServer)tree.RootServerGroup.Servers.First();
            Assert.AreEqual(directoryServers[2], server3.ToDataContract());

            var server4 = (IServerTreeDicomServer)tree.RootServerGroup.Servers.Skip(1).First();
            Assert.AreEqual(directoryServers[3], server4.ToDataContract());
        }

        [Test]
        public void TestSerializeStoredTree()
        {
            //var rootGroup = CreateStoredServerGroup1();
            //var serialized = StoredServerGroupSerializationHelper.Serialize(rootGroup);
            //var deserialized = StoredServerGroupSerializationHelper.Deserialize(serialized);
            //Assert.AreEqual(2, deserialized.ChildGroups.Count);
            //Assert.AreEqual(2, deserialized.DirectoryServerReferences.Count);
            //Assert.AreEqual(2, deserialized.ChildGroups[1].DirectoryServerReferences.Count);

            //var server1 = deserialized.ChildGroups.Skip(1).First().DirectoryServerReferences.First();
            //Assert.AreEqual("server1", server1.Name);

            //var server2 = deserialized.ChildGroups.Skip(1).First().DirectoryServerReferences.Skip(1).First();
            //Assert.AreEqual("server2", server2.Name);

            //var server3 = deserialized.DirectoryServerReferences.First();
            //Assert.AreEqual("server3", server3.Name);

            //var server4 = deserialized.DirectoryServerReferences.Skip(1).First();
            //Assert.AreEqual("server4", server4.Name);
        }

        #region Legacy Tests

        [Test]
        public void TestCreateFromLegacyXml()
        {
            var tree = TestHelper.CreateTestTree1();
            Assert.AreEqual(2, tree.RootServerGroup.ChildGroups.Count);

        }

        [Test]
        public void TestIgnoreDuplicateNamesOnLoadFromLegacyXml()
        {
        }

        #endregion
    }
}

#endif