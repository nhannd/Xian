using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Represents a node in an action model.
    /// </summary>
    public class ActionModelNode
    {
        private ActionPathSegment _pathSegment;
        private List<ActionModelNode> _childNodes;
        private IAction _action; // null if this is not a leaf node

        protected ActionModelNode(ActionPathSegment pathSegment)
        {
            _pathSegment = pathSegment;
            _childNodes = new List<ActionModelNode>();
        }

        /// <summary>
        /// Derived classes must override this method to return a new node of their own type.
        /// </summary>
        /// <param name="pathSegment">The path segment which this node represents</param>
        /// <returns>A new node of this type.</returns>
        protected virtual ActionModelNode CreateNode(ActionPathSegment pathSegment)
        {
            return new ActionModelNode(pathSegment);
        }

        /// <summary>
        /// The action path segment represented by this node.
        /// </summary>
        public ActionPathSegment PathSegment
        {
            get { return _pathSegment; }
        }

        /// <summary>
        /// The action associated with this node, or null if this node is not a leaf node.
        /// </summary>
        public IAction Action
        {
            get { return _action; }
            set { _action = value; }
        }

        /// <summary>
        /// Reports whether this node is a leaf node.
        /// </summary>
        public bool IsLeaf
        {
            get { return _childNodes.Count == 0; }
        }

        /// <summary>
        /// The set of child nodes of this node.
        /// </summary>
        public ActionModelNode[] ChildNodes
        {
            get { return _childNodes.ToArray(); }
        }

        /// <summary>
        /// Merges the specified model into this model.
        /// </summary>
        /// <param name="other">The other model</param>
        public void Merge(ActionModelNode other)
        {
            foreach (ActionModelNode otherChild in other._childNodes)
            {
                ActionModelNode thisChild = FindChild(otherChild.PathSegment);
                if (thisChild != null)
                {
                    thisChild.Merge(otherChild);
                }
                else
                {
                    _childNodes.Add(otherChild.CloneTree());
                }
            }
        }

        /// <summary>
        /// Performs an in-order traversal of this model and returns the set of actions as an array.
        /// </summary>
        /// <returns>An array of <see cref="IAction"/> objects</returns>
        public IAction[] GetActionsInOrder()
        {
            List<IAction> actions = new List<IAction>();
            GetActionsInOrder(actions);
            return actions.ToArray();
        }

        protected void InsertAction(IAction action, int pathDepth)
        {
            int segmentCount = action.Path.Segments.Length;

            // if there is not at least one path item, the action can't be inserted
            if (segmentCount == 0)
                return;

            ActionPathSegment segment = action.Path.Segments[pathDepth];
            if (pathDepth + 1 == segmentCount)
            {
                // this is the last path segment -> leaf node
                ActionModelNode child = new ActionModelNode(segment);
                child.Action = action;
                _childNodes.Add(child);
            }
            else
            {
                ActionModelNode child = FindChild(segment);
                if (child == null)
                {
                    child = new ActionModelNode(segment);
                    _childNodes.Add(child);
                }
                child.InsertAction(action, pathDepth + 1);
            }
        }

        protected ActionModelNode FindChild(ActionPathSegment segment)
        {
            foreach (ActionModelNode child in _childNodes)
            {
                // define node equality in terms of the localized text
                // (eg two menu items with the same name should be the same menu item, 
                // even if a different resource key was used)
                if (child.PathSegment.LocalizedText == segment.LocalizedText)
                    return child;
            }
            return null;
        }

        /// <summary>
        /// Creates a copy of the subtree beginning at this node.
        /// </summary>
        /// <returns></returns>
        protected ActionModelNode CloneTree()
        {
            ActionModelNode clone = CreateNode(this.PathSegment);
            clone._action = this._action;
            foreach (ActionModelNode child in _childNodes)
            {
                clone._childNodes.Add(child.CloneTree());
            }
            return clone;
        }

        private void GetActionsInOrder(List<IAction> actions)
        {
            if (_action == null)
            {
                foreach (ActionModelNode child in _childNodes)
                {
                    child.GetActionsInOrder(actions);
                }
            }
            else
            {
                actions.Add(_action);
            }
        }

    }
}
