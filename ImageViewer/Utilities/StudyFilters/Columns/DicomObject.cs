using System;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public struct DicomObject : IComparable<DicomObject>, IComparable, IEquatable<DicomObject>
	{
		public static readonly DicomObject Empty = new DicomObject(-1, string.Empty);

		public readonly long Length;
		public readonly string VR;

		public DicomObject(long size, string vr)
		{
			this.Length = size;
			this.VR = vr;
		}

		public int CompareTo(DicomObject other)
		{
			return this.Length.CompareTo(other.Length);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			if (obj is DicomObject)
				return this.CompareTo((DicomObject) obj);
			throw new ArgumentException("Parameter must be a DicomObject.", "obj");
		}

		public bool Equals(DicomObject other)
		{
			return this.CompareTo(other) == 0;
		}

		public override bool Equals(object obj)
		{
			if (obj is DicomObject)
				return this.Equals((DicomObject) obj);
			return false;
		}

		public override int GetHashCode()
		{
			return -0x6D22B552 ^ this.Length.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(SR.LabelBinaryTagValue, this.VR, this.Length);
		}
	}
}