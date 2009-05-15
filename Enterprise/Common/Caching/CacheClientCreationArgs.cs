using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Caching
{
	public class CacheClientCreationArgs
	{
		private string _region;
		private TimeSpan _expirationTime;


		public CacheClientCreationArgs(string region, TimeSpan expirationTime)
		{
			_region = region;
			_expirationTime = expirationTime;
		}

		public string Region
		{
			get { return _region; }
			set { _region = value; }
		}

		public TimeSpan ExpirationTime
		{
			get { return _expirationTime; }
			set { _expirationTime = value; }
		}
	}
}
