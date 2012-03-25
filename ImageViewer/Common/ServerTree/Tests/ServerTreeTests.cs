using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom.ServiceModel;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Common.ServerTree.Tests
{
    [TestFixture]
    public class ServerTreeTests
    {
        [Test]
        public void TestCreateExampleTree()
        {
            var tree = new ServerTree(null);
            AssertExampleTree(tree);

            tree = new ServerTree(null, null);
            AssertExampleTree(tree);
        }

        [Test]
        public void TestGetAllServers()
        {
            var tree = CreateTestTree1();
            var allServers = tree.RootServerGroup.GetAllServers();
            Assert.AreEqual(4, allServers.Count);
        }

        [Test]
        public void TestCreateStoredTreeFromDirectory()
        {
            List<ApplicationEntity> directoryServers;
            var tree = CreateTestTree1(out directoryServers);

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
            var tree = CreateTestTree1();
            Assert.AreEqual(2, tree.RootServerGroup.ChildGroups.Count);

        }

        [Test]
        public void TestIgnoreDuplicateNamesOnLoadFromLegacyXml()
        {
        }

        #endregion

        private static ServerTree CreateTestTree1()
        {
            List<ApplicationEntity> dummy;
            return CreateTestTree1(out dummy);
        }

        private static ServerTree CreateTestTree1(out List<ApplicationEntity> directoryServers)
        {
            var rootGroup = CreateStoredServerGroup1();
            directoryServers = CreateDirectoryServers1();
            return new ServerTree(rootGroup, directoryServers);
        }

        private static StoredServerGroup CreateStoredServerGroup1()
        {
            return new StoredServerGroup(@"My Servers")
                       {
                           ChildGroups =
                               {
                                   new StoredServerGroup("Empty"),
                                   new StoredServerGroup("Test")
                                       {
                                           DirectoryServerReferences =
                                               {
                                                   new DirectoryServerReference("server1"),
                                                   new DirectoryServerReference("server2")
                                               }
                                       }

                               },
                           DirectoryServerReferences =
                               {
                                   new DirectoryServerReference("server3"),
                                   new DirectoryServerReference("server4")
                               }
                       };
        }

        private static List<ApplicationEntity> CreateDirectoryServers1()
        {
            return new List<ApplicationEntity>
                       {
                           new ApplicationEntity
                               {
                                   Name = "server1",
                                   AETitle = "server1",
                                   ScpParameters = new ScpParameters("server1", 104)
                               },
                           new ApplicationEntity
                               {
                                   Name = "server2",
                                   AETitle = "server2",
                                   ScpParameters = new ScpParameters("server2", 104)
                               },
                           new ApplicationEntity
                               {
                                   Name = "server3",
                                   AETitle = "server3",
                                   ScpParameters = new ScpParameters("server3", 104)
                               },
                           new ApplicationEntity
                               {
                                   Name = "server4",
                                   AETitle = "server4",
                                   ScpParameters = new ScpParameters("server4", 104)
                               }
                       };
        }

        private static void AssertExampleTree(ServerTree tree)
        {
            Assert.IsNotNull(tree.LocalServer);
            Assert.IsNotNull(tree.RootServerGroup);
            Assert.AreEqual(1, tree.RootServerGroup.Servers.Count);
            Assert.AreEqual(1, tree.RootServerGroup.ChildGroups.Count);

            Assert.AreEqual(SR.ExampleServer, tree.RootServerGroup.Servers[0].Name);
            Assert.AreEqual(SR.ExampleGroup, tree.RootServerGroup.ChildGroups[0].Name);
        }
    }
}
