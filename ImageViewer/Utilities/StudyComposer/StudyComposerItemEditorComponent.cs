using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Utilities.StudyBuilder;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer
{
	[ExtensionPoint]
	public class StudyComposerItemEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (StudyComposerItemEditorComponentViewExtensionPoint))]
	public class StudyComposerItemEditorComponent : ApplicationComponent
	{
		private readonly IStudyComposerItem _item;

		public StudyComposerItemEditorComponent(IStudyComposerItem item)
		{
			_item = item;
		}

		public string Name
		{
			get { return _item.Name; }
			set { _item.Name = value; }
		}

		public string Description
		{
			get { return _item.Description; }
		}

		public Image Icon
		{
			get { return _item.Icon; }
		}

		public StudyBuilderNode Node
		{
			get { return _item.Node; }
		}

		public void Ok()
		{
			Apply();
			base.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			base.Exit(ApplicationComponentExitCode.None);
		}

		public void Apply()
		{
		}
	}
}