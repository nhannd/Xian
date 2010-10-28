#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[Serializable]
	public class SupervisorValidationException : RequestValidationException
	{
		public SupervisorValidationException()
			: base(SR.ExceptionSupervisorRequired)
		{
		}

		public SupervisorValidationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}