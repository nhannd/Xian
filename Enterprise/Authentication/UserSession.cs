using System;
using System.Collections;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Authentication {


    /// <summary>
    /// UserSession entity
    /// </summary>
	public partial class UserSession : ClearCanvas.Enterprise.Core.Entity
	{
        public virtual SessionToken GetToken()
        {
            return new SessionToken(_sessionId, _expiryTime);
        }

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}