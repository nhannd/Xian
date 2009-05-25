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

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public abstract string Key { get; }

		public abstract bool Equals(StudyFilterColumn other);

		public abstract string GetValue(StudyItem item);

		public override sealed string ToString()
		{
			return this.Name;
		}

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

			public override string GetValue(StudyItem item)
			{
				return item[_dicomTag];
			}

			internal override TableColumnBase<StudyItem> CreateColumn()
			{
				return new TableColumn<StudyItem, string>(_tagName, this.GetValue);
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

			public override string GetValue(StudyItem item)
			{
				return item.File.Length.ToString();
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

			public override string GetValue(StudyItem item)
			{
				return item.File.Name;
			}

			internal override TableColumnBase<StudyItem> CreateColumn()
			{
				return new TableColumn<StudyItem, string>(this.Name, this.GetValue);
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

			public override string GetValue(StudyItem item)
			{
				return item.File.FullName;
			}

			internal override TableColumnBase<StudyItem> CreateColumn()
			{
				return new TableColumn<StudyItem, string>(this.Name, this.GetValue);
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

			public override string GetValue(StudyItem item)
			{
				return item.File.DirectoryName;
			}

			internal override TableColumnBase<StudyItem> CreateColumn()
			{
				return new TableColumn<StudyItem, string>(this.Name, this.GetValue);
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

			public override string GetValue(StudyItem item)
			{
				return item.File.Extension;
			}

			internal override TableColumnBase<StudyItem> CreateColumn()
			{
				return new TableColumn<StudyItem, string>(this.Name, this.GetValue);
			}
		}
	}
}