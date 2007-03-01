using System;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Data;

namespace ClearCanvas.Enterprise.Time
{
    public interface ITimeBroker : IPersistenceBroker
    {
        DateTime GetTime();
    }
}
