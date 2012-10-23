#if UNIT_TESTS

#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Configuration.Tests
{
    [TestFixture]
    public class CustomizeActionModelTests
    {
        private IAction _mainMenu;
        private IAction _contextMenu;

        [TestFixtureSetUp]
        public void Initialize()
        {
            Platform.SetExtensionFactory(new NullExtensionFactory());
        }

        private void RefreshTool()
        {
            var tool = new CustomizeViewerActionModelTool();
            var actions = tool.Actions;

            _mainMenu = actions.Select(a => a.ActionID == CustomizeViewerActionModelTool._mainMenuCustomizeId).First();
            _contextMenu = actions.Select(a => a.ActionID == CustomizeViewerActionModelTool._contextMenuCustomizeId).First();
        }

        [Test]
        public void TestDefault()
        {
            RefreshTool();
            Assert.AreEqual(true, _mainMenu.Available);
            Assert.AreEqual(false, _contextMenu.Available);
        }

        [Test]
        public void TestUserShowsBoth()
        {
            RefreshTool();
            _contextMenu.Available = true;

            Assert.AreEqual(true, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            RefreshTool();
            _mainMenu.Available = false;
            _contextMenu.Available = false;

            Assert.AreEqual(false, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            _mainMenu.Available = true;
            _contextMenu.Available = true;

            Assert.AreEqual(true, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            RefreshTool();
            _mainMenu.Available = false;
            _contextMenu.Available = false;

            _contextMenu.Available = true;
            _mainMenu.Available = true;

            Assert.AreEqual(true, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);
        }

        [Test]
        public void TestUserHidesBoth()
        {
            RefreshTool();
            _contextMenu.Available = true;

            Assert.AreEqual(true, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            _mainMenu.Available = false;
            _contextMenu.Available = false;

            Assert.AreEqual(false, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            RefreshTool();
            _contextMenu.Available = false;
            _mainMenu.Available = false;

            Assert.AreEqual(false, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);
        }

        [Test]
        public void TestUserShowsContextMenu()
        {
            RefreshTool();
            _mainMenu.Available = false;
            _contextMenu.Available = true;

            Assert.AreEqual(false, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            RefreshTool();
            _mainMenu.Available = false;

            Assert.AreEqual(false, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            _contextMenu.Available = true;

            Assert.AreEqual(false, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);
        }

        [Test]
        public void TestUserShowsMainMenu()
        {
            RefreshTool();
            _mainMenu.Available = false;
            _contextMenu.Available = true;

            Assert.AreEqual(false, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);

            _mainMenu.Available = true;

            Assert.AreEqual(true, _mainMenu.Available);
            Assert.AreEqual(true, _contextMenu.Available);
        }
    }
}

#endif