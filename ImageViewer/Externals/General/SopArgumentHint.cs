#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Globalization;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Externals.General
{
	public class SopArgumentHint : IArgumentHint
	{
		private ISopDataSource _dataSource;

		public SopArgumentHint(ISopDataSource dataSource)
		{
			_dataSource = dataSource;
		}

		public SopArgumentHint(Sop sop) : this(sop.DataSource) {}

		public void Dispose()
		{
			_dataSource = null;
		}

		public ArgumentHintValue this[string key]
		{
			get
			{
				uint tag;
				if (!uint.TryParse(key, NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture.NumberFormat, out tag))
					return ArgumentHintValue.Empty;
				DicomAttribute attribute;
				if (!_dataSource.TryGetAttribute(tag, out attribute) || attribute == null || attribute.IsEmpty)
					return ArgumentHintValue.Empty;
				return new ArgumentHintValue(attribute.ToString());
			}
		}
	}
}