using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Actions
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
            return AddActionHelper(key, displayName, icon, displayName, null, null);
        }

        public ClickAction AddAction(object key, string displayName, string icon, ClickHandlerDelegate clickHandler)
        {
            return AddActionHelper(key, displayName, icon, displayName, clickHandler, null);
        }

        public ClickAction AddAction(object key, string displayName, string icon, string tooltip)
        {
            return AddActionHelper(key, displayName, icon, tooltip, null, null);
        }

        public ClickAction AddAction(object key, string displayName, string icon, string tooltip, ClickHandlerDelegate clickHandler)
        {
            return AddActionHelper(key, displayName, icon, tooltip, clickHandler, null);
        }

        public ClickAction AddAction(object key, string displayName, string icon, string tooltip, ClickHandlerDelegate clickHandler, string authorityToken)
        {
            return AddActionHelper(key, displayName, icon, tooltip, clickHandler, authorityToken);
        }

        private ClickAction AddActionHelper(object key, string displayName, string icon, string tooltip, ClickHandlerDelegate clickHandler, string authorityToken)
        {
            Platform.CheckForNullReference(key, "key");

            ClickAction action = new ClickAction(displayName, new ActionPath(string.Format("root/{0}", displayName), _resolver), ClickActionFlags.None, _resolver);
            action.Tooltip = tooltip;
            action.Label = displayName;
            action.IconSet = new IconSet(IconScheme.Colour, icon, icon, icon);
            if (clickHandler != null)
            {
                action.SetClickHandler(clickHandler);
            }
            if (authorityToken != null)
            {
                action.PermissionSpecification = new PrincipalPermissionSpecification(authorityToken);
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
