using System;

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
    public interface ITimeBroker : IPersistenceBroker
    {
        DateTime GetTime();
    }
}
