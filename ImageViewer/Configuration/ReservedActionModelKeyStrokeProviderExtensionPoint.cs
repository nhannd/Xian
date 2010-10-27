#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Configuration
{
	[ExtensionPoint]
	public sealed class ReservedActionModelKeyStrokeProviderExtensionPoint : ExtensionPoint<IReservedActionModelKeyStrokeProvider>
	{
		internal static IList<XKeys> GetReservedActionModelKeyStrokes(IImageViewer imageViewer)
		{
			Platform.CheckForNullReference(imageViewer, "imageViewer");

			var reserved = new List<XKeys>();
			var xp = new ReservedActionModelKeyStrokeProviderExtensionPoint();
			foreach (IReservedActionModelKeyStrokeProvider provider in xp.CreateExtensions())
			{
				provider.SetViewer(imageViewer);
				reserved.AddRange(provider.ReservedKeyStrokes);
			}
			reserved.RemoveAll(k => k == XKeys.None);
			return reserved.AsReadOnly();
		}
	}

	public interface IReservedActionModelKeyStrokeProvider
	{
		void SetViewer(IImageViewer imageViewer);

		IEnumerable<XKeys> ReservedKeyStrokes { get; }
	}

	public abstract class ReservedActionModelKeyStrokeProviderBase : IReservedActionModelKeyStrokeProvider
	{
		private IImageViewer _imageViewer;

		protected IImageViewer ImageViewer
		{
			get { return _imageViewer; }
		}

		protected IDesktopWindow DesktopWindow
		{
			get { return _imageViewer.DesktopWindow; }
		}

		void IReservedActionModelKeyStrokeProvider.SetViewer(IImageViewer imageViewer)
		{
			_imageViewer = imageViewer;
		}

		public abstract IEnumerable<XKeys> ReservedKeyStrokes { get; }
	}
}