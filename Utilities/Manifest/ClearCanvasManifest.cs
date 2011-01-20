#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// A manifest file for a ClearCanvas product.
    /// </summary>
    [XmlRoot(ElementName = "ClearCanvasManifest", Namespace = "http://www.clearcanvas.ca")]
    public class ClearCanvasManifest
    {
        #region Public Properties

        /// <summary>
        /// If the manifest is for a Package, the actual Package Manifest
        /// </summary>
        [XmlElement("PackageManifest")]
        [DefaultValue(null)]
        public PackageManifest PackageManifest { get; set; }

        /// <summary>
        /// If the manifest is for a Product, the actual Product Manifest
        /// </summary>
        [XmlElement("ProductManifest")]
        [DefaultValue(null)]
        public ProductManifest ProductManifest { get; set; }

        #endregion Public Properties

        #region Public Static Methods

        public static void Serialize(string filename, ClearCanvasManifest manifest)
        {
            using (FileStream fs = new FileStream(filename, FileMode.CreateNew))
            {
                Serialize(fs, manifest);
                fs.Close();
            }
        }

        public static void Serialize(Stream stream, ClearCanvasManifest manifest)
        {
            XmlSerializer theSerializer = new XmlSerializer(typeof (ClearCanvasManifest));

            XmlWriterSettings settings = new XmlWriterSettings
                                             {
                                                 Indent = true,
                                                 IndentChars = "  ",
                                                 Encoding = Encoding.UTF8,
                                             };

            XmlWriter writer = XmlWriter.Create(stream, settings);
            if (writer != null)
                theSerializer.Serialize(writer, manifest);
            stream.Flush();
        }

        public static XmlDocument Serialize(ClearCanvasManifest manifest)
        {
            using (MemoryStream fs = new MemoryStream())
            {
                Serialize(fs, manifest);

                fs.Seek(0, SeekOrigin.Begin);
                fs.Flush();

                XmlDocument doc = new XmlDocument();
                doc.Load(fs);
                return doc;
            }
        }


        public static ClearCanvasManifest Deserialize(string filename)
        {
            XmlSerializer theSerializer = new XmlSerializer(typeof(ClearCanvasManifest));

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                ClearCanvasManifest input = (ClearCanvasManifest)theSerializer.Deserialize(fs);

                return input;
            }
        }

        #endregion Public Static Methods
    }
}
