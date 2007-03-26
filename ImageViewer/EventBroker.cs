using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A central place from where image viewer events are raised.
	/// </summary>
	public class EventBroker
	{
		private event EventHandler<ImageDrawingEventArgs> _imageDrawingEvent;

		private event EventHandler<ImageBoxSelectedEventArgs> _imageBoxSelectedEvent;
		private event EventHandler<DisplaySetSelectedEventArgs> _displaySetSelectedEvent;
		private event EventHandler<TileSelectedEventArgs> _tileSelectedEvent;
		private event EventHandler<PresentationImageSelectedEventArgs> _presentationImageSelectedEvent;
		private event EventHandler<GraphicSelectedEventArgs> _graphicSelectedEvent;

		private event EventHandler<StudyEventArgs> _studyLoadedEvent;
		private event EventHandler<SopEventArgs> _imageLoadedEvent;

		public EventBroker()
		{

		}

		/// <summary>
		/// Occurs when a <see cref="PresentationImage"/> is about to be drawn.
		/// </summary>
		public event EventHandler<ImageDrawingEventArgs> ImageDrawing
		{
			add { _imageDrawingEvent += value; }
			remove { _imageDrawingEvent -= value; }
		}

		internal void OnImageDrawing(ImageDrawingEventArgs args)
		{
			EventsHelper.Fire(_imageDrawingEvent, this, args);
		}

		/// <summary>
		/// Occurs when an <see cref="IImageBox"/> is selected.
		/// </summary>
		public event EventHandler<ImageBoxSelectedEventArgs> ImageBoxSelected
		{
			add { _imageBoxSelectedEvent += value; }
			remove { _imageBoxSelectedEvent -= value; }
		}

		internal void OnImageBoxSelected(ImageBoxSelectedEventArgs args)
		{
			EventsHelper.Fire(_imageBoxSelectedEvent, this, args);
		}

		/// <summary>
		/// Occurs when an <see cref="IDisplaySet"/> is selected.
		/// </summary>
		public event EventHandler<DisplaySetSelectedEventArgs> DisplaySetSelected
		{
			add { _displaySetSelectedEvent += value; }
			remove { _displaySetSelectedEvent -= value; }
		}

		internal void OnDisplaySetSelected(DisplaySetSelectedEventArgs args)
		{
			EventsHelper.Fire(_displaySetSelectedEvent, this, args);
		}

		/// <summary>
		/// Occurs when an <see cref="ITile"/> is selected.
		/// </summary>
		public event EventHandler<TileSelectedEventArgs> TileSelected
		{
			add { _tileSelectedEvent += value; }
			remove { _tileSelectedEvent -= value; }
		}

		internal void OnTileSelected(TileSelectedEventArgs args)
		{
			EventsHelper.Fire(_tileSelectedEvent, this, args);
		}

		/// <summary>
		/// Occurs when an <see cref="IPresentationImage"/> is selected.
		/// </summary>
		public event EventHandler<PresentationImageSelectedEventArgs> PresentationImageSelected
		{
			add { _presentationImageSelectedEvent += value; }
			remove { _presentationImageSelectedEvent -= value; }
		}

		internal void OnPresentationImageSelected(PresentationImageSelectedEventArgs args)
		{
			EventsHelper.Fire(_presentationImageSelectedEvent, this, args);
		}

		/// <summary>
		/// Occurs when a <see cref="Graphic"/> in the currently selected
		/// <see cref="PresentationImage"/>'s scene graph is selected.
		/// </summary>
		public event EventHandler<GraphicSelectedEventArgs> GraphicSelected
		{
			add { _graphicSelectedEvent += value; }
			remove { _graphicSelectedEvent -= value; }
		}

		internal void OnGraphicSelected(GraphicSelectedEventArgs args)
		{
			EventsHelper.Fire(_graphicSelectedEvent, this, args);
		}

		/// <summary>
		/// Occurs when a DICOM study is loaded.
		/// </summary>
		public event EventHandler<StudyEventArgs> StudyLoaded
		{
			add { _studyLoadedEvent += value; }
			remove { _studyLoadedEvent -= value; }
		}

		internal void OnStudyLoaded(StudyEventArgs studyEventArgs)
		{
			EventsHelper.Fire(_studyLoadedEvent, this, studyEventArgs);
		}

		/// <summary>
		/// Occurs when a DICOM image is loaded.
		/// </summary>
		public event EventHandler<SopEventArgs> ImageLoaded
		{
			add { _imageLoadedEvent += value; }
			remove { _imageLoadedEvent -= value; }
		}

		internal void OnImageLoaded(SopEventArgs sopEventArgs)
		{
			EventsHelper.Fire(_imageLoadedEvent, this, sopEventArgs);
		}
	}
}
