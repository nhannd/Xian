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

namespace MyPlugin.TextEditor
{
	[ExtensionPoint]
	public class TextEditorConfigComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (TextEditorConfigComponentViewExtensionPoint))]
	public class TextEditorConfigComponent : ConfigurationApplicationComponent
	{
		private bool _wordWrap;

		public bool WordWrap
		{
			get { return _wordWrap; }
			set
			{
				if (_wordWrap != value)
				{
					_wordWrap = value;

					base.NotifyPropertyChanged("WordWrap");
					base.Modified = true;
				}
			}
		}

		public override void Start()
		{
			_wordWrap = TextEditorSettings.Default.WordWrap;

			base.Start();
		}

		public override void Save()
		{
			TextEditorSettings.Default.WordWrap = _wordWrap;
			TextEditorSettings.Default.Save();
		}
	}
}