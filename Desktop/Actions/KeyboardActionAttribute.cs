using System;

namespace ClearCanvas.Desktop.Actions
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class KeyboardActionAttribute : ClickActionAttribute
	{
		public KeyboardActionAttribute(string actionID, string pathHint)
			: base(actionID, pathHint)
		{
		}

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
