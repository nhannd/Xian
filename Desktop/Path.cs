#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a path.
    /// </summary>
    public class Path
    {
        const char SEPARATOR = '/';

        private PathSegment[] _segments;

        /// <summary>
        /// Creates a new <see cref="Path"/> from the specified path string, resolving
        /// resource keys in the path string using the specified <see cref="ResourceResolver"/>.
        /// </summary>
        /// <remarks>
        /// The path string may contain any combination of literals and resource keys.  Localization
        /// will be attempted on each path segment by treating the segment as a resource key,
        /// and path segments that fail as resource keys will be treated as literals.
        /// </remarks>
        /// <param name="pathString">The path string to parse</param>
        /// <param name="resolver">The <see cref="ResourceResolver"/> to use for localization</param>
        public Path(string pathString, IResourceResolver resolver)
        {
            string[] parts = pathString.Split(new char[] { SEPARATOR });

            int n = parts.Length;
            _segments = new PathSegment[n];
            for (int i = 0; i < n; i++)
            {
                _segments[i] = new PathSegment(parts[i], resolver != null ? resolver.LocalizeString(parts[i]) : parts[i]);
            }
        }

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="segments"></param>
        internal Path(PathSegment[] segments)
        {
            _segments = segments;
        }

        /// <summary>
        /// The set of individual segments contained in this path.
        /// </summary>
        public PathSegment[] Segments
        {
            get { return _segments; }
            set { _segments = value; }
        }

        /// <summary>
        /// The final segment in this path, or null if this path is empty
        /// </summary>
        public PathSegment LastSegment
        {
            get { return _segments.Length > 0 ? _segments[_segments.Length - 1] : null; }
        }

		public string LocalizedPath
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				foreach (PathSegment segment in _segments)
				{
					if (sb.Length > 0)
						sb.Append(SEPARATOR);
					sb.Append(segment.LocalizedText);
				}
				return sb.ToString();
			}
		}

        public string SubPath(int depth)
        {
            Platform.CheckIndexRange(depth, 0, _segments.Length - 1, "depth");

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= depth; i++)
            {
                if (sb.Length > 0)
                    sb.Append(SEPARATOR);

                sb.Append(_segments[i].LocalizedText);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts this path back to the original string from which it was created.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (PathSegment segment in _segments)
            {
                if (sb.Length > 0)
                    sb.Append(SEPARATOR);
                sb.Append(segment.ResourceKey);
            }
            return sb.ToString();
        }
    }
}
