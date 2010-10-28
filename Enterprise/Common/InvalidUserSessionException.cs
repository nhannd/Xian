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
