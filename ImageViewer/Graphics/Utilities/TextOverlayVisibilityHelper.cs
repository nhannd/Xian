#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.Graphics.Utilities
{
	public class TextOverlayVisibilityHelper
	{
		private readonly IAnnotationLayoutProvider _provider;
		private readonly bool _visible;

		public TextOverlayVisibilityHelper(IPresentationImage image)
		{
			_provider = image as IAnnotationLayoutProvider;
			if (_provider != null)
				_visible = _provider.AnnotationLayout.Visible;
		}

		public void Show()
		{
			SetVisible(true);
		}

		public void Hide()
		{
			SetVisible(false);
		}

		public void Restore()
		{
			SetVisible(_visible);
		}

		private void SetVisible(bool visible)
		{
			if (_provider != null)
				_provider.AnnotationLayout.Visible = visible;
		}

		public static void Hide(IPresentationImage image, out bool oldVisibility)
		{
			oldVisibility = false;
			var provider = image as IAnnotationLayoutProvider;
			if (provider == null)
				return;

			oldVisibility = provider.AnnotationLayout.Visible;
			provider.AnnotationLayout.Visible = false;
		}

		public static void Show(IPresentationImage image)
		{
			var provider = image as IAnnotationLayoutProvider;
			if (provider != null)
				provider.AnnotationLayout.Visible = true;
		}

		public static bool IsVisible(IPresentationImage image)
		{
			var provider = image as IAnnotationLayoutProvider;
			return provider == null ? false : provider.AnnotationLayout.Visible;
		}
	}
}
