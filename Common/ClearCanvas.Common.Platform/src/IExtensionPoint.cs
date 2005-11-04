using System;

namespace ClearCanvas.Common
{
	/// <summary>
	/// An interface that marks an interface or class as an extension point.
	/// </summary>
	/// <remarks>
	/// If an interface or class defined in a host plugin can be implemented or
	/// subclassed by a conrete class in an extension plugin, it must be marked
	/// with the <b>IExtensionPoint</b> interface.  In the future, instead of
	/// using a marker interface, an extension point interface or class may
	/// be marked with an extension point attribute, as that seems to be the natural
	/// C# idiom to use.
	/// </remarks>
	public interface IExtensionPoint
	{
	}
}
