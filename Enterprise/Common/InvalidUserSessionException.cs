using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
	[Serializable]
	public class InvalidUserSessionException : Exception
	{
		public InvalidUserSessionException()
		{
		}

		public InvalidUserSessionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
