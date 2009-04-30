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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using System.IO;

namespace ClearCanvas.Desktop.Actions
{
#if DEBUG   // only include this tool in debug builds

	/// <summary>
	/// Exports the in-memory action model to a file.
	/// </summary>
	[MenuAction("apply", "global-menus/MenuTools/MenuUtilities/Export Action Model", "Apply")]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class ExportActionModelTool : Tool<IDesktopToolContext>
	{
		internal void Apply()
		{
			FileDialogResult result = this.Context.DesktopWindow.ShowSaveFileDialogBox(new FileDialogCreationArgs("actionmodel.xml"));
			if(result.Action == DialogBoxAction.Ok)
			{
				try
				{
					using (StreamWriter sw = File.CreateText(result.FileName))
					{
						using (XmlTextWriter writer = new XmlTextWriter(sw))
						{
							writer.Formatting = Formatting.Indented;
							ActionModelSettings.Default.Export(writer);
						}
					}
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, this.Context.DesktopWindow);
				}
			}
		}
	}

#endif
}
