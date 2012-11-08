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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Common.Utilities.Imaging;
using ClearCanvas.ImageViewer.Web.Common;
using ClearCanvas.ImageViewer.Web.Common.Events;
using ClearCanvas.ImageViewer.Web.Utiltities;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Entities;
using ClearCanvas.Web.Common.Events;
using ClearCanvas.Web.Services.View;
using TileEntity = ClearCanvas.ImageViewer.Web.Common.Entities.Tile;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.InputManagement;
using System.Drawing;
using System.Drawing.Imaging;
using ClearCanvas.ImageViewer.Web.Common.Messages;
using Message=ClearCanvas.Web.Common.Message;
using MouseWheelMessage=ClearCanvas.ImageViewer.Web.Common.Messages.MouseWheelMessage;
using Rectangle=System.Drawing.Rectangle;
using ClearCanvas.Web.Common.Messages;
using ClearCanvas.Common;
using Cursor=ClearCanvas.ImageViewer.Web.Common.Entities.Cursor;
using Image = ClearCanvas.Web.Common.Image;
using MouseLeaveMessage = ClearCanvas.ImageViewer.Web.Common.Messages.MouseLeaveMessage;

namespace ClearCanvas.ImageViewer.Web.View
{
	internal class ContextMenuContainer : IDisposable
	{
		private readonly List<WebActionView> _contextMenuViews;

		public ContextMenuContainer(ActionModelNode modelNode)
		{
			_contextMenuViews = WebActionView.Create(modelNode.ChildNodes);
		}

		public WebActionNode[] GetWebActions()
		{
			return _contextMenuViews.Select(a => (WebActionNode)a.GetEntity()).ToArray();
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (_contextMenuViews == null)
				return;

			foreach (WebActionView view in _contextMenuViews)
				view.Dispose();

			_contextMenuViews.Clear();
		}

		#endregion
	}

    public class TileView : WebView<TileEntity>
    {
        private static class LogNames
        {
            public const string MessageProcessing = "TileView.MessageProcessing";
            public const string ServerRendering = "TileView.Rendering.Server";
            public const string ClientRendering = "TileView.Rendering.Client";
            public const string ClientStackRendering = "TileView.Rendering.Client.Stacking";
        }

        private class Counter
        {
            private int _count;

            public int Count
            {
                get { return Thread.VolatileRead(ref _count); }
            }

            public void Increment()
            {
                Interlocked.Increment(ref _count);
            }

            public void Decrement()
            {
                Interlocked.Decrement(ref _count);
            }
        }

        private string _mimeType;
        private int _quality;
        private bool _compareImageQuality = DiagnosticsSettings.Default.CompareImageQuality;

		private Tile _tile;
		private Point _mousePosition;
		private bool _hasCapture;
        private bool _hasWheelCapture;

		private WebTileInputTranslator _tileInputTranslator;
		private TileController _tileController;

		private IRenderingSurface _surface;
		private Bitmap _bitmap;
        private MemoryStream _memoryStream;

        private readonly IJpegCompressor _jpegCompressor;
        private readonly IPngEncoder _pngEncoder;

        private long? _lastMouseMessageProcessedTicks;
        private Message _currentMessage;

		[ThreadStatic]
        private static ContextMenuContainer _contextMenu;
        //Static, so it's the sum of all unreleased/undelivered images in all tiles.
        [ThreadStatic]
        private static Counter _unreleasedImageCounter;
        [ThreadStatic]
        private static Counter _undeliveredImageCounter;


        public TileView()
		{
            _jpegCompressor = JpegCompressor.Create();
            _pngEncoder = PngEncoder.Create();
		}

		private Rectangle ClientRectangle
		{
			get { return _tileController.TileClientRectangle; }
			set
			{
				if (_tileController.TileClientRectangle.Equals(value))
					return;

			    _tileController.TileClientRectangle = value;
				OnClientRectangleChanged();
			}
		}

