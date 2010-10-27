#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// A basic, immutable, single-plane slice view of an MPR <see cref="Volume"/>.
	/// </summary>
	public class MprStaticSliceSet : MprSliceSet, IMprStandardSliceSet
	{
		private readonly IVolumeSlicerParams _slicerParams;

		public MprStaticSliceSet(Volume volume, IVolumeSlicerParams slicerParams)
			: base(volume)
		{
			Platform.CheckForNullReference(slicerParams, "slicerParams");
			_slicerParams = slicerParams;

			base.Description = slicerParams.Description;
			this.Reslice();
		}

		public IVolumeSlicerParams SlicerParams
		{
			get { return _slicerParams; }
		}

		bool IMprStandardSliceSet.IsReadOnly
		{
			get { return true; }
		}

		IVolumeSlicerParams IMprStandardSliceSet.SlicerParams
		{
			get { return this.SlicerParams; }
			set { throw new NotSupportedException(); }
		}

		event EventHandler IMprStandardSliceSet.SlicerParamsChanged
		{
			add { }
			remove { }
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

		public static MprStaticSliceSet CreateIdentitySliceSet(Volume volume)
		{
			return new MprStaticSliceSet(volume, VolumeSlicerParams.Identity);
		}
	}
}