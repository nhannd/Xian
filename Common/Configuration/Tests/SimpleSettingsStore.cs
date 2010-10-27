#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS
#pragma warning disable 1591

using System.Collections.Generic;
using System.Configuration;

namespace ClearCanvas.Common.Configuration.Tests
{
	internal class SimpleSettingsStore
	{
		private enum Store
		{
			CurrentUser,
			PreviousUser,
			CurrentShared,
			PreviousShared
		}
		public static SimpleSettingsStore Instance  = new SimpleSettingsStore();

		private readonly Dictionary<Store, SettingsPropertyValueCollection> _stores;

		private SimpleSettingsStore()
		{
			_stores = new Dictionary<Store, SettingsPropertyValueCollection>();
			_stores[Store.CurrentUser] = CurrentUserValues = new SettingsPropertyValueCollection();
			_stores[Store.PreviousUser] = PreviousUserValues = new SettingsPropertyValueCollection();
			_stores[Store.CurrentShared] = CurrentSharedValues = new SettingsPropertyValueCollection();
			_stores[Store.PreviousShared] = PreviousSharedValues = new SettingsPropertyValueCollection();
		}

		public SettingsPropertyValueCollection PreviousUserValues { get; set; }
		public SettingsPropertyValueCollection CurrentUserValues { get; set; }
		public SettingsPropertyValueCollection PreviousSharedValues { get; set; }
		public SettingsPropertyValueCollection CurrentSharedValues { get; set; }

		public void Reset()
		{
			PreviousUserValues.Clear();
			CurrentUserValues.Clear();
			PreviousSharedValues.Clear();
			CurrentSharedValues.Clear();
		}
	}
}

#endif