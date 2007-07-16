using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public interface IDesktopWindowView : IDesktopObjectView
    {
        IWorkspaceView CreateWorkspaceView(Workspace workspace);
        IShelfView CreateShelfView(Shelf shelf);
        IDialogBoxView CreateDialogBoxView(DialogBox dialog);

        void SetMenuModel(ActionModelNode model);
        void SetToolbarModel(ActionModelNode model);

        DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons);
    }
}
