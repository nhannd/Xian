using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Actions
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class KeyboardActionAttribute : ClickActionAttribute
	{
		public KeyboardActionAttribute(string actionID, string pathHint)
			: base(actionID, pathHint)
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
