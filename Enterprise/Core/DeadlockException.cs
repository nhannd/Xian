#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Exception throw when an operation cannot complete because of a deadlock state.
	/// </summary>
	[Serializable]
	public class DeadlockException : Exception
	{
		public DeadlockException()
		{
		}

		public DeadlockException(Exception inner)
			: base(SR.ExceptionDeadlockDetected, inner)
		{
		}
		public DeadlockException(string message)
			: base(message)
		{
		}

		public DeadlockException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected DeadlockException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}	
}
