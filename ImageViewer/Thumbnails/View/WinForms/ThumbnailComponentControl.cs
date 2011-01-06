#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using System;

namespace ClearCanvas.ImageViewer.Thumbnails.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ThumbnailComponent"/>.
    /// </summary>
    public partial class ThumbnailComponentControl : ApplicationComponentUserControl
    {
        private ThumbnailComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ThumbnailComponentControl(ThumbnailComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

			_galleryView.DataSource = _component.Thumbnails;

        	_imageSetTree.SelectionChanged += 
				delegate
            	{
            		_component.TreeSelection = _imageSetTree.Selection;
            	};

        	_imageSetTree.TreeBackColor = Color.FromKnownColor(KnownColor.Black);
			_imageSetTree.TreeForeColor = Color.FromKnownColor(KnownColor.ControlLight);
			_imageSetTree.TreeLineColor = Color.FromKnownColor(KnownColor.ControlLight);

			_component.PropertyChanged += OnPropertyChanged;

			_imageSetTree.Tree = _component.Tree;
        	_imageSetTree.VisibleChanged += OnTreeVisibleChanged;
		}

		private void OnTreeVisibleChanged(object sender, EventArgs e)
		{
			_imageSetTree.VisibleChanged -= OnTreeVisibleChanged;
			//the control isn't really visible until it's been drawn, so the selection can't be set until then.
			_imageSetTree.Selection = _component.TreeSelection;
		}

    	private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Tree")
			{
				_imageSetTree.Tree = _component.Tree;
			}
			else if (e.PropertyName == "TreeSelection")
			{
				_imageSetTree.Selection = _component.TreeSelection;
			}
		}
    }
}
