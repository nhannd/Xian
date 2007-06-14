using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tests
{
    class StubWorkspace : Workspace
    {
        class StubWorkspaceView : IWorkspaceView
        {
            #region IWorkspaceView Members

            public void SetWorkspace(IWorkspace workspace)
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

        public StubWorkspace(IDesktopWindow window)
            : base("Stub", window)
        {
        }

        public override IActionSet Actions
        {
            get { return new ActionSet(); }
        }
    }
}