		private Point MousePosition
		{
			get { return _mousePosition; }
			set
			{
				if (_mousePosition.Equals(value))
					return;

				_mousePosition = value;
				NotifyEntityPropertyChanged("MousePosition", new Position(_mousePosition));
			}
		}

		private bool HasCapture
		{
			get { return _hasCapture; }	
			set
			{
				if (value == _hasCapture)
					return;

				_hasCapture = value;
				NotifyEntityPropertyChanged("HasCapture", _hasCapture);
			}
		}

        private bool HasWheelCapture
        {
            get { return _hasWheelCapture; }
            set
            {
                if (value == _hasWheelCapture)
                    return;

                _hasWheelCapture = value;
                NotifyEntityPropertyChanged("HasWheelCapture", _hasWheelCapture);
            }
        }

		private IRenderer Renderer
		{
			get { return CurrentImage != null ? CurrentImage.ImageRenderer : null; }
		}

		private IRenderingSurface Surface
		{
			get
			{
				if (_surface == null && Renderer != null && ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
					_surface = Renderer.GetRenderingSurface(IntPtr.Zero, ClientRectangle.Width, ClientRectangle.Height);
				
				return _surface;
			}
		}

		private Bitmap Bitmap
		{
			get
			{
				if (_bitmap == null && _surface != null)
					_bitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format24bppRgb);

				return _bitmap;
			}
		}

        private MemoryStream MemoryStream
        {
            get { return _memoryStream ?? (_memoryStream = new MemoryStream()); }
        }

        private PresentationImage CurrentImage
		{
			get { return _tile.PresentationImage as PresentationImage; }
		}

        private Counter UnreleasedImageCounter
        {
            get { return _unreleasedImageCounter ?? (_unreleasedImageCounter = new Counter()); }
        }

        private Counter UndeliveredImageCounter
        {
            get { return _undeliveredImageCounter ?? (_undeliveredImageCounter = new Counter()); }
        }

		private Common.Entities.InformationBox CreateInformationBox()
		{
			if (_tile.InformationBox == null)
				return null;

			return new Common.Entities.InformationBox
									{
										Data = _tile.InformationBox.Data,
										Visible = _tile.InformationBox.Visible,
										Location = _tile.InformationBox.DestinationPoint
									};
		}

		private void DisposeSurface()
		{
			if (_surface != null)
			{
				_surface.Dispose();
				_surface = null;
			}

			if (_bitmap != null)
			{
				_bitmap.Dispose();
				_bitmap = null;
			}

            if (_memoryStream != null)
            {
                _memoryStream.Dispose();
                _memoryStream = null;
            }
		}

		public override void SetModelObject(object modelObject)
		{
			_tile = (Tile)modelObject;
			_tileInputTranslator = new WebTileInputTranslator();
			_tileController = new TileController(_tile, ((ImageViewerComponent)_tile.ImageViewer).ShortcutManager);
		}

        protected override void Initialize()
        {
            _tileController.CursorTokenChanged += OnCursorTokenChanged;
            _tileController.ContextMenuRequested += OnContextMenuRequested;

            _tile.Drawing += OnTileDrawing;
            _tile.RendererChanged += OnRendererChanged;
            _tile.SelectionChanged += OnSelectionChanged;
            _tileController.CaptureChanging += OnCaptureChanging;
            _tileController.WheelCaptureChanging += OnWheelCaptureChanging;
            _tile.InformationBoxChanged += OnInformationBoxChanged;
        }

        protected override void UpdateEntity(Common.Entities.Tile entity)
		{
			entity.NormalizedRectangle = _tile.NormalizedRectangle;
			entity.ClientRectangle = ClientRectangle;
			entity.MousePosition = MousePosition;
			entity.Selected = _tile.Selected;
			entity.HasCapture = HasCapture;
			entity.Cursor = CreateCursor();
			entity.InformationBox = CreateInformationBox();
		    entity.Image = CreateImage();
		}

		private void OnContextMenuRequested(object sender, ItemEventArgs<Point> e)
		{
			FireContextMenuEvent();
		}

