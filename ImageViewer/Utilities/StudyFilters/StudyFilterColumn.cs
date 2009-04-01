using System;
using System.Collections.Generic;
using System.Globalization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	public abstract class StudyFilterColumn : IEquatable<StudyFilterColumn>
	{
		private StudyFilterColumn() {}

		public abstract string Name { get; }

		public override sealed bool Equals(object obj)
		{
			if (obj is StudyFilterColumn)
				return this.Equals((StudyFilterColumn) obj);
			return false;
		}

		public abstract bool Equals(StudyFilterColumn other);

		public override sealed string ToString()
		{
			return this.Name;
		}

		public abstract string Key { get; }

		internal abstract TableColumnBase<StudyItem> CreateColumn();

		public static StudyFilterColumn GetColumn(string key)
		{
			StudyFilterColumn column = CollectionUtils.SelectFirst(GetSpecialColumns(), delegate(StudyFilterColumn test) { return test.Key == key; });
			if (column != null)
				return column;

			try
			{
				uint dicomTag = uint.Parse(key, NumberStyles.AllowHexSpecifier);
				return new DicomTagColumn(dicomTag);
			}
			catch (Exception) {}

			return null;
		}

		public static StudyFilterColumn GetDicomTagColumn(uint dicomTag)
		{
			return new DicomTagColumn(dicomTag);
		}

		public static StudyFilterColumn GetDicomTagColumn(DicomTag dicomTag)
		{
			return GetDicomTagColumn(dicomTag.TagValue);
		}

		public static IEnumerable<StudyFilterColumn> GetSpecialColumns()
		{
			yield return Filename;
			yield return Directory;
			yield return Path;
			yield return Extension;
			yield return FileSize;
		}

		public static readonly StudyFilterColumn Filename = new FilenameColumn();
		public static readonly StudyFilterColumn Directory = new DirectoryColumn();
		public static readonly StudyFilterColumn Path = new PathColumn();
		public static readonly StudyFilterColumn Extension = new ExtensionColumn();
		public static readonly StudyFilterColumn FileSize = new FileSizeColumn();

		private class DicomTagColumn : StudyFilterColumn
		{
			private readonly uint _dicomTag;
			private readonly string _tagName;

			public DicomTagColumn(uint dicomTag)
			{
				_dicomTag = dicomTag;

				DicomTag tag = DicomTagDictionary.GetDicomTag(dicomTag);
				if (tag == null)
					_tagName = string.Format(SR.FormatUnknownDicomTag, (dicomTag >> 16) & 0x0000FFFF, dicomTag & 0x0000FFFF);
				else if ((dicomTag & 0xFFE10000) == 0x60000000)
					_tagName = string.Format(SR.FormatRepeatingDicomTag, tag.Name, (dicomTag >> 16) & 0x0000FFFF, dicomTag & 0x0000FFFF, ((dicomTag >> 16) & 0x000000FF)/2 + 1);
				else
					_tagName = string.Format(SR.FormatDicomTag, tag.Name, (dicomTag >> 16) & 0x0000FFFF, dicomTag & 0x0000FFFF);
			}

			public override string Name
			{
				get { return _tagName; }
			}

			public override string Key
			{
				get { return _dicomTag.ToString("x8"); }
			}

			public override int GetHashCode()
			{
				return BitConverter.ToInt32(BitConverter.GetBytes(_dicomTag ^ 0x752F9B6D), 0);
			}

			public override bool Equals(StudyFilterColumn other)
			{
				if (other is DicomTagColumn)
					return this._dicomTag == ((DicomTagColumn) other)._dicomTag;
				return false;
			}

			internal override TableColumnBase<StudyItem> CreateColumn()
			{
				return new TableColumn<StudyItem, string>(_tagName, delegate(StudyItem item) { return item[_dicomTag]; });
			}
		}

		private class FileSizeColumn : StudyFilterColumn
		{
			public override int GetHashCode()
			{
				return 0x5645E200;
			}

			public override string Name
			{
				get { return SR.FileSize; }
			}

			public override string Key
			{
				get { return "FileSize"; }
			}

			public override bool Equals(StudyFilterColumn other)
			{
				return other is FileSizeColumn;
			}

			internal override TableColumnBase<StudyItem> CreateColumn()
			{
				return new TableColumn<StudyItem, string>(this.Name, GetFileSizeValue, null, 1f, CompareFileSizeValues);
			}

			private static string GetFileSizeValue(StudyItem item)
			{
				long size = item.File.Length;

				if (size < 768) // less than 768 bytes
					return string.Format(SR.FormatFileSizeBytes, size);
				else if (size < 786432) // between 768 bytes and 768 KiB
					return string.Format(SR.FormatFileSizeKB, size/1024.0);
				else if (size < 805306368) // between 768 KiB and 768 MiB
					return string.Format(SR.FormatFileSizeMB, size/1048576.0);
				else if (size < 824633720832) // between 768 MiB and 768 GiB
					return string.Format(SR.FormatFileSizeGB, size/1073741824.0);

				// and finally, in the event of having a file greater than 768 GiB...
				return string.Format(SR.FormatFileSizeTB, size/1099511627776.0);
			}

			private static int CompareFileSizeValues(StudyItem x, StudyItem y)
			{
				return x.File.Length.CompareTo(y.File.Length);
			}
		}

		private class FilenameColumn : StudyFilterColumn
		{
			public override string Name
			{
				get { return SR.Filename; }
			}

			public override string Key
			{
				get { return "Filename"; }
			}

			public override int GetHashCode()
			{
				return 0x3CE181E1;
			}

			public override bool Equals(StudyFilterColumn other)
			{
				return other is FilenameColumn;
			}

			internal override TableColumnBase<StudyItem> CreateColumn()
			{
				return new TableColumn<StudyItem, string>(this.Name, delegate(StudyItem item) { return item.File.Name; });
			}
		}

		private class PathColumn : StudyFilterColumn
		{
			public override string Name
			{
				get { return SR.Path; }
			}

			public override string Key
			{
				get { return "Path"; }
			}

			public override int GetHashCode()
			{
				return -0x2C08BB91;
			}

			public override bool Equals(StudyFilterColumn other)
			{
				return other is PathColumn;
			}

			internal override TableColumnBase<StudyItem> CreateColumn()
			{
				return new TableColumn<StudyItem, string>(this.Name, delegate(StudyItem item) { return item.File.FullName; });
			}
		}

		private class DirectoryColumn : StudyFilterColumn
		{
			public override string Name
			{
				get { return SR.Directory; }
			}

			public override string Key
			{
				get { return "Directory"; }
			}

			public override int GetHashCode()
			{
				return 0x032D888C;
			}

			public override bool Equals(StudyFilterColumn other)
			{
				return other is DirectoryColumn;
			}

			internal override TableColumnBase<StudyItem> CreateColumn()
			{
				return new TableColumn<StudyItem, string>(this.Name, delegate(StudyItem item) { return item.File.DirectoryName; });
			}
		}

		private class ExtensionColumn : StudyFilterColumn
		{
			public override string Name
			{
				get { return SR.Extension; }
			}

			public override string Key
			{
				get { return "Extension"; }
			}

			public override int GetHashCode()
			{
				return 0x146B4377;
			}

			public override bool Equals(StudyFilterColumn other)
			{
				return other is ExtensionColumn;
			}

			internal override TableColumnBase<StudyItem> CreateColumn()
			{
				return new TableColumn<StudyItem, string>(this.Name, delegate(StudyItem item) { return item.File.Extension; });
			}
		}
	}
}