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