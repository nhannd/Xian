using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{

    public class ActionModelRenderer
    {
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

        private string GetHTML(string actionPath, string actionLabel)
        {
            return String.Format("<a href=\"action://{0}\">{1}</a>", actionPath, actionLabel);
        }
    }
}
