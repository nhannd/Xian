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
            myDsg = dst.MyServerGroup;
            serverCount = 1;

            string groupName1 = "Group1";
            string groupName2 = "Group2";
            string serverName1 = "S11_1";
            string serverName2 = "S22";

            // Add a new group ("Group1")under "My Servers"
            DicomServerGroup dsg = new DicomServerGroup(groupName1, myDsg.ChildServers[0].ServerPath + "/" + myDsg.ChildServers[0].ServerName);
            ((DicomServerGroup)myDsg.ChildServers[0]).AddChild(dsg);
            // Add a new group ("Group2") under the newly added group ("Group1") 
            DicomServerGroup dsg2 = new DicomServerGroup(groupName2, dsg.ServerPath + "/" + dsg.ServerName);
            dsg.AddChild(dsg2);
            // Add a new server under ("Group2"), the number of child servers are 2 now 
            DicomServer ds = new DicomServer(serverName1, serverName1, serverName1, serverName1, serverName1, 222);
            ds.ServerPath = dsg2.ServerPath + "/" + dsg2.ServerName;
            dsg2.AddChild(ds);
            serverCount += 1;

            // Add a new server "S22" under "My Servers"/"Group1"
            dsg = (DicomServerGroup)((DicomServerGroup)myDsg.ChildServers[0]).ChildServers[0];
            ds = new DicomServer(serverName2, serverName2, serverName2, serverName2, serverName2, 333);
            ds.ServerPath = dsg.ServerPath + "/" + dsg.ServerName;
            dsg.AddChild(ds);
            serverCount += 1;
            // Add the same new server "S22" under "My Servers"
            dsg = (DicomServerGroup)myDsg.ChildServers[0];
            ds = new DicomServer(serverName2, serverName2, serverName2, serverName2, serverName2, 333);
            ds.ServerPath = dsg.ServerPath + "/" + dsg.ServerName;
            dsg.AddChild(ds);
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
        }

        [Test]
        public void SelectChangedTest()
        {
        }

        [Test]
        public void NodeMovedTest()
        {
        }

    }
}

#endif
