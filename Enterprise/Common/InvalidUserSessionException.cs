using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Indicates that a user session is invalid or has expired.
	/// </summary>
	[Serializable]
	public class InvalidUserSessionException : Exception
	{
		public InvalidUserSessionException()
			: base(SR.ExceptionInvalidUserSession)
		{
		}

		public InvalidUserSessionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