		private void FireContextMenuEvent()
		{
			if (_tileController.ContextMenuProvider != null && _tileController.ContextMenuEnabled)
			{
				ActionModelNode actionModelNode = _tileController.ContextMenuProvider.GetContextMenuModel(_tileController);
				if (_contextMenu != null)
					_contextMenu.Dispose();

				_contextMenu = new ContextMenuContainer(actionModelNode);

				FireEvent(new ContextMenuEvent
				{
					ActionModelRoot = new WebActionNode { Children = _contextMenu.GetWebActions() }
				});
			}
		}

        private void OnInformationBoxUpdated(object sender, EventArgs e)
        {
            NotifyEntityPropertyChanged("InformationBox", CreateInformationBox());
        }

	    private void OnInformationBoxChanged(object sender, InformationBoxChangedEventArgs e)
	    {
            if (e.InformationBox != null)
				e.InformationBox.Updated += OnInformationBoxUpdated;
			else
                NotifyEntityPropertyChanged("InformationBox", null);
	    }

	    private void OnRendererChanged(object sender, EventArgs e)
		{
			DisposeSurface();
		}

		private void OnClientRectangleChanged()
		{
			DisposeSurface();
			_tile.Draw();
		}

		private void OnCursorTokenChanged(object sender, EventArgs e)
		{
		    MousePosition = _tileController.Location;
			NotifyEntityPropertyChanged("Cursor", CreateCursor());
		}

		private Cursor CreateCursor()
		{
		    return CursorFactory.CreateCursor(_tileController.CursorToken);
		}

		private void OnCaptureChanging(object sender, ItemEventArgs<IMouseButtonHandler> e)
		{
			HasCapture = e.Item != null;
		}

        private void OnWheelCaptureChanging(object sender, ItemEventArgs<IMouseWheelHandler> e)
        {
            HasWheelCapture = e.Item != null;
        }

		private void OnSelectionChanged(object sender, ItemEventArgs<ITile> e)
		{
			NotifyEntityPropertyChanged("Selected", _tile.Selected);
		}

		private void OnTileDrawing(object sender, EventArgs e)
		{
			Draw(false);
		}

		public void Draw(bool refresh)
		{
            //Grab a reference because the callback will come in on a worker thread, and this thing is thread static.
            var unreleasedCounter = UnreleasedImageCounter;
            unreleasedCounter.Increment();

            var undeliveredCounter = UndeliveredImageCounter;
            undeliveredCounter.Increment();

            Event ev = new PropertyChangedEvent
                            {
                                PropertyName = "Image",
                                Value = CreateImage(),
                                TriggeringMessageId = _currentMessage != null ? _currentMessage.Identifier : (Guid?)null,
                                ReleasedCallback = e => unreleasedCounter.Decrement(),
                                DeliveredCallback = e => undeliveredCounter.Decrement()
                            };

            FireEvent(ev);
		}

