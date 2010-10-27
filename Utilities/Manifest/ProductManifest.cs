#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Xml.Serialization;

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// Root element used for Product Manifests
    /// </summary>
    [XmlRoot(ElementName = "ProductManifest")]
    public class ProductManifest
    {
        private List<ManifestFile> _files;

        public Product Product { get; set;}

        [XmlArray("Files")]
        [XmlArrayItem("File")]
        public List<ManifestFile> Files
        {
            get
            {
                if (_files == null)
                    _files = new List<ManifestFile>();
                return _files;
            }
            set { _files = value; }
        }
    }
}
