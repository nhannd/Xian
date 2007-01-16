using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Common
{
    public class SimpleActionModel : ActionModelRoot
    {
        private IResourceResolver _resolver;
        private Dictionary<object, ClickAction> _actions;

        public SimpleActionModel(IResourceResolver resolver)
        {
            _resolver = resolver;
            _actions = new Dictionary<object, ClickAction>();
        }

        public ClickAction AddAction(object key, string displayName, string icon)
        {
            return AddActionHelper(key, displayName, icon, displayName, null);
        }

        public ClickAction AddAction(object key, string displayName, string icon, ClickHandlerDelegate clickHandler)
        {
            return AddActionHelper(key, displayName, icon, displayName, clickHandler);
        }

        public ClickAction AddAction(object key, string displayName, string icon, string tooltip)
        {
            return AddActionHelper(key, displayName, icon, tooltip, null);
        }

        public ClickAction AddAction(object key, string displayName, string icon, string tooltip, ClickHandlerDelegate clickHandler)
        {
            return AddActionHelper(key, displayName, icon, tooltip, clickHandler);
        }

        private ClickAction AddActionHelper(object key, string displayName, string icon, string tooltip, ClickHandlerDelegate clickHandler)
        {
            ClickAction action = new ClickAction(displayName, new ActionPath(string.Format("root/{0}", displayName), _resolver), ClickActionFlags.None, _resolver);
            action.Tooltip = tooltip;
            action.Label = displayName;
            action.IconSet = new IconSet(IconScheme.Colour, icon, icon, icon);
            if (clickHandler != null)
            {
                action.SetClickHandler(clickHandler);
            }

            this.InsertAction(action);

            _actions[key] = action;

            return action;
        }

        public ClickAction this[object key]
        {
            get { return _actions[key]; }
        }
    }
}
