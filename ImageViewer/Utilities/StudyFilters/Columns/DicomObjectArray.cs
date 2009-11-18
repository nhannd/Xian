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