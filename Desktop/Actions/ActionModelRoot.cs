#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Represents the root node of an action model.
    /// </summary>
    public class ActionModelRoot : ActionModelNode
    {
		private readonly string _site;
		
		/// <summary>
        /// Creates the action model with the specified namespace and site, using the specified
        /// set of actions as input.
        /// </summary>
        /// <remarks>
        /// If an action model specification for the namespace/site
        /// does not exist, it will be created.  If it does exist, it will be used as guidance
        /// in constructing the action model tree.
        /// </remarks>
        /// <param name="namespace">A namespace to qualify the site, typically the class name of the calling class is a good choice.</param>
        /// <param name="site">The site (<see cref="ActionPath.Site"/>).</param>
        /// <param name="actions">The set of actions from which to construct the model.</param>
        /// <returns>An action model tree.</returns>
        public static ActionModelRoot CreateModel(string @namespace, string site, IActionSet actions)
        {
			return ActionModelSettings.Default.BuildAndSynchronize(@namespace, site, actions.Select(delegate(IAction action) { return action.Path.Site == site; }));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActionModelRoot()
            :this(null)
        {
        }

        /// <summary>
		/// Constructor.
        /// </summary>
        /// <param name="site">The site to which this model corresponds.</param>
        public ActionModelRoot(string site)
            : base(null)
        {
            _site = site;
        }

        /// <summary>
        /// Gets the site (the first component of the path).
        /// </summary>
        public string Site
        {
            get { return _site; }
        }

        /// <summary>
        /// Inserts the specified actions into this model in the specified order.
        /// </summary>
        /// <param name="actions">The actions to insert.</param>
        public void InsertActions(IAction[] actions)
        {
            foreach (IAction action in actions)
            {
                InsertAction(action);
            }
        }

        /// <summary>
        /// Inserts the specified action into this model.
        /// </summary>
        /// <param name="action">The action to insert.</param>
        public void InsertAction(IAction action)
        {
			Insert(action.Path, 1,
				delegate(PathSegment segment)
				{
					return new ActionNode(segment, action);
				});
		}

		/// <summary>
		/// Inserts a separator into the action model at the specified path.
		/// </summary>
		/// <param name="separatorPath"></param>
		public void InsertSeparator(Path separatorPath)
		{
			Insert(separatorPath, 1,
				delegate(PathSegment segment)
				{
					return new SeparatorNode(segment);
				});
		}

		/// <summary>
        /// Used by the <see cref="ActionModelNode.CloneTree"/> method.
        /// </summary>
        /// <param name="pathSegment">The path segment which this node represents.</param>
        /// <returns>A new node of this type.</returns>
        protected override ActionModelNode CloneNode(PathSegment pathSegment)
        {
            return new ActionModelRoot();
        }
    }
}
