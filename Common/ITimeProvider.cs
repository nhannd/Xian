using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    public interface ITimeProvider
    {
        DateTime CurrentTime { get; }
    }
}
