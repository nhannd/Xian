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

namespace ClearCanvas.Desktop
{
	public class SplitPane
	{
        private IApplicationComponent _component;
		private string _name;
        private float _weight;
		private bool _fixed;

        private SplitComponentContainer.SplitPaneHost _host;

		public SplitPane(string name, IApplicationComponent component, bool fix)
		{
			_name = name;
			_component = component;
			_weight = 0F;
			_fixed = fix;
		}

		/// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the pane</param>
        /// <param name="component">Component that the pane will host</param>
        /// <param name="initialWeight">Initial weight of the pane, relative to other panes</param>
        public SplitPane(string name, IApplicationComponent component, float initialWeight)
        {
            _name = name;
            _component = component;
            _weight = initialWeight;
			_fixed = false;
        }

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

		public bool Fixed
		{
			get { return _fixed; }
		}

        /// <summary>
        /// Gets the component host for this pane.  For internal use only.
        /// </summary>
        public SplitComponentContainer.SplitPaneHost ComponentHost
        {
            get { return _host; }
            set { _host = value; }
        }
	}
}
