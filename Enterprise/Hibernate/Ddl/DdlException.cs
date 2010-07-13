using System;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	public class DdlException : Exception
	{
		public DdlException(string message, Exception inner)
			:base(message, inner)
		{
		}
	}
}
