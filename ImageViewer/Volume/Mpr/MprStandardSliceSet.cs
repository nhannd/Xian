#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public interface IMprStandardSliceSet : IMprSliceSet
	{
		bool IsReadOnly { get; }
		IVolumeSlicerParams SlicerParams { get; set; }
		event EventHandler SlicerParamsChanged;
	}

	/// <summary>
	/// A basic, mutable, single-plane slice view of an MPR <see cref="Volume"/>.
	/// </summary>
	public class MprStandardSliceSet : MprSliceSet, IMprStandardSliceSet
	{
		private event EventHandler _slicerParamsChanged;
		private IVolumeSlicerParams _slicerParams;

		public MprStandardSliceSet(Volume volume, IVolumeSlicerParams slicerParams) : base(volume)
		{
			Platform.CheckForNullReference(slicerParams, "slicerParams");
			_slicerParams = slicerParams;

			base.Description = slicerParams.Description;
			this.Reslice();
		}

		bool IMprStandardSliceSet.IsReadOnly
		{
			get { return false; }
		}

		public IVolumeSlicerParams SlicerParams
		{
			get { return _slicerParams; }
			set
			{
				if (_slicerParams != value)
				{
					_slicerParams = value;
					this.OnSlicerParamsChanged();
				}
			}
		}

		public event EventHandler SlicerParamsChanged
		{
			add { _slicerParamsChanged += value; }
			remove { _slicerParamsChanged -= value; }
		}

		protected virtual void OnSlicerParamsChanged()
		{
			this.Reslice();
			base.Description = this.SlicerParams.Description;
			EventsHelper.Fire(_slicerParamsChanged, this, EventArgs.Empty);
		}

		protected void Reslice()
		{
			base.SuspendSliceSopsChangedEvent();
			try
			{
				base.ClearAndDisposeSops();

				using (VolumeSlicer slicer = new VolumeSlicer(base.Volume, _slicerParams, base.Uid))
				{
					foreach (ISopDataSource dataSource in slicer.CreateSlices())
					{
						base.SliceSops.Add(new MprSliceSop(dataSource));
					}
				}
			}
			finally
			{
				base.ResumeSliceSopsChangedEvent(true);
			}
		}
	}
}