using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{

    public class ActionSet : IActionSet
    {
        private List<IAction> _actions;

        public ActionSet()
            : this(null)
        {
        }

        public ActionSet(IEnumerable<IAction> actions)
        {
            _actions = new List<IAction>();

            if(actions != null)
                _actions.AddRange(actions);
        }

        public IActionSet Select(ActionSelectorDelegate selector)
        {
            List<IAction> subset = new List<IAction>();
            foreach (IAction action in _actions)
            {
                if (selector(action))
                    subset.Add(action);
            }
            return new ActionSet(subset);
        }

        public int Count
        {
            get { return _actions.Count; }
        }

        public IActionSet Add(IActionSet other)
        {
            List<IAction> union = new List<IAction>();
            union.AddRange(this);
            union.AddRange(other);
            return new ActionSet(union);
        }

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
