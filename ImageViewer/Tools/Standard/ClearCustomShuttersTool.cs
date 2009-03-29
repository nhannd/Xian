using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Common;
using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.DicomGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("clear", "imageviewer-contextmenu/MenuClearCustomShutters", "Clear")]
	[IconSet("clear", IconScheme.Colour, "Icons.ClearCustomShuttersToolSmall.png", "Icons.ClearCustomShuttersToolMedium.png", "Icons.ClearCustomShuttersToolLarge.png")]
	[VisibleStateObserver("clear", "Visible", "VisibleChanged")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ClearCustomShuttersTool : ImageViewerTool
	{
		private bool _visible;

		public ClearCustomShuttersTool()
		{
		}

		public bool Visible
		{
			get { return _visible; }	
			set
			{
				if (_visible == value)
					return;

				_visible = value;
				EventsHelper.Fire(VisibleChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler VisibleChanged;

		public override void Initialize()
		{
			base.Initialize();
			base.Context.Viewer.EventBroker.ImageDrawing += OnImageDrawing;
		}

		protected override void Dispose(bool disposing)
		{
			base.Context.Viewer.EventBroker.ImageDrawing -= OnImageDrawing;
			base.Dispose(disposing);
		}

		public void Clear()
		{
			if (base.SelectedPresentationImage == null)
				return;

			if (base.SelectedPresentationImage is IDicomPresentationImage)
			{
				IDicomPresentationImage dicomImage = (IDicomPresentationImage)base.SelectedPresentationImage;
				GeometricShuttersGraphic shuttersGraphic = DrawShutterTool.GetGeometricShuttersGraphic(dicomImage);
				DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(shuttersGraphic);
				foreach (GeometricShutter shutter in shuttersGraphic.CustomShutters)
					historyCommand.Enqueue(new RemoveGeometricShutterUndoableCommand(shuttersGraphic, shutter));

				historyCommand.Execute();

				historyCommand.Name = SR.CommandClearCustomShutters;
				base.Context.Viewer.CommandHistory.AddCommand(historyCommand);
				Visible = false;
			}
		}

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			base.OnTileSelected(sender, e);
			UpdateVisible();
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			base.OnPresentationImageSelected(sender, e);
			UpdateVisible();
		}

		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
			if (e.PresentationImage == base.SelectedPresentationImage)
				UpdateVisible();
		}

		private void UpdateVisible()
		{
			Visible = HasCustomShutters(base.SelectedPresentationImage);
		}

		private static bool HasCustomShutters(IPresentationImage image)
		{
			if (image != null && image is IDicomPresentationImage)
			{
				IDicomPresentationImage dicomImage = (IDicomPresentationImage)image;
				GeometricShuttersGraphic shuttersGraphic = DrawShutterTool.GetGeometricShuttersGraphic(dicomImage);
				if (shuttersGraphic != null)
					return shuttersGraphic.CustomShutters.Count > 0;
			}

			return false;
		}
	}
}
