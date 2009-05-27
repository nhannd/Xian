using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Caching
{
	public class CacheException : Exception
	{
		public CacheException(string message)
			:base(message)
		{
		}
	}
}
