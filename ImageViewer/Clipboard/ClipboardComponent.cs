#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Annotations;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard
{
	[ExtensionPoint()]
	public sealed class ClipboardToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	/// <summary>
	/// Extension point for views onto <see cref="ClipboardComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ClipboardComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ClipboardComponent class.
	/// </summary>
	[AssociateView(typeof(ClipboardComponentViewExtensionPoint))]
	public class ClipboardComponent : ApplicationComponent
	{
		private class ClipboardToolContext : ToolContext, IClipboardToolContext
		{
			readonly ClipboardComponent _component;

			public ClipboardToolContext(ClipboardComponent component)
			{
				Platform.CheckForNullReference(component, "component");
				_component = component;
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public IList<IClipboardItem> ClipboardItems
			{
				get { return _component.ClipboardItemWrapper; }
			}

			public ReadOnlyCollection<IClipboardItem> SelectedClipboardItems
			{
				get { return _component.SelectedItems; }
			}

			public event EventHandler ClipboardItemsChanged
			{
				add { _component._clipboardItemsChanged += value; }
				remove { _component._clipboardItemsChanged -= value; }
			}

			public event EventHandler SelectedClipboardItemsChanged
			{
				add { _component._selectedClipboardItemsChanged += value; }
				remove { _component._selectedClipboardItemsChanged -= value; }
			}
		}

		private ToolSet _toolSet;
		private ActionModelRoot _toolbarModel;
		private ActionModelRoot _contextMenuModel;
		private ISelection _selection;
		private event EventHandler _selectedClipboardItemsChanged;
		private event EventHandler _clipboardItemsChanged;

		private readonly ClipboardItemList _clipboardItemWrapper;

		private static readonly BindingList<IClipboardItem> _clipboardItems
			= new BindingList<IClipboardItem>();

		/// <summary>
		/// Constructor.
		/// </summary>
		internal ClipboardComponent()
		{
			_clipboardItemWrapper = new ClipboardItemList(_clipboardItems);
		}

		// Specifically for tools to limit removal of clipboard items.
		internal IList<IClipboardItem> ClipboardItemWrapper
		{
			get { return _clipboardItemWrapper; }
		}

		#region Presentation Model

		public BindingList<IClipboardItem> ClipboardItems
		{
			get { return _clipboardItemWrapper.BindingList; }
		}

		public ReadOnlyCollection<IClipboardItem> SelectedItems
		{
			get
			{
				List<IClipboardItem> selectedItems = new List<IClipboardItem>();

				if (_selection != null)
				{
					foreach (IClipboardItem item in _selection.Items)
						selectedItems.Add(item);
				}

				return selectedItems.AsReadOnly();
			}
		}

		public ActionModelRoot ToolbarModel
		{
			get { return _toolbarModel; }
		}

		public ActionModelRoot ContextMenuModel
		{
			get { return _contextMenuModel; }
		}

		public void SetSelection(ISelection selection)
		{
			if (_selection != selection)
			{
				_selection = selection;
				EventsHelper.Fire(_selectedClipboardItemsChanged, this, EventArgs.Empty);
			}
		}

		#endregion

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			base.Start();

			_toolSet = new ToolSet(new ClipboardToolExtensionPoint(), new ClipboardToolContext(this));
			_toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "clipboard-toolbar", _toolSet.Actions);
			_contextMenuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "clipboard-contextmenu", _toolSet.Actions);

			_clipboardItems.ListChanged += OnBindingListChanged;
		}

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		public override void Stop()
		{
			_clipboardItems.ListChanged -= OnBindingListChanged;

			_toolSet.Dispose();
			_toolSet = null;

			base.Stop();
		}

		private void OnBindingListChanged(object sender, ListChangedEventArgs e)
		{
			EventsHelper.Fire(_clipboardItemsChanged, this, EventArgs.Empty);
		}

		internal static void AddToClipboard(IPresentationImage image)
		{
			Platform.CheckForNullReference(image, "image");

			Rectangle clientRectangle = image.ClientRectangle;
			
			image = image.Clone();
			HideTextOverlay(image);

			Bitmap bmp = IconCreator.CreatePresentationImageIcon(image);
			_clipboardItems.Add(new ClipboardItem(image, bmp, "", clientRectangle));
		}

		internal static void AddToClipboard(IDisplaySet displaySet)
		{
			AddToClipboard(displaySet, null);
		}

		internal static void AddToClipboard(IDisplaySet displaySet, IImageSelectionStrategy selectionStrategy)
		{
			Platform.CheckForNullReference(displaySet, "displaySet");

			if (displaySet.ImageBox == null || 
				displaySet.ImageBox.SelectedTile == null || 
				displaySet.ImageBox.SelectedTile.PresentationImage == null)
			{
				throw new ArgumentException("DisplaySet must have a selected image.");
			}

			Rectangle clientRectangle = displaySet.ImageBox.SelectedTile.PresentationImage.ClientRectangle;
			if (selectionStrategy == null)
			{
				displaySet = displaySet.Clone();
			}
			else
			{
				string name = String.Format("{0} - {1}", selectionStrategy.Description, displaySet.Name);
				IDisplaySet sourceDisplaySet = displaySet;
				displaySet = new DisplaySet(name, displaySet.Uid);
				foreach (IPresentationImage image in selectionStrategy.GetImages(sourceDisplaySet))
					displaySet.PresentationImages.Add(image.Clone());
			}

			HideTextOverlay(displaySet.PresentationImages);

			Bitmap bmp = IconCreator.CreateDisplaySetIcon(displaySet, clientRectangle);
			ClipboardItem item = new ClipboardItem(displaySet, bmp, displaySet.Name, clientRectangle);
			_clipboardItems.Add(item);
		}

		internal static void HideTextOverlay(IEnumerable<IPresentationImage> images)
		{
			foreach (IPresentationImage image in images)
				HideTextOverlay(image);
		}

		internal static void HideTextOverlay(IPresentationImage image)
		{
			if (image is IAnnotationLayoutProvider)
				((IAnnotationLayoutProvider) image).AnnotationLayout.Visible = false;
		}
	}
}
