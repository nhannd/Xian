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