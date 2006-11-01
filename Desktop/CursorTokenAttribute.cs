using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Desktop
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class CursorTokenAttribute : Attribute
	{
		private CursorToken _token;

		public CursorTokenAttribute(CursorToken.SystemCursors systemCursor)
		{
			_token = new CursorToken(systemCursor);
		}

		public CursorTokenAttribute(string resourceName, Type resourceAssemblyType)
		{
			_token = new CursorToken(resourceName, resourceAssemblyType.Assembly);
		}

		public CursorToken CursorToken
		{
			get { return _token; }
		}
	}
}
