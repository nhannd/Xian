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

using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.BaseTools
{
	partial class MouseImageViewerTool
	{
		private static void UpdateMouseButtonIconSet(IActionSet actions, XMouseButtons mouseButton)
		{
			foreach (IAction action in actions)
			{
				if (action.IconSet is MouseButtonIconSet)
					((MouseButtonIconSet) action.IconSet).AssignedButton = mouseButton;
			}
		}

		/// <summary>
		/// Declares a set of icon resources to associate with an action that activates a mouse button tool.
		/// </summary>
		protected class MouseButtonIconSetAttribute : IconSetAttribute
		{
			/// <summary>
			/// Attribute constructor.
			/// </summary>
			/// <param name="actionId">The logical action identifier to which this attribute applies.</param>
			/// <param name="scheme">The scheme of this icon set.</param>
			/// <param name="smallIcon">The resource name of the small icon.</param>
			/// <param name="mediumIcon">The resource name of the medium icon.</param>
			/// <param name="largeIcon">The resource name of the large icon.</param>
			public MouseButtonIconSetAttribute(string actionId, IconScheme scheme, string smallIcon, string mediumIcon, string largeIcon)
				: base(actionId, scheme, smallIcon, mediumIcon, largeIcon) {}

			///<summary>
			/// Attribute constructor.
			///</summary>
			/// <param name="actionId">The logical action identifier to which this attribute applies.</param>
			///<param name="icon">The resource name of the icon.</param>
			public MouseButtonIconSetAttribute(string actionId, string icon)
				: base(actionId, icon) {}

			/// <summary>
			/// Sets the icon set for an <see cref="IAction"/>, via the specified <see cref="IActionBuildingContext"/>.
			/// </summary>
			public override void Apply(IActionBuildingContext builder)
			{
				XMouseButtons assignedButton = XMouseButtons.None;
				if (builder.ActionTarget is MouseImageViewerTool)
					assignedButton = ((MouseImageViewerTool) builder.ActionTarget).MouseButton;
				builder.Action.IconSet = new MouseButtonIconSet(this.IconSet, assignedButton);
			}
		}

		/// <summary>
		/// Represents a set of icon resources that specify the same logical icon in different sizes with an overlay to indicate mapped mouse button.
		/// </summary>
		protected class MouseButtonIconSet : IconSet
		{
			private XMouseButtons _assignedButton;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="baseIconSet">A template <see cref="IconSet"/> from which to copy resource names.</param>
			/// <param name="assignedButton">The mouse button that is assigned to the associated tool.</param>
			public MouseButtonIconSet(IconSet baseIconSet, XMouseButtons assignedButton)
				: base(baseIconSet.Scheme, baseIconSet.SmallIcon, baseIconSet.MediumIcon, baseIconSet.LargeIcon)
			{
				_assignedButton = assignedButton;
			}

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="scheme">The scheme of this icon set.</param>
			/// <param name="smallIcon">The resource name of the small icon.</param>
			/// <param name="mediumIcon">The resource name of the medium icon.</param>
			/// <param name="largeIcon">The resource name of the large icon.</param>
			/// <param name="assignedButton">The mouse button that is assigned to the associated tool.</param>
			public MouseButtonIconSet(IconScheme scheme, string smallIcon, string mediumIcon, string largeIcon, XMouseButtons assignedButton)
				: base(scheme, smallIcon, mediumIcon, largeIcon)
			{
				_assignedButton = assignedButton;
			}

			/// <summary>
			/// Constructor that assumes all the icons are colour and have the same size.
			/// </summary>
			/// <param name="icon">The resource name of the icon.</param>
			/// <param name="assignedButton">The mouse button that is assigned to the associated tool.</param>
			public MouseButtonIconSet(string icon, XMouseButtons assignedButton)
				: base(icon)
			{
				_assignedButton = assignedButton;
			}

			/// <summary>
			/// Gets or sets the mouse button that is assigned to the associated tool.
			/// </summary>
			public XMouseButtons AssignedButton
			{
				get { return _assignedButton; }
				set { _assignedButton = value; }
			}

			/// <summary>
			/// Gets an appropriate icon overlay to indicate the mouse button assigned to the associated tool.
			/// </summary>
			/// <param name="iconSize">The desired version of the icon overlay.</param>
			/// <returns>The icon overlay as an <see cref="Image"/>.</returns>
			public Image GetButtonOverlay(IconSize iconSize)
			{
				string[] resourceNames;
				switch (_assignedButton)
				{
					case XMouseButtons.Left:
						resourceNames = new string[] {"BaseTools.LeftMouseButtonOverlaySmall.png", "BaseTools.LeftMouseButtonOverlayMedium.png", "BaseTools.LeftMouseButtonOverlayLarge.png"};
						break;
					case XMouseButtons.Right:
						resourceNames = new string[] {"BaseTools.RightMouseButtonOverlaySmall.png", "BaseTools.RightMouseButtonOverlayMedium.png", "BaseTools.RightMouseButtonOverlayLarge.png"};
						break;
					case XMouseButtons.Middle:
						resourceNames = new string[] {"BaseTools.MiddleMouseButtonOverlaySmall.png", "BaseTools.MiddleMouseButtonOverlayMedium.png", "BaseTools.MiddleMouseButtonOverlayLarge.png"};
						break;
					case XMouseButtons.XButton1:
					case XMouseButtons.XButton2:
					case XMouseButtons.None:
					default:
						return null;
				}

				IResourceResolver resourceResolver = new ResourceResolver(this.GetType().Assembly);
				switch (iconSize)
				{
					case IconSize.Small:
						return new Bitmap(resourceResolver.OpenResource(resourceNames[0]));
					case IconSize.Medium:
						return new Bitmap(resourceResolver.OpenResource(resourceNames[1]));
					case IconSize.Large:
					default:
						return new Bitmap(resourceResolver.OpenResource(resourceNames[2]));
				}
			}

			/// <summary>
			/// Creates an icon using the specified icon resource and resource resolver.
			/// </summary>
			/// <param name="iconSize">The size of the desired icon.</param>
			/// <param name="resourceResolver">The resource resolver with which to resolve the requested icon resource.</param>
			/// <returns>An <see cref="Image"/> constructed from the requested resource.</returns>
			public override Image CreateIcon(IconSize iconSize, IResourceResolver resourceResolver)
			{
				Image iconBase = base.CreateIcon(iconSize, resourceResolver);
				if (MouseToolSettings.Default.ShowMouseButtonIconOverlay)
				{
					Image iconOverlay = GetButtonOverlay(iconSize);
					if (iconOverlay != null)
					{
						using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(iconBase))
						{
							g.DrawImageUnscaledAndClipped(iconOverlay, new Rectangle(Point.Empty, iconBase.Size));
						}
						iconOverlay.Dispose();
					}
				}
				return iconBase;
			}
		}
	}
}