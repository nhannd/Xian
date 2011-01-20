#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// Package definition for use in a <see cref="ClearCanvasManifest"/>.
    /// </summary>
    public class Package
    {
        /// <summary>
        /// The name of the package.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The name of the Package's manifest file.
        /// </summary>
        public string Manifest { get; set; }
        /// <summary>
        /// The <see cref="Product"/> that the Package is compatible with.
        /// </summary>
        public Product Product { get; set; }
    }
}
