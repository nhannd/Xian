using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class AttributeTagDicomTagColumn : UnsignedDicomTagColumn
	{
		public AttributeTagDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomArray<uint> GetTypedValue(DicomAttribute attribute)
		{
			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomArray<uint>();

			uint?[] result;
			try
			{
				result = new uint?[CountValues(attribute)];
				for (int n = 0; n < result.Length; n++)
				{
					uint value;
					if (attribute.TryGetUInt32(n, out value))
						result[n] = value;
				}
			}
			catch (DicomException)
			{
				return null;
			}
			return new DicomArray<uint>(result, FormatArray(result));
		}

		private static string FormatArray(IEnumerable<uint?> input)
		{
			StringBuilder sb = new StringBuilder();
			foreach (uint? element in input)
			{
				if (element.HasValue)
					sb.AppendFormat("({0:x4},{1:x4})", (element.Value >> 16) & 0x0000ffff, element.Value & 0x0000ffff);
				sb.Append('\\');
			}
			return sb.ToString(0, Math.Max(0, sb.Length - 1));
		}
	}
}