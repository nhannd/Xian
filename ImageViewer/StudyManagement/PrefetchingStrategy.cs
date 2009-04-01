using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public abstract class PrefetchingStrategy : IPrefetchingStrategy
	{
		private IImageViewer _imageViewer;
		private readonly string _name;
		private readonly string _description;

		protected PrefetchingStrategy(string name, string description)
		{
			_name = name;
			_description = description;
		}

		protected IImageViewer ImageViewer
		{
			get { return _imageViewer; }
		}

		protected abstract void OnStart();
		protected abstract void OnStop();

		#region IPrefetchingStrategy Members

		public string Name
		{
			get { return _name; }
		}

		public string Description
		{
			get { return _description; }
		}

		public void Start(IImageViewer imageViewer)
		{
			Platform.CheckForNullReference(imageViewer, "imageViewer");
			_imageViewer = imageViewer;
			OnStart();
		}

		public void Stop()
		{
			if(_imageViewer != null)
			{
				OnStop();
				_imageViewer = null;
			}
		}

		#endregion
	}
}
