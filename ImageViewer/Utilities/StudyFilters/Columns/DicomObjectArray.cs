using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public class DicomObjectArray<T> : IComparable<DicomObjectArray<T>>, IComparable, IEquatable<DicomObjectArray<T>>, IEnumerable<T> where T : class, IComparable<T>, IEquatable<T>
	{
		public delegate bool ElementParserDelegate(string s, out T result);

		private readonly IList<T> _values;
		private readonly string _displayString;

		public DicomObjectArray()
		{
			_values = new List<T>().AsReadOnly();
			_displayString = string.Empty;
		}

		public DicomObjectArray(params T[] values) : this(values, null) {}

		public DicomObjectArray(IEnumerable<T> values) : this(values, null) {}

		public DicomObjectArray(IEnumerable<T> values, string displayString)
		{
			Platform.CheckForNullReference(values, "values");
			_values = new List<T>(values).AsReadOnly();

			if (displayString != null)
				_displayString = displayString;
			else
				_displayString = FormatArray(values);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			if (obj is DicomObjectArray<T>)
				return this.CompareTo((DicomObjectArray<T>)obj);
			throw new ArgumentException(string.Format("Parameter must be a DicomObjectArray<{0}>.", typeof(T).Name), "obj");
		}

		public int CompareTo(DicomObjectArray<T> other)
		{
			return Compare(this, other, CompareElement);
		}

		public bool Equals(DicomObjectArray<T> other)
		{
			return this.CompareTo(other) == 0;
		}

		public override bool Equals(object obj)
		{
			if (obj is DicomObjectArray<T>)
				return this.Equals((DicomObjectArray<T>) obj);
			return false;
		}

		public override int GetHashCode()
		{
			int hashcode = 0x6A25AC99 ^ typeof (T).GetHashCode();
			foreach (T t in _values)
			{
				if (t == null)
					hashcode ^= -0x15788718;
				else
					hashcode ^= t.GetHashCode();
			}
			return hashcode;
		}

		public override string ToString()
		{
			return _displayString;
		}

		public static DicomObjectArray<T> Parse(string s, ElementParserDelegate elementParser)
		{
			DicomObjectArray<T> array;
			if (TryParse(s, elementParser, out array))
				return array;
			throw new FormatException("Parameter was not in the expected format.");
		}

		public static bool TryParse(string s, ElementParserDelegate elementParser, out DicomObjectArray<T> result)
		{
			Platform.CheckForNullReference(elementParser, "elementParser");

			if (s != null)
			{
				if (s.Length == 0)
				{
					result = new DicomObjectArray<T>();
					return true;
				}

				List<T> list = new List<T>();
				foreach (string elementString in s.Split(new char[] {'\\'}, StringSplitOptions.None))
				{
					T element;
					if (elementParser(elementString, out element))
						list.Add(element);
					else
						list.Add(null);
				}
				result = new DicomObjectArray<T>(list);
				return true;
			}
			result = null;
			return false;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (T t in _values)
				yield return t;
		}

		private static string FormatArray(IEnumerable<T> input)
		{
			StringBuilder sb = new StringBuilder();
			foreach (T element in input)
			{
				if (element != null)
					sb.Append(element.ToString());
				sb.Append('\\');
			}
			return sb.ToString(0, Math.Max(0, sb.Length - 1));
		}

		private static int CompareElement(T x, T y)
		{
			if (x == null && y == null)
				return 0;
			if (x == null)
				return -1;
			if (y == null)
				return 1;
			return x.CompareTo(y);
		}

		public static int Compare(DicomObjectArray<T> x, DicomObjectArray<T> y)
		{
			return Compare(x, y, CompareElement);
		}

		public static int Compare(DicomObjectArray<T> x, DicomObjectArray<T> y, Comparison<T> comparer)
		{
			if (comparer == null)
				return 0;
			if (x == null && y == null)
				return 0;
			if (x == null)
				return -1;
			if (y == null)
				return 1;

			IEnumerator<T> xnumerator = x._values.GetEnumerator();
			IEnumerator<T> ynumerator = y._values.GetEnumerator();

			int result = 0;
			do
			{
				bool xHasMore = xnumerator.MoveNext();
				bool yHasMore = ynumerator.MoveNext();

				if (xHasMore ^ yHasMore)
				{
					result = xHasMore ? 1 : -1;
				}
				else if (xHasMore) // note that yHasMore == xHasMore
				{
					result = comparer(xnumerator.Current, ynumerator.Current);
				}
				else
				{
					result = 0;
					break;
				}
			} while (result == 0);

			return result;
		}
	}
}