        private Image CreateImage()
		{
			if (_tile.PresentationImage == null)
				DisposeSurface();

			if (Surface == null)
				return null;

            ImageQualityManager.Instance.GetImageQualityParameters(out _mimeType, out _quality);

            var imageStats = new RenderingStatistics(_mimeType);

            //long t0 = Environment.TickCount;
            imageStats.DrawToBitmapTime.Start();

            Bitmap bitmap = Bitmap;

            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
            {
                Surface.ContextID = graphics.GetHdc();
                Surface.ClipRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

                var drawArgs = new DrawArgs(Surface, null, Rendering.DrawMode.Render);
                CurrentImage.Draw(drawArgs);
                drawArgs = new DrawArgs(Surface, null, Rendering.DrawMode.Refresh);
                CurrentImage.Draw(drawArgs);
                graphics.ReleaseHdc(Surface.ContextID);
            }

            imageStats.DrawToBitmapTime.End();

            Bitmap bmp1 = null;
            if (DiagnosticsSettings.Default.CompareImageQuality)
            {
                // make a copy in case Bitmap.Save() has any side effects.
                bmp1 = (Bitmap)Bitmap.Clone();
            }
           

            imageStats.SaveTime.Start();
            if (_mimeType.Equals(Image.MimeTypes.Jpeg))
            {
                MemoryStream.SetLength(0);
                _jpegCompressor.Compress(bitmap, _quality, MemoryStream);
            }
            else if (_mimeType.Equals(Image.MimeTypes.Png))
            {
                MemoryStream.SetLength(0);
                _pngEncoder.Encode(bitmap, MemoryStream);
            }
            else if (_mimeType.Equals(Image.MimeTypes.Bmp))
            {
                MemoryStream.SetLength(0);
                bitmap.Save(MemoryStream, ImageFormat.Bmp);
            }

            imageStats.SaveTime.End();

            byte[] imageBuffer = MemoryStream.ToArray();
            if (!ApplicationContext.BlobsSupported)
                imageBuffer = Encoding.UTF8.GetBytes(Convert.ToBase64String(imageBuffer));

            imageStats.ImageSize.Value = (ulong)imageBuffer.LongLength;
            ApplicationContext.LogStatistics(LogNames.ServerRendering, imageStats);

            if (_compareImageQuality)
            {
                MemoryStream.Position = 0;
                var bmp2 = new Bitmap(MemoryStream);
                ImageComparisonResult result = BitmapComparison.Compare(ref bmp1, ref bmp2);
                Console.WriteLine("BMP vs {0} w/ client size: {1}x{2}", _mimeType, bmp2.Height, bmp2.Width);
                Console.WriteLine("\tR: MinError={2:0.00} MaxError={3:0.00}  Mean={0:0.00}  STD={1:0.00}", result.Channels[0].MeanError, result.Channels[0].StdDeviation, Math.Abs(result.Channels[0].MinError), Math.Abs(result.Channels[0].MaxError));
                Console.WriteLine("\tG: MinError={2:0.00} MaxError={3:0.00}  Mean={0:0.00}  STD={1:0.00}", result.Channels[1].MeanError, result.Channels[1].StdDeviation, Math.Abs(result.Channels[1].MinError), Math.Abs(result.Channels[1].MaxError));
                Console.WriteLine("\tB: MinError={2:0.00} MaxError={3:0.00}  Mean={0:0.00}  STD={1:0.00}", result.Channels[2].MeanError, result.Channels[2].StdDeviation, Math.Abs(result.Channels[2].MinError), Math.Abs(result.Channels[2].MaxError));

            }

            return new Image { Data = imageBuffer, MimeType = _mimeType, IsBase64 = !ApplicationContext.BlobsSupported, IsDataZipped = false };
		}

        public override void ProcessMessage(Message message)
		{
            _currentMessage = message;

            if (message is MouseLeaveMessage)
            {
                _tileController.ProcessMessage(new InputManagement.MouseLeaveMessage());
            }
            else if (message is MouseMoveMessage)
		    {
                ProcessMouseMoveMessage((MouseMoveMessage)message);
		        //TODO: ideally, the tilecontroller would have an event and the handler would listen
		        MousePosition = _tileController.Location;
		    }
            else if (message is MouseMessage)
		    {
                ProcessMouseMessage((MouseMessage)message);
		        //TODO: ideally, the tilecontroller would have an event and the handler would listen
		        MousePosition = _tileController.Location;
		    }
		    else if (message is MouseWheelMessage)
		    {
                ProcessMouseWheelMessage((MouseWheelMessage)message);
		    }
            else if (message is UpdatePropertyMessage)
		    {
                ProcessUpdateMessage((UpdatePropertyMessage)message);
		    }

		    _currentMessage = null;
		    ProcessClientStatsMessage(message);
		}

