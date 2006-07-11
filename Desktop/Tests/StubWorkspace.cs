using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Tests
{
    class StubWorkspace : Workspace
    {
        class StubWorkspaceView : IWorkspaceView
        {
            #region IWorkspaceView Members

            public void SetWorkspace(Workspace workspace)
            {
            }

            #endregion

            #region IView Members

            public GuiToolkitID GuiToolkitID
            {
                get { return GuiToolkitID.WinForms; }
            }

            public object GuiElement
            {
                get { return null; }
            }

            #endregion
        }

        class StubToolsExtensionPoint : ExtensionPoint<ITool>
        {
        }

        private IWorkspaceView _view;

        public StubWorkspace()
            : base("Stub")
        {
        }

        public override IWorkspaceView View
        {
            get { if (_view == null) _view = new StubWorkspaceView(); return _view; }
        }

        public override void Cleanup()
        {
        }

        public override IToolSet ToolSet
        {
            get { return new ToolSet(new StubToolsExtensionPoint(), new ToolContext()); }
        }
    }
}
