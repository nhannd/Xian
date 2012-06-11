#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ClearCanvas.Common.UsageTracking
{
	[ExtensionPoint]
	public sealed class UsageApplicationDataProviderExtensionPoint : ExtensionPoint<IUsageApplicationDataProvider>
	{
		public new IList<IUsageApplicationDataProvider> CreateExtensions()
		{
			return base.CreateExtensions().OfType<IUsageApplicationDataProvider>().ToList();
		}
	}

	public interface IUsageApplicationDataProvider
	{
		UsageApplicationData GetData(UsageType type);
	}

	public abstract class UsageApplicationDataProvider : IUsageApplicationDataProvider
	{
		private readonly string _key;

		protected UsageApplicationDataProvider(string key)
		{
			Platform.CheckForEmptyString(key, "key");
			_key = key;
		}

		public string Key
		{
			get { return _key; }
		}

		public abstract string GetData(UsageType type);

		protected virtual bool HasData(UsageType type)
		{
			return true;
		}

		protected virtual ExtensionDataObject GetExtensionData()
		{
			return null;
		}

		UsageApplicationData IUsageApplicationDataProvider.GetData(UsageType type)
		{
			try
			{
				if (HasData(type)) return new UsageApplicationData {Key = Key, Value = GetData(type), ExtensionData = GetExtensionData()};
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Error getting application data for usage tracking.");
			}
			return null;
		}
	}
}