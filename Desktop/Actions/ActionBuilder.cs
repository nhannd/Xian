using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Default implementation of <see cref="IActionBuilder"/>.  Used by the framework to construct
    /// actions from action attributes.
    /// </summary>
    internal class ActionBuilder : IActionBuilder
    {
        private string _actionID;
        private object _actionTarget;
        private Action _action;
        private ResourceResolver _resolver;

        internal ActionBuilder(string actionID, object actionTarget)
        {
            _actionID = actionID;
            _actionTarget = actionTarget;

            _resolver = new ActionResourceResolver(_actionTarget);
        }

        public string ActionID
        {
            get { return _actionID; }
        }

        public Action Action
        {
            get { return _action; }
        }

        public void Apply(ButtonActionAttribute a)
        {
            // assert _action == null
			ActionPath path = new ActionPath(a.Path, _resolver);
			_action = new ButtonAction(_actionID, path, a.Flags, _resolver);
			((ClickAction)_action).SetKeyStroke(a.KeyStroke);
			_action.Label = path.LastSegment.LocalizedText;
		}

        public void Apply(MenuActionAttribute a)
        {
            // assert _action == null
            ActionPath path = new ActionPath(a.Path, _resolver);
            _action = new MenuAction(_actionID, path, a.Flags, _resolver);
			((ClickAction)_action).SetKeyStroke(a.KeyStroke);
            _action.Label = path.LastSegment.LocalizedText;
        }

		public void Apply(KeyboardActionAttribute a)
		{
			// assert _action == null
			ActionPath path = new ActionPath(a.Path, _resolver);
			_action = new KeyboardAction(_actionID, path, a.Flags, _resolver);
			((ClickAction)_action).SetKeyStroke(a.KeyStroke);
            _action.Label = path.LastSegment.LocalizedText;
        }

		public void Apply(ClickHandlerAttribute a)
        {
            // assert _action != null && is ClickAction
            ClickAction clickAction = (ClickAction)_action;

            // check that the method exists, etc
            ValidateClickHandler(a.HandlerMethodName);

            try
            {
                ClickHandlerDelegate clickHandler = 
                    (ClickHandlerDelegate)Delegate.CreateDelegate(typeof(ClickHandlerDelegate), _actionTarget, a.HandlerMethodName);
                clickAction.SetClickHandler(clickHandler);
            }
            catch (Exception e)
            {
                //TODO unable to bind to the specified method, for whatever reason
                throw e;
            }
        }

        public void Apply(IconSetAttribute a)
        {
            // assert _action != null

            // note that the IconSetAttribute may appear more than once to provide
            // multiple icon schemes
            // it is the responsibility of the Action to decide whether it will
            // act upon the SetIcon request
            // typically the Action should only act on the request if is the first
            // request, or if the IconScheme
            // of the supplied IconSet more closely matches whatever scheme is currently in use
            _action.IconSet = a.IconSet;
        }

        public void Apply(TooltipAttribute a)
        {
            // assert _action != null
            _action.Tooltip = _resolver.LocalizeString(a.TooltipText);
        }

		public void Apply(LabelValueObserverAttribute a)
		{
			// check that property, event exist, etc.
			ValidateProperty(a.PropertyName, typeof(string));
			ValidateEvent(a.ChangeEventName);

            IObservablePropertyBinding<string> toolBinding = new DynamicObservablePropertyBinding<string>(_actionTarget, a.PropertyName, a.ChangeEventName);
            IObservablePropertyBinding<string> actionBinding = new DynamicObservablePropertyBinding<string>(_action, "Label", "LabelChanged");

            ObservablePropertyCoupler<string>.Couple(toolBinding, actionBinding);
		}
		
		public void Apply(TooltipValueObserverAttribute a)
		{
			// check that property, event exist, etc.
			ValidateProperty(a.PropertyName, typeof(string));
			ValidateEvent(a.ChangeEventName);

            IObservablePropertyBinding<string> toolBinding = new DynamicObservablePropertyBinding<string>(_actionTarget, a.PropertyName, a.ChangeEventName);
            IObservablePropertyBinding<string> actionBinding = new DynamicObservablePropertyBinding<string>(_action, "Tooltip", "TooltipChanged");

            ObservablePropertyCoupler<string>.Couple(toolBinding, actionBinding);
        }

		public void Apply(VisibleStateObserverAttribute a)
		{
			// check that property, event exist, etc.
			ValidateProperty(a.PropertyName, typeof(bool));
			ValidateEvent(a.ChangeEventName);

            IObservablePropertyBinding<bool> toolBinding = new DynamicObservablePropertyBinding<bool>(_actionTarget, a.PropertyName, a.ChangeEventName);
            IObservablePropertyBinding<bool> actionBinding = new DynamicObservablePropertyBinding<bool>(_action, "Visible", "VisibleChanged");

            ObservablePropertyCoupler<bool>.Couple(toolBinding, actionBinding);
        }

		public void Apply(CheckedStateObserverAttribute a)
        {
            // assert _action != null && is ClickAction
            ClickAction clickAction = (ClickAction)_action;

            // check that property, event exist, etc.
			ValidateProperty(a.PropertyName, typeof(bool));
            ValidateEvent(a.ChangeEventName);

            IObservablePropertyBinding<bool> toolBinding = new DynamicObservablePropertyBinding<bool>(_actionTarget, a.PropertyName, a.ChangeEventName);
            IObservablePropertyBinding<bool> actionBinding = new DynamicObservablePropertyBinding<bool>(_action, "Checked", "CheckedChanged");

            ObservablePropertyCoupler<bool>.Couple(toolBinding, actionBinding);
        }

        public void Apply(EnabledStateObserverAttribute a)
        {
            // assert _action != null 

            // check that property, event exist, etc.
			ValidateProperty(a.PropertyName, typeof(bool));
            ValidateEvent(a.ChangeEventName);

            IObservablePropertyBinding<bool> toolBinding = new DynamicObservablePropertyBinding<bool>(_actionTarget, a.PropertyName, a.ChangeEventName);
            IObservablePropertyBinding<bool> actionBinding = new DynamicObservablePropertyBinding<bool>(_action, "Enabled", "EnabledChanged");

            ObservablePropertyCoupler<bool>.Couple(toolBinding, actionBinding);
        }

        private void ValidateClickHandler(string methodName)
        {
			MethodInfo info = _actionTarget.GetType().GetMethod(
				methodName, BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,
				null,
				Type.EmptyTypes,
				null);

			if (info == null)
            {
                throw new ActionBuilderException(
					string.Format(SR.ExceptionActionBuilderMethodDoesNotExist, methodName, _actionTarget.GetType().FullName));
            }
        }

        private void ValidateEvent(string eventName)
        {
            EventInfo info = _actionTarget.GetType().GetEvent(eventName);
            if (info == null)
            {
                throw new ActionBuilderException(
					string.Format(SR.ExceptionActionBuilderEventDoesNotExist, eventName, _actionTarget.GetType().FullName));
            }
        }

        private void ValidateProperty(string propertyName, Type type)
        {
			PropertyInfo info = _actionTarget.GetType().GetProperty(propertyName, type);
            if (info == null)
            {
                throw new ActionBuilderException(
					string.Format(SR.ExceptionActionBuilderPropertyDoesNotExist, propertyName, _actionTarget.GetType().FullName));
            }

            MethodInfo getter = info.GetGetMethod();
            if (getter == null)
            {
                throw new ActionBuilderException(
					string.Format(SR.ExceptionActionBuilderPropertyDoesNotHavePublicGetMethod, propertyName, _actionTarget.GetType().FullName));
            }
        }
    }
}
