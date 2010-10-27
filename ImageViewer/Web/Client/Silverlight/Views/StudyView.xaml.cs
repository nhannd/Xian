#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.AppServiceReference;


namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Views
{
	public partial class StudyView : UserControl
    {
        private List<ImageBoxView> _imageBoxViews;
        private DelayedEventPublisher<EventArgs> _resizePublisher;

        public StudyView()
        {
            InitializeComponent();
            _imageBoxViews = new List<ImageBoxView>();

			SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
            _resizePublisher = new DelayedEventPublisher<EventArgs>(ResizeEvent, 350);
        }

        #region Private Methods

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
            //Dispatcher.BeginInvoke(() => SetImageBoxesParentSize());
            _resizePublisher.Publish(null,null);
		}

        private void ResizeEvent(object s, EventArgs a)
        {
            Dispatcher.BeginInvoke(() => SetImageBoxesParentSize());
        }
		
        private void SetImageBoxesParentSize()
		{
			foreach (ImageBoxView boxView in _imageBoxViews)
				boxView.SetParentSize(new System.Windows.Size(StudyViewCanvas.ActualWidth, StudyViewCanvas.ActualHeight));
		}

		private void DestroyImageBoxViews()
		{
			StudyViewCanvas.Children.Clear();

			if (_imageBoxViews != null)
			{
				foreach (ImageBoxView imageBox in _imageBoxViews)
					imageBox.Destroy();

				_imageBoxViews = null;
			}
		}

        #endregion

        #region Public Methods

        public void SetImageBoxes(IEnumerable<ImageBox> imageBoxes)
        {
            DestroyImageBoxViews();
            _imageBoxViews = new List<ImageBoxView>();

            foreach (ImageBox box in imageBoxes)
            {
                ImageBoxView boxView = new ImageBoxView(box);
                StudyViewCanvas.Children.Add(boxView);
                _imageBoxViews.Add(boxView);

            }

            SetImageBoxesParentSize();
        }
        
        public void Destroy()
        {
			DestroyImageBoxViews();
        }

        #endregion
    }
}