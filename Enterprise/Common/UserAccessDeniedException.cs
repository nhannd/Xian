#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
