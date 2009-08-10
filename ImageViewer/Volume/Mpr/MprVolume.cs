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
	public interface IMprVolume : IDisposable
	{
		string Uid { get; }
		string Description { get; }
		Volume Volume { get; }
		IList<IMprSliceSet> SliceSets { get; }
	}

	public class MprVolume : IMprVolume
	{
		private readonly string _uid = DicomUid.GenerateUid().UID;

		private Volume _volume;
		private ObservableDisposableList<IMprSliceSet> _sliceSets;

		public MprVolume(Volume volume) : this(volume, CreateDefaultSliceSets(volume)) {}

		public MprVolume(Volume volume, IEnumerable<IVolumeSlicerParams> slicerParams) : this(volume, CreateStandardSliceSets(volume, slicerParams)) {}

		public MprVolume(Volume volume, IEnumerable<IMprSliceSet> sliceSets)
		{
			Platform.CheckForNullReference(volume, "volume");

			// MprVolume is the de jure owner of the Volume
			// Everything else (like the SOPs) just hold transient references
			_volume = volume;

			_sliceSets = new ObservableDisposableList<IMprSliceSet>();
			if (sliceSets != null)
			{
				foreach (IMprSliceSet sliceSet in sliceSets)
				{
					if (sliceSet is IInternalMprSliceSet)
						((IInternalMprSliceSet) sliceSet).Parent = this;
					_sliceSets.Add(sliceSet);
				}
			}
			_sliceSets.EnableEvents = true;
			_sliceSets.ItemAdded += OnItemAdded;
			_sliceSets.ItemChanged += OnItemAdded;
			_sliceSets.ItemChanging += OnItemRemoved;
			_sliceSets.ItemRemoved += OnItemRemoved;
		}

		public Volume Volume
		{
			get { return _volume; }
		}

		public IList<IMprSliceSet> SliceSets
		{
			get { return _sliceSets; }
		}

		public string Uid
		{
			get { return _uid; }
		}

		public string Description
		{
			get { return _volume.Description; }
		}

		private static IEnumerable<IMprSliceSet> CreateDefaultSliceSets(Volume volume)
		{
			// The default slice sets consist of a fixed view of the original image plane,
			// and three mutable slice sets showing the other two planes perpendicular to the original
			// plus one oblique slice set halfway in between these two perpendicular planes.
			if (volume != null)
			{
				yield return MprStaticSliceSet.CreateIdentitySliceSet(volume);
				yield return new MprStandardSliceSet(volume, VolumeSlicerParams.OrthogonalX);
				yield return new MprStandardSliceSet(volume, VolumeSlicerParams.OrthogonalY);
				yield return new MprStandardSliceSet(volume, new VolumeSlicerParams(90, 0, 135));
			}
		}

		private static IEnumerable<IMprSliceSet> CreateStandardSliceSets(Volume volume, IEnumerable<IVolumeSlicerParams> slicerParams)
		{
			if (volume != null && slicerParams != null)
			{
				foreach (IVolumeSlicerParams slicerParam in slicerParams)
					yield return new MprStandardSliceSet(volume, slicerParam);
			}
		}

		protected virtual void OnSliceSetRemoved(IMprSliceSet item)
		{
			if (item is IInternalMprSliceSet)
				((IInternalMprSliceSet) item).Parent = null;
		}

		protected virtual void OnSliceSetAdded(IMprSliceSet item)
		{
			if (item is IInternalMprSliceSet)
				((IInternalMprSliceSet)item).Parent = this;
		}

		private void OnItemRemoved(object sender, ListEventArgs<IMprSliceSet> e)
		{
			this.OnSliceSetRemoved(e.Item);
		}

		private void OnItemAdded(object sender, ListEventArgs<IMprSliceSet> e)
		{
			this.OnSliceSetAdded(e.Item);
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
				if (_sliceSets != null)
				{
					_sliceSets.ItemAdded -= OnItemAdded;
					_sliceSets.ItemChanged -= OnItemAdded;
					_sliceSets.ItemChanging -= OnItemRemoved;
					_sliceSets.ItemRemoved -= OnItemRemoved;
					_sliceSets.Dispose();
					_sliceSets = null;
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

	/// <summary>
	/// Same as <see cref="IMprSliceSet"/>, but adds an internal <see cref="Parent"/> setter.
	/// </summary>
	/// <remarks>
	/// This internal interface is only used to let <see cref="MprVolume"/> decide whether or not
	/// to automatically manage the parent relationship of an <see cref="IMprSliceSet"/>. If a
	/// class implements the <see cref="IMprSliceSet"/> interface directly, it is responsible for
	/// managing the parent relationship on its own. Do <b><i>not</i></b> make this interface
	/// public just to <see cref="MprVolume"/> do your work for you.
	/// </remarks>
	internal interface IInternalMprSliceSet : IMprSliceSet
	{
		new IMprVolume Parent { get; set; }
	}
}