using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class DateTimeDicomTagColumn : MultiValuedDicomTagColumn<DateTime>, ITemporalSortableColumn
	{
		public DateTimeDicomTagColumn(DicomTag dicomTag) : base(dicomTag) {}

		public override DicomArray<DateTime> GetTypedValue(DicomAttribute attribute)
		{
			if (attribute == null)
				return null;
			if (attribute.IsNull)
				return new DicomArray<DateTime>();
			
			// try parsing as a normal DateTime - if that fails, then default to parsing as a DICOM encoded date/time
			DicomArray<DateTime> normalParseResult;
			if (DicomArray<DateTime>.TryParse(attribute.ToString(), DateTime.TryParse, out normalParseResult))
			{
				bool anyNonNull = false;
				foreach (DateTime? dateTime in normalParseResult)
					anyNonNull |= dateTime.HasValue;
				if(anyNonNull)
					return normalParseResult;
			}

			DateTime?[] result;
			try
			{
				result = new DateTime?[CountValues(attribute)];
				for (int n = 0; n < result.Length; n++)
				{
					DateTime value;
					if (attribute.TryGetDateTime(n, out value))
						result[n] = value;
				}
			}
			catch (DicomException)
			{
				return null;
			}

			string elementFormat = "f";
			if (base.VR == "DA")
				elementFormat = "d";
			else if (base.VR == "TM")
				elementFormat = "t";

			return new DicomArray<DateTime>(result, FormatArray(result, elementFormat));
		}

		public override int Compare(StudyItem x, StudyItem y)
		{
			return this.CompareTemporally(x, y);
		}

		public int CompareTemporally(StudyItem x, StudyItem y)
		{
			return DicomArray<DateTime>.Compare(this.GetTypedValue(x), this.GetTypedValue(y));
		}

		private static string FormatArray(IEnumerable<DateTime?> input, string elementFormat)
		{
			StringBuilder sb = new StringBuilder();
			foreach (DateTime? element in input)
			{
				if (element.HasValue)
					sb.Append(element.Value.ToString(elementFormat));
				sb.Append('\\');
			}
			return sb.ToString(0, Math.Max(0, sb.Length - 1));
		}
	}
}