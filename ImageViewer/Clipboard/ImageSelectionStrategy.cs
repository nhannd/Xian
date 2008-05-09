using System.Collections.Generic;
using ClearCanvas.Common;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard
{
	public delegate IEnumerable<IPresentationImage> GetImagesDelegate(IDisplaySet displaySet);

	public class ImageSelectionStrategy : IImageSelectionStrategy
	{
		private readonly string _description;
		private readonly GetImagesDelegate _getImagesDelegate;

		public ImageSelectionStrategy(string description, GetImagesDelegate getImagesDelegate)
		{
			Platform.CheckForNullReference(getImagesDelegate, "getImagesDelegate");
			_description = description ?? "";
			_getImagesDelegate = getImagesDelegate;
		}

		#region IImageSelectionStrategy Members

		public string Description
		{
			get { return _description; }
		}

		public IEnumerable<IPresentationImage> GetImages(IDisplaySet displaySet)
		{
			return _getImagesDelegate(displaySet);
		}

		#endregion
	}
}