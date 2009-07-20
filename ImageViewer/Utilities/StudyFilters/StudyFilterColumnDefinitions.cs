using System;
using System.Collections.Generic;
using System.Reflection;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	partial class StudyFilterColumn
	{
		public abstract class ColumnDefinition
		{
			public readonly string Name;
			public readonly string Key;

			internal ColumnDefinition(string key, string name)
			{
				this.Key = key;
				this.Name = name;
			}

			public abstract StudyFilterColumn Create();

			public override bool Equals(object obj)
			{
				if (obj is ColumnDefinition)
					return this.Key == ((ColumnDefinition) obj).Key;
				return false;
			}

			public override int GetHashCode()
			{
				return 0x35522EF9 ^ this.Key.GetHashCode();
			}

			public override string ToString()
			{
				return this.Name;
			}
		}

		public static IEnumerable<ColumnDefinition> ColumnDefinitions
		{
			get
			{
				foreach (ColumnDefinition definition in SpecialColumnDefinitions)
					yield return definition;
				foreach (ColumnDefinition definition in DicomTagColumnDefinitions)
					yield return definition;
			}
		}

		public static ColumnDefinition GetColumnDefinition(string key)
		{
			// initialize defintion table
			StudyFilterColumn.SpecialColumnDefinitions.GetHashCode();

			if (_specialColumnDefinitions.ContainsKey(key))
				return _specialColumnDefinitions[key];

			// initialize defintion table
			StudyFilterColumn.DicomTagColumnDefinitions.GetHashCode();

			if (_dicomColumnDefinitions.ContainsKey(key))
				return _dicomColumnDefinitions[key];

			return null;
		}

		public static StudyFilterColumn CreateColumn(string key)
		{
			ColumnDefinition definition = StudyFilterColumn.GetColumnDefinition(key);
			if (definition != null)
				return definition.Create();
			return null;
		}

		#region Special Columns

		private static Dictionary<string, ColumnDefinition> _specialColumnDefinitions;

		public static IEnumerable<ColumnDefinition> SpecialColumnDefinitions
		{
			get
			{
				if (_specialColumnDefinitions == null)
				{
					_specialColumnDefinitions = new Dictionary<string, ColumnDefinition>();

					SpecialColumnExtensionPoint xp = new SpecialColumnExtensionPoint();
					foreach (ISpecialColumn prototype in xp.CreateExtensions())
					{
						// not worried about the default constructor not existing, since it had to have one for CreateExtensions to work
						_specialColumnDefinitions.Add(prototype.Key, new SpecialColumnDefinition(prototype.Key, prototype.Name, prototype.GetType().GetConstructor(Type.EmptyTypes)));
					}
				}
				return _specialColumnDefinitions.Values;
			}
		}

		private class SpecialColumnDefinition : ColumnDefinition
		{
			private readonly ConstructorInfo _constructor;

			public SpecialColumnDefinition(string key, string name, ConstructorInfo constructor) : base(key, name)
			{
				_constructor = constructor;
			}

			public override StudyFilterColumn Create()
			{
				return (StudyFilterColumn) _constructor.Invoke(null);
			}
		}

		#endregion

		#region DICOM Tag Columns

		private static Dictionary<string, ColumnDefinition> _dicomColumnDefinitions;

		public static IEnumerable<ColumnDefinition> DicomTagColumnDefinitions
		{
			get
			{
				if (_dicomColumnDefinitions == null)
				{
					_dicomColumnDefinitions = new Dictionary<string, ColumnDefinition>();

					foreach (DicomTag dicomTag in DicomTagDictionary.GetDicomTagList())
					{
						ColumnDefinition definition = CreateDefinition(dicomTag);
						_dicomColumnDefinitions.Add(definition.Key, definition);
					}
				}
				return _dicomColumnDefinitions.Values;
			}
		}

		public static ColumnDefinition GetColumnDefinition(uint dicomTag)
		{
			DicomTag tag = DicomTagDictionary.GetDicomTag(dicomTag);
			if (tag == null)
				tag = new DicomTag(dicomTag, string.Empty, string.Empty, DicomVr.UNvr, false, uint.MinValue, uint.MaxValue, false);
			return GetColumnDefinition(tag);
		}

		public static ColumnDefinition GetColumnDefinition(DicomTag dicomTag)
		{
			// initialize defintion table
			DicomTagColumnDefinitions.GetHashCode();

			string key = dicomTag.TagValue.ToString("x8");

			if (_dicomColumnDefinitions.ContainsKey(key))
				return _dicomColumnDefinitions[key];

			return CreateDefinition(dicomTag);
		}

		private static ColumnDefinition CreateDefinition(DicomTag dicomTag)
		{
			switch (dicomTag.VR.Name)
			{
				case "AE":
				case "CS":
				case "LO":
				case "PN":
				case "SH":
				case "UI":
					// multi-valued strings
					return new StringDicomColumnDefintion(dicomTag);
				case "LT":
				case "ST":
				case "UT":
					// single-valued strings
					return new TextDicomColumnDefintion(dicomTag);
				case "IS":
				case "SL":
				case "SS":
					// multi-valued integers
					return new IntegerDicomColumnDefintion(dicomTag);
				case "UL":
				case "US":
					// multi-valued unsigned integers
					return new UnsignedDicomColumnDefintion(dicomTag);
				case "DS":
				case "FL":
				case "FD":
					// multi-valued floating-point numbers
					return new FloatingPointDicomColumnDefintion(dicomTag);
				case "DA":
				case "DT":
				case "TM":
					// multi-valued dates/times
					return new DateTimeDicomColumnDefintion(dicomTag);
				case "AS":
					// multi-valued time spans
					return new AgeDicomColumnDefintion(dicomTag);
				case "AT":
					// multi-valued DICOM tags
					return new AttributeTagDicomColumnDefintion(dicomTag);
				case "SQ":
				case "OB":
				case "OF":
				case "OW":
				case "UN":
				default:
					// sequence, binary and unknown data
					return new BinaryDicomColumnDefintion(dicomTag);
			}
		}

		#region Definitions

		private class StringDicomColumnDefintion : ColumnDefinition
		{
			private readonly DicomTag _tag;

			public StringDicomColumnDefintion(DicomTag tag) : base(tag.TagValue.ToString("x8"), tag.Name)
			{
				_tag = tag;
			}

			public override StudyFilterColumn Create()
			{
				return new StringDicomTagColumn(_tag);
			}
		}

		private class IntegerDicomColumnDefintion : ColumnDefinition
		{
			private readonly DicomTag _tag;

			public IntegerDicomColumnDefintion(DicomTag tag)
				: base(tag.TagValue.ToString("x8"), tag.Name)
			{
				_tag = tag;
			}

			public override StudyFilterColumn Create()
			{
				return new IntegerDicomTagColumn(_tag);
			}
		}

		private class UnsignedDicomColumnDefintion : ColumnDefinition
		{
			private readonly DicomTag _tag;

			public UnsignedDicomColumnDefintion(DicomTag tag)
				: base(tag.TagValue.ToString("x8"), tag.Name)
			{
				_tag = tag;
			}

			public override StudyFilterColumn Create()
			{
				return new UnsignedDicomTagColumn(_tag);
			}
		}

		private class FloatingPointDicomColumnDefintion : ColumnDefinition
		{
			private readonly DicomTag _tag;

			public FloatingPointDicomColumnDefintion(DicomTag tag)
				: base(tag.TagValue.ToString("x8"), tag.Name)
			{
				_tag = tag;
			}

			public override StudyFilterColumn Create()
			{
				return new FloatingPointDicomTagColumn(_tag);
			}
		}

		private class AgeDicomColumnDefintion : ColumnDefinition
		{
			private readonly DicomTag _tag;

			public AgeDicomColumnDefintion(DicomTag tag)
				: base(tag.TagValue.ToString("x8"), tag.Name)
			{
				_tag = tag;
			}

			public override StudyFilterColumn Create()
			{
				return new AgeDicomTagColumn(_tag);
			}
		}

		private class DateTimeDicomColumnDefintion : ColumnDefinition
		{
			private readonly DicomTag _tag;

			public DateTimeDicomColumnDefintion(DicomTag tag)
				: base(tag.TagValue.ToString("x8"), tag.Name)
			{
				_tag = tag;
			}

			public override StudyFilterColumn Create()
			{
				return new DateTimeDicomTagColumn(_tag);
			}
		}

		private class TextDicomColumnDefintion : ColumnDefinition
		{
			private readonly DicomTag _tag;

			public TextDicomColumnDefintion(DicomTag tag)
				: base(tag.TagValue.ToString("x8"), tag.Name)
			{
				_tag = tag;
			}

			public override StudyFilterColumn Create()
			{
				return new TextDicomTagColumn(_tag);
			}
		}

		private class AttributeTagDicomColumnDefintion : ColumnDefinition
		{
			private readonly DicomTag _tag;

			public AttributeTagDicomColumnDefintion(DicomTag tag)
				: base(tag.TagValue.ToString("x8"), tag.Name)
			{
				_tag = tag;
			}

			public override StudyFilterColumn Create()
			{
				return new AttributeTagDicomTagColumn(_tag);
			}
		}

		private class BinaryDicomColumnDefintion : ColumnDefinition
		{
			private readonly DicomTag _tag;

			public BinaryDicomColumnDefintion(DicomTag tag)
				: base(tag.TagValue.ToString("x8"), tag.Name)
			{
				_tag = tag;
			}

			public override StudyFilterColumn Create()
			{
				return new BinaryDicomTagColumn(_tag);
			}
		}

		#endregion

		#endregion

		[Obsolete]
		public static StudyFilterColumn CreateColumn(uint dicomTag)
		{
			DicomTag tag = DicomTagDictionary.GetDicomTag(dicomTag);
			if (tag == null)
				tag = new DicomTag(dicomTag, string.Empty, string.Empty, DicomVr.UNvr, false, uint.MinValue, uint.MaxValue, false);
			return CreateColumn(tag);
		}

		[Obsolete]
		public static StudyFilterColumn CreateColumn(DicomTag dicomTag)
		{
			switch (dicomTag.VR.Name)
			{
				case "AE":
				case "CS":
				case "LO":
				case "PN":
				case "SH":
				case "UI":
					// multi-valued strings
					return new StringDicomTagColumn(dicomTag);
				case "LT":
				case "ST":
				case "UT":
					// single-valued strings
					return new TextDicomTagColumn(dicomTag);
				case "IS":
				case "SL":
				case "SS":
					// multi-valued integers
					return new IntegerDicomTagColumn(dicomTag);
				case "UL":
				case "US":
					// multi-valued unsigned integers
					return new UnsignedDicomTagColumn(dicomTag);
				case "DS":
				case "FL":
				case "FD":
					// multi-valued floating-point numbers
					return new FloatingPointDicomTagColumn(dicomTag);
				case "DA":
				case "DT":
				case "TM":
					// multi-valued dates/times
					return new DateTimeDicomTagColumn(dicomTag);
				case "AS":
					// multi-valued time spans
					return new AgeDicomTagColumn(dicomTag);
				case "AT":
					// multi-valued DICOM tags
					return new AttributeTagDicomTagColumn(dicomTag);
				case "SQ":
				case "OB":
				case "OF":
				case "OW":
				case "UN":
				default:
					// sequence, binary and unknown data
					return new BinaryDicomTagColumn(dicomTag);
			}
		}

		#region Obsolescence

		[Obsolete]
		public static StudyFilterColumn GetColumn(string key)
		{
			return CreateColumn(key);
		}

		[Obsolete]
		public static StudyFilterColumn GetDicomTagColumn(uint dicomTag)
		{
			return CreateColumn(dicomTag.ToString("x8"));
		}

		[Obsolete]
		public static StudyFilterColumn GetDicomTagColumn(DicomTag dicomTag)
		{
			return GetDicomTagColumn(dicomTag.TagValue);
		}

		[Obsolete]
		public static IEnumerable<StudyFilterColumn> GetSpecialColumns()
		{
			yield return Filename;
			yield return Directory;
			yield return Path;
			yield return Extension;
			yield return FileSize;
		}

		[Obsolete]
		public static readonly StudyFilterColumn Filename = new FilenameColumn();

		[Obsolete]
		public static readonly StudyFilterColumn Directory = new DirectoryColumn();

		[Obsolete]
		public static readonly StudyFilterColumn Path = new PathColumn();

		[Obsolete]
		public static readonly StudyFilterColumn Extension = new ExtensionColumn();

		[Obsolete]
		public static readonly StudyFilterColumn FileSize = new FileSizeColumn();

		#endregion
	}
}