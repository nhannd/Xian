#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
using System.Xml;
using System.Xml.Serialization;

namespace ClearCanvas.Utilities.ProductSettingsGenerator
{
	[Serializable]
	public class EncryptedProductSettings
	{
		public EncryptedProductSettings() {}

		public EncryptedProductSettings(string component, string product, string edition, string release, string version, string suffix, string copyright, string license)
		{
			Product = ScrambleString(product);
			Component = ScrambleString(component);
			Edition = ScrambleString(edition);
			Version = ScrambleString(version);
			VersionSuffix = ScrambleString("*" + suffix);
			Copyright = ScrambleString(copyright);
			License = ScrambleString(license);
			Release = ScrambleString("*" + release);
		}

		public byte[] Component { get; set; }
		public byte[] Product { get; set; }
		public byte[] Edition { get; set; }
		public byte[] Release { get; set; }
		public byte[] Version { get; set; }
		public byte[] VersionSuffix { get; set; }
		public byte[] Copyright { get; set; }
		public byte[] License { get; set; }

		public void Save()
		{
			Save("ProductSettings.xml");
		}

		public void Save(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName");

			XmlSerializer serializer = new XmlSerializer(typeof (EncryptedProductSettings));

			XmlWriterSettings writerSettings = new XmlWriterSettings
			                                   	{
			                                   		NewLineOnAttributes = true,
			                                   		OmitXmlDeclaration = false,
			                                   		Indent = true,
			                                   		Encoding = Encoding.UTF8,
			                                   		CheckCharacters = true
			                                   	};

			XmlWriter writer = XmlWriter.Create(fileName, writerSettings);
			serializer.Serialize(writer, this);
			writer.Close();
		}

		private byte[] ScrambleString(string @string)
		{
			byte[] encrypted = EncryptRC2(@string);
			string decrypted = DecryptRC2(encrypted);

			// perform roundtrip verification of result
			if (!string.IsNullOrEmpty(@string) && @string != decrypted)
				throw new ApplicationException("Roundtrip verification failed.");

			//the xml serializer will automatically convert the byte array to base64
			return encrypted;
		}

		private static byte[] EncryptRC2(string @string)
		{
			if (String.IsNullOrEmpty(@string))
				return null;

			RC2CryptoServiceProvider cryptoService = new RC2CryptoServiceProvider();
			cryptoService.Key = Encoding.UTF8.GetBytes("ClearCanvas");
			cryptoService.IV = Encoding.UTF8.GetBytes("IsSoCool");
			cryptoService.UseSalt = false;

			ICryptoTransform encryptor = cryptoService.CreateEncryptor();

			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

			byte[] bytes = Encoding.UTF8.GetBytes(@string);

			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.FlushFinalBlock();
			cryptoStream.Flush();
			cryptoStream.Close();
			memoryStream.Close();

			return memoryStream.ToArray();
		}

		private static string DecryptRC2(byte[] bytes)
		{
			if (bytes == null || bytes.Length == 0)
				return null;

			RC2CryptoServiceProvider cryptoService = new RC2CryptoServiceProvider();
			cryptoService.Key = Encoding.UTF8.GetBytes("ClearCanvas");
			cryptoService.IV = Encoding.UTF8.GetBytes("IsSoCool");
			cryptoService.UseSalt = false;

			ICryptoTransform decryptor = cryptoService.CreateDecryptor();

			MemoryStream memoryStream = new MemoryStream(bytes);
			CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

			StreamReader reader = new StreamReader(cryptoStream);
			string decrypted = reader.ReadToEnd();
			reader.Close();

			return decrypted;
		}
	}
}