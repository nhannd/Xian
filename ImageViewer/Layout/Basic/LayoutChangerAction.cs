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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	/// <summary>
	/// Extension point for views of the <see cref="LayoutChangerAction"/>.
	/// </summary>
	[ExtensionPoint]
	public class LayoutChangerActionViewExtensionPoint : ExtensionPoint<IActionView> {}

	/// <summary>
	/// A custom action that allows the user to select a layout size.
	/// </summary>
	[AssociateView(typeof (LayoutChangerActionViewExtensionPoint))]
	public class LayoutChangerAction : Action
	{
		private readonly SetLayoutCallback _setLayoutCallback;
		private readonly int _maxRows;
		private readonly int _maxColumns;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionID">The logical action ID.</param>
		/// <param name="maxRows">The maximum number of rows that the user can select.</param>
		/// <param name="maxColumns">The maximum number of columns that the user can select.</param>
		/// <param name="callback">A <see cref="SetLayoutCallback"/> delegate that will be called when the user selects a layout size.</param>
		/// <param name="path">The action path.</param>
		/// <param name="resourceResolver">A resource resolver that will be used to resolve icons associated with this action.</param>
		public LayoutChangerAction(string actionID, int maxRows, int maxColumns, SetLayoutCallback callback, ActionPath path, IResourceResolver resourceResolver)
			: base(actionID, path, resourceResolver)
		{
			Platform.CheckForNullReference(callback, "callback");

			base.Label = path.LastSegment.LocalizedText;
			_setLayoutCallback = callback;
			_maxRows = maxRows;
			_maxColumns = maxColumns;
		}

		/// <summary>
		/// Gets the maximum number of rows that the user can select.
		/// </summary>
		public int MaxRows
		{
			get { return _maxRows; }
		}

		/// <summary>
		/// Gets the maximum number of columns that the user can select.
		/// </summary>
		public int MaxColumns
		{
			get { return _maxColumns; }
		}

		/// <summary>
		/// The method called by the view to set the user-selected layout size.
		/// </summary>
		/// <remarks>
		/// This method invokes the callback delegate provided in the
		/// <see cref="LayoutChangerAction(string, int, int, SetLayoutCallback, ActionPath, IResourceResolver)"/> constructor.
		/// </remarks>
		/// <param name="rows">The number of rows that the user selected.</param>
		/// <param name="columns">The number of columns that the user selected.</param>
		public void SetLayout(int rows, int columns)
		{
			_setLayoutCallback(rows, columns);
		}
	}

	/// <summary>
	/// Callback method to set the layout size.
	/// </summary>
	/// <param name="rows">The number of rows that the user selected.</param>
	/// <param name="columns">The number of columns that the user selected.</param>
	public delegate void SetLayoutCallback(int rows, int columns);
}