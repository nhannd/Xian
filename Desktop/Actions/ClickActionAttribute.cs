using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Abstract base class for the set of attributes that are used to declare "click" actions.
    /// </summary>
    public abstract class ClickActionAttribute : ActionInitiatorAttribute
    {
        private string _path;
        private string _clickHandler;
        private ClickActionFlags _flags;
		private XKeys _keyStroke;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actionID">The logical action ID.</param>
        /// <param name="path">The action path.</param>
        /// <param name="clickHandler">The name of the method that will be invoked when the action is clicked.</param>
        public ClickActionAttribute(string actionID, string path, string clickHandler)
            :base(actionID)
        {
            _path = path;
            _clickHandler = clickHandler;
            _flags = ClickActionFlags.None; // default value, will override if named parameter is specified
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actionID">The logical action ID.</param>
        /// <param name="path">The action path.</param>
        public ClickActionAttribute(string actionID, string path)
            : this(actionID, path, null)
        {
        }

        /// <summary>
        /// Gets or sets the flags that customize the behaviour of the action.
        /// </summary>
        public ClickActionFlags Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        /// <summary>
        /// Gets or sets the key-stroke that should invoke the action from the keyboard.
        /// </summary>
		public XKeys KeyStroke
		{
			get { return _keyStroke; }
			set { _keyStroke = value; }
		}

        public override void Apply(IActionBuildingContext builder)
        {
            // assert _action == null
            ActionPath path = new ActionPath(this.Path, builder.ResourceResolver);
            builder.Action = CreateAction(builder.ActionID, path, this.Flags, builder.ResourceResolver);
            builder.Action.Persistent = true;
            ((ClickAction)builder.Action).SetKeyStroke(this.KeyStroke);
            builder.Action.Label = path.LastSegment.LocalizedText;

            if (!string.IsNullOrEmpty(_clickHandler))
            {
                // check that the method exists, etc
                ValidateClickHandler(builder.ActionTarget, _clickHandler);

                ClickHandlerDelegate clickHandler =
                    (ClickHandlerDelegate)Delegate.CreateDelegate(typeof(ClickHandlerDelegate), builder.ActionTarget, _clickHandler);
                ((ClickAction)builder.Action).SetClickHandler(clickHandler);
            }
        }

        protected abstract ClickAction CreateAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resolver);

        /// <summary>
        /// The suggested location of the action in the action model.
        /// </summary>
        public string Path { get { return _path; } }

        private static void ValidateClickHandler(object target, string methodName)
        {
            MethodInfo info = target.GetType().GetMethod(
                methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                Type.EmptyTypes,
                null);

            if (info == null)
            {
                throw new ActionBuilderException(
                    string.Format(SR.ExceptionActionBuilderMethodDoesNotExist, methodName, target.GetType().FullName));
            }
        }
    }
}
