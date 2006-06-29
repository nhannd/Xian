using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    public interface IApplicationComponent
    {
        void SetHost(IApplicationComponentHost host);
    }
}
