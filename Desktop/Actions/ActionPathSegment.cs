using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Actions
{
    /// <summary>
    /// Represents a single segment of an <see cref="ActionPath"/>.
    /// </summary>
    public class ActionPathSegment
    {
        private string _key;
        private string _localized;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="key">The resource key or unlocalized path segment string</param>
        /// <param name="localized">The localized path segment string</param>
        internal ActionPathSegment(string key, string localized)
        {
            _key = key;
            _localized = localized;
        }

        /// <summary>
        /// The resource key or unlocalized text
        /// </summary>
        public string ResourceKey
        {
            get { return _key; }
        }

        /// <summary>
        /// The localized text
        /// </summary>
        public string LocalizedText
        {
            get { return _localized; }
        }
    }
}
