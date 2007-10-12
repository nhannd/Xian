#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
        private readonly PathSegment _pathSegment;
        private readonly ActionModelNodeList _childNodes;
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
        /// Used by the <see cref="CloneTree"/> method.  Derived classes must override this method to return a new node of their own type.
        /// </summary>
        /// <param name="pathSegment">The path segment which this node represents.</param>
        /// <returns>A new node of this type.</returns>
        protected virtual ActionModelNode CreateNode(PathSegment pathSegment)
        {
            return new ActionModelNode(pathSegment);
        }

        /// <summary>
        /// Gets the action path segment represented by this node.
        /// </summary>
        public PathSegment PathSegment
        {
            get { return _pathSegment; }
        }

        /// <summary>
        /// Gets the action associated with this node, or null if this node is not a leaf node.
        /// </summary>
        public IAction Action
        {
            get { return _action; }
            set { _action = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this node is a leaf node.
        /// </summary>
        public bool IsLeaf
        {
            get { return _childNodes.Count == 0; }
        }

        /// <summary>
        /// Gets the list of child nodes of this node.
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
				throw new ArgumentException(SR.ExceptionInvalidActionPath);

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
