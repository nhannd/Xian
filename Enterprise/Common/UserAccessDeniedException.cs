using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Indicates that the user was denied access to the system.
	/// </summary>
	[Serializable]
	public class UserAccessDeniedException : Exception
	{
        public UserAccessDeniedException()
			: base(SR.ExceptionUserAccessDenied)
        {
        }

		public UserAccessDeniedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
	}
}
