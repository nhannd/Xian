using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Threading;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Implementation of <see cref="IUserCredentialsProvider"/> that obtains credentials from
    /// the <see cref="Thread.CurrentPrincipal"/>, assuming that the current principal
    /// implements the <see cref="IUserCredentialsProvider"/> interface.
    /// </summary>
	public class DefaultUserCredentialsProvider : IUserCredentialsProvider
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
                throw new InvalidOperationException("Thread.CurrentPrincipal value does not implement IUserCredentialsProvider.");

			return provider;
		}
	}
}
