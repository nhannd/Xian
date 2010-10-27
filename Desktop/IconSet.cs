#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Resources;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a set of icon resources that specify the same logical icon in different sizes.
    /// </summary>
    public class IconSet
    {
        private string _small;
        private string _medium;
        private string _large;
        private IconScheme _scheme;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="scheme">The scheme of this icon set.</param>
        /// <param name="smallIcon">The resource name of the small icon.</param>
        /// <param name="mediumIcon">The resource name of the medium icon.</param>
        /// <param name="largeIcon">The resource name of the large icon.</param>
        public IconSet(IconScheme scheme, string smallIcon, string mediumIcon, string largeIcon)
        {
            _scheme = scheme;
            _small = smallIcon;
            _medium = mediumIcon;
            _large = largeIcon;
        }

        /// <summary>
        /// Constructor that assumes all the icons are colour and have the same size.
        /// </summary>
        /// <param name="icon">The resource name of the icon.</param>
        public IconSet(string icon)
        {
            _scheme = IconScheme.Colour;
            _small = icon;
            _medium = icon;
            _large = icon;
        }

        /// <summary>
        /// The scheme of this icon set.
        /// </summary>
        public IconScheme Scheme { get { return _scheme; } }

		/// <summary>
		/// Gets the name of the resource for the specified <see cref="IconSize"/>.
		/// </summary>
    	public string this[IconSize iconSize]
    	{
    		get
    		{
				if (iconSize == IconSize.Small)
					return SmallIcon;
				if (iconSize == IconSize.Medium)
					return MediumIcon;
				
				return LargeIcon;
    		}	
    	}

        /// <summary>
        /// The resource name of the small icon.
        /// </summary>
        public string SmallIcon { get { return _small; } }

        /// <summary>
        /// The resource name of the medium icon.
        /// </summary>
        public string MediumIcon { get { return _medium; } }

        /// <summary>
        /// The resource name of the large icon.
        /// </summary>
        public string LargeIcon { get { return _large; } }

    	/// <summary>
    	/// Creates an icon using the specified icon resource and resource resolver.
    	/// </summary>
    	/// <remarks>
    	/// The base implementation resolves the specified image resource using the provided
    	/// <paramref name="resourceResolver"/> and deserializes the resource stream into a <see cref="Bitmap"/>.
    	/// </remarks>
    	/// <param name="iconSize">The size of the desired icon.</param>
    	/// <param name="resourceResolver">The resource resolver with which to resolve the requested icon resource.</param>
    	/// <returns>An <see cref="Image"/> constructed from the requested resource.</returns>
    	/// <exception cref="ArgumentNullException">Thrown if <paramref name="resourceResolver"/> is null.</exception>
    	/// <exception cref="ArgumentException">Thrown if <paramref name="resourceResolver"/> was unable to resolve the requested icon resource.</exception>
    	public virtual Image CreateIcon(IconSize iconSize, IResourceResolver resourceResolver)
    	{
    		Platform.CheckForNullReference(resourceResolver, "resourceResolver");
    		try
    		{
    			return new Bitmap(resourceResolver.OpenResource(this[iconSize]));
    		}
    		catch (MissingManifestResourceException ex)
    		{
    			throw new ArgumentException("The provided resource resolver was unable to resolve the requested icon resource.", ex);
    		}
    	}

    	/// <summary>
    	/// Gets a string identifier that uniquely identifies the resolved icon, suitable for dictionary keying purposes.
    	/// </summary>
    	/// <remarks>
    	/// The base implementation resolves the specified image resource using the provided
    	/// <paramref name="resourceResolver"/> and returns the resource's fully qualified resource name.
    	/// </remarks>
    	/// <param name="iconSize">The size of the desired icon.</param>
    	/// <param name="resourceResolver">The resource resolver with which to resolve the requested icon resource.</param>
    	/// <returns>A string identifier that uniquely identifies the resolved icon.</returns>
    	/// <exception cref="ArgumentNullException">Thrown if <paramref name="resourceResolver"/> is null.</exception>
    	/// <exception cref="ArgumentException">Thrown if <paramref name="resourceResolver"/> was unable to resolve the requested icon resource.</exception>
    	public virtual string GetIconKey(IconSize iconSize, IResourceResolver resourceResolver)
    	{
    		Platform.CheckForNullReference(resourceResolver, "resourceResolver");
    		try
    		{
    			return resourceResolver.ResolveResource(this[iconSize]);
    		}
    		catch (MissingManifestResourceException ex)
    		{
    			throw new ArgumentException("The provided resource resolver was unable to resolve the requested icon resource.", ex);
    		}
    	}
    }
}
