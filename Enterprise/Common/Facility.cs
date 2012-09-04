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
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common
{
	public interface ILoginFacilityProvider
	{
		IList<FacilityInfo> GetAvailableFacilities();

		FacilityInfo CurrentFacility { get; set; }
	}

	[ExtensionPoint]
	public sealed class LoginFacilityProviderExtensionPoint : ExtensionPoint<ILoginFacilityProvider>
	{
		private static ILoginFacilityProvider _provider;

		public static ILoginFacilityProvider GetProvider()
		{
			return _provider ?? (_provider = CreateProvider());
		}

		private static ILoginFacilityProvider CreateProvider()
		{
			try
			{
				var provider = (ILoginFacilityProvider) new LoginFacilityProviderExtensionPoint().CreateExtension();
				if (provider != null) return provider;
			}
			catch (NotSupportedException ex)
			{
				Platform.Log(LogLevel.Debug, ex, "No facilities provider defined.");
			}
			return new DefaultLoginFacilityProvider();
		}

		private class DefaultLoginFacilityProvider : ILoginFacilityProvider
		{
			private static readonly FacilityInfo[] _emptyList = new FacilityInfo[0];

			public IList<FacilityInfo> GetAvailableFacilities()
			{
				return _emptyList;
			}

			public FacilityInfo CurrentFacility
			{
				get { return null; }
				set { }
			}
		}
	}

	[DataContract]
	public class FacilityInfo : DataContractBase, IEquatable<FacilityInfo>
	{
		public FacilityInfo() {}

		public FacilityInfo(string code, string name)
		{
			Code = code;
			Name = name;
		}

		[DataMember]
		public string Code { get; set; }

		[DataMember]
		public string Name { get; set; }

		public bool Equals(FacilityInfo facilitySummary)
		{
			return facilitySummary != null && Equals(Code, facilitySummary.Code);
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || Equals(obj as FacilityInfo);
		}

		public override int GetHashCode()
		{
			return -0x6EBEF583 ^ (string.IsNullOrEmpty(Code) ? 0 : Code.GetHashCode());
		}
	}
}