using System;

namespace ClearCanvas.Ris.Application.Common
{
	/// <summary>
	/// Identifies a service contract as being part of the RIS application.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class RisApplicationServiceAttribute : Attribute
    {
    }
}
