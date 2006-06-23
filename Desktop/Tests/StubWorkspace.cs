using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Application.Tools;

namespace ClearCanvas.Common.Application.Tests
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

        class StubToolContext : ToolContext
        {
            public StubToolContext(IExtensionPoint xp)
                : base(xp)
            {
            }
        }

        class StubToolsExtensionPoint : ExtensionPoint<ITool>
        {
        }

        private IWorkspaceView _view;

        public override IWorkspaceView View
        {
            get { if (_view == null) _view = new StubWorkspaceView(); return _view; }
        }

        public override void Cleanup()
        {
        }

        protected override ClearCanvas.Common.Application.Tools.ToolContext CreateToolContext()
        {
            return new StubToolContext(new StubToolsExtensionPoint());
        }
    }
}
