#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Represents a simple, in memory action model that is created in code, not via attributes.
	/// </summary>
	/// <remarks>
	/// The <see cref="SimpleActionModel"/> is particularly useful for action models that 
	/// are created in code and/or are not intended to be dynamic or extensible.
	/// </remarks>
    public class SimpleActionModel : ActionModelRoot
    {
        private IResourceResolver _resolver;
        private Dictionary<object, ClickAction> _actions;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="resolver">The <see cref="IResourceResolver"/> used to resolve the path and icons for the actions.</param>
        public SimpleActionModel(IResourceResolver resolver)
        {
            _resolver = resolver;
            _actions = new Dictionary<object, ClickAction>();
        }

        /// <summary>
        /// Adds an action to the action model.
        /// </summary>
        /// <param name="key">The action key, so that actions can be easily retrieve via the <see cref="SimpleActionModel.this"/> indexer.</param>
        /// <param name="displayName">The display name for the action.</param>
        /// <param name="icon">The resource name of the icon.</param>
		public ClickAction AddAction(object key, string displayName, string icon)
        {
            return AddActionHelper(key, displayName, icon, displayName, null, null);
        }

		/// <summary>
		/// Adds an action to the action model.
		/// </summary>
		/// <param name="key">The action key, so that actions can be easily retrieve via the <see cref="SimpleActionModel.this"/> indexer.</param>
		/// <param name="displayName">The display name for the action.</param>
		/// <param name="icon">The resource name of the icon.</param>
		/// <param name="clickHandler">The click handler of the action.</param>
		public ClickAction AddAction(object key, string displayName, string icon, ClickHandlerDelegate clickHandler)
        {
            return AddActionHelper(key, displayName, icon, displayName, clickHandler, null);
        }

		/// <summary>
		/// Adds an action to the action model.
		/// </summary>
		/// <param name="key">The action key, so that actions can be easily retrieve via the <see cref="SimpleActionModel.this"/> indexer.</param>
		/// <param name="displayName">The display name for the action.</param>
		/// <param name="icon">The resource name of the icon.</param>
		/// <param name="tooltip">The action tooltip.</param>
		public ClickAction AddAction(object key, string displayName, string icon, string tooltip)
        {
            return AddActionHelper(key, displayName, icon, tooltip, null, null);
        }

		/// <summary>
		/// Adds an action to the action model.
		/// </summary>
		/// <param name="key">The action key, so that actions can be easily retrieve via the <see cref="SimpleActionModel.this"/> indexer.</param>
		/// <param name="displayName">The display name for the action.</param>
		/// <param name="icon">The resource name of the icon.</param>
		/// <param name="tooltip">The action tooltip.</param>
		/// <param name="clickHandler">The click handler of the action.</param>
        public ClickAction AddAction(object key, string displayName, string icon, string tooltip, ClickHandlerDelegate clickHandler)
        {
            return AddActionHelper(key, displayName, icon, tooltip, clickHandler, null);
        }

		/// <summary>
		/// Adds an action to the action model.
		/// </summary>
		/// <param name="key">The action key, so that actions can be easily retrieve via the <see cref="SimpleActionModel.this"/> indexer.</param>
		/// <param name="displayName">The display name for the action.</param>
		/// <param name="icon">The resource name of the icon.</param>
		/// <param name="tooltip">The action tooltip.</param>
		/// <param name="clickHandler">The click handler of the action.</param>
		/// <param name="authorityToken">The authority token for the action.</param>
		public ClickAction AddAction(object key, string displayName, string icon, string tooltip, ClickHandlerDelegate clickHandler, string authorityToken)
        {
            return AddActionHelper(key, displayName, icon, tooltip, clickHandler, new PrincipalPermissionSpecification(authorityToken));
        }

		/// <summary>
		/// Adds an action to the action model.
		/// </summary>
		/// <param name="key">The action key, so that actions can be easily retrieve via the <see cref="SimpleActionModel.this"/> indexer.</param>
		/// <param name="displayName">The display name for the action.</param>
		/// <param name="icon">The resource name of the icon.</param>
		/// <param name="tooltip">The action tooltip.</param>
		/// <param name="clickHandler">The click handler of the action.</param>
		/// <param name="permissionSpec">The permission specification for the action.</param>
		public ClickAction AddAction(object key, string displayName, string icon, string tooltip, ClickHandlerDelegate clickHandler, ISpecification permissionSpec)
		{
			return AddActionHelper(key, displayName, icon, tooltip, clickHandler, permissionSpec);
		}
		
		private ClickAction AddActionHelper(object key, string displayName, string icon, string tooltip, ClickHandlerDelegate clickHandler, ISpecification permissionSpec)
        {
            Platform.CheckForNullReference(key, "key");

            ClickAction action = new ClickAction(displayName, new ActionPath(string.Format("root/{0}", displayName), _resolver), ClickActionFlags.None, _resolver);
            action.Tooltip = tooltip;
            action.Label = displayName;
            if (icon != null)
				action.IconSet = new IconSet(IconScheme.Colour, icon, icon, icon);

            if (clickHandler != null)
            {
                action.SetClickHandler(clickHandler);
            }
            if (permissionSpec != null)
            {
				action.SetPermissibility(permissionSpec);
            }

            this.InsertAction(action);

            _actions[key] = action;

            return action;
        }

        /// <summary>
        /// Indexer; gets actions according to a key.
        /// </summary>
		public ClickAction this[object key]
        {
            get { return _actions[key]; }
        }
    }
}
