using System;
using System.Collections.Generic;

namespace ClearCanvas.Desktop.Actions
{
    public delegate bool ActionSelectorDelegate(IAction action);

    /// <summary>
    /// Represents an unordered set of actions.
    /// </summary>
    public interface IActionSet : IEnumerable<IAction>
    {
        /// <summary>
        /// Returns the number of actions in the set
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns a subset of this set containing only the elements for which the selector returns true.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        IActionSet Select(ActionSelectorDelegate selector);

        /// <summary>
        /// Returns a set that corresponds to the union of this set with another set.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        IActionSet Union(IActionSet other);
    }
}
