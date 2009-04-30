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

using System.Collections.Generic;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Used by <see cref="ActionModelNode"/> to hold the list of child nodes.
    /// </summary>
    public class ActionModelNodeList : List<ActionModelNode>
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ActionModelNodeList()
		{
		}

    	/// <summary>
        /// Returns the child node whose <see cref="ActionModelNode.PathSegment"/> 
        /// <see cref="ClearCanvas.Desktop.PathSegment.LocalizedText"/> property
        /// is equal to the specified value.
        /// </summary>
        /// <param name="name">The name of the node to retrieve.</param>
        /// <returns>The corresponding child node, or null if no such node exists.</returns>
        public ActionModelNode this[string name]
        {
            get
            {
                foreach (ActionModelNode node in this)
                {
                    // define node equality in terms of the localized text
                    // (eg two menu items with the same name should be the same menu item, 
                    // even if a different resource key was used)
                    if (node.PathSegment.LocalizedText == name)
                        return node;
                }
                return null;
            }
        }
    }
}
