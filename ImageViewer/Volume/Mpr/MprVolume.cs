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
				yield return new MprStandardSliceSet(volume, new VolumeSlicerParams(90, 0, 270));
				yield return new MprStandardSliceSet(volume, new VolumeSlicerParams(90, 0, 315));
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