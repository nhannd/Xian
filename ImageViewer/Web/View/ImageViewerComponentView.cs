#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Web.Common;
using ClearCanvas.ImageViewer.Web.Common.Entities;
using ClearCanvas.Web.Common.Entities;
using ClearCanvas.Web.Services;
using ClearCanvas.Web.Services.View;

namespace ClearCanvas.ImageViewer.Web.View
{
    [ExtensionOf(typeof(ImageViewerComponentViewExtensionPoint))]
    public class ImageViewerComponentView : WebApplicationComponentView<Viewer>
	{
        public class ExtensionEntitiesExtensionPoint : ExtensionPoint<IEntityHandler>
        {
        }

        [ThreadStatic] internal static ImageViewerComponentView Current;

		private ImageViewerComponent _viewer;

	    private readonly ToolStripSettings _toolStripSettings = new ToolStripSettings();
		private readonly List<ImageBoxView> _imageBoxViews = new List<ImageBoxView>();
		private readonly List<WebActionView> _actionViews = new List<WebActionView>();
        private readonly List<IEntityHandler> _extensionHandlers = new List<IEntityHandler>();

        private Entity[] GetExtensionEntities()
        {
            return _extensionHandlers.Select(h => h.GetEntity()).ToArray();
        }

        private Common.Entities.ImageBox[] GetImageBoxEntities()
	    {
	        return _imageBoxViews.Select(v => v.GetEntity()).ToArray();
	    }

		private WebActionNode[] GetToolbarActionEntities()
		{
			var toolbarActions = _actionViews.Select(v => (WebActionNode)v.GetEntity()).ToArray();
			return FlattenWebActions(toolbarActions).ToArray();
		}

        public override void SetModelObject(object modelObject)
		{
            // TODO (Phoenix5): Hack for now until we fully support the desktop view.
		    Current = this;

            _viewer = (ImageViewerComponent)modelObject;
		}

        protected override void UpdateEntity(Viewer entity)
		{
		    entity.ToolStripIconSize = LoadToolbarIconSizeFromSettings();
			entity.ImageBoxes = GetImageBoxEntities();
			entity.ToolbarActions = GetToolbarActionEntities();
		    entity.Extensions = GetExtensionEntities();
		}

		private void RefreshImageBoxViews(bool notify)
		{
			DisposeImageBoxViews();

			foreach (IImageBox imageBox in _viewer.PhysicalWorkspace.ImageBoxes)
			{
				var newView = new ImageBoxView();
				((IWebView)newView).SetModelObject(imageBox);
				_imageBoxViews.Add(newView);
			}

			if (notify)
				base.NotifyEntityPropertyChanged("ImageBoxes", GetImageBoxEntities()); 
		}

		public override void ProcessMessage(Message message)
		{
		}

        protected override void Initialize()
        {
            _viewer.PhysicalWorkspace.LayoutCompleted += OnLayoutCompleted;
            _viewer.PhysicalWorkspace.Drawing += OnPhysicalWorkspaceDrawing;
            _viewer.EventBroker.PresentationImageSelected += OnPresentationImageSelected;

            UpdateActionModel(false);
            RefreshImageBoxViews(false);

            try
            {
                foreach (var extensionHandler in new ExtensionEntitiesExtensionPoint().CreateExtensions().Cast<IEntityHandler>())
                {
                    // TODO (CR Nov 2012): Should the model object be the viewer?
                    extensionHandler.SetModelObject(_viewer);
                    _extensionHandlers.Add(extensionHandler);
                }
            }
            catch (NotSupportedException)
            {
            }
        }
        
        private void OnLayoutCompleted(object sender, EventArgs e)
		{
            RefreshImageBoxViews(true);
		}

		private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			//TODO (CR May 2010): this is not ideal.  We should actually implement DropDownMenuModelChanged on the
			//dropdown action classes, which is really the only reason for this being here.
			foreach (var actionView in _actionViews)
				actionView.Update();
		}

		void OnPhysicalWorkspaceDrawing(object sender, EventArgs e)
		{
            foreach (ImageBoxView imageBoxView in _imageBoxViews)
                imageBoxView.Draw(true);
		}

		private void UpdateActionModel(bool notify)
		{
			DisposeActionViews();
			_actionViews.AddRange(WebActionView.Create(_viewer.ToolbarModel.ChildNodes));
			if (!notify)
				return;

            NotifyEntityPropertyChanged("ToolbarActions", GetToolbarActionEntities());
		}

        private WebIconSize LoadToolbarIconSizeFromSettings()
        {
            try
            {
                switch (_toolStripSettings.IconSize)
                {
                    case IconSize.Large:
                        return WebIconSize.Large;
                    case IconSize.Medium:
                        return WebIconSize.Medium;
                    case IconSize.Small:
                        return WebIconSize.Small;

                    default:
                        return WebIconSize.Medium;
                }
            }
            catch (Exception ex)
            {
                // eat it since it's not really important
                Platform.Log(LogLevel.Error, ex, "Unable to read icon size from settings. Default to Medium.");
                return WebIconSize.Medium;
            }
        }


		private static List<WebActionNode> FlattenWebActions(IEnumerable<WebActionNode> actionNodes)
		{
			var nodes = new List<WebActionNode>();

			foreach (WebActionNode node in actionNodes)
			{
				//TODO (CR May 2010): use some kind of interface or attribute (e.g. AssociateHandlerAttribute)?
				if (node is WebDropDownButtonAction || node is WebClickAction || node is WebDropDownAction)
					nodes.Add(node);
				else if (node.Children != null && node.Children.Length > 0)
					nodes.AddRange(FlattenWebActions(node.Children));
			}

			return nodes;
		}

		private void DisposeImageBoxViews()
		{
			foreach (ImageBoxView imageBoxView in _imageBoxViews)
				imageBoxView.Dispose();

			_imageBoxViews.Clear();
		}

		private void DisposeActionViews()
		{
			foreach (WebActionView actionView in _actionViews)
				actionView.Dispose();

			_actionViews.Clear();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing && _viewer != null)
			{
				_viewer.PhysicalWorkspace.LayoutCompleted -= OnLayoutCompleted;
				_viewer.PhysicalWorkspace.Drawing -= OnPhysicalWorkspaceDrawing;
				_viewer.EventBroker.PresentationImageSelected -= OnPresentationImageSelected;

				DisposeActionViews();
				DisposeImageBoxViews();

				_viewer = null;
			}
		}
	}
}
