#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	partial class ContextMenuLayoutTool
	{
		#region Action Factory Definition

		[ExtensionPoint]
		public class ActionFactoryExtensionPoint : ExtensionPoint<IActionFactory>
		{
		}

		public interface IActionFactoryContext
		{
			IDesktopWindow DesktopWindow { get; }
			IImageViewer ImageViewer { get; }
			ActionPlaceholder ActionPlaceholder { get; }
			string Namespace { get; }
			string BasePath { get; }
			IImageSet ImageSet { get; }

			string GetNextActionId();
			string GetFullyQualifiedActionId(string actionId);

			bool ExcludeDefaultActions { get; set; }
		}

		public interface IActionFactory
		{
			IAction[] CreateActions(IActionFactoryContext context);
		}

		#endregion

		#region Action Factory Context

		private class ActionFactoryContext : IActionFactoryContext
		{
			private int _nextActionNumber;
			private bool _excludeDefaultActions;

			internal ActionFactoryContext()
			{
			}

			public IDesktopWindow DesktopWindow { get; internal set; }
			public IImageViewer ImageViewer { get; internal set; }
			public ActionPlaceholder ActionPlaceholder { get; internal set; }
			public string Namespace { get; internal set; }
			public string BasePath { get; internal set; }

			public IImageSet ImageSet { get; internal set; }

			public bool ExcludeDefaultActions
			{
				get { return _excludeDefaultActions; }
				set
				{
					if (value)
						_excludeDefaultActions = true;
				}
			}

			internal void Initialize(IImageSet imageSet, string basePath)
			{
				ImageSet = imageSet;
				BasePath = basePath;
				_excludeDefaultActions = false;	
			}

			public string GetNextActionId()
			{
				return String.Format("imageSetAction{0}", ++_nextActionNumber);
			}

			public string GetFullyQualifiedActionId(string actionId)
			{
				return String.Format("{0}:{1}", Namespace, actionId);
			}
		}

		#endregion

		#region ActionFactory Base class

		public abstract class ActionFactory : IActionFactory
		{
			protected ActionFactory()
			{
			}

			protected MenuAction CreateMenuAction(IActionFactoryContext context, string label, ClickHandlerDelegate clickHandler)
			{
				Platform.CheckForEmptyString(label, "label");
				Platform.CheckForNullReference(clickHandler, "clickHandler");

				MenuAction menuAction = CreateMenuAction(context);
				menuAction.Label = label;
				menuAction.SetClickHandler(clickHandler);
				return menuAction;
			}

			protected MenuAction CreateMenuAction(IActionFactoryContext context)
			{
				Platform.CheckForNullReference(context, "context");

				string actionId = context.GetNextActionId();
				string fullyQualifiedActionId = context.GetFullyQualifiedActionId(actionId);

				// build the path suffix by ripping off the site and appending the action id
				Path path = new Path(context.BasePath);
				path = path.SubPath(1, path.Segments.Count - 1);
				path = path.Append(new PathSegment(actionId));
				return context.ActionPlaceholder.CreateMenuAction(fullyQualifiedActionId, path.ToString(), ClickActionFlags.CheckParents, null);
			}

			#region IContextMenuActionFactory Members

			public abstract IAction[] CreateActions(IActionFactoryContext context);

			#endregion
		}

		#endregion
	}
}