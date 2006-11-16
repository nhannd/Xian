#if	UNIT_TESTS

using System;
using System.Collections;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.Tests
{
	[TestFixture]
	public class AENavigatorComponentTest
	{
        private AENavigatorComponent component;
        private DicomServerTree dst;
        private DicomServerGroup myDsg;
        private int serverCount;

        public AENavigatorComponentTest()
        {
        }

        [TestFixtureSetUp]
        public void Init()
        {
            component = new AENavigatorComponent();
            dst = component.DicomServerTree;
            dst.LoadDicomServers(true);
            myDsg = dst.MyServerGroup;
            serverCount = 1;

        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
        }

        [Test]
        public void SelectChangedTest()
        {
            string groupName1 = "Group1";
            string groupName2 = "Group2";
            string groupName3 = "Group3";
            string serverName1 = "S11";
            string serverName2 = "S22";
            string serverName3 = "S33";

            DicomServerGroup myServers = (DicomServerGroup)myDsg.ChildServers[0];
            // Add a new group ("Group1")under "My Servers"
            DicomServerGroup dsg1 = new DicomServerGroup(groupName1, myServers.ServerPath + "/" + myServers.ServerName);
            myServers.AddChild(dsg1);
            // Add a new group ("Group2") under the newly added group ("Group1") 
            DicomServerGroup dsg2 = new DicomServerGroup(groupName2, dsg1.ServerPath + "/" + dsg1.ServerName);
            dsg1.AddChild(dsg2);
            // Add a new group ("Group3") under "My Servers"
            DicomServerGroup dsg3 = new DicomServerGroup(groupName3, myServers.ServerPath + "/" + myServers.ServerName);
            myServers.AddChild(dsg3);

            // Add a new server "S11" under ("Group2")
            DicomServer ds1 = new DicomServer(serverName1, serverName1, serverName1, serverName1, serverName1, 222);
            ds1.ServerPath = dsg2.ServerPath + "/" + dsg2.ServerName;
            dsg2.AddChild(ds1);
            serverCount += 1;

            // Add a new server "S22" under "My Servers"/"Group1"
            DicomServer ds2 = new DicomServer(serverName2, serverName2, serverName2, serverName2, serverName2, 333);
            ds2.ServerPath = dsg1.ServerPath + "/" + dsg1.ServerName;
            dsg1.AddChild(ds2);
            serverCount += 1;
            // Add the same new server "S22" under "My Servers"
            ds2 = new DicomServer(serverName2, serverName2, serverName2, serverName2, serverName2, 333);
            ds2.ServerPath = myServers.ServerPath + "/" + myServers.ServerName;
            myServers.AddChild(ds2);

            // Add a new server "S33" under ("Group3") 
            DicomServer ds3 = new DicomServer(serverName3, serverName3, serverName3, serverName3, serverName3, 666);
            ds3.ServerPath = dsg3.ServerPath + "/" + dsg3.ServerName;
            dsg3.AddChild(ds3);
            serverCount += 1;
            // Add the same new server "S33" under ("Group2")
            ds3 = new DicomServer(serverName3, serverName3, serverName3, serverName3, serverName3, 666);
            ds3.ServerPath = dsg2.ServerPath + "/" + dsg2.ServerName;
            dsg2.AddChild(ds3);

            component.SelectChanged(myDsg.ChildServers[1]);
            Assert.AreEqual(component.SelectedServers.Servers.Count, 1);
            Assert.IsTrue(component.SelectedServers.Servers[0].ServerName.Equals(AENavigatorComponent.MyDatastoreTitle));
            Assert.IsTrue(component.SelectedServers.GroupID.Equals("./" + AENavigatorComponent.MyDatastoreTitle));
            Assert.IsTrue(component.SelectedServers.IsLocalDatastore);

            component.SelectChanged(ds3);
            Assert.AreEqual(component.SelectedServers.Servers.Count, 1);
            Assert.IsTrue(component.SelectedServers.Servers[0].ServerName.Equals(serverName3));
            Assert.IsTrue(component.SelectedServers.GroupID.Equals(ds3.ServerPath + "/" + ds3.ServerName));
            Assert.IsFalse(component.SelectedServers.IsLocalDatastore);

            component.SelectChanged(myServers);
            Assert.AreEqual(component.SelectedServers.Servers.Count, 3);
            Assert.IsTrue(component.SelectedServers.Servers[0].ServerName.Equals(serverName1)
                            || component.SelectedServers.Servers[0].ServerName.Equals(serverName2)
                            || component.SelectedServers.Servers[0].ServerName.Equals(serverName3));
            Assert.IsTrue(component.SelectedServers.GroupID.Equals(myServers.ServerPath + "/" + myServers.ServerName));
            Assert.IsFalse(component.SelectedServers.IsLocalDatastore);

            component.SelectChanged(dsg1);
            Assert.AreEqual(component.SelectedServers.Servers.Count, 3);
            Assert.IsTrue(component.SelectedServers.Servers[0].ServerName.Equals(serverName1)
                            || component.SelectedServers.Servers[0].ServerName.Equals(serverName2)
                            || component.SelectedServers.Servers[0].ServerName.Equals(serverName3));
            Assert.IsTrue(component.SelectedServers.GroupID.Equals(dsg1.ServerPath + "/" + dsg1.ServerName));
            Assert.IsFalse(component.SelectedServers.IsLocalDatastore);

            component.SelectChanged(dsg2);
            Assert.AreEqual(component.SelectedServers.Servers.Count, 2);
            Assert.IsTrue(component.SelectedServers.Servers[0].ServerName.Equals(serverName1)
                            || component.SelectedServers.Servers[0].ServerName.Equals(serverName3));
            Assert.IsTrue(component.SelectedServers.GroupID.Equals(dsg2.ServerPath + "/" + dsg2.ServerName));
            Assert.IsFalse(component.SelectedServers.IsLocalDatastore);

            // Restore the default for the next tests
            dst.LoadDicomServers(true);
            myDsg = dst.MyServerGroup;
            serverCount = 1;
        }

        [Test]
        public void NodeMovedTest()
        {
            string groupName1 = "Group1";
            string groupName2 = "Group2";
            string groupName3 = "Group3";
            string serverName1 = "S11";
            string serverName2 = "S22";
            string serverName3 = "S33";

            DicomServerGroup myServers = (DicomServerGroup)myDsg.ChildServers[0];
            // Add a new group ("Group1")under "My Servers"
            DicomServerGroup dsg1 = new DicomServerGroup(groupName1, myServers.ServerPath + "/" + myServers.ServerName);
            myServers.AddChild(dsg1);
            // Add a new group ("Group2") under the newly added group ("Group1") 
            DicomServerGroup dsg2 = new DicomServerGroup(groupName2, dsg1.ServerPath + "/" + dsg1.ServerName);
            dsg1.AddChild(dsg2);
            // Add a new group ("Group3") under "My Servers"
            DicomServerGroup dsg3 = new DicomServerGroup(groupName3, myServers.ServerPath + "/" + myServers.ServerName);
            myServers.AddChild(dsg3);

            // Add a new server "S11" under ("Group1")
            DicomServer ds1 = new DicomServer(serverName1, serverName1, serverName1, serverName1, serverName1, 222);
            ds1.ServerPath = dsg1.ServerPath + "/" + dsg1.ServerName;
            dsg1.AddChild(ds1);
            serverCount += 1;

            // Add a new server "S22" under "My Servers"/"Group3"
            DicomServer ds2 = new DicomServer(serverName2, serverName2, serverName2, serverName2, serverName2, 333);
            ds2.ServerPath = dsg3.ServerPath + "/" + dsg3.ServerName;
            dsg3.AddChild(ds2);
            serverCount += 1;
            // Add the same new server "S22" under "My Servers"
            ds2 = new DicomServer(serverName2, serverName2, serverName2, serverName2, serverName2, 333);
            ds2.ServerPath = myServers.ServerPath + "/" + myServers.ServerName;
            myServers.AddChild(ds2);

            // Add a new server "S33" under ("Group3") 
            DicomServer ds3 = new DicomServer(serverName3, serverName3, serverName3, serverName3, serverName3, 666);
            ds3.ServerPath = dsg3.ServerPath + "/" + dsg3.ServerName;
            dsg3.AddChild(ds3);
            serverCount += 1;
            // Add the same new server "S33" under ("Group2")
            ds3 = new DicomServer(serverName3, serverName3, serverName3, serverName3, serverName3, 666);
            ds3.ServerPath = dsg2.ServerPath + "/" + dsg2.ServerName;
            dsg2.AddChild(ds3);

            // Try to move "S22" from "Group3" to "My Servers": fail because of duplication
            component.NodeMoved(myServers, ds2);
            Assert.AreEqual(dsg3.ChildServers.Count, 2);
            Assert.AreEqual(myServers.ChildServers.Count, 3);
            Assert.IsTrue(dsg3.ChildServers[0].ServerName.Equals(serverName2)
                            || dsg3.ChildServers[1].ServerName.Equals(serverName2));

            // Try to move "S33" to "S11": fail because destination should be server group
            component.NodeMoved(ds1, ds3);
            Assert.AreEqual(dsg3.ChildServers.Count, 2);
            Assert.AreEqual(dsg1.ChildServers.Count, 2);
            Assert.IsTrue(dsg3.ChildServers[0].ServerName.Equals(serverName3)
                            || dsg3.ChildServers[1].ServerName.Equals(serverName3));

            // Try to move "S33" to "My Datastore": fail because destination should be server group
            component.NodeMoved(myDsg.ChildServers[1], ds3);
            Assert.AreEqual(dsg3.ChildServers.Count, 2);
            Assert.IsTrue(dsg3.ChildServers[0].ServerName.Equals(serverName3)
                            || dsg3.ChildServers[1].ServerName.Equals(serverName3));

            // Move "S22" from "My Servers" to "Group2"
            component.NodeMoved(dsg2, ds2);
            Assert.AreEqual(dsg2.ChildServers.Count, 2);
            Assert.AreEqual(myServers.ChildServers.Count, 2);
            Assert.IsTrue(dsg2.ChildServers[0].ServerName.Equals(serverName2)
                            || dsg2.ChildServers[1].ServerName.Equals(serverName2));
            Assert.IsFalse(myServers.ChildServers[0].ServerName.Equals(serverName2)
                            || myServers.ChildServers[1].ServerName.Equals(serverName2));

            // Move "Group3" to "Group2"
            component.NodeMoved(dsg2, dsg3);
            Assert.AreEqual(dsg2.ChildServers.Count, 3);
            Assert.AreEqual(myServers.ChildServers.Count, 1);
            Assert.IsTrue(dsg2.ChildServers[0].ServerName.Equals(groupName3)
                            || dsg2.ChildServers[1].ServerName.Equals(groupName3)
                            || dsg2.ChildServers[2].ServerName.Equals(groupName3));
            Assert.IsFalse(myServers.ChildServers[0].ServerName.Equals(groupName3));
            if (dsg2.ChildServers[0].ServerName.Equals(groupName3))
                dsg3 = (DicomServerGroup)dsg2.ChildServers[0];
            else if (dsg2.ChildServers[1].ServerName.Equals(groupName3))
                dsg3 = (DicomServerGroup)dsg2.ChildServers[1];
            else
                dsg3 = (DicomServerGroup)dsg2.ChildServers[2];
            Assert.AreEqual(dsg3.ChildServers.Count, 2);
            Assert.IsTrue(dsg3.ChildServers[0].ServerName.Equals(serverName2)
                            || dsg3.ChildServers[1].ServerName.Equals(serverName2)
                            || dsg3.ChildServers[0].ServerName.Equals(serverName3)
                            || dsg3.ChildServers[1].ServerName.Equals(serverName3));

            // Restore the default for the next tests
            dst.LoadDicomServers(true);
            myDsg = dst.MyServerGroup;
            serverCount = 1;
        }

    }
}

#endif
