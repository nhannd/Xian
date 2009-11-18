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

				string pathString = String.Format("{0}/{1}", context.BasePath, actionId);
				ActionPath path = new ActionPath(pathString, null);
				return new MenuAction(fullyQualifiedActionId, path, ClickActionFlags.CheckParents, null);
			}

			#region IContextMenuActionFactory Members

			public abstract IAction[] CreateActions(IActionFactoryContext context);

			#endregion
		}

		#endregion
	}
}