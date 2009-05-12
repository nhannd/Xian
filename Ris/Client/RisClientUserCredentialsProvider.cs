using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Implementation of <see cref="IUserCredentialsProvider"/> for RIS client.
	/// </summary>
	//NOTE: this class is loaded dynamically - do not remove it even though there are no direct references to it
	internal class RisClientUserCredentialsProvider : IUserCredentialsProvider
	{
		#region IUserCredentialsProvider Members

		/// <summary>
		/// Gets the user-name.
		/// </summary>
		public string UserName
		{
			get { return LoginSession.Current.UserName; }
		}

		/// <summary>
		/// Gets the session token ID.
		/// </summary>
		public string SessionTokenId
		{
			get { return LoginSession.Current.SessionToken; }
		}

		#endregion
	}
}
