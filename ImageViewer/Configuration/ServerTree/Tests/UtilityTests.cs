#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.Configuration.Tests;
using ClearCanvas.ImageViewer.Common.DicomServer.Tests;
using ClearCanvas.ImageViewer.Common.ServerDirectory.Tests;
using ClearCanvas.ImageViewer.Common.StudyManagement.Tests;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree.Tests
{
    [TestFixture]
    public class UtilityTests
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            DicomServerTestServiceProvider.Reset();
            StudyStoreTestServiceProvider.Reset();
            ServerDirectoryTestServiceProvider.Reset();

            var factory = new UnitTestExtensionFactory
                              {
                                  { typeof (ServiceProviderExtensionPoint), typeof (DicomServerTestServiceProvider) },
                                  { typeof (ServiceProviderExtensionPoint), typeof (StudyStoreTestServiceProvider) },
                                  { typeof (ServiceProviderExtensionPoint), typeof (ServerDirectoryTestServiceProvider) },
                                  {typeof(ServiceProviderExtensionPoint), typeof(TestSystemConfigurationServiceProvider)}
                              };
            Platform.SetExtensionFactory(factory);
        }

        [Test]
        public void TestToDataContract()
        {
            var treeServer = new ServerTreeDicomServer("name", "location", "host", "aetitle", 105, true, 51121, 51122);
            var ae = treeServer.ToDataContract();
            Assert.AreEqual("name", ae.Name);
            Assert.AreEqual("location", ae.Location);
            Assert.AreEqual("aetitle", ae.AETitle);
            Assert.AreEqual("host", ae.ScpParameters.HostName);
            Assert.AreEqual(105, ae.ScpParameters.Port);
            Assert.IsNotNull(ae.StreamingParameters);
            Assert.AreEqual(51121, ae.StreamingParameters.HeaderServicePort);
            Assert.AreEqual(51122, ae.StreamingParameters.WadoServicePort);

            treeServer.IsStreaming = false;
            ae = treeServer.ToDataContract();
            Assert.AreEqual("name", ae.Name);
            Assert.AreEqual("location", ae.Location);
            Assert.AreEqual("aetitle", ae.AETitle);
            Assert.AreEqual("host", ae.ScpParameters.HostName);
            Assert.AreEqual(105, ae.ScpParameters.Port);
            Assert.IsNull(ae.StreamingParameters);
        }

        [Test]
        public void TestToDicomServiceNodes_Local()
        {
            var tree = new ServerTree(null, null);
            tree.CurrentNode = tree.LocalServer;
            var serviceNodes = tree.CurrentNode.ToDicomServiceNodes();
            Assert.AreEqual(1, serviceNodes.Count);
            var ae = serviceNodes.First();

            Assert.AreEqual(@"<local>", ae.Name);
            Assert.AreEqual(string.Empty, ae.Location ?? "");
            Assert.AreEqual("AETITLE", ae.AETitle);
            Assert.AreEqual("localhost", ae.ScpParameters.HostName);
            Assert.AreEqual(104, ae.ScpParameters.Port);
            Assert.IsNull(ae.StreamingParameters);
        }

        [Test]
        public void TestToDicomServiceNodes_Remote()
        {
            var tree = TestHelper.CreateTestTree1();

            tree.CurrentNode = tree.RootServerGroup;
            var serviceNodes = tree.CurrentNode.ToDicomServiceNodes();
            Assert.AreEqual(4, serviceNodes.Count);
        }
    }
}

#endif