        private void ProcessClientStatsMessage(Message message)
        {
            if (!ApplicationContext.IsStatisticsLoggingEnabled)
                return;

            var renderingStats = message as RenderingStatsMessage;
            if (renderingStats != null)
            {
                var stat = new StatisticsSet("ClientRendering");
                var loadTime = TimeSpan.FromMilliseconds(renderingStats.LoadTimeMilliseconds);
                stat.AddField(new TimeSpanStatistics("LoadTime") { Value = loadTime });

                var renderingTime = TimeSpan.FromMilliseconds(renderingStats.RenderTimeMilliseconds);
                stat.AddField(new TimeSpanStatistics("RenderingTime") { Value = renderingTime});

                var roundTripRenderingTime = TimeSpan.FromMilliseconds(renderingStats.RoundTripRenderingTimeMilliseconds);
                stat.AddField(new TimeSpanStatistics("RoundTripRenderingTime") { Value = roundTripRenderingTime });

                ApplicationContext.LogStatistics(LogNames.ClientRendering, stat);
            }

            var stackingRendering = message as StackRenderingTimesMessage;
            if (stackingRendering != null)
            {
                var fps = new AverageStatistics<double>("AverageStackRendering") { Unit = "fps" };
                var stat = new StatisticsSet("StackRendering");
                stat.AddField(fps);

                foreach (var valueMilliseconds in stackingRendering.ValuesMilliseconds)
                {
                    fps.AddSample(1 / (valueMilliseconds/1000.0));

                    var renderingTime = TimeSpan.FromMilliseconds(valueMilliseconds);
                    var substat = new StatisticsSet("ImageRendering");
                    substat.AddField(new TimeSpanStatistics("ImageRenderingTime") {Value = renderingTime});
                    stat.SubStatistics.Add(substat);
                }
                ApplicationContext.LogStatistics(LogNames.ClientStackRendering, stat);
            }
        }

        private void ProcessUpdateMessage(UpdatePropertyMessage message)
		{
			switch(message.PropertyName)
			{
				case "ClientRectangle":
					ClientRectangle = (ClearCanvas.Web.Common.Rectangle)message.Value;
					break;
			}
		}

		private void ProcessMouseWheelMessage(MouseWheelMessage message)
		{
            var messageProcessingStatistics = new MessageProcessingStatistics(message.GetType());
            if (_lastMouseMessageProcessedTicks.HasValue)
            {
                if (message.IsDiscardable)
                {
                    var receivedBeforeLastMessageProcessed = (_lastMouseMessageProcessedTicks.Value - message.ReceiveTimeTicks) >= 0;
                    if (receivedBeforeLastMessageProcessed)
                    {
                        Platform.Log(LogLevel.Debug, "Mouse wheel message discarded - image was processing when received.");
                        return;
                    }
                    if (UndeliveredImageCounter.Count > 2)
                    {
                        Platform.Log(LogLevel.Debug, "Mouse wheel message discarded - undelivered images.");
                        return;
                    }
                }

                messageProcessingStatistics.TimeSinceLastMessageProcessed.Value = TimeSpan.FromMilliseconds(Environment.TickCount - _lastMouseMessageProcessedTicks.Value);
            }

            messageProcessingStatistics.ProcessingTime.Start();

            var e = new MouseEventArgs(MouseButtons.None, 1, 0, 0, message.Delta);
            object msg = _tileInputTranslator.OnMouseWheel(e);
			_tileController.ProcessMessage(msg);

            messageProcessingStatistics.ProcessingTime.End();
            ApplicationContext.LogStatistics(LogNames.MessageProcessing, messageProcessingStatistics);

            _lastMouseMessageProcessedTicks = Environment.TickCount;
        }

