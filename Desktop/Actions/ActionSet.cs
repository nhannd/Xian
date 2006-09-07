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
        private List<IAction> _actions;

        /// <summary>
        /// Constructor
        /// </summary>
        public ActionSet()
            : this(null)
        {
        }

        /// <summary>
        /// Constructs an action set containing all actions in the specified <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="actions"></param>
        public ActionSet(IEnumerable<IAction> actions)
        {
            _actions = new List<IAction>();

            if(actions != null)
                _actions.AddRange(actions);
        }

        #region IActionSet members

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

        public int Count
        {
            get { return _actions.Count; }
        }

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
