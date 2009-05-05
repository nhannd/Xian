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
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;


namespace ClearCanvas.Enterprise.Authentication {

    // This class is based on code found here:
    // http://msdn.microsoft.com/msdnmag/issues/03/08/SecurityBriefs/

    /// <summary>
    /// Password component
    /// </summary>
	public partial class Password
	{
        private const int saltLength = 6;

        /// <summary>
        /// Creates a new <see cref="Password"/> object from the specified clear-text password string,
        /// and assigns the specified expiry time.
        /// </summary>
        /// <param name="clearTextPassword"></param>
        /// <param name="expiryTime"></param>
        /// <returns></returns>
        public static Password CreatePassword(string clearTextPassword, DateTime? expiryTime)
        {
            Platform.CheckForNullReference(clearTextPassword, "clearTextPassword");

            AuthenticationSettings settings = new AuthenticationSettings();
            if(!Regex.Match(clearTextPassword, settings.ValidPasswordRegex).Success)
                throw new EntityValidationException(settings.ValidPasswordMessage);

            return CreatePasswordHelper(clearTextPassword, expiryTime);
        }

        /// <summary>
        /// Creates a new <see cref="Password"/> object that represents the default temporary
        /// password defined in <see cref="AuthenticationSettings"/> and expires immediately.
        /// </summary>
        /// <returns></returns>
        public static Password CreateTemporaryPassword()
        {
            AuthenticationSettings settings = new AuthenticationSettings();
            return CreatePasswordHelper(settings.DefaultTemporaryPassword, Platform.Time);
        }

        /// <summary>
        /// Verifies whether the specified password string matches this <see cref="Password"/> object.
        /// Does not consider the <see cref="ExpiryTime"/>.
        /// </summary>
        /// <param name="clearTextPassword"></param>
        /// <returns></returns>
        public bool Verify(string clearTextPassword)
        {
            Platform.CheckForNullReference(clearTextPassword, "clearTextPassword");

            string h = CalculateHash(_salt, clearTextPassword);
            return _saltedHash.Equals(h);
        }

        /// <summary>
        /// Gets a value indicating if the <see cref="ExpiryTime"/> has been exceeded
        /// with respect to the current time.
        /// </summary>
        public bool IsExpired(DateTime currentTime)
        {
			return _expiryTime.HasValue && _expiryTime < currentTime;
        }

        #region Utilities

        private static Password CreatePasswordHelper(string clearTextPassword, DateTime? expiryTime)
        {
            string salt = CreateSalt();
            string hash = CalculateHash(salt, clearTextPassword);
            return new Password(salt, hash, expiryTime);
        }

        private static string CreateSalt()
        {
            byte[] r = CreateRandomBytes(saltLength);
            return Convert.ToBase64String(r);
        }

        private static byte[] CreateRandomBytes(int len)
        {
            byte[] r = new byte[len];
            new RNGCryptoServiceProvider().GetBytes(r);
            return r;
        }

        private static string CalculateHash(string salt, string password)
        {
            byte[] data = ToByteArray(salt + password);
            byte[] hash = CalculateHash(data);
            return Convert.ToBase64String(hash);
        }

        private static byte[] CalculateHash(byte[] data)
        {
            return new SHA1CryptoServiceProvider().ComputeHash(data);
        }

        private static byte[] ToByteArray(string s)
        {
            return System.Text.Encoding.UTF8.GetBytes(s);
        }

        #endregion


        /// <summary>
        /// Not used.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}