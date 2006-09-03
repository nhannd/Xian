using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("open", "dicomstudybrowser-toolbar/Open")]
	[MenuAction("open", "dicomstudybrowser-contextmenu/Open")]
	[ClickHandler("open", "OpenStudy")]
	[EnabledStateObserver("open", "OpenStudyEnabled", "OpenStudyEnabledChanged")]
	[Tooltip("open", "Open Study")]
	[IconSet("open", IconScheme.Colour, "Icons.OpenStudySmall.png", "Icons.OpenStudySmall.png", "Icons.OpenStudySmall.png")]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class OpenStudyTool : Tool
	{
		private bool _openStudyEnabled;
		private event EventHandler _openStudyEnabledChangedEvent;

		public OpenStudyTool()
		{

		}

		protected IStudyBrowserToolContext Context
		{
			get { return this.ContextBase as IStudyBrowserToolContext; }
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Context.DefaultActionHandler = OpenStudy;
			this.Context.StudyBrowserComponent.SelectedStudyChanged += new EventHandler(OnSelectedStudyChanged);
		}

		void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			if (this.Context.StudyBrowserComponent.SelectedStudy != null)
				this.OpenStudyEnabled = true;
			else
				this.OpenStudyEnabled = false;
		}

		public void OpenStudy()
		{
			this.Context.StudyBrowserComponent.Open();
		}

		public bool OpenStudyEnabled
		{
			get { return _openStudyEnabled; }
			set
			{
				if (_openStudyEnabled != value)
				{
					_openStudyEnabled = value;
					EventsHelper.Fire(_openStudyEnabledChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler OpenStudyEnabledChanged
		{
			add { _openStudyEnabledChangedEvent += value; }
			remove { _openStudyEnabledChangedEvent -= value; }
		}
	}
}
