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
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Text;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("create", "dicomstudybrowser-contextmenu/Create Study Xml", "Create")]

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class StudyXmlTestTool : StudyBrowserTool
	{
		public StudyXmlTestTool()
		{
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			base.Enabled = true;
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			base.Enabled = true;
		}

		public void Create()
		{
			
			SelectFolderDialogCreationArgs args = new SelectFolderDialogCreationArgs();
			args.Path = @"c:\stewart";
			FileDialogResult result = base.Context.DesktopWindow.ShowSelectFolderDialogBox(args);
			if (result.Action == DialogBoxAction.Ok)
			{
				StudyLoaderExtensionPoint xp = new StudyLoaderExtensionPoint();
				IStudyLoader loader = (IStudyLoader)CollectionUtils.SelectFirst(xp.CreateExtensions(),
					delegate(object extension) { return ((IStudyLoader) extension).Name == "DICOM_LOCAL";});

				StudyItem selected = base.Context.SelectedStudy;

				loader.Start(new StudyLoaderArgs(selected.StudyInstanceUid, selected.Server));
				StudyXml xml = new StudyXml();
				Sop sop;
				
				while (null != (sop = loader.LoadNextSop()))
				{
					xml.AddFile(((ILocalSopDataSource) sop.DataSource).File);
				}

				StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
				settings.IncludePrivateValues = StudyXmlTagInclusion.IgnoreTag;
				settings.IncludeUnknownTags = StudyXmlTagInclusion.IgnoreTag;
				settings.MaxTagLength = 100 * 1024;
				settings.IncludeSourceFileName = true;

				XmlDocument doc = xml.GetMemento(settings);
				string fileName = System.IO.Path.Combine(result.FileName, "studyxml.xml");

				XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8);
				writer.Formatting = Formatting.Indented;
				writer.Indentation = 5;
				
				doc.Save(writer);
			}
		}
	}
}
