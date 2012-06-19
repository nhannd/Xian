#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	[ExtensionPoint]
	public sealed class LanguagePickerActionViewExtensionPoint : ExtensionPoint<IActionView> {}

	[AssociateView(typeof (LanguagePickerActionViewExtensionPoint))]
	public class LanguagePickerAction : Action
	{
		private InstalledLocales.Locale _selectedLocale;
		private event EventHandler _selectedLocaleChanged;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionId">The logical action ID.</param>
		/// <param name="actionPath">The action path.</param>
		/// <param name="resourceResolver">A resource resolver that will be used to resolve icons associated with this action.</param>
		public LanguagePickerAction(string actionId, string actionPath, IResourceResolver resourceResolver)
			: base(actionId, new ActionPath(actionPath, resourceResolver), resourceResolver) {}

		public InstalledLocales.Locale SelectedLocale
		{
			get { return _selectedLocale; }
			set
			{
				if (_selectedLocale != value)
				{
					_selectedLocale = value;
					EventsHelper.Fire(_selectedLocaleChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler SelectedLocaleChanged
		{
			add { _selectedLocaleChanged += value; }
			remove { _selectedLocaleChanged -= value; }
		}
	}
}