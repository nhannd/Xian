#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;

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
			var provider = Thread.CurrentPrincipal as IUserCredentialsProvider;
			if(provider == null)
                throw new InvalidOperationException("Thread.CurrentPrincipal value does not implement IUserCredentialsProvider.");

			return provider;
		}
	}
}
