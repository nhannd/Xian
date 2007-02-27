using System;

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise
{
    public interface ITimeBroker : IPersistenceBroker
    {
        DateTime GetTime();
    }
}
