#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// Class for Encryption/Decription of the ProductSettings in the app.config.
    /// </summary>
    internal static class ProductSettingsEncryption
    {
        /// <summary>
        /// Decrypt a Product Settings string.
        /// </summary>
        /// <param name="string">The string to decrypt.</param>
        /// <returns>The decrypted string.</returns>
        public static string Decrypt(string @string)
        {
            if (String.IsNullOrEmpty(@string))
                return @string;

            string result;
            try
            {
                byte[] bytes = Convert.FromBase64String(@string);
                using (MemoryStream dataStream = new MemoryStream(bytes))
                {
                    RC2CryptoServiceProvider cryptoService = new RC2CryptoServiceProvider
                    {
                        Key = Encoding.UTF8.GetBytes("ClearCanvas"),
                        IV = Encoding.UTF8.GetBytes("IsSoCool"),
                        UseSalt = false
                    };
                    using (CryptoStream cryptoStream = new CryptoStream(dataStream, cryptoService.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream, Encoding.UTF8))
                        {
                            result = reader.ReadToEnd();
                            reader.Close();
                        }
                        cryptoStream.Close();
                    }
                    dataStream.Close();
                }
            }
            catch (Exception)
            {
                result = string.Empty;
            }
            return result;
        }
    }
}
