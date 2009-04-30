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
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Abstract base class providing a default implementation of <see cref="IAction"/>.  
    /// </summary>
    /// <remarks>
	/// Action classes should inherit from this class rather than implement <see cref="IAction"/> directly.
	/// </remarks>
    public abstract class Action : IAction
    {
        private readonly string _actionID;
		
		private ActionPath _path;
        private readonly IResourceResolver _resourceResolver;

		private GroupHint _groupHint;

        private IconSet _iconSet;
		private event EventHandler _iconSetChanged;

        private bool _enabled;
        private event EventHandler _enabledChanged;

        private bool _visible;
        private event EventHandler _visibleChanged;

        private string _tooltip;
        private event EventHandler _tooltipChanged;

        private string _label;
        private event EventHandler _labelChanged;

		private bool _persistent;

        private ISpecification _permissionSpec;

		/// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionID">The logical action ID.</param>
        /// <param name="path">The action path.</param>
        /// <param name="resourceResolver">A resource resolver that will be used to resolve icons associated with this action.</param>
        protected Action(string actionID, ActionPath path, IResourceResolver resourceResolver)
        {
            _actionID = actionID;
            _path = path;
            _resourceResolver = resourceResolver;

            // smart defaults
            _enabled = true;
            _visible = true;

			_persistent = false;
        }

        /// <summary>
        /// Sets the <see cref="ISpecification"/> that is tested to establish whether the 
        /// current user has sufficient privileges to access the action.
        /// </summary>
        /// <remarks>
		/// This overload is useful when an actions permissibility is a boolean function of a
		/// multiple authority tokens.  Use the <see cref="PrincipalPermissionSpecification"/>, in 
		/// combination with the <see cref="AndSpecification"/> and <see cref="OrSpecification"/> classes
		/// to build up a complex specification for permissibility.
		/// </remarks>
		public void SetPermissibility(ISpecification permissionSpecification)
        {
        	_permissionSpec = permissionSpecification;
        }

		/// <summary>
		/// Sets a single authority token that is tested to establish whether the 
		/// current user has sufficient privileges to access the action.
		/// </summary>
		/// <remarks>
		/// This overload is useful in the common case where an actions permissibility
		/// is tied to a single authority token.  To handle a situation where the permissibility
		/// is a function of multiple authority tokens, use the <see cref="SetPermissibility(ISpecification)"/>
		/// overload.
		/// </remarks>
		/// <param name="authorityToken"></param>
		public void SetPermissibility(string authorityToken)
		{
			SetPermissibility(new PrincipalPermissionSpecification(authorityToken));
		}

		/// <summary>
		/// Provides internal access to the permission specification.
		/// </summary>
    	internal ISpecification PermissionSpecification
    	{
			get { return _permissionSpec; }
			set { _permissionSpec = value; }
    	}

        #region IAction members

        /// <summary>
        /// Gets the fully-qualified logical identifier for this action.
        /// </summary>
        public string ActionID
        {
            get { return _actionID; }
        }

        /// <summary>
        /// Gets the resource resolver associated with this action, that will be used to resolve
        /// action path and icon resources when required.
        /// </summary>
        public IResourceResolver ResourceResolver
        {
            get { return _resourceResolver; }
        }

        /// <summary>
        /// Gets or sets the menu or toolbar path for this action.
        /// </summary>
        public ActionPath Path
        {
            get { return _path; }
            set { _path = value; }
        }

        /// <summary>
        /// Gets or sets the group hint for this action.
        /// </summary>
        /// <remarks>
        /// The GroupHint for an action must not be null.  If an action has no groupHint,
        /// the GroupHint should be "" (default).
        /// </remarks>
        public GroupHint GroupHint
		{
			get
			{
				if (_groupHint == null)
					_groupHint = new GroupHint("");

				return _groupHint;
			}
			set 
			{
				_groupHint = value;

				if (_groupHint == null)
					_groupHint = new GroupHint("");
			}
		}

        /// <summary>
        /// Gets the icon that the action presents in the UI.
        /// </summary>
        public IconSet IconSet
        {
            get { return _iconSet; }
            set
            {
				if (_iconSet == value)
					return;

            	_iconSet = value;
				EventsHelper.Fire(_iconSetChanged, this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the label that the action presents in the UI.
        /// </summary>
        public string Label
        {
            get { return _label; }
            set
            {
                if (value != _label)
                {
                    _label = value;
                    EventsHelper.Fire(_labelChanged, this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the tooltip that the action presents in the UI.
        /// </summary>
        public string Tooltip
        {
            get { return _tooltip; }
            set
            {
                if (value != _tooltip)
                {
                    _tooltip = value;
                    EventsHelper.Fire(_tooltipChanged, this, EventArgs.Empty);
                }
            }
		}

        /// <summary>
        /// Gets the enablement state that the action presents in the UI.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (value != _enabled)
                {
                    _enabled = value;
                    EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the visibility state that the action presents in the UI.
        /// </summary>
        public bool Visible
		{
            get
            {
                return _visible;
            }
            set
            {
                if (value != _visible)
                {
                    _visible = value;
                    EventsHelper.Fire(_visibleChanged, this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the action is 'persistent'.
        /// </summary>
        /// <remarks>
        /// Actions created via the Action attributes are considered persistent and are
        /// committed to the <see cref="ActionModelSettings"/>,
        /// otherwise they are considered generated and they are not committed.
        /// </remarks>
        public bool Persistent
		{
			get { return _persistent; }
			set { _persistent = value; }
		}

        /// <summary>
        /// Occurs when the <see cref="IAction.Enabled"/> property of this action changes.
        /// </summary>
        public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

        /// <summary>
        /// Occurs when the <see cref="IAction.Visible"/> property of this action changes.
        /// </summary>
        public event EventHandler VisibleChanged
		{
            add { _visibleChanged += value; }
            remove { _visibleChanged -= value; }
        }

        /// <summary>
        /// Occurs when the <see cref="IAction.Label"/> property of this action changes.
        /// </summary>
        public event EventHandler LabelChanged
		{
            add { _labelChanged += value; }
            remove { _labelChanged -= value; }
        }

        /// <summary>
        /// Occurs when the <see cref="IAction.Tooltip"/> property of this action changes.
        /// </summary>
        public event EventHandler TooltipChanged
		{
            add { _tooltipChanged += value; }
            remove { _tooltipChanged -= value; }
        }

		/// <summary>
		/// Occurs when the <see cref="IAction.IconSet"/> property of this action changes.
		/// </summary>
		public event EventHandler IconSetChanged
		{
			add { _iconSetChanged += value; }
			remove { _iconSetChanged -= value; }
		}
		
		/// <summary>
        /// Gets a value indicating whether this action is permissible.
        /// </summary>
        /// <remarks>
        /// In addition to the <see cref="IAction.Visible"/> and <see cref="IAction.Enabled"/> properties, the view
        /// will use this property to control whether the action can be invoked.  Typically
        /// this property is implemented to indicate whether the current user has permission
        /// to execute the action.
        /// </remarks>
        public bool Permissible
        {
            get
            {
                // no permission spec, so assume this action is not protected at all
                if (_permissionSpec == null)
                    return true;

                // test this action against the permission spec
                return _permissionSpec.Test(this).Success;
            }
        }
		
		#endregion


    }
}
