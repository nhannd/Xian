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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    // TODO (CR Mar 2012): Move this and associated classes to their own namespace.
	/// <summary>
	/// Abstract base class for <see cref="IPrefetchingStrategy"/>.
	/// </summary>
	public abstract class PrefetchingStrategy : IPrefetchingStrategy
	{
		private IImageViewer _imageViewer;
		private readonly string _name;
		private readonly string _description;

		/// <summary>
		/// Constructs a new <see cref="PrefetchingStrategy"/> with the given <paramref name="name"/>
		/// and <paramref name="description"/>.
		/// </summary>
		protected PrefetchingStrategy(string name, string description)
		{
			_name = name;
			_description = description;
		}

		/// <summary>
		/// Gets the <see cref="IImageViewer"/> for which data is to be prefetched.
		/// </summary>
		protected IImageViewer ImageViewer
		{
			get { return _imageViewer; }
		}

		/// <summary>
		/// Starts prefetching.
		/// </summary>
		protected abstract void Start();

		/// <summary>
		/// Stops prefetching.
		/// </summary>
		protected abstract void Stop();

		#region IPrefetchingStrategy Members

		/// <summary>
		/// Gets the friendly name of the prefetching strategy.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets the friendly description of the prefetching strategy.
		/// </summary>
		public string Description
		{
			get { return _description; }
		}

		void IPrefetchingStrategy.Start(IImageViewer imageViewer)
		{
			Platform.CheckForNullReference(imageViewer, "imageViewer");

			//Only start if we haven't already been started.
			if (_imageViewer == null)
			{
				_imageViewer = imageViewer;
				Start();
			}
		}

		void IPrefetchingStrategy.Stop()
		{
			if(_imageViewer != null)
			{
				Stop();
				_imageViewer = null;
			}
		}

		#endregion
	}
}
