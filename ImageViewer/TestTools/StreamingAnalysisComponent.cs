#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyLoaders.Streaming;
using ClearCanvas.ImageViewer.StudyManagement;
using System;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using System.Collections.Generic;
using System.Threading;

namespace ClearCanvas.ImageViewer.TestTools
{
	[ExtensionPoint]
	public sealed class StreamingAnalysisComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(StreamingAnalysisComponentViewExtensionPoint))]
	public class StreamingAnalysisComponent : ApplicationComponent
	{
		private int _retrieveConcurrency = 5;
		private int _decompressConcurrency = 5;

		private List<StreamingPerformanceInfo> _performanceInfo = new List<StreamingPerformanceInfo>();
		private SynchronizationContext _synchronizationContext;
		private readonly IStudyBrowserToolContext _browserToolContext;
		private BlockingCollection<IFrameReference> _framesToRetrieve;
		private BlockingCollection<IFrameReference> _framesToDecompress;
		private StudyLoaders.Streaming.BlockingThreadPool<IFrameReference> _retrievePool;
		private StudyLoaders.Streaming.BlockingThreadPool<IFrameReference> _decompressPool;

		public StreamingAnalysisComponent(IStudyBrowserToolContext browserToolContext)
		{
			_browserToolContext = browserToolContext;
		}

		public IList<StreamingPerformanceInfo> PerformanceInfo
		{
			get { return _performanceInfo.AsReadOnly(); }
		}

		public bool RetrieveActive
		{
			get { return _retrievePool.Active; }
			set
			{
				if (_retrievePool.Active)
					_retrievePool.Stop(false);
				else 
					_retrievePool.Start();

				NotifyPropertyChanged("RetrieveActive");
			}
		}

		public int RetrieveConcurrency
		{
			get { return _retrieveConcurrency; }
			set
			{
				if (_retrieveConcurrency != value)
				{
					bool start = _retrievePool.Active;
					_retrievePool.Stop(false);
					_retrieveConcurrency = value;
					_retrievePool.Concurrency = _retrieveConcurrency;
					if (start)
						_retrievePool.Start();

					NotifyPropertyChanged("RetrieveConcurrency");
				}
			}
		}

		public ThreadPriority RetrieveThreadPriority
		{
			get { return _retrievePool.ThreadPriority; }
			set
			{
				if (RetrieveThreadPriority != value)
				{
					bool start = _retrievePool.Active;
					_retrievePool.Stop(false);
					_retrievePool.ThreadPriority = value;
					if (start)
						_retrievePool.Start();

					NotifyPropertyChanged("RetrieveThreadPriority");
				}
			}
		}

		public int NumberOfRetrieveItems
		{
			get { return _framesToRetrieve.Count; }	
		}

		public bool DecompressActive
		{
			get { return _decompressPool.Active; }
			set
			{
				if (_decompressPool.Active)
					_decompressPool.Stop(false);
				else
					_decompressPool.Start();

				NotifyPropertyChanged("DecompressActive");
			}
		}

		public int DecompressConcurrency
		{
			get { return _decompressConcurrency; }
			set
			{
				if (_decompressConcurrency != value)
				{
					bool start = _decompressPool.Active;
					_decompressPool.Stop(false);
					_decompressConcurrency = value;
					_decompressPool.Concurrency = _decompressConcurrency;
					if (start)
						_decompressPool.Start();
					
					NotifyPropertyChanged("DecompressConcurrency");
				}
			}
		}

		public ThreadPriority DecompressThreadPriority
		{
			get { return _decompressPool.ThreadPriority; }
			set
			{
				if (DecompressThreadPriority != value)
				{
					bool start = _decompressPool.Active;
					_decompressPool.Stop(false);
					_decompressPool.ThreadPriority = value;
					if (start)
						_decompressPool.Start();

					NotifyPropertyChanged("DecompressThreadPriority");
				}
			}
		}
		public int NumberOfDecompressItems
		{
			get { return _framesToDecompress.Count; }
		}

		public bool CanAddSelectedStudies
		{
			get
			{
				if (_browserToolContext.SelectedStudy == null)
					return false;

				foreach (StudyItem study in _browserToolContext.SelectedStudies)
				{
					if (study.Server == null)
						return false;
					else if (study.Server is ApplicationEntity)
					{
						ApplicationEntity ae = (ApplicationEntity)study.Server;
						if (!ae.IsStreaming)
							return false;
					}
				}

				return true;
			}
		}

		public void AddSelectedStudies()
		{
			BlockingOperation.Run(delegate
			                      	{
			                      		foreach (StudyItem study in _browserToolContext.SelectedStudies)
			                      			LoadStudy(study);
			                      	});
		}

		public void ClearRetrieveItems()
		{
			_performanceInfo.Clear();

			bool restart = _retrievePool.Active;
			_retrievePool.Stop(false);

			List<IFrameReference> remaining;
			_framesToRetrieve.Clear(out remaining);
			foreach (IFrameReference reference in remaining)
				reference.Dispose();

			if (restart)
				_retrievePool.Start();

			NotifyPropertyChanged("NumberOfRetrieveItems");
		}

		public void ClearDecompressItems()
		{
			bool restart = _decompressPool.Active;
			_decompressPool.Stop(false);

			List<IFrameReference> remaining;
			_framesToDecompress.Clear(out remaining);
			foreach (IFrameReference reference in remaining)
				reference.Dispose();

			if (restart)
				_decompressPool.Start();

			NotifyPropertyChanged("NumberOfDecompressItems");
		}

		public override void Start()
		{
			_synchronizationContext = SynchronizationContext.Current;
			_browserToolContext.SelectedStudyChanged += new EventHandler(OnSelectedStudyChanged);
			_framesToRetrieve = new BlockingCollection<IFrameReference>();
			_retrievePool = new StudyLoaders.Streaming.BlockingThreadPool<IFrameReference>(_framesToRetrieve, RetrieveFrame);
			_retrievePool.Concurrency = _retrieveConcurrency;

			_framesToDecompress = new BlockingCollection<IFrameReference>();
			_decompressPool = new StudyLoaders.Streaming.BlockingThreadPool<IFrameReference>(_framesToDecompress, DecompressFrame);
			_decompressPool.Concurrency = _decompressConcurrency;

			base.Start();
		}

		public override void Stop()
		{
			_retrievePool.Stop(false);
			_decompressPool.Stop(false);

			ClearRetrieveItems();
			ClearDecompressItems();

			base.Stop();
		}

		private void LoadStudy(StudyItem study)
		{
			IStudyLoader loader = (IStudyLoader)CollectionUtils.SelectFirst(new StudyLoaderExtensionPoint().CreateExtensions(),
				delegate(object extension) { return ((IStudyLoader)extension).Name == "CC_STREAMING"; });

			List<IFrameReference> frames = new List<IFrameReference>();

			loader.Start(new StudyLoaderArgs(study.StudyInstanceUid, study.Server));
			Sop sop;
			while ((sop = loader.LoadNextSop()) != null)
			{
				using (sop)
				{
					if (sop.IsImage)
					{
						foreach (Frame frame in ((ImageSop)sop).Frames)
							frames.Add(frame.CreateTransientReference());
					}
				}
			}

			_framesToRetrieve.AddRange(frames);
			NotifyPropertyChanged("NumberOfRetrieveItems");
		}

		private void RetrieveFrame(IFrameReference frameReference)
		{
			try
			{

				Frame frame = frameReference.Frame;
				IStreamingSopFrameData frameData =
					(IStreamingSopFrameData)frame.ParentImageSop.DataSource.GetFrameData(frame.FrameNumber);
				frameData.RetrievePixelData();

				_framesToDecompress.Add(frameReference);
				_synchronizationContext.Post(OnRetrievedFrame, frameData.LastRetrievePerformanceInfo);
			}
			catch (Exception e)
			{
				frameReference.Dispose();
				Platform.Log(LogLevel.Error, e);
			}
		}

		private void DecompressFrame(IFrameReference frameReference)
		{
			try
			{
				frameReference.Frame.GetNormalizedPixelData();
				_synchronizationContext.Post(OnDecompressedFrame, frameReference);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
			finally
			{
				frameReference.Dispose();
			}
		}

		private void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			NotifyPropertyChanged("CanAddSelectedStudies");
		}

		private void OnRetrievedFrame(object frameDataTransferInfo)
		{
			StreamingPerformanceInfo transferInfo = frameDataTransferInfo as StreamingPerformanceInfo;
			if (transferInfo != null)
				_performanceInfo.Add(transferInfo);

			NotifyPropertyChanged("NumberOfRetrieveItems");
			NotifyPropertyChanged("NumberOfDecompressItems");
		}

		private void OnDecompressedFrame(object nothing)
		{
			NotifyPropertyChanged("NumberOfDecompressItems");
		}
	}
}
