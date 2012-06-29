#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace ClearCanvas.Dicom.Utilities.Xml
{
	/// <summary>
	/// Class used in conjunction with <see cref="StudyXml"/> for reading and writing an XML representation of a Study.
	/// </summary>
	public static class StudyXmlIo
	{
		public static void Write(XmlDocument theDoc, Stream theStream)
		{
		    var xmlSettings = new XmlWriterSettings
		                          {
		                              Encoding = Encoding.UTF8,
		                              ConformanceLevel = ConformanceLevel.Document,
		                              Indent = false,
		                              NewLineOnAttributes = false,
		                              CheckCharacters = true,
		                              IndentChars = string.Empty
		                          };


		    XmlWriter tw = XmlWriter.Create(theStream, xmlSettings);
			theDoc.WriteTo(tw);
			tw.Flush();
			tw.Close();
		}

        public static void Write(XmlDocument theDoc, string filename)
        {
            var xmlSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                ConformanceLevel = ConformanceLevel.Document,
                Indent = false,
                NewLineOnAttributes = false,
                CheckCharacters = true,
                IndentChars = string.Empty
            };

            XmlWriter tw = XmlWriter.Create(filename, xmlSettings);
            theDoc.WriteTo(tw);
            tw.Flush();
            tw.Close();
        }

		public static void WriteGzip(XmlDocument theDoc, Stream theStream)
		{
			var ms = new MemoryStream();
		    var xmlSettings = new XmlWriterSettings
		                          {
		                              Encoding = Encoding.UTF8,
		                              ConformanceLevel = ConformanceLevel.Document,
		                              Indent = false,
		                              NewLineOnAttributes = false,
		                              CheckCharacters = true,
		                              IndentChars = string.Empty
		                          };


		    XmlWriter tw = XmlWriter.Create(ms, xmlSettings);

			theDoc.WriteTo(tw);
			tw.Flush();
			tw.Close();

			byte[] buffer = ms.GetBuffer();

			var compressedzipStream = new GZipStream(theStream, CompressionMode.Compress, true);
			compressedzipStream.Write(buffer, 0, buffer.Length);
			// Close the stream.
			compressedzipStream.Flush();
			compressedzipStream.Close();

			// Force a flush
			theStream.Flush();
		}

		public static void WriteXmlAndGzip(XmlDocument theDoc, Stream theXmlStream, Stream theGzipStream)
		{
			// Write to a memory stream, then flush to disk and to gzip file
			MemoryStream ms = new MemoryStream();
			XmlWriterSettings xmlSettings = new XmlWriterSettings();

			xmlSettings.Encoding = Encoding.UTF8;
			xmlSettings.ConformanceLevel = ConformanceLevel.Document;
			xmlSettings.Indent = false;
			xmlSettings.NewLineOnAttributes = false;
			xmlSettings.CheckCharacters = true;
			xmlSettings.IndentChars = "";

			XmlWriter tw = XmlWriter.Create(ms, xmlSettings);

			theDoc.WriteTo(tw);

			tw.Flush();
			tw.Close();

			byte[] buffer = ms.GetBuffer();
			
			GZipStream compressedzipStream = new GZipStream(theGzipStream, CompressionMode.Compress, true);
			compressedzipStream.Write(buffer, 0, (int)ms.Length);

			// Close the stream.
			compressedzipStream.Flush();
			compressedzipStream.Close();

			theXmlStream.Write(buffer, 0, (int)ms.Length);

			// Force a flush.
			theXmlStream.Flush();
			theGzipStream.Flush();
		}

		public static void ReadGzip(XmlDocument theDoc, Stream theStream)
		{
			GZipStream zipStream = new GZipStream(theStream, CompressionMode.Decompress);

			theDoc.Load(zipStream);

			return;
		}

		public static void Read(XmlDocument theDoc, Stream theStream)
		{
			theDoc.Load(theStream);
		}
	}
}