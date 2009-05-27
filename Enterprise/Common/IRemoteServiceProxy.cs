using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Defines an interface to an object that acts a proxy to a remote service.
	/// </summary>
	public interface IRemoteServiceProxy
	{
		/// <summary>
		/// Gets the channel object.
		/// </summary>
		/// <returns></returns>
		object GetChannel();
	}
}
