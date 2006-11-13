#if	UNIT_TESTS

using System;
using System.Collections;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.Tests
{
	[TestFixture]
	public class DicomServerTreeTest
	{
        private DicomServerTree dst;
        private DicomServerGroup myDsg;

        public DicomServerTreeTest()
        {
        }

        [TestFixtureSetUp]
        public void Init()
        {
            dst = new DicomServerTree();
            myDsg = dst.MyServerGroup;
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
        }

        [Test]
        public void DicomServerDefaultTest()
        {
            Assert.AreEqual(myDsg.ChildServers.Count, 2);
            Assert.IsTrue(myDsg.ChildServers[1].ServerName.Equals(AENavigatorComponent.MyDatastoreTitle));
            Assert.IsTrue(myDsg.ChildServers[0].ServerName.Equals(AENavigatorComponent.MyServersTitle));
            dst.CurrentServer = myDsg.ChildServers[1];
            Assert.IsFalse(dst.DicomServerValidation(AENavigatorComponent.MyDatastoreTitle, "", "", 100).Equals(""));
            Assert.IsFalse(dst.DicomServerGroupNameValidation(AENavigatorComponent.MyServersTitle).Equals(""));
            dst.FindChildServers(myDsg);
            Assert.AreEqual(dst.ChildServers.Count, 1);
            DicomServer ds = new DicomServer("S1", "S1", "S1", "S1", "S1", 111);
            dst.ReplaceDicomServer(ds);
            Assert.IsFalse(myDsg.ChildServers[1].ServerName.Equals(AENavigatorComponent.MyDatastoreTitle));
            Assert.IsTrue(myDsg.ChildServers[1].ServerName.Equals("S1"));
            dst.RenameDicomServerGroup((DicomServerGroup)myDsg.ChildServers[0], "Group2", "", "", 0);
            Assert.IsFalse(myDsg.ChildServers[0].ServerName.Equals(AENavigatorComponent.MyServersTitle));
            Assert.IsTrue(myDsg.ChildServers[0].ServerName.Equals("Group2"));
            dst = new DicomServerTree();
            myDsg = dst.MyServerGroup;
            Assert.IsTrue(myDsg.ChildServers[1].ServerName.Equals(AENavigatorComponent.MyDatastoreTitle));
            Assert.IsTrue(myDsg.ChildServers[0].ServerName.Equals(AENavigatorComponent.MyServersTitle));
        }

        [Test]
        public void DicomServerGroupTest()
        {
            int servercount = 1;
            string groupName1 = "Group1";
            string groupName2 = "Group2";
            string groupName3 = "Group3";
            string serverName1 = "S11_1";
            DicomServerGroup dsg = new DicomServerGroup(groupName1, myDsg.ChildServers[0].ServerPath + "/" + myDsg.ChildServers[0].ServerName);
            ((DicomServerGroup)myDsg.ChildServers[0]).AddChild(dsg);
            dst.CurrentServer = dsg;
            Assert.AreEqual(myDsg.ChildServers.Count, 2);
            Assert.IsTrue(((DicomServerGroup)myDsg.ChildServers[0]).ChildServers[0].ServerName.Equals(groupName1));
            DicomServerGroup dsg2 = new DicomServerGroup(groupName2, dsg.ServerPath + "/" + dsg.ServerName);
            dsg.AddChild(dsg2);
            DicomServer ds = new DicomServer(serverName1, serverName1, serverName1, serverName1, serverName1, 222);
            ds.ServerPath = dsg2.ServerPath + "/" + dsg2.ServerName;
            dsg2.AddChild(ds);
            dst.FindChildServers(myDsg);
            Assert.AreEqual(dst.ChildServers.Count, (servercount+1));
            
            foreach (IDicomServer ids in ((DicomServerGroup)dst.CurrentServer).ChildServers)
            {
                Assert.Greater(ids.ServerPath.IndexOf(groupName1), 1);
                if (!ids.IsServer)
                {
                    foreach (IDicomServer ids2 in ((DicomServerGroup)ids).ChildServers)
                    {
                        Assert.Greater(ids2.ServerPath.IndexOf(groupName1), 1);
                    }
                }
            }
            dst.RenameDicomServerGroup((DicomServerGroup)dst.CurrentServer, groupName3, "", "", 0);
            Assert.IsTrue(dst.CurrentServer.ServerName.Equals(groupName3));
            foreach (IDicomServer ids in ((DicomServerGroup)dst.CurrentServer).ChildServers)
            {
                Assert.Less(ids.ServerPath.IndexOf(groupName1), 1);
                if (!ids.IsServer)
                {
                    foreach (IDicomServer ids2 in ((DicomServerGroup)ids).ChildServers)
                    {
                        Assert.Less(ids2.ServerPath.IndexOf(groupName1), 1);
                    }
                }
            }
        }

    }
}

#endif