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
	public interface IFacilityProvider
	{
		IList<Facility> GetAvailableFacilities();

		Facility CurrentFacility { get; set; }
	}

	[ExtensionPoint]
	public sealed class FacilityProviderExtensionPoint : ExtensionPoint<IFacilityProvider>
	{
		private static IFacilityProvider _provider;

		public static IFacilityProvider GetProvider()
		{
			return _provider ?? (_provider = CreateProvider());
		}

		private static IFacilityProvider CreateProvider()
		{
			try
			{
				var provider = (IFacilityProvider) new FacilityProviderExtensionPoint().CreateExtension();
				if (provider != null) return provider;
			}
			catch (NotSupportedException ex)
			{
				Platform.Log(LogLevel.Debug, ex, "No facilities provider defined.");
			}
			return new DefaultFacilityProvider();
		}

		private class DefaultFacilityProvider : IFacilityProvider
		{
			private static readonly Facility[] _emptyList = new Facility[0];

			public IList<Facility> GetAvailableFacilities()
			{
				return _emptyList;
			}

			public Facility CurrentFacility
			{
				get { return null; }
				set { }
			}
		}
	}

	[DataContract]
	public class Facility : DataContractBase, IEquatable<Facility>
	{
		public Facility() {}

		public Facility(string code)
		{
			Code = code;
		}

		[DataMember]
		public string Code { get; set; }

		public bool Equals(Facility facilitySummary)
		{
			return facilitySummary != null && Equals(Code, facilitySummary.Code);
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || Equals(obj as Facility);
		}

		public override int GetHashCode()
		{
			return -0x6EBEF583 ^ (string.IsNullOrEmpty(Code) ? 0 : Code.GetHashCode());
		}
	}
}