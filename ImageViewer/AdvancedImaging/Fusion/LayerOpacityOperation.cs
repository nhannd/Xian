#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	/// <summary>
	/// A specialization of the <see cref="BasicImageOperation"/> where the
	/// originator is an <see cref="ILayerOpacityManager"/>.
	/// </summary>
	public class LayerOpacityOperation : BasicImageOperation
	{
		/// <summary>
		/// Mandatory constructor.
		/// </summary>
		public LayerOpacityOperation(ApplyDelegate applyDelegate)
			: base(GetLayerOpacityManager, applyDelegate) {}

		/// <summary>
		/// Returns the <see cref="ILayerOpacityManager"/> associated with the 
		/// <see cref="IPresentationImage"/>, or null.
		/// </summary>
		/// <remarks>
		/// When used in conjunction with an <see cref="ImageOperationApplicator"/>,
		/// it is always safe to cast the return value directly to <see cref="ILayerOpacityManager"/>
		/// without checking for null from within the <see cref="BasicImageOperation.ApplyDelegate"/> 
		/// specified in the constructor.
		/// </remarks>
		public override IMemorable GetOriginator(IPresentationImage image)
		{
			return base.GetOriginator(image) as ILayerOpacityManager;
		}

		private static IMemorable GetLayerOpacityManager(IPresentationImage image)
		{
			if (image is FusionPresentationImage)
				return ((FusionPresentationImage) image).LayerOpacityManager;

			return null;
		}
	}
}