using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Defines an interface that allows an object to write out its property values.
	/// </summary>
	public interface IObjectWriter
	{
		/// <summary>
		/// Writes the specified property name and value.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		void WriteProperty(string name, object value);
	}
}
