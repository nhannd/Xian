using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Threading;

namespace ClearCanvas.Enterprise.Core.ServiceModel
{
	public class WcfServiceUserCredentialsProvider : IUserCredentialsProvider
	{
		#region IUserCredentialsProvider Members

		public string UserName
		{
			get { return GetThreadCredentials().UserName; }
		}

		public string SessionTokenId
		{
			get { return GetThreadCredentials().SessionTokenId; }
		}

		#endregion

		private static IUserCredentialsProvider GetThreadCredentials()
		{
			IUserCredentialsProvider provider = Thread.CurrentPrincipal as IUserCredentialsProvider;
			if(provider == null)
				throw new NotSupportedException(""); //TODO elaborate

			return provider;
		}
	}
}
