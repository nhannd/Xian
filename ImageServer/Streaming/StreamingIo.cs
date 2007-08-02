using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.IO.Compression;

namespace ClearCanvas.ImageServer.Streaming
{
    public static class StreamingIo
    {
        public static void Write(XmlDocument theDoc, Stream theStream)
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings();

            xmlSettings.Encoding = Encoding.UTF8;
            xmlSettings.ConformanceLevel = ConformanceLevel.Document;
            xmlSettings.Indent = false;
            xmlSettings.NewLineOnAttributes = false;
            xmlSettings.CheckCharacters = true;
            xmlSettings.IndentChars = "";

            XmlWriter tw = XmlWriter.Create(theStream, xmlSettings);

            theDoc.WriteTo(tw);

            tw.Close();
        }

        public static void WriteGzip(XmlDocument theDoc, Stream theStream)
        {
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

            tw.Close();

            byte[] buffer = ms.GetBuffer();

            GZipStream compressedzipStream = new GZipStream(theStream, CompressionMode.Compress, true);
            compressedzipStream.Write(buffer, 0, buffer.Length);
            // Close the stream.
            compressedzipStream.Close();
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
