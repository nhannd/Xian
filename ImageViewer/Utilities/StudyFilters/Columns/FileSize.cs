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
using System.Text.RegularExpressions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	public struct FileSize : IComparable<FileSize>, IComparable, IEquatable<FileSize>
	{
		private static readonly Regex _pattern = new Regex("^(\\d+(?:[.]\\d+)?)\\s*([A-Za-z]*)$", RegexOptions.Compiled);
		public static readonly FileSize Empty = new FileSize(-1);

		public readonly long Size;

		public FileSize(long size)
		{
			this.Size = Math.Max(-1, size);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
				return 1;
			if (obj is FileSize)
				return this.CompareTo((FileSize) obj);
			throw new ArgumentException("Parameter must be a FileSize.", "obj");
		}

		public int CompareTo(FileSize other)
		{
			return this.Size.CompareTo(other.Size);
		}

		public bool Equals(FileSize other)
		{
			return this.CompareTo(other) == 0;
		}

		public override bool Equals(object obj)
		{
			if (obj is FileSize)
				return this.Equals((FileSize) obj);
			return false;
		}

		public override int GetHashCode()
		{
			return -0x09CE58F3 ^ this.Size.GetHashCode();
		}

		public override string ToString()
		{
			if (this.Size < 0) // file doesn't exist!
				return string.Empty;
			else if (this.Size < 768) // less than 768 bytes
				return string.Format(SR.FormatFileSizeBytes, this.Size);
			else if (this.Size < 786432) // between 768 bytes and 768 KiB
				return string.Format(SR.FormatFileSizeKB, this.Size/1024.0);
			else if (this.Size < 805306368) // between 768 KiB and 768 MiB
				return string.Format(SR.FormatFileSizeMB, this.Size/1048576.0);
			else if (this.Size < 824633720832) // between 768 MiB and 768 GiB
				return string.Format(SR.FormatFileSizeGB, this.Size/1073741824.0);

			// and finally, in the event of having a file greater than 768 GiB...
			return string.Format(SR.FormatFileSizeTB, this.Size/1099511627776.0);
		}

		public static FileSize Parse(string s)
		{
			FileSize size;
			if (TryParse(s, out size))
				return size;
			throw new FormatException("Parameter was not in the expected format - file size can be specified in bytes, KiB, MiB, GiB, TiB, or a synonym thereof.");
		}

		public static bool TryParse(string s, out FileSize fileSize)
		{
			if (!string.IsNullOrEmpty(s))
			{
				Match m = _pattern.Match(s.Trim());
				if (m.Success)
				{
					long byteCount;
					double value = double.Parse(m.Groups[1].Value);
					switch (m.Groups[2].Value.ToLowerInvariant())
					{
						case "tb":
						case "tbyte":
						case "tbytes":
						case "terabyte":
						case "terabytes":
						case "tib":
						case "tibibyte":
						case "tibibytes":
							byteCount = (long) Math.Ceiling(value*1024*1024*1024*1024);
							break;
						case "gb":
						case "gbyte":
						case "gbytes":
						case "gigabyte":
						case "gigabytes":
						case "gib":
						case "gibibyte":
						case "gibibytes":
							byteCount = (long) Math.Ceiling(value*1024*1024*1024);
							break;
						case "mb":
						case "mbyte":
						case "mbytes":
						case "megabyte":
						case "megabytes":
						case "mib":
						case "mebibyte":
						case "mebibytes":
							byteCount = (long) Math.Ceiling(value*1024*1024);
							break;
						case "kb":
						case "kbyte":
						case "kbytes":
						case "kilobyte":
						case "kilobytes":
						case "kib":
						case "kibibyte":
						case "kibibytes":
							byteCount = (long) Math.Ceiling(value*1024);
							break;
						case "b":
						case "byte":
						case "bytes":
						case "":
							byteCount = (long) value;
							break;
						default:
							fileSize = Empty;
							return false;
					}
					fileSize = new FileSize(byteCount);
					return true;
				}
			}
			fileSize = Empty;
			return false;
		}

		public static implicit operator long(FileSize x)
		{
			return x.Size;
		}

		public static implicit operator FileSize(long x)
		{
			return new FileSize(x);
		}

		#region Operators

		public static bool operator ==(FileSize x, FileSize y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(FileSize x, FileSize y)
		{
			return !x.Equals(y);
		}

		public static bool operator >(FileSize x, FileSize y)
		{
			return x.CompareTo(y) > 0;
		}

		public static bool operator <(FileSize x, FileSize y)
		{
			return x.CompareTo(y) < 0;
		}

		public static bool operator >=(FileSize x, FileSize y)
		{
			return x.CompareTo(y) >= 0;
		}

		public static bool operator <=(FileSize x, FileSize y)
		{
			return x.CompareTo(y) <= 0;
		}

		#endregion
	}
}