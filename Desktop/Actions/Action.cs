using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Abstract base class providing a default implementation of <see cref="IAction"/>.  Action classes should
    /// inherit this class rather than implement <see cref="IAction"/> directly.
    /// </summary>
    public abstract class Action : IAction
    {
        private string _actionID;
		
		private ActionPath _path;
        private IResourceResolver _resourceResolver;

		private GroupHint _groupHint;

        private IconSet _iconSet;

        private bool _enabled;
        private event EventHandler _enabledChanged;

        private bool _visible;
        private event EventHandler _visibleChanged;

        private string _tooltip;
        private event EventHandler _tooltipChanged;

        private string _label;
        private event EventHandler _labelChanged;

		private bool _persistent;

		/// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actionID">The logical action ID</param>
        /// <param name="path">The action path</param>
        /// <param name="resourceResolver">A resource resolver that will be used to resolve icons associated with this action</param>
        public Action(string actionID, ActionPath path, IResourceResolver resourceResolver)
        {
            _actionID = actionID;
            _path = path;
            _resourceResolver = resourceResolver;

            // smart defaults
            _enabled = true;
            _visible = true;

			_persistent = false;
        }

        #region IAction members

        public string ActionID
        {
            get { return _actionID; }
        }

        public IResourceResolver ResourceResolver
        {
            get { return _resourceResolver; }
        }

        public ActionPath Path
        {
            get { return _path; }
            set { _path = value; }
        }

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

        public IconSet IconSet
        {
            get { return _iconSet; }
            set { _iconSet = value; }
        }

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

		public bool Visible
		{
            get { return _visible; }
            set
            {
                if (value != _visible)
                {
                    _visible = value;
                    EventsHelper.Fire(_visibleChanged, this, EventArgs.Empty);
                }
            }
        }

		public bool Persistent
		{
			get { return _persistent; }
			set { _persistent = value; }
		}
		
		public event EventHandler EnabledChanged
        {
            add { _enabledChanged += value; }
            remove { _enabledChanged -= value; }
        }

		public event EventHandler VisibleChanged
		{
            add { _visibleChanged += value; }
            remove { _visibleChanged -= value; }
        }

		public event EventHandler LabelChanged
		{
            add { _labelChanged += value; }
            remove { _labelChanged -= value; }
        }

		public event EventHandler TooltipChanged
		{
            add { _tooltipChanged += value; }
            remove { _tooltipChanged -= value; }
        }

		
		#endregion

    }
}
