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
using System.Diagnostics;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using Timer=ClearCanvas.Common.Utilities.Timer;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.TestTools
{
	[ExtensionPoint]
	public sealed class MemoryAnalysisComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(MemoryAnalysisComponentViewExtensionPoint))]
	public class MemoryAnalysisComponent : ApplicationComponent
	{
		private readonly List<byte[]> _heldMemory;
		private long _memoryHeldKB = 0;
		private int _memoryIncrementKB = 512;
		private Timer _timer;
		private double _memoryMark;
		private IDesktopWindow _desktopWindow;

		public MemoryAnalysisComponent(IDesktopWindow desktopWindow)
		{
			_desktopWindow = desktopWindow;
			_heldMemory = new List<byte[]>();
		}

		public string TotalLargeObjectMemoryKB
		{
			get
			{
				return (MemoryManager.LargeObjectBytesCount / 1024F).ToString("F2");
			}	
		}

		public string HeapMemoryKB
		{
			get { return (GC.GetTotalMemory(false)/1024F).ToString("F2"); }
		}

		public int MemoryIncrementKB
		{
			get { return _memoryIncrementKB; }
			set
			{
				if (_memoryIncrementKB > 0)
					_memoryIncrementKB = value;

				NotifyPropertyChanged("MemoryIncrementKB");
			}
		}

		public string HeldMemoryKB
		{
			get { return _memoryHeldKB.ToString("F2"); }	
		}

		public double MemoryMarkKB
		{
			get { return _memoryMark; }
		}

		public double MemoryDifferenceKB
		{
			get { return GC.GetTotalMemory(false)/1024F - _memoryMark; }	
		}

		public void MarkMemory()
		{
			_memoryMark = GC.GetTotalMemory(true)/1024F;
			NotifyPropertyChanged("MemoryMark");
			NotifyPropertyChanged("HeapMemoryKB");
			NotifyPropertyChanged("MemoryDifferenceKB");
		}

		public void AddHeldMemory()
		{
			try
			{
				if (_memoryIncrementKB > 0)
				{
					_heldMemory.Add(new byte[_memoryIncrementKB*1024]);
					_memoryHeldKB += _memoryIncrementKB;
				}
			}
			catch(OutOfMemoryException)
			{
				_desktopWindow.ShowMessageBox("Out of memory!", MessageBoxActions.Ok);
			}

			NotifyPropertyChanged("HeapMemoryKB");
			NotifyPropertyChanged("HeldMemoryKB");
		}

		public void ReleaseHeldMemory()
		{
			_heldMemory.Clear();
			_memoryHeldKB = 0;
			NotifyPropertyChanged("HeapMemoryKB");
			NotifyPropertyChanged("HeldMemoryKB");
		}

		public void ConsumeMaximumMemory()
		{
			try
			{
				List<byte[]> memory = new List<byte[]>();
				while (true)
				{
					memory.Add(new byte[_memoryIncrementKB*1024]);
				}
			}
			catch(OutOfMemoryException)
			{
			}

			NotifyPropertyChanged("HeapMemoryKB");
			NotifyPropertyChanged("HeldMemoryKB");
		}

		public void Collect()
		{
			MemoryManager.Collect(true);
			GC.Collect();
		}

		public override void Start()
		{
			base.Start();

			_timer = new Timer(OnTimer, null, 500);
			_timer.Start();
		}

		public override void Stop()
		{
			base.Stop();

			_timer.Stop();
			_timer.Dispose();
			_timer = null;
		}

		private void OnTimer(object nothing)
		{
			if (_timer != null)
			{
				NotifyPropertyChanged("HeapMemoryKB");
				NotifyPropertyChanged("MemoryDifferenceKB");
				NotifyPropertyChanged("TotalLargeObjectMemoryKB");
			}
		}

		public void UnloadPixelData()
		{
			bool keepGoing = true;
			EventHandler<MemoryCollectedEventArgs> del = delegate(object sender, MemoryCollectedEventArgs args)
															{
																if (args.IsLast)
																	keepGoing = args.BytesCollectedCount > 0;
															};

			MemoryManager.MemoryCollected += del;

			try
			{

				MemoryManager.Execute(delegate
				                      	{
											if (keepGoing)
				                      			throw new OutOfMemoryException();
				                      	}, TimeSpan.FromSeconds(30));
			}
			catch (Exception)
			{
			}
			finally
			{
				MemoryManager.MemoryCollected -= del;
			}

			GC.Collect();
			NotifyPropertyChanged("TotalLargeObjectMemoryKB");
		}
	}
}
