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

using System.Reflection;
using ClearCanvas.Common.Utilities;
using System;
using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a path.
    /// </summary>
    /// <remarks>
    /// Instances of this class are immutable.
    /// </remarks>
    public class Path
    {
		/// <summary>
		/// Gets the empty <see cref="Path"/> object.
		/// </summary>
		public static Path Empty = new Path(new PathSegment[]{});


		private const string SEPARATOR = "/";
    	private const string ESCAPED_SEPARATOR = "//";
		private const string TEMP = "__$:$__";

        private readonly List<PathSegment> _segments;

        /// <summary>
        /// Creates a new <see cref="Path"/> from the specified path string, resolving
        /// resource keys in the path string using the specified <see cref="ResourceResolver"/>.
        /// </summary>
        /// <remarks>
        /// The path string may contain any combination of literals and resource keys.  Localization
        /// will be attempted on each path segment by treating the segment as a resource key,
        /// and path segments that fail as resource keys will be treated as literals.
        /// </remarks>
        /// <param name="pathString">The path string to parse.</param>
        /// <param name="resolver">The <see cref="IResourceResolver"/> to use for localization.</param>
        public Path(string pathString, IResourceResolver resolver)
			:this(ParsePathString(pathString, resolver))
        {
        }

		/// <summary>
		/// Creates a new <see cref="Path"/> from the specified path string, with no resource resolver.
		/// </summary>
		/// <remarks>
		/// The path string must only contain literals, because there is no resource resolver to perform
		/// localization.
		/// </remarks>
		/// <param name="pathString"></param>
		public Path(string pathString)
			:this(ParsePathString(pathString, new ResourceResolver(new Assembly[]{})))
		{
		}

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="segments"></param>
        private Path(IEnumerable<PathSegment> segments)
        {
            _segments = new List<PathSegment>(segments);
        }

        /// <summary>
        /// Gets the individual segments contained in this path.
        /// </summary>
        public IList<PathSegment> Segments
        {
            get { return _segments.AsReadOnly(); }
        }

        /// <summary>
        /// The final segment in this path, or null if this path is empty.
        /// </summary>
        public PathSegment LastSegment
        {
            get { return CollectionUtils.LastElement(_segments); }
        }

		/// <summary>
		/// Gets a new <see cref="Path"/> object representing the specified sub-path.
		/// </summary>
		public Path SubPath(int start, int count)
		{
			return new Path(_segments.GetRange(start, count));
		}

		/// <summary>
		/// Gets the full path string, localized.
		/// </summary>
		public string LocalizedPath
		{
			get
			{
				return StringUtilities.Combine(_segments, SEPARATOR,
					s => s.LocalizedText.Replace(SEPARATOR, ESCAPED_SEPARATOR), false);
			}
		}

		/// <summary>
		/// Returns a new <see cref="Path"/> object obtained by appending <paramref name="other"/> path
		/// to this path.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Path Append(Path other)
		{
			var combined = new List<PathSegment>(_segments);
			combined.AddRange(other.Segments);
			
			return new Path(combined);
		}

        /// <summary>
        /// Converts this path back to a string.
        /// </summary>
        public override string ToString()
        {
			return StringUtilities.Combine(_segments, SEPARATOR,
				s => s.ResourceKey.Replace(SEPARATOR, ESCAPED_SEPARATOR), false);
		}

		/// <summary>
		/// Returns a new <see cref="Path"/> object representing the longest common path
		/// between this object and <paramref name="other"/>.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public Path GetCommonPath(Path other)
		{
			var commonPath = new List<PathSegment>();
			for(var i = 0; i < Math.Min(_segments.Count, other.Segments.Count); i++)
			{
				if(_segments[i] == other.Segments[i])
					commonPath.Add(_segments[i]);
				else
					break;	// must break as soon as paths differ
			}

			return new Path(commonPath);
		}

        /// <summary>
        /// Returns true if this path starts with <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool StartsWith(Path other)
        {
            // if other path is longer, then this path can't possibly "start with" it
            if (other.Segments.Count > _segments.Count)
                return false;

            // check that segments are equal up to length of other path
            for (var i = 0; i < other.Segments.Count; i++)
            {
                if (!Equals(_segments[i], other.Segments[i]))
                    return false;
            }
            return true;
        }

        private static PathSegment[] ParsePathString(string pathString, IResourceResolver resolver)
		{
			// replace any escaped separators with some weird temporary string
			pathString = StringUtilities.EmptyIfNull(pathString).Replace(ESCAPED_SEPARATOR, TEMP);

			// split string by separator
			var parts = pathString.Split(new[] { SEPARATOR }, StringSplitOptions.None);
			var n = parts.Length;
			var segments = new PathSegment[n];
			for (var i = 0; i < n; i++)
			{
				// replace the temp string with the unescaped separator
				parts[i] = parts[i].Replace(TEMP, SEPARATOR);
				segments[i] = new PathSegment(parts[i], resolver != null ? resolver.LocalizeString(parts[i]) : parts[i]);
			}
			return segments;
		}

	}
}
