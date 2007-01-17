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
            // assert _action == null
            ActionPath path = new ActionPath(this.Path, builder.ResourceResolver);
            builder.Action = new KeyboardAction(builder.ActionID, path, this.Flags, builder.ResourceResolver);
            ((ClickAction)builder.Action).SetKeyStroke(this.KeyStroke);
            builder.Action.Label = path.LastSegment.LocalizedText;
        }
	}
}
