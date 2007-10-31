#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A central place from where image viewer events are raised.
	/// </summary>
	public class EventBroker
	{
		#region Private fields

		private event EventHandler<ImageDrawingEventArgs> _imageDrawingEvent;

		private event EventHandler<ImageBoxSelectedEventArgs> _imageBoxSelectedEvent;
		private event EventHandler<DisplaySetSelectedEventArgs> _displaySetSelectedEvent;
		private event EventHandler<TileSelectedEventArgs> _tileSelectedEvent;
		private event EventHandler<PresentationImageSelectedEventArgs> _presentationImageSelectedEvent;
		private event EventHandler<GraphicSelectionChangedEventArgs> _graphicSelectionChangedEvent;

		private event EventHandler<StudyEventArgs> _studyLoadedEvent;
		private event EventHandler<SopEventArgs> _imageLoadedEvent;

		private event EventHandler<ItemEventArgs<IMouseButtonHandler>> _activeMouseButtonHandlerChanged;
		private event EventHandler<ItemEventArgs<IMouseWheelHandler>> _activeMouseWheelHandlerChanged;


		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="EventBroker"/>.
		/// </summary>
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
		/// Occurs when the selected <see cref="Graphic"/> in the currently selected
		/// <see cref="PresentationImage"/>'s scene graph has changed.
		/// </summary>
		public event EventHandler<GraphicSelectionChangedEventArgs> GraphicSelectionChanged
		{
			add { _graphicSelectionChangedEvent += value; }
			remove { _graphicSelectionChangedEvent -= value; }
		}

		internal void OnGraphicSelectionChanged(GraphicSelectionChangedEventArgs args)
		{
			EventsHelper.Fire(_graphicSelectionChangedEvent, this, args);
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

		/// <summary>
		/// Occurs when the active <see cref="IMouseButtonHandler"/> has changed.
		/// </summary>
		public event EventHandler<ItemEventArgs<IMouseButtonHandler>> ActiveMouseButtonHandlerChanged
		{
			add { _activeMouseButtonHandlerChanged += value; }
			remove { _activeMouseButtonHandlerChanged -= value; }
		}

		internal void OnActiveMouseButtonHandlerChanged(IMouseButtonHandler handler)
		{
			EventsHelper.Fire(_activeMouseButtonHandlerChanged, this, new ItemEventArgs<IMouseButtonHandler>(handler));
		}

		/// <summary>
		/// Occurs when the active <see cref="IMouseWheelHandler"/> has changed.
		/// </summary>
		public event EventHandler<ItemEventArgs<IMouseWheelHandler>> ActiveMouseWheelHandlerChanged
		{
			add { _activeMouseWheelHandlerChanged += value; }
			remove { _activeMouseWheelHandlerChanged -= value; }
		}

		internal void OnActiveMouseWheelHandlerChanged(IMouseWheelHandler handler)
		{
			EventsHelper.Fire(_activeMouseWheelHandlerChanged, this, new ItemEventArgs<IMouseWheelHandler>(handler));
		}
	}
}
