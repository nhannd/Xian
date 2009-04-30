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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a single page in a <see cref="NavigatorComponentContainer"/>.
    /// </summary>
    public class NavigatorPage : ContainerPage
    {

        private Path _path;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The path to this page in the navigation tree.</param>
        /// <param name="component">The application component to be displayed by this page</param>
        public NavigatorPage(string path, IApplicationComponent component)
            :base(component)
        {
			Platform.CheckForEmptyString(path, "path");
			Platform.CheckForNullReference(component, "component");

            _path = new ClearCanvas.Desktop.Path(path, new ResourceResolver(new Assembly[] { component.GetType().Assembly }));
        }

        /// <summary>
        /// Gets the path to this page.
        /// </summary>
        public Path Path
        {
            get { return _path; }
        }

		/// <summary>
		/// Returns <see cref="ClearCanvas.Desktop.Path.ToString"/>.
		/// </summary>
		public override string ToString()
		{
			return this.Path.ToString();
		}
    }
}
