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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Models a user-interface action, such as a menu or toolbar item, in a GUI-toolkit independent way.
    /// </summary>
    /// <remarks>
    /// Provides the base interface for a set of types that model user-interface actions
    /// independent of any particular GUI-toolkit.
    /// </remarks>
    public interface IAction
    {
		/// <summary>
        /// Occurs when the <see cref="Enabled"/> property of this action changes.
        /// </summary>
        event EventHandler EnabledChanged;

        /// <summary>
        /// Occurs when the <see cref="Visible"/> property of this action changes.
        /// </summary>
        event EventHandler VisibleChanged;

        /// <summary>
        /// Occurs when the <see cref="Label"/> property of this action changes.
        /// </summary>
		event EventHandler LabelChanged;

        /// <summary>
        /// Occurs when the <see cref="Tooltip"/> property of this action changes.
        /// </summary>
		event EventHandler TooltipChanged;

		/// <summary>
		/// Occurs when the <see cref="IconSet"/> property of this action changes.
		/// </summary>
		event EventHandler IconSetChanged;

        /// <summary>
        /// Gets the fully-qualified logical identifier for this action.
        /// </summary>
        string ActionID { get; }

        /// <summary>
        /// Gets or sets the menu or toolbar path for this action.
        /// </summary>
        ActionPath Path { get; set; }

		/// <summary>
		/// Gets or sets the group hint for this action.
		/// </summary>
		/// <remarks>
        /// The GroupHint for an action must not be null.  If an action has no groupHint,
		/// the GroupHint should be "" (default).
		/// </remarks>
		GroupHint GroupHint { get; set; }

        /// <summary>
        /// Gets the label that the action presents in the UI.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the tooltip that the action presents in the UI.
        /// </summary>
        string Tooltip { get; }

        /// <summary>
        /// Gets the icon that the action presents in the UI.
        /// </summary>
        IconSet IconSet { get; }

        /// <summary>
        /// Gets the enablement state that the action presents in the UI.
        /// </summary>
        bool Enabled { get; }

		/// <summary>
        /// Gets the visibility state that the action presents in the UI.
		/// </summary>
		bool Visible { get; }

		/// <summary>
		/// Gets a value indicating whether or not the action is 'persistent'.
		/// </summary>
        /// <remarks>
        /// Actions created via the Action Attributes are considered persistent and are
        /// committed to the <see cref="ActionModelSettings"/>,
        /// otherwise they are considered generated and they are not committed.
		/// </remarks>
		bool Persistent { get; }

        /// <summary>
        /// Gets the resource resolver associated with this action, that will be used to resolve
        /// action path and icon resources when required.
        /// </summary>
        IResourceResolver ResourceResolver { get; }

        /// <summary>
        /// Gets a value indicating whether this action is permissible.
        /// </summary>
        /// <remarks>
        /// In addition to the <see cref="Visible"/> and <see cref="Enabled"/> properties, the view
        /// will use this property to control whether the action can be invoked.  Typically
        /// this property is implemented to indicate whether the current user has permission
        /// to execute the action.
        /// </remarks>
        bool Permissible { get; }
   }
}
