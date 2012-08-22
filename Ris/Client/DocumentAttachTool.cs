#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("apply", "attached-document-items/Add", "Apply")]
	[ButtonAction("apply", "attached-document-items/Add", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.AddToolSmall.png", "Icons.AddToolSmall.png", "Icons.AddToolSmall.png")]
	[VisibleStateObserver("apply", "Visible", "VisibleChanged")]
	[ExtensionOf(typeof(AttachedDocumentToolExtensionPoint))]
	public class DocumentAttachTool : Tool<IAttachedDocumentToolContext>
	{

		public bool Visible
		{
			get { return !this.Context.IsReadonly; }
		}

		public event EventHandler VisibleChanged;

		public void Apply()
		{
			var component = new AttachDocumentComponent(this.Context.Site);
			var exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, component, "Attach File");
			if(exitCode == ApplicationComponentExitCode.Accepted)
			{
				this.Context.AddAttachment(component.Document, component.Category);
			}
		}
	}
}