		private void ProcessMouseMoveMessage(MouseMoveMessage message)
		{
            var messageProcessingStatistics = new MessageProcessingStatistics(message.GetType());
            if (_lastMouseMessageProcessedTicks.HasValue)
            {
                if (message.IsDiscardable)
                {
                    var receivedBeforeLastMessageProcessed = (_lastMouseMessageProcessedTicks.Value - message.ReceiveTimeTicks) >= 0;
                    if (receivedBeforeLastMessageProcessed)
                    {
                        Platform.Log(LogLevel.Debug, "Mouse move message discarded - image was processing when received.");
                        return;
                    }
                    if (UndeliveredImageCounter.Count > 2)
                    {
                        Platform.Log(LogLevel.Debug, "Mouse move message discarded - undelivered images.");
                        return;
                    }
                }

                messageProcessingStatistics.TimeSinceLastMessageProcessed.Value = TimeSpan.FromMilliseconds(Environment.TickCount - _lastMouseMessageProcessedTicks.Value);
            }

            messageProcessingStatistics.ProcessingTime.Start();

            MouseButtons mouseButtons = MouseButtons.None;

			switch (message.Button)
			{
				case MouseButton.Left: mouseButtons = MouseButtons.Left; break;
				case MouseButton.Right: mouseButtons = MouseButtons.Right; break;
			}

			var e = new MouseEventArgs(mouseButtons, 0, message.MousePosition.X, message.MousePosition.Y, 0);
			var msg = _tileInputTranslator.OnMouseMove(e);
			_tileController.ProcessMessage(msg);

            messageProcessingStatistics.ProcessingTime.End();
            ApplicationContext.LogStatistics(LogNames.MessageProcessing, messageProcessingStatistics);

            _lastMouseMessageProcessedTicks = Environment.TickCount;
        }

        private void ProcessMouseMessage(MouseMessage message)
		{
			if (message.Button == MouseButton.Left)
			{
				if (message.MouseButtonState == MouseButtonState.Down)
				{
					var e = new MouseEventArgs(MouseButtons.Left, message.ClickCount, message.MousePosition.X, message.MousePosition.Y, 0);
					object msg = _tileInputTranslator.OnMouseDown(e);
				    _tileController.ProcessMessage(msg);
				}
				else if (message.MouseButtonState == MouseButtonState.Up)
				{
					var e = new MouseEventArgs(MouseButtons.Left, 1, message.MousePosition.X, message.MousePosition.Y, 0);
					object msg = _tileInputTranslator.OnMouseUp(e);
					_tileController.ProcessMessage(msg);

					//do a mouse move to set the focus state of graphics
					e = new MouseEventArgs(MouseButtons.None, 0, message.MousePosition.X, message.MousePosition.Y, 0);
					msg = _tileInputTranslator.OnMouseMove(e);
					_tileController.ProcessMessage(msg);
				}
			}
			else if (message.Button == MouseButton.Right)
			{
				if (message.MouseButtonState == MouseButtonState.Down)
				{
					var e = new MouseEventArgs(MouseButtons.Right, message.ClickCount, message.MousePosition.X, message.MousePosition.Y, 0);
					object msg = _tileInputTranslator.OnMouseDown(e);
					_tileController.ProcessMessage(msg);
				}
				else if (message.MouseButtonState == MouseButtonState.Up)
				{
					//TODO (CR May 2010): should we be calling this when the tilecontroller fires an event?
					FireContextMenuEvent();
					var e = new MouseEventArgs(MouseButtons.Right, 1, message.MousePosition.X, message.MousePosition.Y, 0);
					object msg = _tileInputTranslator.OnMouseUp(e);
					_tileController.ProcessMessage(msg);

					//do a mouse move to set the focus state of graphics
					e = new MouseEventArgs(MouseButtons.None, 0, message.MousePosition.X, message.MousePosition.Y, 0);
					msg = _tileInputTranslator.OnMouseMove(e);
					_tileController.ProcessMessage(msg);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				_tile.Drawing -= OnTileDrawing;
				_tile.RendererChanged -= OnRendererChanged;
				_tile.SelectionChanged -= OnSelectionChanged;
				_tileController.CursorTokenChanged -= OnCursorTokenChanged;
				_tileController.ContextMenuRequested -= OnContextMenuRequested;
				_tileController.CaptureChanging -= OnCaptureChanging;
                _tileController.WheelCaptureChanging -= OnWheelCaptureChanging;
                _tile.InformationBoxChanged -= OnInformationBoxChanged;

				if (_contextMenu != null)
				{
					_contextMenu.Dispose();
					_contextMenu = null;
				}

				try
				{
					DisposeSurface();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Debug, e);
				}
			}
		}
	}
}
