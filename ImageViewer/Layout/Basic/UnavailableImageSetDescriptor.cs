using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	#region Descriptor

	public class UnavailableImageSetDescriptor : DicomImageSetDescriptor
	{
		internal UnavailableImageSetDescriptor(StudyItem studyItem, Exception loadStudyError)
			: base(studyItem)
		{
			LoadStudyError = loadStudyError;
		}

		public readonly Exception LoadStudyError;

		public new StudyItem SourceStudy
		{
			get { return (StudyItem)base.SourceStudy; }
		}

		protected override string GetName()
		{
			string serverName;
			if (SourceStudy.Server == null)
				serverName = SR.LabelUnknownServer;
			else
				serverName = SourceStudy.Server.ToString();

			return String.Format("({0}) {1}", serverName, base.GetName());
		}

		public bool IsOffline
		{
			get { return LoadStudyError is OfflineLoadStudyException; }
		}

		public bool IsNearline
		{
			get { return LoadStudyError is NearlineLoadStudyException; }
		}

		public bool IsInUse
		{
			get { return LoadStudyError is InUseLoadStudyException; }
		}

		public bool IsNotLoadable
		{
			get { return LoadStudyError is StudyLoaderNotFoundException; }
		}
	}

	#endregion

	#region Context Menu 

	[ExtensionOf(typeof(ContextMenuActionFactoryExtensionPoint))]
	public class UnavailableImageSetContextMenuActionFactory : ContextMenuActionFactory
	{
		public UnavailableImageSetContextMenuActionFactory()
		{ }

		private string GetActionMessage(UnavailableImageSetDescriptor descriptor)
		{
			if (descriptor.IsOffline)
				return SR.MessageActionStudyOffline;
			else if (descriptor.IsNearline)
				return SR.MessageActionStudyNearline;
			else if (descriptor.IsInUse)
				return SR.MessageActionStudyInUse;
			else if (descriptor.IsNotLoadable)
				return SR.MessageActionNoStudyLoader;
			else
				return SR.MessageActionStudyCouldNotBeLoaded;
		}

		private string GetActionLabel(UnavailableImageSetDescriptor descriptor)
		{
			if (descriptor.IsOffline)
				return String.Format(SR.LabelFormatStudyUnavailable, SR.Offline);
			else if (descriptor.IsNearline)
				return String.Format(SR.LabelFormatStudyUnavailable, SR.Nearline);
			else if (descriptor.IsInUse)
				return String.Format(SR.LabelFormatStudyUnavailable, SR.InUse);
			else if (descriptor.IsNotLoadable)
				return String.Format(SR.LabelFormatStudyUnavailable, SR.Unavailable);
			else
				return SR.LabelStudyCouldNotBeLoaded;
		}


		private IClickAction CreateUnavailableStudyAction(ContextMenuActionFactoryArgs args)
		{
			UnavailableImageSetDescriptor descriptor = (UnavailableImageSetDescriptor)args.ImageSet.Descriptor;

			MenuAction action = CreateAction(args);
			action.Label = GetActionLabel(descriptor);
			action.SetClickHandler(delegate
			{
				args.DesktopWindow.ShowMessageBox(GetActionMessage(descriptor), MessageBoxActions.Ok);
			});

			return action;
		}

		#region IContextMenuActionFactory Members

		public override IAction[] CreateActions(ContextMenuActionFactoryArgs args)
		{
			List<IAction> actions = new List<IAction>();
			if (args.ImageSet.Descriptor is UnavailableImageSetDescriptor)
				actions.Add(CreateUnavailableStudyAction(args));

			return actions.ToArray();
		}

		#endregion
	}

	#endregion
}