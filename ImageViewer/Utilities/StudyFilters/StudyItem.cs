#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public class SopDataSourceStudyItem : StudyItem
	{
		private readonly string _filename;
		private ISopReference _sopReference;

		public SopDataSourceStudyItem(Sop sop)
		{
			if (sop.DataSource is ILocalSopDataSource)
			{
				_filename = ((ILocalSopDataSource) sop.DataSource).Filename;
				_sopReference = sop.CreateTransientReference();
			}
		}

		public SopDataSourceStudyItem(ILocalSopDataSource sopDataSource)
		{
			_filename = sopDataSource.Filename;
			using (Sop sop = new Sop(sopDataSource))
			{
				_sopReference = sop.CreateTransientReference();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_sopReference != null)
				{
					_sopReference.Dispose();
					_sopReference = null;
				}
			}
			base.Dispose(disposing);
		}

		public override string Filename
		{
			get { return _filename; }
		}

		public override DicomAttribute this[uint tag]
		{
			get { return _sopReference.Sop[tag]; }
		}
	}

	public class LocalStudyItem : StudyItem
	{
		private readonly string _filename;
		private readonly DicomFile _dcf;

		public LocalStudyItem(string filename)
		{
			_filename = filename;
			_dcf = new DicomFile(filename);
			_dcf.Load(DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences);
		}

		public override string Filename
		{
			get { return _filename; }
		}

		public override DicomAttribute this[uint tag]
		{
			get
			{
				DicomAttribute attribute;
				if (!_dcf.DataSet.TryGetAttribute(tag, out attribute))
				{
					if (!_dcf.MetaInfo.TryGetAttribute(tag, out attribute))
						return null;
				}
				return attribute;
			}
		}
	}

	public abstract class StudyItem : IStudyItem
	{
		protected StudyItem() {}

		~StudyItem()
		{
			this.Dispose(false);
		}

		public abstract string Filename { get; }

		public abstract DicomAttribute this[uint tag] { get; }

		protected virtual void Dispose(bool disposing) {}

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
	}
}