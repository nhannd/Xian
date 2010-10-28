#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
