using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layers;

namespace ClearCanvas.ImageViewer
{
	public class EventBroker
	{
		private event EventHandler<ImageDrawingEventArgs> _imageDrawingEvent;

		private event EventHandler<ImageBoxSelectedEventArgs> _imageBoxSelectedEvent;
		private event EventHandler<DisplaySetSelectedEventArgs> _displaySetSelectedEvent;
		private event EventHandler<TileSelectedEventArgs> _tileSelectedEvent;
		private event EventHandler<PresentationImageSelectedEventArgs> _presentationImageSelectedEvent;

		private event EventHandler<LayerGroupSelectedEventArgs> _layerGroupSelectedEvent;
		private event EventHandler<ImageLayerSelectedEventArgs> _imageLayerSelectedEvent;
		private event EventHandler<GraphicLayerSelectedEventArgs> _graphicLayerSelectedEvent;
		private event EventHandler<GraphicSelectedEventArgs> _graphicSelectedEvent;

		public EventBroker()
		{

		}

		public event EventHandler<ImageDrawingEventArgs> ImageDrawing
		{
			add { _imageDrawingEvent += value; }
			remove { _imageDrawingEvent -= value; }
		}

		internal void OnImageDrawing(ImageDrawingEventArgs args)
		{
			EventsHelper.Fire(_imageDrawingEvent, this, args);
		}

		public event EventHandler<ImageBoxSelectedEventArgs> ImageBoxSelected
		{
			add { _imageBoxSelectedEvent += value; }
			remove { _imageBoxSelectedEvent -= value; }
		}

		internal void OnImageBoxSelected(ImageBoxSelectedEventArgs args)
		{
			EventsHelper.Fire(_imageBoxSelectedEvent, this, args);
		}

		public event EventHandler<DisplaySetSelectedEventArgs> DisplaySetSelected
		{
			add { _displaySetSelectedEvent += value; }
			remove { _displaySetSelectedEvent -= value; }
		}

		internal void OnDisplaySetSelected(DisplaySetSelectedEventArgs args)
		{
			EventsHelper.Fire(_displaySetSelectedEvent, this, args);
		}

		public event EventHandler<TileSelectedEventArgs> TileSelected
		{
			add { _tileSelectedEvent += value; }
			remove { _tileSelectedEvent -= value; }
		}

		internal void OnTileSelected(TileSelectedEventArgs args)
		{
			EventsHelper.Fire(_tileSelectedEvent, this, args);
		}

		public event EventHandler<PresentationImageSelectedEventArgs> PresentationImageSelected
		{
			add { _presentationImageSelectedEvent += value; }
			remove { _presentationImageSelectedEvent -= value; }
		}

		internal void OnPresentationImageSelected(PresentationImageSelectedEventArgs args)
		{
			EventsHelper.Fire(_presentationImageSelectedEvent, this, args);
		}

		public event EventHandler<LayerGroupSelectedEventArgs> LayerGroupSelected
		{
			add { _layerGroupSelectedEvent += value; }
			remove { _layerGroupSelectedEvent -= value; }
		}

		internal void OnLayerGroupSelected(LayerGroupSelectedEventArgs args)
		{
			EventsHelper.Fire(_layerGroupSelectedEvent, this, args);
		}

		public event EventHandler<ImageLayerSelectedEventArgs> ImageLayerSelected
		{
			add { _imageLayerSelectedEvent += value; }
			remove { _imageLayerSelectedEvent -= value; }
		}

		internal void OnImageLayerSelected(ImageLayerSelectedEventArgs args)
		{
			EventsHelper.Fire(_imageLayerSelectedEvent, this, args);
		}

		public event EventHandler<GraphicLayerSelectedEventArgs> GraphicLayerSelected
		{
			add { _graphicLayerSelectedEvent += value; }
			remove { _graphicLayerSelectedEvent -= value; }
		}

		internal void OnGraphicLayerSelected(GraphicLayerSelectedEventArgs args)
		{
			EventsHelper.Fire(_graphicLayerSelectedEvent, this, args);
		}

		public event EventHandler<GraphicSelectedEventArgs> GraphicSelected
		{
			add { _graphicSelectedEvent += value; }
			remove { _graphicSelectedEvent -= value; }
		}

		internal void OnGraphicSelected(GraphicSelectedEventArgs args)
		{
			EventsHelper.Fire(_graphicSelectedEvent, this, args);
		}

	}
}
