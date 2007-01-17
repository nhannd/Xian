using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Declares a 'group hint' for an action.  Group Hints are used to determine
	/// as appropriate a place as possible to place an action in the <see cref="ActionModelStore"/>.
	/// </summary>
	public class GroupHintAttribute : ActionDecoratorAttribute
	{
		private string _groupHint;

		public GroupHintAttribute(string actionID, string groupHint)
			:base(actionID)
		{
			if (groupHint == null)
				groupHint = "";

			_groupHint = groupHint;
		}

        public override void Apply(IActionBuildingContext builder)
		{
			builder.Action.GroupHint = new GroupHint(_groupHint);
		}
	}
}
