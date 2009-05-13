#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using Timer=ClearCanvas.Common.Utilities.Timer;

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

		public MemoryAnalysisComponent()
		{
			_heldMemory = new List<byte[]>();
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
				Platform.ShowMessageBox("Out of memory!");
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
			for (int i = 0; i < 5; ++i)
			{
				Thread.Sleep(500);
				GC.Collect();
			}
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
			}
		}
	}
}
