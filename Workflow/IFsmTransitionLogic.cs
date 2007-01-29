using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Workflow
{
    public interface IFsmTransitionLogic<TStatusEnum>
    {
        bool IsAllowed(TStatusEnum from, TStatusEnum to);
        bool IsTerminal(TStatusEnum state);
    }
}
