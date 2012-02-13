#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard.Configuration
{
	[ExtensionPoint]
	public sealed class ToolConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ToolConfigurationComponentViewExtensionPoint))]
	public class ToolConfigurationComponent : ConfigurationApplicationComponent
	{
		private ToolModalityBehaviorCollection _modalityBehavior;
		private bool _invertZoomDirection;

		public ToolModalityBehaviorCollection ModalityBehavior
		{
			get { return _modalityBehavior; }
			set
			{
				if (_modalityBehavior != value)
				{
					_modalityBehavior = value;
					NotifyPropertyChanged("ModalityBehavior");
					Modified = true;
				}
			}
		}

		public bool InvertZoomDirection
		{
			get { return _invertZoomDirection; }
			set
			{
				if (_invertZoomDirection != value)
				{
					_invertZoomDirection = value;
					NotifyPropertyChanged("InvertZoomDirection");
					Modified = true;
				}
			}
		}

		public override void Start()
		{
			base.Start();

			var settings = ToolSettings.Default;
			_modalityBehavior = settings.CachedToolModalityBehavior;
			_invertZoomDirection = settings.InvertedZoomToolOperation;
		}

		public override void Save()
		{
			var settings = ToolSettings.Default;
			settings.ToolModalityBehavior = _modalityBehavior;
			settings.InvertedZoomToolOperation = _invertZoomDirection;
			settings.Save();
		}
	}
}