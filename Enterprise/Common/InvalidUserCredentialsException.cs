using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
	[Serializable]
	public class InvalidUserCredentialsException : Exception
	{
        public InvalidUserCredentialsException()
        {
        }

		public InvalidUserCredentialsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
	}
}
