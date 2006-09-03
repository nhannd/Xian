using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("delete", "dicomstudybrowser-toolbar/Delete")]
	[MenuAction("delete", "dicomstudybrowser-contextmenu/Delete")]
	[ClickHandler("delete", "DeleteStudy")]
	[EnabledStateObserver("delete", "DeleteStudyEnabled", "DeleteStudyEnabledChanged")]
	[Tooltip("delete", "Delete Study")]
	[IconSet("delete", IconScheme.Colour, "Icons.DeleteStudySmall.png", "Icons.DeleteStudySmall.png", "Icons.DeleteStudySmall.png")]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class DeleteStudyTool : Tool
	{
		private bool _openEnabled;
		private event EventHandler _openEnabledChangedEvent;

		public DeleteStudyTool()
		{

		}

		protected IStudyBrowserToolContext Context
		{
			get { return this.ContextBase as IStudyBrowserToolContext; }
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Context.DefaultActionHandler = DeleteStudy;
			this.Context.StudyBrowserComponent.SelectedStudyChanged += new EventHandler(OnSelectedStudyChanged);
		}

		void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			if (this.Context.StudyBrowserComponent.SelectedStudy != null)
				this.DeleteStudyEnabled = true;
			else
				this.DeleteStudyEnabled = false;
		}

		public void DeleteStudy()
		{
			//this.Context.StudyBrowserComponent.Delete();
		}

		public bool DeleteStudyEnabled
		{
			get { return _openEnabled; }
			set
			{
				if (_openEnabled != value)
				{
					_openEnabled = value;
					EventsHelper.Fire(_openEnabledChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler DeleteStudyEnabledChanged
		{
			add { _openEnabledChangedEvent += value; }
			remove { _openEnabledChangedEvent -= value; }
		}
	}
}
