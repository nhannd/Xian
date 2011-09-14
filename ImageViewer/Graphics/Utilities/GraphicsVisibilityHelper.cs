#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Graphics.Utilities
{
	public class GraphicsVisibilityHelper
	{
		private readonly GraphicCollection _graphics;
		private readonly Dictionary<IGraphic, bool> _visibility = new Dictionary<IGraphic, bool>();

		public GraphicsVisibilityHelper(GraphicCollection graphics)
		{
			_graphics = graphics;
			foreach (var graphic in _graphics)
				_visibility[graphic] = graphic.Visible;
		}

		public void ShowAll()
		{
			SetVisible(true);
		}

		public void HideAll()
		{
			SetVisible(false);
		}

		public void RestoreAll()
		{
			foreach (var graphic in _graphics)
			{
				bool visible;
				if (_visibility.TryGetValue(graphic, out visible))
					graphic.Visible = visible;
			}
		}

		//TODO: extension method?
		public static void HideAll(GraphicCollection graphics)
		{
			SetVisible(graphics, false);
		}

		//TODO: extension method?
		public static void ShowAll(GraphicCollection graphics)
		{
			SetVisible(graphics, true);
		}

		public static GraphicsVisibilityHelper CreateForApplicationGraphics(IPresentationImage image)
		{
			var provider = image as IApplicationGraphicsProvider;
			if (provider == null)
				return null;

			return new GraphicsVisibilityHelper(provider.ApplicationGraphics);
		}

		public static GraphicsVisibilityHelper CreateForOverlayGraphics(IPresentationImage image)
		{
			var provider = image as IOverlayGraphicsProvider;
			if (provider == null)
				return null;

			return new GraphicsVisibilityHelper(provider.OverlayGraphics);
		}

		private void SetVisible(bool visible)
		{
			SetVisible(_graphics, visible);
		}

		private static void SetVisible(GraphicCollection graphics, bool visible)
		{
			foreach (var graphic in graphics)
				graphic.Visible = visible;

		}
	}
}
