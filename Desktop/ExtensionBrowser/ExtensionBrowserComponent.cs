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

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

using ClearCanvas.Desktop.ExtensionBrowser.PluginView;
using ClearCanvas.Desktop.ExtensionBrowser.ExtensionPointView;

namespace ClearCanvas.Desktop.ExtensionBrowser
{
    /// <summary>
    /// Defines an extension point for views onto the <see cref="ExtensionBrowserComponent"/>
    /// </summary>
    [ExtensionPoint()]
	public sealed class ExtensionBrowserComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// Component that displays the set of installed extensions in a tree view.
    /// </summary>
    [AssociateView(typeof(ExtensionBrowserComponentViewExtensionPoint))]
    public class ExtensionBrowserComponent : ApplicationComponent
    {
        private PluginViewRootNode _pluginViewTree;
        private ExtensionPointViewRootNode _extPtViewTree;


        public override void Start()
        {
            base.Start();

            // create the browser trees here - this will not incur any real cost,
            // because each level of the browser tree does not load it's children until requested
            _pluginViewTree = new PluginViewRootNode();
            _extPtViewTree = new ExtensionPointViewRootNode();
        }

        /// <summary>
        /// Gets a tree model that represents a "plugin-centric" view of the extensions
        /// </summary>
        public PluginViewRootNode PluginTree
        {
            get { return _pluginViewTree; }
        }

        /// <summary>
        /// Gets a tree model that represents an "extension point centric" view of the extensions
        /// </summary>
        public ExtensionPointViewRootNode ExtensionPointTree
        {
            get { return _extPtViewTree; }
        }
    }
}
