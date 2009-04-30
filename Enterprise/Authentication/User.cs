#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;
using ClearCanvas.Enterprise.Core;
using Iesi.Collections.Generic;
using ClearCanvas.Common;


namespace ClearCanvas.Enterprise.Authentication {


    /// <summary>
    /// User entity
    /// </summary>
	public partial class User : Entity
	{
        #region Public methods

        /// <summary>
        /// Creates a new user with the specified initial password.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="initialPassword"></param>
        /// <param name="authorityGroups"></param>
        /// <returns></returns>
        public static User CreateNewUser(UserInfo userInfo, Password initialPassword, ISet<AuthorityGroup> authorityGroups)
        {
            return new User(
                userInfo.UserName,
                initialPassword,
                userInfo.DisplayName,
                userInfo.ValidFrom,
                userInfo.ValidUntil,
                true, // initially enabled
                Platform.Time, // creation time
                null, // last login time
                authorityGroups,
                null  // current session
                );
        }

        /// <summary>
        /// Creates a new user, assigning the default temporary password.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static User CreateNewUser(UserInfo userInfo)
        {
            return CreateNewUser(userInfo, Password.CreateTemporaryPassword(), new HashedSet<AuthorityGroup>());
        }

        /// <summary>
        /// Changes the user's password, setting a new expiry date according to the
        /// value defined in <see cref="AuthenticationSettings.PasswordExpiryDays"/>.
        /// </summary>
        /// <param name="newPassword"></param>
        public virtual void ChangePassword(string newPassword)
        {
            AuthenticationSettings settings = new AuthenticationSettings();
            DateTime? expiryTime = Platform.Time.AddDays(settings.PasswordExpiryDays);
            _password = Authentication.Password.CreatePassword(newPassword, expiryTime);
        }

        /// <summary>
        /// Resets the user's password to the default temporary password,
        /// set to expire immediately.
        /// </summary>
        public virtual void ResetPassword()
        {
            AuthenticationSettings settings = new AuthenticationSettings();
            _password = Authentication.Password.CreateTemporaryPassword();
        }

        /// <summary>
        /// Gets a value indicating whether this account is currently active.
        /// </summary>
        public virtual bool IsActive
        {
            get
            {
                DateTime currentTime = Platform.Time;
                return _enabled
                       && (_validFrom == null || _validFrom < currentTime)
                       && (_validUntil == null || _validUntil > currentTime);
            }
        }

        #endregion

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}