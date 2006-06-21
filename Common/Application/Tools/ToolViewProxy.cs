using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Common.Application.Tools
{
    /// <summary>
    /// Undocumented - API subject to change
    /// </summary>
    public class ToolViewProxy
    {
        private ITool _tool;
        private IExtensionPoint _viewExtensionPoint;
        private string _title;
        private ToolViewDisplayHint _displayHint;
        private IToolView _view;
        private IObservablePropertyBinding<bool> _viewActivePropertyBinding;

        private event EventHandler<ActivationChangedEventArgs> _toolViewActivationChanged;

        internal ToolViewProxy(ITool tool, IExtensionPoint viewExtensionPoint,
            string title, ToolViewDisplayHint displayHint, IObservablePropertyBinding<bool> viewActivePropertyBinding)
        {
            _tool = tool;
            _viewExtensionPoint = viewExtensionPoint;
            _title = title;
            _displayHint = displayHint;
            _viewActivePropertyBinding = viewActivePropertyBinding;

            _viewActivePropertyBinding.PropertyChanged += delegate(object sender, EventArgs args)
            {
                FireToolViewActivationChanged(this.Active);
            };
        }

        public bool Active
        {
            get
            {
                return _viewActivePropertyBinding.PropertyValue;
            }
            set
            {
                _viewActivePropertyBinding.PropertyValue = value;
            }
        }

        public event EventHandler<ActivationChangedEventArgs> ActivationChanged
        {
            add { _toolViewActivationChanged += value; }
            remove { _toolViewActivationChanged -= value; }
        }

        public IToolView View
        {
            get
            {
                if (_view == null)
                {
                    _view = (IToolView)ViewFactory.CreateView(_viewExtensionPoint);
                    _view.SetTool(_tool);
                }
                return _view;
            }
        }

        public string Title
        {
            get { return _title; }
        }

        public ToolViewDisplayHint DisplayHint
        {
            get { return _displayHint; }
        }

        private void FireToolViewActivationChanged(bool activate)
        {
            ActivationChangedEventArgs args = new ActivationChangedEventArgs(activate);
            EventsHelper.Fire(_toolViewActivationChanged, this, args);
        }
    }
}
