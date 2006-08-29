using System;
using System.Collections.Generic;

namespace ClearCanvas.Desktop.Actions
{
    public delegate bool ActionSelectorDelegate(IAction action);

    public interface IActionSet : IEnumerable<IAction>
    {
        int Count { get; }
        IActionSet Select(ActionSelectorDelegate selector);
        IActionSet Add(IActionSet other);
    }
}
