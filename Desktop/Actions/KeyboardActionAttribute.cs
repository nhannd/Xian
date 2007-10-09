using System;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Declares a keyboard action with the specifed action identifier and path hint.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class KeyboardActionAttribute : ClickActionAttribute
	{
        /// <summary>
        /// Declares a keyboard action with the specified action ID and path hint.
        /// </summary>
        /// <param name="actionID"></param>
        /// <param name="pathHint"></param>
		public KeyboardActionAttribute(string actionID, string pathHint)
			: base(actionID, pathHint)
		{
		}

        /// <summary>
        /// Declares a keyboard action with the specified action ID, path hint and click-handler.
        /// </summary>
        /// <param name="actionID"></param>
        /// <param name="pathHint"></param>
        /// <param name="clickHandler"></param>
		public KeyboardActionAttribute(string actionID, string pathHint, string clickHandler)
			: base(actionID, pathHint, clickHandler)
		{
		}
		
		public override void Apply(IActionBuildingContext builder)
		{
            base.Apply(builder);

            // don't need to persist keyboard actions
            builder.Action.Persistent = false;
        }

        protected override ClickAction CreateAction(string actionID, ActionPath path, ClickActionFlags flags, ClearCanvas.Common.Utilities.IResourceResolver resolver)
        {
            return new KeyboardAction(actionID, path, flags, resolver);
        }
    }
}
