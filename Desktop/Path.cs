using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents an action path.  An action path specifies the location of an action within
    /// a menu or toolbar hierarchy.
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
        /// <param name="path">The path string to parse</param>
        /// <param name="resolver">The <see cref="ResourceResolver"/> to use for localization</param>
        public Path(string pathString, ResourceResolver resolver)
        {
            string[] parts = pathString.Split(new char[] { SEPARATOR });

            int n = parts.Length;
            _segments = new PathSegment[n];
            for (int i = 0; i < n; i++)
            {
                _segments[i] = new PathSegment(parts[i], resolver != null ? resolver.Resolve(parts[i]) : parts[i]);
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
