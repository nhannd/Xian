#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
	[Obsolete("Construct a System.Drawing.Bitmap directly from the resolved System.IO.Stream")]
    public static class IconFactory
    {
        /// <summary>
        /// Attempts to create an icon using the specified image resource and resource resolver.
        /// </summary>
        /// <param name="resource">The name of the image resource</param>
        /// <param name="resolver">A resource resolver</param>
        /// <returns>a bitmap constructed from the specified image resource</returns>
        public static Bitmap CreateIcon(string resource, IResourceResolver resolver)
        {
            return new Bitmap(resolver.OpenResource(resource));
        }
    }
}
