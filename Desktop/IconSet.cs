using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a set of icon resources that specify the same logical icon in different sizes.
    /// </summary>
    public class IconSet
    {
        private string _small;
        private string _medium;
        private string _large;
        private IconScheme _scheme;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="scheme">The scheme of this icon set</param>
        /// <param name="smallIcon">The resource name of the small icon</param>
        /// <param name="mediumIcon">The resource name of the medium icon</param>
        /// <param name="largeIcon">The resource name of the large icon</param>
        public IconSet(IconScheme scheme, string smallIcon, string mediumIcon, string largeIcon)
        {
            _scheme = scheme;
            _small = smallIcon;
            _medium = mediumIcon;
            _large = largeIcon;
        }

        /// <summary>
        /// The scheme of this icon set.
        /// </summary>
        public IconScheme Scheme { get { return _scheme; } }

        /// <summary>
        /// The resource name of the small icon
        /// </summary>
        public string SmallIcon { get { return _small; } }

        /// <summary>
        /// The resource name of the medium icon
        /// </summary>
        public string MediumIcon { get { return _medium; } }

        /// <summary>
        /// The resource name of the large icon
        /// </summary>
        public string LargeIcon { get { return _large; } }
    }
}
