#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// <see cref="CommandLine"/> for the <see cref="ManifestGenerationApplication"/> and 
    /// <see cref="ManifestInputGenerationApplication"/> applications.
    /// </summary>
    public class ManifestCommandLine : CommandLine
    {
        [CommandLineParameter("dist", "d", "Specifies the root directory of the distribution to generate a manifest for.")]
        public string DistributionDirectory { get; set; }

        [CommandLineParameter("manifest", "m", "The path to the generated manifest file.")]
        public string Manifest { get; set; }

        [CommandLineParameter("package", "p", "True if the manifest is for a package.")]
        [DefaultValue(false)]
        public bool Package { get; set; }

        [CommandLineParameter("productmanifest", "pm", "The path to the product manifest the package works against (only used when /p is specified).")]
        public string ProductManifest { get; set; }

        [CommandLineParameter("packagename", "pn", "The name of the package (only used when /p is specified).")]
        public string PackageName { get; set; }

        [CommandLineParameter("certificate", "c", "The x509 certificate for signing the manifest.")]
        public string Certificate { get; set; }

        [CommandLineParameter("password", "pw", "The x509 certificate password for signing the manifest.")]
        public string Password { get; set; }

    }
}
