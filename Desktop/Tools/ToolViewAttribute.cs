using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Tools
{
    /// <summary>
    /// Declares a tool view for a tool class.
    /// </summary>
    /// <remarks>
    /// Tool views allow tools to contribute user-interface components to be integrated into the main
    /// workstation view.  The activation (e.g. visibility) of the view is bound to a boolean property
    /// of the tool, allowing the tool itself to control when the view is active and when it is not.  Other
    /// behaviours of the view can be customized through the <see cref="ToolViewDisplayHint"/> flags.  The
    /// tool must define an extension point for the view, so that the framework can instantiate the view
    /// class as an extension.  The extension point must be defined on the <see cref="IToolView"/> interface,
    /// and the view extension class must implement this interface.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ToolViewAttribute : ToolAttribute
    {
        private Type _viewExtensionPoint;
        private string _title;
        private ToolViewDisplayHint _displayHint;
        private string _activatedProperty;
        private string _activatedChangeEvent;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="viewExtensionPoint">The extension point that will create the view class</param>
        /// <param name="title">The title that should be applied to the view</param>
        /// <param name="displayHint">Flags that control the behaviour of the view</param>
        /// <param name="activatedProperty">The name of a public boolean get/set property to bind the activation of the view to</param>
        /// <param name="activatedChangeEvent">The name of the property change notification event for the activation property</param>
        public ToolViewAttribute(Type viewExtensionPoint, string title, ToolViewDisplayHint displayHint, string activatedProperty, string activatedChangeEvent)
        {
            _viewExtensionPoint = viewExtensionPoint;
            _title = title;
            _displayHint = displayHint;
            _activatedProperty = activatedProperty;
            _activatedChangeEvent = activatedChangeEvent;
        }

        /// <summary>
        /// The extension point that will create the view class
        /// </summary>
        public Type ViewExtensionPoint
        {
            get { return _viewExtensionPoint; }
        }

        /// <summary>
        /// The title that should be applied to the view
        /// </summary>
        public string Title
        {
            get { return _title; }
        }

        /// <summary>
        /// Flags that control the behaviour of the view
        /// </summary>
        public ToolViewDisplayHint DisplayHint
        {
            get { return _displayHint; }
        }

        /// <summary>
        /// The name of the property to bind the activation of the view to
        /// </summary>
        public string ActivatedPropertyName
        {
            get { return _activatedProperty; }
        }

        /// <summary>
        /// The name of the property change notification event for the activation property
        /// </summary>
        public string ActivatedChangeEventName
        {
            get { return _activatedChangeEvent; }
        }

        public override void Apply(ToolBuilder builder)
        {
            builder.Apply(this);
        }

    }
}
