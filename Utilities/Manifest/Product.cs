#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// Product definition for use in a <see cref="ClearCanvasManifest"/>.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// The name of the product.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The component name.
        /// </summary>
        [DefaultValue(null)]
        public string Component { get; set; }
        /// <summary>
        /// The edition of the component.
        /// </summary>
        [DefaultValue(null)]
        public string Edition { get; set; }
        /// <summary>
        /// The version associated with the product.
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// The version suffix (e.g., SP1, etc.)
        /// </summary>
        public string Suffix { get; set; }
        /// <summary>
        /// The name of the manifest associated with the product.
        /// </summary>
        [DefaultValue(null)]
        public string Manifest { get; set; }
    }
}
