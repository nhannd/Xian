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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public interface IMprSliceSet : IDisposable
	{
		string Uid { get; }
		string Description { get; }
		Volume Volume { get; }
		IList<Sop> SliceSops { get; }
	}

	public abstract class MprSliceSet : IMprSliceSet
	{
		private readonly string _uid = DicomUid.GenerateUid().UID;
		private string _description = string.Empty;
		private IVolumeReference _volume;
		private ObservableDisposableList<Sop> _sliceSops;

		protected MprSliceSet(Volume volume)
		{
			Platform.CheckForNullReference(volume, "volume");
			_volume = volume.CreateTransientReference();

			_sliceSops = new ObservableDisposableList<Sop>();
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

		public IList<Sop> SliceSops
		{
			get { return _sliceSops; }
		}

		protected void ClearAndDisposeSops()
		{
			// not quite the same as ObservableDisposableList<Sop>.Dispose() since we want to keep our list!
			List<Sop> temp = new List<Sop>(_sliceSops);
			_sliceSops.EnableEvents = false;
			_sliceSops.Clear();
			foreach (Sop sop in temp)
				sop.Dispose();
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