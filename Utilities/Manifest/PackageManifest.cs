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
    /// Root element for Package Manifests.
    /// </summary>
    [XmlRoot(ElementName = "PackageManifest")]
    public class PackageManifest
    {
        #region Private Members

        private List<ManifestFile> _files;

        #endregion Private Members

        #region Public Properties

        public Package Package { get; set; }

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

        #endregion Public Properties
    }
}
