using System;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public struct FileDateTime : IComparable<FileDateTime>, IComparable, IEquatable<FileDateTime>
	{
		public static readonly FileDateTime Empty = new FileDateTime(null);
		public static readonly FileDateTime MinValue = new FileDateTime(DateTime.MinValue);
		public static readonly FileDateTime MaxValue = new FileDateTime(DateTime.MaxValue);

		private readonly DateTime? _value;

		public FileDateTime(DateTime? value)
		{
			_value = value;
		}

		public DateTime Value
		{
			get { return _value.Value; }
		}

		public bool HasValue
		{
			get { return _value.HasValue; }
		}

		public DateTime GetValueOrDefault(DateTime defaultValue)
		{
			return _value.GetValueOrDefault(defaultValue);
		}

		public int CompareTo(object obj)
		{
			if (obj is FileDateTime)
				return this.CompareTo((FileDateTime) obj);
			throw new ArgumentException("Parameter must be a FileDateTime.", "obj");
		}

		public int CompareTo(FileDateTime other)
		{
			if (!this._value.HasValue && !other._value.HasValue)
				return 0;
			if (!this._value.HasValue)
				return -1;
			if (!other._value.HasValue)
				return 1;

			return this._value.Value.CompareTo(other._value.Value);
		}

		public bool Equals(FileDateTime other)
		{
			return this.CompareTo(other) == 0;
		}

		public override bool Equals(object obj)
		{
			if (obj is FileDateTime)
				return this.Equals((FileDateTime) obj);
			return false;
		}

		public override int GetHashCode()
		{
			int hash = -0x61C751D1;
			if (_value.HasValue)
				hash ^= _value.Value.GetHashCode();
			return hash;
		}

		public override string ToString()
		{
			if (!_value.HasValue)
				return string.Empty;
			return _value.Value.ToString();
		}

		public string ToString(string format)
		{
			if (!_value.HasValue)
				return string.Empty;
			return _value.Value.ToString(format);
		}

		public string ToString(IFormatProvider provider)
		{
			if (!_value.HasValue)
				return string.Empty;
			return _value.Value.ToString(provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			if (!_value.HasValue)
				return string.Empty;
			return _value.Value.ToString(format, provider);
		}

		public static FileDateTime Parse(string s)
		{
			FileDateTime time;
			if (TryParse(s, out time))
				return time;
			throw new FormatException("Parameter was not in the expected format.");
		}

		public static bool TryParse(string s, out FileDateTime fileDateTime)
		{
			if(string.IsNullOrEmpty(s))
			{
				fileDateTime = new FileDateTime(null);
				return true;
			}

			DateTime dateTime;
			if (DateTime.TryParse(s, out dateTime))
			{
				fileDateTime = new FileDateTime(dateTime);
				return true;
			}

			fileDateTime = Empty;
			return false;
		}

		public static implicit operator DateTime? (FileDateTime x)
		{
			return x._value;
		}

		public static implicit operator FileDateTime (DateTime? x)
		{
			return new FileDateTime(x);
		}

		public static explicit operator DateTime (FileDateTime x)
		{
			return x.Value;
		}

		public static implicit operator FileDateTime(DateTime x)
		{
			return new FileDateTime(x);
		}

		#region Comparison Operators

		public static bool operator ==(FileDateTime x, FileDateTime y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(FileDateTime x, FileDateTime y)
		{
			return !x.Equals(y);
		}

		public static bool operator >(FileDateTime x, FileDateTime y)
		{
			return x.CompareTo(y) > 0;
		}

		public static bool operator <(FileDateTime x, FileDateTime y)
		{
			return x.CompareTo(y) < 0;
		}

		public static bool operator >=(FileDateTime x, FileDateTime y)
		{
			return x.CompareTo(y) >= 0;
		}

		public static bool operator <=(FileDateTime x, FileDateTime y)
		{
			return x.CompareTo(y) <= 0;
		}

		#endregion
	}
}