using System;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Identifies a service contract as being part of the Enterprise core service layer.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
	public class EnterpriseCoreServiceAttribute : Attribute
    {
    }
}
