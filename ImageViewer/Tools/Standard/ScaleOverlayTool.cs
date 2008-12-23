using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("showHide", "global-menus/MenuTools/MenuStandard/MenuShowHideScaleOverlay", "ShowHide")]
	[ButtonAction("showHide", "global-toolbars/ToolbarStandard/ToolbarShowHideScaleOverlay", "ShowHide")]
	[Tooltip("showHide", "TooltipShowHideScaleOverlay")]
	[GroupHint("showHide", "Tools.Image.Overlays.Scale.ShowHide")]
	[IconSet("showHide", IconScheme.Colour, "Icons.ScaleOverlayToolSmall.png", "Icons.ScaleOverlayToolMedium.png", "Icons.ScaleOverlayToolLarge.png")]
	
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class ScaleOverlayTool : ImageViewerTool
	{
		private event EventHandler _visibleChanged;
		private bool _visible;

		public ScaleOverlayTool()
		{
			_visible = true;
		}

		public bool Visible
		{
			get { return _visible; }
			set
			{
				if (_visible != value)
				{
					_visible = value;
					OnVisibleChanged();
				}
			}
		}

		public event EventHandler VisibleChanged
		{
			add { _visibleChanged += value; }
			remove { _visibleChanged -= value; }
		}

		public void ShowHide()
		{
			this.Visible = !this.Visible;
		}

		private void RefreshGraphics()
		{
			if (_visible)
			{
				foreach (IPresentationImage image in EnumerateVisiblePresentationImages(base.ImageViewer))
				{
					CompositeScaleGraphic scale = GetCompositeScaleGraphic(image, true);
					if (scale != null)
					{
						scale.Visible = true;
						scale.Draw();
					}
				}
			}
			else
			{
				foreach (IPresentationImage image in EnumerateVisiblePresentationImages(base.ImageViewer))
				{
					CompositeScaleGraphic scale = GetCompositeScaleGraphic(image, false);
					if (scale != null)
					{
						scale.Visible = false;
						scale.Draw();
					}
				}
			}
		}

		private void RefreshGraphic(IPresentationImage image)
		{
			if (_visible)
			{
				CompositeScaleGraphic scale = GetCompositeScaleGraphic(image, true);
				if (scale != null)
				{
					scale.Visible = true;
					scale.Draw();
				}
			}
			else
			{
				CompositeScaleGraphic scale = GetCompositeScaleGraphic(image, false);
				if (scale != null)
				{
					scale.Visible = false;
					scale.Draw();
				}
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			base.ImageViewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;
		}

		protected override void Dispose(bool disposing)
		{
			base.ImageViewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;
			base.Dispose(disposing);
		}

		private void OnVisibleChanged()
		{
			RefreshGraphics();
			EventsHelper.Fire(_visibleChanged, this, new EventArgs());
		}

		private void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			RefreshGraphics();
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			base.OnPresentationImageSelected(sender, e);
			RefreshGraphic(e.SelectedPresentationImage);
		}

		private static CompositeScaleGraphic GetCompositeScaleGraphic(IPresentationImage image, bool createIfNull)
		{
			if (image is IOverlayGraphicsProvider)
			{
				GraphicCollection overlayGraphics = ((IOverlayGraphicsProvider) image).OverlayGraphics;
				CompositeScaleGraphic scale = CollectionUtils.SelectFirst(overlayGraphics,
				                                                          delegate(IGraphic graphic) { return graphic is CompositeScaleGraphic; }
				                              	) as CompositeScaleGraphic;

				if (scale == null && createIfNull)
					overlayGraphics.Insert(0, scale = new CompositeScaleGraphic());

				return scale;
			}

			return null;
		}

		private static IEnumerable<IPresentationImage> EnumerateVisiblePresentationImages(IImageViewer imageViewer)
		{
			foreach (IImageBox imageBox in imageViewer.PhysicalWorkspace.ImageBoxes)
			{
				foreach (ITile tile in imageBox.Tiles)
				{
					if (tile.PresentationImage != null)
						yield return tile.PresentationImage;
				}
			}
		}

		private class CompositeScaleGraphic : CompositeGraphic
		{
			private event EventHandler _changed;
			private readonly ScaleGraphic _horizontalScale;
			private readonly ScaleGraphic _verticalScale;

			public CompositeScaleGraphic()
			{
				base.Graphics.Add(_horizontalScale = new ScaleGraphic());
				base.Graphics.Add(_verticalScale = new ScaleGraphic());

				_horizontalScale.Visible = false;
				_horizontalScale.IsMirrored = true;

				_verticalScale.Visible = false;
				_verticalScale.IsMirrored = true;
			}

			/// <summary>
			/// Indicates that the ScaleGraphic have changed.
			/// </summary>
			public event EventHandler Changed
			{
				add { _changed += value; }
				remove { _changed -= value; }
			}

			/// <summary>
			/// Gets or sets a value indicating the visibility of the <see cref="IGraphic"/>.
			/// </summary>
			public override bool Visible
			{
				get { return base.Visible; }
				set
				{
					if (base.Visible != value)
					{
						base.Visible = value;
						OnChanged();
					}
				}
			}

			/// <summary>
			/// Fires the <see cref="Graphic.Drawing"/> event.  Should be called by an <see cref="IRenderer"/>
			/// for each object just before it is drawn/rendered, hence the reason it is public.
			/// </summary>
			public override void OnDrawing()
			{
				try
				{
					base.CoordinateSystem = CoordinateSystem.Destination;

					// compute baseline
					Rectangle clientRectangle = base.ParentPresentationImage.ClientRectangle;
					int width5p = (int) (0.10*clientRectangle.Width);
					int height5p = (int) (0.10*clientRectangle.Height);

					_horizontalScale.CoordinateSystem = CoordinateSystem.Destination;
					_horizontalScale.SetEndPoints(new PointF(2*width5p, clientRectangle.Height - height5p), new SizeF(clientRectangle.Width - 4*width5p, 0));
					_horizontalScale.Visible = true;

					_verticalScale.CoordinateSystem = CoordinateSystem.Destination;
					_verticalScale.SetEndPoints(new PointF(clientRectangle.Width - width5p, 2*height5p), new SizeF(0, clientRectangle.Height - 4*height5p));
					_verticalScale.Visible = true;
				}
				catch (ScaleGraphic.UncalibratedImageException)
				{
					_horizontalScale.Visible = false;
					_verticalScale.Visible = false;
				}
				finally
				{
					base.ResetCoordinateSystem();
					_horizontalScale.ResetCoordinateSystem();
					_verticalScale.ResetCoordinateSystem();
				}

				base.OnDrawing();
			}

			protected virtual void OnChanged()
			{
				EventsHelper.Fire(_changed, this, EventArgs.Empty);
				base.Draw();
			}
		}
	}
}