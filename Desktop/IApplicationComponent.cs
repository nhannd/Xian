using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    public interface IApplicationComponent
    {
        void SetHost(IApplicationComponentHost host);
        IToolSet ToolSet { get; }
    }
}
