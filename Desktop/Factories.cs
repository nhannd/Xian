using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    [ExtensionPoint]
    public class DesktopWindowFactoryExtensionPoint : ExtensionPoint<IDesktopWindowFactory>
    {
    }

    public interface IDesktopWindowFactory
    {
        DesktopWindow CreateWindow(DesktopWindowCreationArgs args, Application application);
    }

    internal class DefaultDesktopWindowFactory : IDesktopWindowFactory
    {
        #region IDesktopWindowFactory Members

        public DesktopWindow CreateWindow(DesktopWindowCreationArgs args, Application application)
        {
            return new DesktopWindow(args, application);
        }

        #endregion
    }

    [ExtensionPoint]
    public class WorkspaceFactoryExtensionPoint : ExtensionPoint<IWorkspaceFactory>
    {
    }

    public interface IWorkspaceFactory
    {
        Workspace CreateWorkspace(WorkspaceCreationArgs args, DesktopWindow window);
    }

    internal class DefaultWorkspaceFactory : IWorkspaceFactory
    {
        #region IWorkspaceFactory Members

        public Workspace CreateWorkspace(WorkspaceCreationArgs args, DesktopWindow window)
        {
            return new Workspace(args, window);
        }

        #endregion
    }

    [ExtensionPoint]
    public class ShelfFactoryExtensionPoint : ExtensionPoint<IShelfFactory>
    {
    }

    public interface IShelfFactory
    {
        Shelf CreateShelf(ShelfCreationArgs args, DesktopWindow window);
    }

    internal class DefaultShelfFactory : IShelfFactory
    {
        #region IShelfFactory Members

        public Shelf CreateShelf(ShelfCreationArgs args, DesktopWindow window)
        {
            return new Shelf(args, window);
        }

        #endregion
    }

    [ExtensionPoint]
    public class DialogBoxFactoryExtensionPoint : ExtensionPoint<IDialogBoxFactory>
    {
    }

    public interface IDialogBoxFactory
    {
        DialogBox CreateDialogBox(DialogBoxCreationArgs args, DesktopWindow window);
    }

    internal class DefaultDialogBoxFactory : IDialogBoxFactory
    {
        #region IDialogBoxFactory Members

        public DialogBox CreateDialogBox(DialogBoxCreationArgs args, DesktopWindow window)
        {
            return new DialogBox(args, window);
        }

        #endregion
    }
}
