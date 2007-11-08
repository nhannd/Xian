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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	// TODO (Norman): Consider removing this class, since you can
	// just create the ILayoutManager extension yourself and 
	// pass it into the ImageViewerComponent 

	/// <summary>
	/// Defines an extension point for image layout management.
	/// </summary>
	[ExtensionPoint()]
	public class LayoutManagerExtensionPoint : ExtensionPoint<ILayoutManager>
	{
	}

	/// <summary>
	/// An <see cref="ImageViewerComponent"/> that supports layouts.
	/// </summary>
	public class DiagnosticImageViewerComponent : ImageViewerComponent
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DiagnosticImageViewerComponent"/>.
		/// </summary>
		/// <remarks>Upon instantiation, an extension of 
		/// <see cref="LayoutManagerExtensionPoint"/> is automatically created.</remarks>
		/// <exception cref="NotSupportedException">An extension of <see cref="LayoutManagerExtensionPoint"/>
		/// could not be found.</exception>
		public DiagnosticImageViewerComponent() : base(CreateLayoutManager())
		{
		}


		#region IApplicationComponent methods

		public override void Start()
		{
			base.Start();
			
			// Can add DiagnosticImageViewerComponent specific tools here by calling 
			// this.ToolSet.AddTool().  We would need to define a 
			// DiagnosticImageViewerToolContext first.

		}

		public override void Stop()
		{
			base.Stop();
		}

		#endregion

		private static ILayoutManager CreateLayoutManager()
		{
			try
			{
				LayoutManagerExtensionPoint xp = new LayoutManagerExtensionPoint();
				ILayoutManager layoutManager = (ILayoutManager)xp.CreateExtension();
				return layoutManager;
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Error, e);
				throw e;
			}
		}
	}
}
