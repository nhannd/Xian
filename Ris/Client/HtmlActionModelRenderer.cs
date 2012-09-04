#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
            foreach (var action in actions)
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