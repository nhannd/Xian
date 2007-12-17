#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

#if	NOCOMPILE

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
        private int serverCount;

        public DicomServerTreeTest()
        {
        }

        [TestFixtureSetUp]
        public void Init()
        {
            dst = new DicomServerTree();
            dst.LoadDicomServers(true);
            myDsg = dst.MyServerGroup;
            serverCount = 1;
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
        }

        [Test]
        public void DicomServerDefaultTest()
        {
            string serverName1 = "S1";
            string groupName1 = "Group2";

            // The default children are only two: "My Studies" and "My Servers"
            Assert.AreEqual(myDsg.ChildServers.Count, 2);
            Assert.IsTrue(myDsg.ChildServers[1].ServerName.Equals(AENavigatorComponent.MyDatastoreTitle));
            Assert.IsTrue(myDsg.ChildServers[0].ServerName.Equals(AENavigatorComponent.MyServersTitle));

            // Do not allow to add another child of "My Studies" or "My Servers", or rename to either of them
            dst.CurrentServer = myDsg.ChildServers[1];
            Assert.IsFalse(dst.DicomServerValidation(AENavigatorComponent.MyDatastoreTitle, "", "", 100).Equals(""));
            Assert.IsFalse(dst.DicomServerGroupNameValidation(AENavigatorComponent.MyServersTitle).Equals(""));

            // There is only one server available: "My Studies"
            dst.FindChildServers(myDsg);
            Assert.AreEqual(dst.ChildServers.Count, 1);
            DicomServer ds = new DicomServer(serverName1, serverName1, serverName1, serverName1, serverName1, 111);

            // Rename a server or a group
            dst.ReplaceDicomServer(ds);
            Assert.IsFalse(myDsg.ChildServers[1].ServerName.Equals(AENavigatorComponent.MyDatastoreTitle));
            Assert.IsTrue(myDsg.ChildServers[1].ServerName.Equals(serverName1));
            dst.RenameDicomServerGroup((DicomServerGroup)myDsg.ChildServers[0], groupName1, "", "", 0);
            Assert.IsFalse(myDsg.ChildServers[0].ServerName.Equals(AENavigatorComponent.MyServersTitle));
            Assert.IsTrue(myDsg.ChildServers[0].ServerName.Equals(groupName1));

            // Restore the default for the next tests
            dst.LoadDicomServers(true);
            myDsg = dst.MyServerGroup;
            serverCount = 1;
            Assert.IsTrue(myDsg.ChildServers[1].ServerName.Equals(AENavigatorComponent.MyDatastoreTitle));
            Assert.IsTrue(myDsg.ChildServers[0].ServerName.Equals(AENavigatorComponent.MyServersTitle));
        }

        [Test]
        public void DicomServerGroupTest()
        {
            string groupName1 = "Group1";
            string groupName2 = "Group2";
            string groupName3 = "Group3";
            string serverName1 = "S11_1";

            // Add a new group ("Group1")under "My Servers"
            DicomServerGroup dsg = new DicomServerGroup(groupName1, myDsg.ChildServers[0].ServerPath + "/" + myDsg.ChildServers[0].ServerName);
            ((DicomServerGroup)myDsg.ChildServers[0]).AddChild(dsg);
            dst.CurrentServer = dsg;

            // The number of children on root is not changed
            Assert.AreEqual(myDsg.ChildServers.Count, 2);
            Assert.IsTrue(((DicomServerGroup)myDsg.ChildServers[0]).ChildServers[0].ServerName.Equals(groupName1));

            // Add a new group ("Group2") under the newly added group ("Group1") 
            DicomServerGroup dsg2 = new DicomServerGroup(groupName2, dsg.ServerPath + "/" + dsg.ServerName);
            dsg.AddChild(dsg2);
            // Add a new server under ("Group2"), the number of child servers are 2 now 
            DicomServer ds = new DicomServer(serverName1, serverName1, serverName1, serverName1, serverName1, 222);
            ds.ServerPath = dsg2.ServerPath + "/" + dsg2.ServerName;
            dsg2.AddChild(ds);
            serverCount += 1;
            dst.FindChildServers(myDsg);
            Assert.AreEqual(dst.ChildServers.Count, serverCount);

            // "Group1" is able to be found among the child server groups under dst.CurrentServer
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
            // Rename dst.CurrentServer from "Group1" to "Group3"
            // "Group1" is not able to be found among the child server groups under dst.CurrentServer
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

            // Restore the default for the next tests
            dst.LoadDicomServers(true);
            myDsg = dst.MyServerGroup;
            serverCount = 1;
        }

        [Test]
        public void DicomServerTest()
        {
            string serverName2 = "S22";
            DicomServerPrepared(1);

            // Add a new server "S22" under "My Servers"/"Group3"
            // The number of child servers is increased to be 3 
            DicomServerGroup dsg = (DicomServerGroup)((DicomServerGroup)myDsg.ChildServers[0]).ChildServers[0];
            DicomServer ds = new DicomServer(serverName2, serverName2, serverName2, serverName2, serverName2, 333);
            ds.ServerPath = dsg.ServerPath + "/" + dsg.ServerName;
            dsg.AddChild(ds);
            serverCount += 1;
            dst.FindChildServers(myDsg);
            Assert.AreEqual(dst.ChildServers.Count, serverCount);

            // Add the same new server "S22" under "My Servers"
            // The number of child servers is not increased, but keep the same as 3, because two newly added server are identical 
            dsg = (DicomServerGroup)myDsg.ChildServers[0];
            ds = new DicomServer(serverName2, serverName2, serverName2, serverName2, serverName2, 333);
            ds.ServerPath = dsg.ServerPath + "/" + dsg.ServerName;
            dsg.AddChild(ds);
            dst.FindChildServers(myDsg);
            Assert.AreEqual(dst.ChildServers.Count, serverCount);

            // Edit one of "S22" under "My Servers", with properties of Location and AETitle and Port changed
            // The number of child servers is not changed
            dst.CurrentServer = ds;
            ds = new DicomServer(serverName2, serverName2, "NewLocation", serverName2, "NewAETitle", 444);
            ds.ServerPath = dsg.ServerPath + "/" + dsg.ServerName;
            dst.ReplaceDicomServer(ds);
            dst.FindChildServers(myDsg);
            Assert.AreEqual(dst.ChildServers.Count, serverCount);

            // For "S22" under "My Servers", all 3 properties of Location and AETitle and Port are changed
            ds = (DicomServer)dsg.ChildServers[dsg.ChildServers.Count - 1];
            Assert.IsTrue(ds.ServerName.Equals(serverName2));
            Assert.IsTrue(ds.ServerLocation.Equals("NewLocation"));
            Assert.IsTrue(ds.DicomAE.AE.Equals("NewAETitle"));
            Assert.AreEqual(ds.DicomAE.Port, 444);

            // For "S22" under "My Servers"/"Group3", both properties of AETitle and Port are changed, but Location is not changed
            dsg = (DicomServerGroup)((DicomServerGroup)myDsg.ChildServers[0]).ChildServers[0];
            ds = (DicomServer)dsg.ChildServers[dsg.ChildServers.Count - 1];
            dst.CurrentServer = ds;
            Assert.IsTrue(ds.ServerName.Equals(serverName2));
            Assert.IsTrue(ds.ServerLocation.Equals(serverName2));
            Assert.IsTrue(ds.DicomAE.AE.Equals("NewAETitle"));
            Assert.AreEqual(ds.DicomAE.Port, 444);

            // Edit "S22" under "My Servers"/"Group3", with the server name and all properties changed to "S33"
            string serverName3 = "S33";
            ds = new DicomServer(serverName3, serverName3, serverName3, serverName3, serverName3, 123);
            ds.ServerPath = dsg.ServerPath + "/" + dsg.ServerName;
            dst.ReplaceDicomServer(ds);

            ds = (DicomServer)dsg.ChildServers[dsg.ChildServers.Count - 1];
            Assert.IsTrue(ds.ServerName.Equals(serverName3));
            Assert.IsTrue(ds.ServerLocation.Equals(serverName3));
            Assert.IsTrue(ds.DicomAE.AE.Equals(serverName3));
            Assert.AreEqual(ds.DicomAE.Port, 123);

            // For "S22" under "My Servers", nothing is changed
            dsg = (DicomServerGroup)myDsg.ChildServers[0];
            ds = (DicomServer)dsg.ChildServers[dsg.ChildServers.Count - 1];
            Assert.IsTrue(ds.ServerName.Equals(serverName2));
            Assert.IsTrue(ds.ServerLocation.Equals("NewLocation"));
            Assert.IsTrue(ds.DicomAE.AE.Equals("NewAETitle"));
            Assert.AreEqual(ds.DicomAE.Port, 444);

            // The number of child servers is increased to be 4 now, because one of "S22" has been changed to "S33"
            dst.FindChildServers(myDsg);
            serverCount += 1;
            Assert.AreEqual(dst.ChildServers.Count, serverCount);

            // Restore the default for the next tests
            dst.LoadDicomServers(true);
            myDsg = dst.MyServerGroup;
            serverCount = 1;
        }

        [Test]
        public void DicomServerDeleteTest()
        {
            DicomServerPrepared(2);
            DicomServerGroup dsg = (DicomServerGroup)myDsg.ChildServers[0];
            int n = dsg.ChildServers.Count;
            DicomServer ds = (DicomServer)dsg.ChildServers[n - 1];
            dst.CurrentServer = ds;

            // Delete "S22" under "My Servers"; The number of child servers is not changed
            dst.DeleteDicomServer();
            dst.FindChildServers(myDsg);
            Assert.AreEqual(dst.ChildServers.Count, serverCount);
            Assert.AreEqual(dsg.ChildServers.Count, n-1);

            // Delete "Group1" from "My Servers", back to default status
            dst.CurrentServer = dsg.ChildServers[0];
            dst.DeleteDicomServer();
            Assert.AreEqual(dsg.ChildServers.Count, 0);
            dst.FindChildServers(myDsg);
            Assert.AreEqual(dst.ChildServers.Count, 1);

            // Restore the default for the next tests
            dst.LoadDicomServers(true);
            myDsg = dst.MyServerGroup;
            serverCount = 1;
        }

        private void DicomServerPrepared(int stage)
        {
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

            if (stage == 1)
                return;

            // Add a new server "S22" under "My Servers"/"Group1"
            // The number of child servers is increased to be 3 
            dsg = (DicomServerGroup)((DicomServerGroup)myDsg.ChildServers[0]).ChildServers[0];
            ds = new DicomServer(serverName2, serverName2, serverName2, serverName2, serverName2, 333);
            ds.ServerPath = dsg.ServerPath + "/" + dsg.ServerName;
            dsg.AddChild(ds);
            serverCount += 1;

            // Add the same new server "S22" under "My Servers"
            // The number of child servers is not increased, but keep the same as 3, because two newly added server are identical 
            dsg = (DicomServerGroup)myDsg.ChildServers[0];
            ds = new DicomServer(serverName2, serverName2, serverName2, serverName2, serverName2, 333);
            ds.ServerPath = dsg.ServerPath + "/" + dsg.ServerName;
            dsg.AddChild(ds);
            dst.FindChildServers(myDsg);
        }

    }
}

#endif