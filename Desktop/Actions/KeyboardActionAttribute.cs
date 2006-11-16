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

		internal override void Apply(IActionBuilder builder)
		{
			builder.Apply(this);
		}
	}
}
