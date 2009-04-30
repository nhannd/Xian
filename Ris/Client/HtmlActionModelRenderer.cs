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

using System;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// A helper class for rendering an action model as HTML.
    /// </summary>
    public class HtmlActionModelRenderer
    {
        /// <summary>
        /// Searches <paramref name="actionNode"/> and returns the action (represented as HTML) whose label matches
        /// <paramref name="labelSearch"/>.
        /// </summary>
        /// <param name="actionNode">The node to be searched.</param>
        /// <param name="labelSearch">The label to match on.</param>
        /// <param name="actionLabel">The new label to be applied to the action in the returned HTML.</param>
        /// <returns>The found action represented as HTML, otherwise an empty string.</returns>
        public string GetHTML(ActionModelNode actionNode, string labelSearch, string actionLabel)
        {
            IAction[] actions = actionNode.GetActionsInOrder();
            if (actions.Length == 0)
                return "";

            // find the action corresponding to the action label, if exist
            foreach (Action action in actions)
            {
                if (action.Label == labelSearch)
                    return GetHTML(action.Path.LocalizedPath, actionLabel);
            }

            return "";
        }

        private static string GetHTML(string actionPath, string actionLabel)
        {
            return String.Format("<a href=\"action://{0}\">{1}</a>", actionPath, actionLabel);
        }
    }
}