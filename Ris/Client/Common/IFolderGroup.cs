using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Common
{
    public interface IFolderGroup
    {
        IFolder[] Folders { get; }
        ActionModelRoot ToolbarModel { get; }
        ActionModelRoot MenuModel { get; }
    }
}
