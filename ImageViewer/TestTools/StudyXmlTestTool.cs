#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		private string _lastFolder = null;

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
			args.Path = _lastFolder ?? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

			FileDialogResult result = base.Context.DesktopWindow.ShowSelectFolderDialogBox(args);
			if (result.Action == DialogBoxAction.Ok)
			{
				_lastFolder = result.FileName;

				StudyLoaderExtensionPoint xp = new StudyLoaderExtensionPoint();
				IStudyLoader loader = (IStudyLoader)CollectionUtils.SelectFirst(xp.CreateExtensions(),
					delegate(object extension) { return ((IStudyLoader) extension).Name == "DICOM_LOCAL";});

				var selected = base.Context.SelectedStudy;

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
