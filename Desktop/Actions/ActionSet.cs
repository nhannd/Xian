using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Default implementation of <see cref="IActionSet"/>
    /// </summary>
    public class ActionSet : IActionSet
    {
        private readonly List<IAction> _actions;

        /// <summary>
        /// Constructor
        /// </summary>
        public ActionSet()
            : this(null)
        {
        }

        /// <summary>
        /// Constructs an action set containing the specified actions.
        /// </summary>
        /// <param name="actions"></param>
        public ActionSet(IEnumerable<IAction> actions)
        {
            _actions = new List<IAction>();

            if(actions != null)
                _actions.AddRange(actions);
        }

        #region IActionSet members

        /// <summary>
        /// Returns a subset of this set containing only the elements for which the predicate is true.
        /// </summary>
        /// <param name="predicate">The predicate to test</param>
        /// <returns></returns>
        public IActionSet Select(Predicate<IAction> predicate)
        {
            List<IAction> subset = new List<IAction>();
            foreach (IAction action in _actions)
            {
                if (predicate(action))
                    subset.Add(action);
            }
            return new ActionSet(subset);
        }

        /// <summary>
        /// Gets the number of actions in the set.
        /// </summary>
        public int Count
        {
            get { return _actions.Count; }
        }

        /// <summary>
        /// Returns a set that corresponds to the union of this set with another set.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public IActionSet Union(IActionSet other)
        {
            List<IAction> union = new List<IAction>();
            union.AddRange(this);
            union.AddRange(other);
            return new ActionSet(union);
        }

        #endregion

        #region IEnumerable<IAction> Members


        public IEnumerator<IAction> GetEnumerator()
        {
            return _actions.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _actions.GetEnumerator();
        }

        #endregion

    }
}
