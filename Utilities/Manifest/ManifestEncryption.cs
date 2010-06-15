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
