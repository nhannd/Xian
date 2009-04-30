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
	/// A <see cref="TabGroup"/> to be hosted within a <see cref="TabGroupComponentContainer"/>.
	/// </summary>
    public class TabGroup
    {
        private float _weight;
        private TabComponentContainer _tabContainer;

        private ApplicationComponentHost _host;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tabContainer">The owning container.</param>
        /// <param name="initialWeight">Initial weight of the tab group, relative to other tab groups.</param>
        public TabGroup(TabComponentContainer tabContainer, float initialWeight)
        {
            _tabContainer = tabContainer;
            _weight = initialWeight;
        }

        /// <summary>
        /// Gets the owner <see cref="TabComponentContainer"/>.
        /// </summary>
        public TabComponentContainer Component
        {
            get { return _tabContainer; }
        }

        /// <summary>
        /// Gets the weight assigned to this group, relative to the other groups.
        /// </summary>
        public float Weight
        {
            get { return _weight; }
        }

        /// <summary>
        /// Gets or sets the component host for this pane.
        /// </summary>
        /// <remarks>
		/// For internal framework use only.
		/// </remarks>
        public ApplicationComponentHost ComponentHost
        {
            get { return _host; }
            internal set { _host = value; }
        }
    }
}
