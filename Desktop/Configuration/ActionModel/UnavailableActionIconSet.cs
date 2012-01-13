#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Drawing;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Configuration.ActionModel
{
	/// <summary>
	/// Represents a set of icon resources that specify the same logical icon in different sizes with an overlay to indicate that the action is unavailable.
	/// </summary>
	internal sealed class UnavailableActionIconSet : IconSet
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="baseIconSet">A template <see cref="IconSet"/> from which to copy resource names.</param>
		public UnavailableActionIconSet(IconSet baseIconSet)
			: base(baseIconSet.SmallIcon, baseIconSet.MediumIcon, baseIconSet.LargeIcon) {}

		/// <summary>
		/// Gets the appropriate icon overlay resource name to indicate an unavailable action.
		/// </summary>
		/// <param name="iconSize">The desired version of the icon overlay.</param>
		/// <returns>The requested icon overlay as an <see cref="Image"/>.</returns>
		private Image GetOverlayIcon(IconSize iconSize)
		{
			var resourceResolver = new ApplicationThemeResourceResolver(GetType().Assembly);
			switch (iconSize)
			{
				case IconSize.Small:
					return new Bitmap(resourceResolver.OpenResource("Icons.UnavailableToolOverlaySmall.png"));
				case IconSize.Medium:
					return new Bitmap(resourceResolver.OpenResource("Icons.UnavailableToolOverlayMedium.png"));
				case IconSize.Large:
				default:
					return new Bitmap(resourceResolver.OpenResource("Icons.UnavailableToolOverlayLarge.png"));
			}
		}

		/// <summary>
		/// Creates an icon using the specified icon resource and resource resolver.
		/// </summary>
		/// <param name="iconSize">The size of the desired icon.</param>
		/// <param name="resourceResolver">The resource resolver with which to resolve the requested icon resource.</param>
		/// <returns>An <see cref="Image"/> constructed from the requested resource.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="resourceResolver"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="resourceResolver"/> was unable to resolve the requested icon resource.</exception>
		public override Image CreateIcon(IconSize iconSize, IResourceResolver resourceResolver)
		{
			var iconBase = base.CreateIcon(iconSize, resourceResolver);
			var iconOverlay = GetOverlayIcon(iconSize);
			if (iconOverlay != null)
			{
				using (var g = Graphics.FromImage(iconBase))
				{
					g.DrawImageUnscaledAndClipped(iconOverlay, new Rectangle(Point.Empty, iconBase.Size));
				}
				iconOverlay.Dispose();
			}
			return iconBase;
		}

		/// <summary>
		/// Gets a string identifier that uniquely identifies the resolved icon, suitable for dictionary keying purposes.
		/// </summary>
		/// <param name="iconSize">The size of the desired icon.</param>
		/// <param name="resourceResolver">The resource resolver with which to resolve the requested icon resource.</param>
		/// <returns>A string identifier that uniquely identifies the resolved icon.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="resourceResolver"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="resourceResolver"/> was unable to resolve the requested icon resource.</exception>
		public override string GetIconKey(IconSize iconSize, IResourceResolver resourceResolver)
		{
			var baseIconKey = base.GetIconKey(iconSize, resourceResolver);
			return string.Format("{0}:unavailable", baseIconKey);
		}
	}
}