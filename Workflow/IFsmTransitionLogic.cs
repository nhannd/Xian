using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Workflow
{
    public interface IFsmTransitionLogic<TStatusEnum>
    {
        bool IsAllowed(TStatusEnum from, TStatusEnum to);
        bool IsInitial(TStatusEnum state);
        bool IsTerminal(TStatusEnum state);
    }
}
