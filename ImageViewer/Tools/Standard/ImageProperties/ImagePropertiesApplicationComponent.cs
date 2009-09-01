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
using System.Collections.Generic;
using System.Collections.Specialized;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	/// <summary>
	/// Extension point for views onto <see cref="ImagePropertiesApplicationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ImagePropertiesApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(ImagePropertiesApplicationComponentViewExtensionPoint))]
	public class ImagePropertiesApplicationComponent : ApplicationComponent
	{
		private readonly IDesktopWindow _desktopWindow;
		private IImageViewer _viewer;

		private readonly List<IImagePropertyProvider> _informationProviders;
		private List<IImageProperty> _imageProperties;
		private string[] _collapsedCategories;

		public ImagePropertiesApplicationComponent(IDesktopWindow desktopWindow)
		{
			_informationProviders = new List<IImagePropertyProvider>();

			try
			{
				foreach(IImagePropertyProvider informationProvider in new ImagePropertyProviderExtensionPoint().CreateExtensions())
					_informationProviders.Add(informationProvider);
			}
			catch(NotSupportedException)
			{
			}

			_desktopWindow = desktopWindow;
			_imageProperties = new List<IImageProperty>();
		}

		public override void  Start()
		{
			SetImageViewer(_desktopWindow.ActiveWorkspace);
			_desktopWindow.Workspaces.ItemActivationChanged += OnActiveWorkspaceChanged;

			base.Start();
		}

		public override void Stop()
		{
			base.Stop();

			_desktopWindow.Workspaces.ItemActivationChanged -= OnActiveWorkspaceChanged;
			SetImageViewer(null);

		}

		/*
		private void InitializeCollapsedCategories()
		{
			StringCollection collapsedCategories = ImagePropertiesSettings.Default.CollapsedCategories ?? new StringCollection();
			_collapsedCategories = new string[collapsedCategories.Count];

			for (int i = 0; i < collapsedCategories.Count; ++i)
				_collapsedCategories[i] = collapsedCategories[i];
		}

		private void SaveSettings()
		{
			ImagePropertiesSettings.Default.CollapsedCategories = new StringCollection();
			foreach (string collapsedCategory in _collapsedCategories)
				ImagePropertiesSettings.Default.CollapsedCategories.AddRange(_collapsedCategories ?? new string[0]);

			ImagePropertiesSettings.Default.Save();
		}
		*/

		private void OnActiveWorkspaceChanged(object sender, ItemEventArgs<Workspace> e)
		{
			SetImageViewer(e.Item);
		}

		private void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			if (e.SelectedTile.PresentationImage == null)
				UpdateImageProperties(e.SelectedTile.PresentationImage);
		}

		private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			UpdateImageProperties(e.SelectedPresentationImage);
		}

		private static IImageViewer CastToImageViewer(Workspace workspace)
		{
			IImageViewer viewer = null;
			if (workspace != null)
				viewer = ImageViewerComponent.GetAsImageViewer(workspace);

			return viewer;
		}

		private void SetImageViewer(Workspace workspace)
		{
			IImageViewer viewer = CastToImageViewer(workspace);
			if (viewer != _viewer)
			{
				if (_viewer != null)
				{
					_viewer.EventBroker.PresentationImageSelected -= OnPresentationImageSelected;
					_viewer.EventBroker.TileSelected -= OnTileSelected;
				}

				_viewer = viewer;
				if (_viewer != null)
				{
					_viewer.EventBroker.PresentationImageSelected += OnPresentationImageSelected;
					_viewer.EventBroker.TileSelected += OnTileSelected;

					UpdateImageProperties(viewer.SelectedPresentationImage);
				}
				else
				{
					UpdateImageProperties(null);
				}
			}
		}

		private void UpdateImageProperties(IPresentationImage presentationImage)
		{
			if (presentationImage == null || _informationProviders.Count == 0)
			{
				ImageProperties = new List<IImageProperty>();
			}
			else
			{
				Dictionary<string, IImageProperty> properties = new Dictionary<string, IImageProperty>();
				foreach (IImagePropertyProvider info in _informationProviders)
				{
					foreach (IImageProperty property in info.GetProperties(presentationImage))
						if (!properties.ContainsKey(property.Name))
							properties[property.Name] = property;
						else
							Platform.Log(LogLevel.Debug, "Image property with name '{0}' already exists; ignoring.", property.Name);
				}

				if (ImagePropertiesSettings.Default.ShowEmptyValues)
				{
					ImageProperties = new List<IImageProperty>(properties.Values);
				}
				else
				{
					ImageProperties = CollectionUtils.Select(properties.Values,
						delegate(IImageProperty property) { return !property.IsEmpty; });
				}
			}
		}

		#region Presentation Model

		public List<IImageProperty> ImageProperties
		{
			get { return _imageProperties; }
			private set
			{
				if (_imageProperties != value)
				{
					_imageProperties = value;
					NotifyPropertyChanged("ImageProperties");
				}
			}
		}

		//Not used right now.
		//public string[] CollapsedCategories
		//{
		//    get
		//    {
		//        return _collapsedCategories;
		//    }
		//    set
		//    {
		//        _collapsedCategories = value;
		//    }
		//}

		#endregion
	}
}
