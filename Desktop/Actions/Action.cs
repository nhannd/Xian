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

        private string _tooltip;
        private IconSet _iconSet;
        private string _label;

        private IObservablePropertyBinding<bool> _enabledPropertyBinding;
		private IObservablePropertyBinding<bool> _visiblePropertyBinding;

		private IObservablePropertyBinding<string> _labelPropertyBinding;
		private IObservablePropertyBinding<string> _tooltipPropertyBinding;

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
        }

        /// <summary>
        /// Sets the observable property that this action monitors for its enablement state
        /// </summary>
        /// <param name="enabledPropertyBinding">The property to monitor</param>
        public void SetEnabledObservable(IObservablePropertyBinding<bool> enabledPropertyBinding)
        {
            _enabledPropertyBinding = enabledPropertyBinding;
        }

		public void SetVisibleObservable(IObservablePropertyBinding<bool> visiblePropertyBinding)
		{
			_visiblePropertyBinding = visiblePropertyBinding;
		}

		public void SetLabelObservable(IObservablePropertyBinding<string> labelPropertyBinding)
		{
			_labelPropertyBinding = labelPropertyBinding;
			_label = _labelPropertyBinding.PropertyValue;
		}

		public void SetTooltipObservable(IObservablePropertyBinding<string> tooltipPropertyBinding)
		{
			_tooltipPropertyBinding = tooltipPropertyBinding;
			_tooltip = _tooltipPropertyBinding.PropertyValue;
		}

		public void SetDefaultLabel(string label)
		{
			if (_labelPropertyBinding != null)
				return;

			_label = label;
		}

		public void SetDefaultTooltip(string tooltip)
		{
			if (_tooltipPropertyBinding != null)
				return;

			_tooltip = tooltip;
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

        public string Label
        {
            get 
			{
				return _labelPropertyBinding == null ? _label : _labelPropertyBinding.PropertyValue;
			}
        }

        public string Tooltip
        {
			get
			{
				return _tooltipPropertyBinding == null ? _tooltip : _tooltipPropertyBinding.PropertyValue;
			}
		}

        public IconSet IconSet
        {
            get { return _iconSet; }
            set { _iconSet = value; }
        }
        
        public bool Enabled
        {
            get
            {
                return _enabledPropertyBinding == null ? true // smart default
                    : _enabledPropertyBinding.PropertyValue;
            }
        }

		public bool Visible
		{
			get
			{
				return _visiblePropertyBinding == null ? true // smart default
					:  _visiblePropertyBinding.PropertyValue;
			}
		}
		
		public event EventHandler EnabledChanged
        {
            add
            {
                if (_enabledPropertyBinding != null)
                    _enabledPropertyBinding.PropertyChanged += value;
            }
            remove
            {
                if (_enabledPropertyBinding != null)
                    _enabledPropertyBinding.PropertyChanged -= value;
            }
        }

		public event EventHandler VisibleChanged
		{
			add
			{
				if (_visiblePropertyBinding != null)
					_visiblePropertyBinding.PropertyChanged += value;
			}
			remove
			{
				if (_visiblePropertyBinding != null)
					_visiblePropertyBinding.PropertyChanged -= value;
			}
		}

		public event EventHandler LabelChanged
		{
			add
			{
				if (_labelPropertyBinding != null)
					_labelPropertyBinding.PropertyChanged += value;
			}
			remove
			{
				if (_labelPropertyBinding != null)
					_labelPropertyBinding.PropertyChanged -= value;
			}
		}

		public event EventHandler TooltipChanged
		{
			add
			{
				if (_tooltipPropertyBinding != null)
					_tooltipPropertyBinding.PropertyChanged += value;
			}
			remove
			{
				if (_tooltipPropertyBinding != null)
					_tooltipPropertyBinding.PropertyChanged -= value;
			}
		}

		
		#endregion

    }
}
