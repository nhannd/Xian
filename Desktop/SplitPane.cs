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
	/// A <see cref="SplitPane"/> hosts a single <see cref="IApplicationComponent"/> in one
	/// side of a <see cref="SplitComponentContainer"/>.
	/// </summary>
	public class SplitPane
	{
        private IApplicationComponent _component;
		private string _name;
        private float _weight;
		private bool _fixed;

		private ApplicationComponentHost _host;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the <see cref="SplitPane"/>.</param>
		/// <param name="component">The <see cref="IApplicationComponent"/> to be hosted.</param>
		/// <param name="fix">Whether or not the pane should be fixed (based on size).  Only one of the two <see cref="SplitPane"/>s can be fixed.</param>
		public SplitPane(string name, IApplicationComponent component, bool fix)
		{
			_name = name;
			_component = component;
			_weight = 0F;
			_fixed = fix;
		}

		/// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="name">The name of the <see cref="SplitPane"/>.</param>
		/// <param name="component">The <see cref="IApplicationComponent"/> to be hosted.</param>
		/// <param name="initialWeight">The initial weighting factor for determing the <see cref="SplitPane"/>'s initial size.</param>
		public SplitPane(string name, IApplicationComponent component, float initialWeight)
        {
            _name = name;
            _component = component;
            _weight = initialWeight;
			_fixed = false;
        }

		/// <summary>
		/// Gets the name of the <see cref="SplitPane"/>.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}
		
		/// <summary>
        /// Gets the component that is displayed on this pane.
        /// </summary>
        public IApplicationComponent Component
        {
            get { return _component; }
        }

        /// <summary>
        /// Gets the weight that was assigned to this pane
        /// </summary>
        public float Weight
        {
            get { return _weight; }
        }

		/// <summary>
		/// Gets whether or not this pane should be 'fixed', based on its size.
		/// </summary>
		public bool Fixed
		{
			get { return _fixed; }
		}

        /// <summary>
        /// Gets the component host for this pane.
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
