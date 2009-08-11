#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public partial class DefineSlicePlaneTool : MouseImageViewerToolMaster<MprViewerTool>
	{
		private static readonly Color[,] _colors = {
		                                           	{Color.Red, Color.Salmon},
		                                           	{Color.DodgerBlue, Color.CornflowerBlue},
		                                           	{Color.Lime, Color.GreenYellow},
		                                           	{Color.Yellow, Color.Gold},
		                                           	{Color.Magenta, Color.Violet},
		                                           	{Color.Cyan, Color.Turquoise},
		                                           	{Color.White, Color.LightGray}
		                                           };

		protected override IEnumerable<MprViewerTool> CreateTools()
		{
			int index = 0;

			if (this.ImageViewer == null)
				yield break;

			// create one instance of the slave tool for each mutable slice set
			foreach (IMprVolume volume in this.ImageViewer.StudyTree)
			{
				foreach (IMprSliceSet sliceSet in volume.SliceSets)
				{
					IMprStandardSliceSet standardSliceSet = sliceSet as IMprStandardSliceSet;
					if (standardSliceSet != null && !standardSliceSet.IsReadOnly)
					{
						DefineSlicePlaneSlaveTool tool = new DefineSlicePlaneSlaveTool();
						tool.SliceSet = standardSliceSet;
						tool.HotColor = _colors[index, 0];
						tool.NormalColor = _colors[index, 1];
						index = (index + 1)%_colors.Length; // advance to next color
						yield return tool;
					}
				}
			}
		}

		public new MprViewerComponent ImageViewer
		{
			get { return base.ImageViewer as MprViewerComponent; }
		}

		/// <summary>
		/// Finds the ImageBox displaying the specified slice set
		/// </summary>
		protected static IImageBox FindImageBox(IMprSliceSet sliceSet, MprViewerComponent viewer)
		{
			if (sliceSet == null || viewer == null)
				return null;

			foreach (IImageBox imageBox in viewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox.DisplaySet != null && imageBox.DisplaySet.Uid == sliceSet.Uid)
					return imageBox;
			}
			return null;
		}

		/// <summary>
		/// Colourises the display set description annotation item in the specified image
		/// </summary>
		private static void ColorizeDisplaySetDescription(IPresentationImage image, Color color)
		{
			if (image is IAnnotationLayoutProvider)
			{
				IAnnotationLayoutProvider provider = (IAnnotationLayoutProvider) image;
				foreach (AnnotationBox annotationBox in provider.AnnotationLayout.AnnotationBoxes)
				{
					if (annotationBox.AnnotationItem != null && annotationBox.AnnotationItem.GetIdentifier() == "Presentation.DisplaySetDescription")
					{
						annotationBox.Color = color.Name;
						annotationBox.Bold = true;
					}
				}
			}
		}

		/// <summary>
		/// Moves the graphic from where ever it is to the target image.
		/// </summary>
		private static void TranslocateGraphic(IGraphic graphic, IPresentationImage targetImage)
		{
			IPresentationImage oldImage = graphic.ParentPresentationImage;
			if (oldImage != targetImage)
			{
				RemoveGraphic(graphic);
				if (oldImage != null)
					oldImage.Draw();
				AddGraphic(graphic, targetImage);
			}
		}

		private static void AddGraphic(IGraphic graphic, IPresentationImage image)
		{
			IApplicationGraphicsProvider applicationGraphicsProvider = image as IApplicationGraphicsProvider;
			if (applicationGraphicsProvider != null)
			{
				applicationGraphicsProvider.ApplicationGraphics.Add(graphic);
			}
		}

		private static void RemoveGraphic(IGraphic graphic)
		{
			IApplicationGraphicsProvider applicationGraphicsProvider = graphic.ParentPresentationImage as IApplicationGraphicsProvider;
			if (applicationGraphicsProvider != null)
			{
				applicationGraphicsProvider.ApplicationGraphics.Remove(graphic);
			}
		}
	}
}