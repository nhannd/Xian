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
        private PathSegment _pathSegment;
        private ActionModelNodeList _childNodes;
        private IAction _action; // null if this is not a leaf node

        /// <summary>
        /// Protected constructor
        /// </summary>
        /// <param name="pathSegment">The segment of the action path to which this node corresponds</param>
        protected ActionModelNode(PathSegment pathSegment)
        {
            _pathSegment = pathSegment;
            _childNodes = new ActionModelNodeList();
        }

        /// <summary>
        /// Derived classes must override this method to return a new node of their own type.
        /// </summary>
        /// <param name="pathSegment">The path segment which this node represents</param>
        /// <returns>A new node of this type.</returns>
        protected virtual ActionModelNode CreateNode(PathSegment pathSegment)
        {
            return new ActionModelNode(pathSegment);
        }

        /// <summary>
        /// The action path segment represented by this node.
        /// </summary>
        public PathSegment PathSegment
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
        public ActionModelNodeList ChildNodes
        {
            get { return _childNodes; }
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
            if (segmentCount < 2)
                throw new ArgumentException("Invalid action path.  Path must have 2 or more segments.");

            PathSegment segment = action.Path.Segments[pathDepth];
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

        protected ActionModelNode FindChild(PathSegment segment)
        {
            return _childNodes[segment.LocalizedText];
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
