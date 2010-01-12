using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Scripting
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ScriptEngineOptionsAttribute : Attribute
	{
		public bool Singleton { get; set; }

		public ScriptEngineThreadingMode ThreadingMode { get; set; }
	}
}
