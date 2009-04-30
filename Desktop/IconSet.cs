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
        /// Constructor.
        /// </summary>
        /// <param name="scheme">The scheme of this icon set.</param>
        /// <param name="smallIcon">The resource name of the small icon.</param>
        /// <param name="mediumIcon">The resource name of the medium icon.</param>
        /// <param name="largeIcon">The resource name of the large icon.</param>
        public IconSet(IconScheme scheme, string smallIcon, string mediumIcon, string largeIcon)
        {
            _scheme = scheme;
            _small = smallIcon;
            _medium = mediumIcon;
            _large = largeIcon;
        }

        /// <summary>
        /// Constructor that assumes all the icons are colour and have the same size.
        /// </summary>
        /// <param name="icon">The resource name of the icon.</param>
        public IconSet(string icon)
        {
            _scheme = IconScheme.Colour;
            _small = icon;
            _medium = icon;
            _large = icon;
        }

        /// <summary>
        /// The scheme of this icon set.
        /// </summary>
        public IconScheme Scheme { get { return _scheme; } }

        /// <summary>
        /// The resource name of the small icon.
        /// </summary>
        public string SmallIcon { get { return _small; } }

        /// <summary>
        /// The resource name of the medium icon.
        /// </summary>
        public string MediumIcon { get { return _medium; } }

        /// <summary>
        /// The resource name of the large icon.
        /// </summary>
        public string LargeIcon { get { return _large; } }
    }
}
