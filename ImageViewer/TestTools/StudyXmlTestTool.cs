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

				loader.Start(new StudyLoaderArgs(selected.StudyInstanceUID, selected.Server));
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
