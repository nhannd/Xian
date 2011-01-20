#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Default implementation of <see cref="IActionSet"/>.
    /// </summary>
    public class ActionSet : IActionSet
    {
        private readonly List<IAction> _actions;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActionSet()
            : this(null)
        {
        }

        /// <summary>
        /// Constructs an action set containing the specified actions.
        /// </summary>
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
        public IActionSet Union(IActionSet other)
        {
            List<IAction> union = new List<IAction>();
            union.AddRange(this);
            union.AddRange(other);
            return new ActionSet(union);
        }

        #endregion

        #region IEnumerable<IAction> Members

		/// <summary>
		/// Gets an enumerator for the <see cref="IAction"/>s in the set.
		/// </summary>
        public IEnumerator<IAction> GetEnumerator()
        {
            return _actions.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

		/// <summary>
		/// Gets an enumerator for the <see cref="IAction"/>s in the set.
		/// </summary>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _actions.GetEnumerator();
        }

        #endregion

    }
}
