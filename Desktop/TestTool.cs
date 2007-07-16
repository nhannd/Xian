using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    /*
    [MenuAction("shelf1", "global-menus/Test/Shelf1")]
    [ClickHandler("shelf1", "Shelf1")]
    [MenuAction("show1", "global-menus/Test/Show Shelf1")]
    [ClickHandler("show1", "ShowShelf1")]
    [MenuAction("hide1", "global-menus/Test/Hide Shelf1")]
    [ClickHandler("hide1", "HideShelf1")]

    [MenuAction("shelf2", "global-menus/Test/Shelf2")]
    [ClickHandler("shelf2", "Shelf2")]
    [MenuAction("show2", "global-menus/Test/Show Shelf2")]
    [ClickHandler("show2", "ShowShelf2")]
    [MenuAction("hide2", "global-menus/Test/Hide Shelf2")]
    [ClickHandler("hide2", "HideShelf2")]

    [MenuAction("ws", "global-menus/Test/New Workspace")]
    [ClickHandler("ws", "NewWorkspace")]
    */
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class TestTool : Tool<IDesktopToolContext>
    {
        private Shelf _shelf1;
        private Shelf _shelf2;
        private int _workspaceCount;

        void Shelf1()
        {
            if (_shelf1 == null)
            {
                TestComponent component = new TestComponent("Shelf1");
                _shelf1 = ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
                    component,
                    "Shelf 1",
                    ShelfDisplayHint.DockAutoHide | ShelfDisplayHint.DockRight | ShelfDisplayHint.HideOnWorkspaceOpen,
                    delegate(IApplicationComponent c) { _shelf1 = null; });
            }
            else
            {
                _shelf1.Activate();
            }
        }

        void ShowShelf1()
        {
            if (_shelf1 != null)
                _shelf1.Show();
        }

        void HideShelf1()
        {
            if (_shelf1 != null)
                _shelf1.Hide();
        }

        void Shelf2()
        {
            if (_shelf2 == null)
            {
                TestComponent component = new TestComponent("Shelf2");
                _shelf2 = ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
                    component,
                    "Shelf 2",
                    ShelfDisplayHint.DockAutoHide | ShelfDisplayHint.DockLeft,
                    delegate(IApplicationComponent c) { _shelf2 = null; });
            }
            else
            {
                _shelf2.Activate();
            }
        }

        void ShowShelf2()
        {
            if (_shelf2 != null)
                _shelf2.Show();
        }

        void HideShelf2()
        {
            if (_shelf2 != null)
                _shelf2.Hide();
        }

        void NewWorkspace()
        {
            string name = "Workspace " + (_workspaceCount++);
            ApplicationComponent.LaunchAsWorkspace(
                this.Context.DesktopWindow,
                new TestComponent(name),
                name,
                null);
        }
    }
}
