#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	//TODO (cr Oct 2009): realized after code review that these could be display set descriptors ... not sure how it would work, though.
	public interface IMprSliceSet : IDisposable
	{
		string Uid { get; }
		string Description { get; }
		Volume Volume { get; }
		IMprVolume Parent { get; }
		IList<MprSliceSop> SliceSops { get; }
		event EventHandler SliceSopsChanged;
	}

	public abstract class MprSliceSet : IInternalMprSliceSet
	{
		private event EventHandler _sliceSopsChanged;
		private readonly string _uid = DicomUid.GenerateUid().UID;
		private bool _sliceSopsChangedSuspended = false;
		private string _description = string.Empty;
		private IMprVolume _parent;
		private IVolumeReference _volume;
		private ObservableDisposableList<MprSliceSop> _sliceSops;

		protected MprSliceSet(Volume volume)
		{
			Platform.CheckForNullReference(volume, "volume");
			_volume = volume.CreateTransientReference();

			_sliceSops = new ObservableDisposableList<MprSliceSop>();
			_sliceSops.EnableEvents = true;
			_sliceSops.ItemAdded += OnItemAdded;
			_sliceSops.ItemChanged += OnItemChanged;
			_sliceSops.ItemChanging += OnItemChanging;
			_sliceSops.ItemRemoved += OnItemRemoved;
		}

		public string Uid
		{
			get { return _uid; }
		}

		public string Description
		{
			get { return _description; }
			protected set { _description = value; }
		}

		public Volume Volume
		{
			get { return _volume.Volume; }
		}

		public IList<MprSliceSop> SliceSops
		{
			get { return _sliceSops; }
		}

		public event EventHandler SliceSopsChanged
		{
			add { _sliceSopsChanged += value; }
			remove { _sliceSopsChanged -= value; }
		}

		public IMprVolume Parent
		{
			get { return _parent; }
		}

		IMprVolume IInternalMprSliceSet.Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		protected void SuspendSliceSopsChangedEvent()
		{
			_sliceSopsChangedSuspended = true;
		}

		protected void ResumeSliceSopsChangedEvent(bool fireNow)
		{
			_sliceSopsChangedSuspended = false;
			if (fireNow)
				this.OnSliceSopsChanged();
		}

		protected void ClearAndDisposeSops()
		{
			// not quite the same as ObservableDisposableList<Sop>.Dispose() since we want to keep our list!
			bool enableEvents = _sliceSops.EnableEvents;
			_sliceSops.EnableEvents = false;
			try
			{
				List<MprSliceSop> temp = new List<MprSliceSop>(_sliceSops);
				_sliceSops.Clear();
				foreach (MprSliceSop sop in temp)
				{
					sop.Parent = null;
					sop.Dispose();
				}
			}
			finally
			{
				_sliceSops.EnableEvents = enableEvents;
			}
		}

		protected virtual void OnSliceSopRemoved(MprSliceSop item)
		{
			item.Parent = null;
		}

		protected virtual void OnSliceSopAdded(MprSliceSop item)
		{
			item.Parent = this;
		}

		protected virtual void OnSliceSopsChanged()
		{
			EventsHelper.Fire(_sliceSopsChanged, this, EventArgs.Empty);
		}

		private void OnItemAdded(object sender, ListEventArgs<MprSliceSop> e)
		{
			this.OnSliceSopAdded(e.Item);
			if (!_sliceSopsChangedSuspended)
				this.OnSliceSopsChanged();
		}

		private void OnItemRemoved(object sender, ListEventArgs<MprSliceSop> e)
		{
			this.OnSliceSopRemoved(e.Item);
			if (!_sliceSopsChangedSuspended)
				this.OnSliceSopsChanged();
		}

		private void OnItemChanging(object sender, ListEventArgs<MprSliceSop> e)
		{
			this.OnSliceSopRemoved(e.Item);
		}

		private void OnItemChanged(object sender, ListEventArgs<MprSliceSop> e)
		{
			this.OnSliceSopAdded(e.Item);
			if (!_sliceSopsChangedSuspended)
				this.OnSliceSopsChanged();
		}

		#region Disposal

		public void Dispose()
		{
			try
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_sliceSops != null)
				{
					_sliceSops.ItemAdded -= OnItemAdded;
					_sliceSops.ItemChanged -= OnItemChanged;
					_sliceSops.ItemChanging -= OnItemChanging;
					_sliceSops.ItemRemoved -= OnItemRemoved;
					_sliceSops.Dispose();
					_sliceSops = null;
				}

				if (_volume != null)
				{
					_volume.Dispose();
					_volume = null;
				}
			}
		}

		#endregion
	}
}