using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Used by the action attribute mechanism to maintain context between attributes that co-operate to build the same action
    /// </summary>
    public interface IActionBuildingContext
    {
        Action Action { get; set; }
        IResourceResolver ResourceResolver { get; }
        object ActionTarget { get; }
        string ActionID { get; }
	}
}
