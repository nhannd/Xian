using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Model
{
    /// <summary>
    /// Actions applied to the duplicate
    /// </summary>
    public enum ProcessDuplicateAction
    {
        Delete,

        OverwriteAsIs,

        OverwriteUseDuplicates,

        OverwriteUseExisting
    }
}