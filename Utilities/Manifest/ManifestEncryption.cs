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
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace ClearCanvas.Utilities.Manifest
{
    internal static class ManifestEncryption
    {
        public static void Encrypt(XmlDocument doc, string elementToEncryptName)
        {
          /*  CspParameters cspParams = new CspParameters
                                          {
                                              KeyContainerName = "XML_ENC_RSA_KEY"
                                          };
            RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);
            // Create a 256 bit Rijndael key.
            RijndaelManaged sessionKey = new RijndaelManaged();
            sessionKey.KeySize = 256;

            
            XmlElement elementToEncrypt = doc.GetElementsByTagName(elementToEncryptName)[0] as XmlElement;

            if (elementToEncrypt == null)
                throw new XmlException("The specified element was not found: " + elementToEncryptName);

            EncryptedXml encXml = new EncryptedXml();
            byte[] encryptedBytes = encXml.EncryptData(elementToEncrypt, sessionKey, false);

            EncryptedData ed = new EncryptedData();
            ed.Type = EncryptedXml.XmlEncElementUrl;
            ed.Id = "Encrypted" + elementToEncryptName;
            ed.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url);
            */
        }
    }
}
