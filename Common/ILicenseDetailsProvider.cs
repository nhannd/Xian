#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Represents a license details provider.
	/// </summary>
	/// <remarks>
	/// For internal framework use only.
	/// </remarks>
	public interface ILicenseDetailsProvider
	{
		/// <summary>
		/// Gets a string indicating license type.
		/// </summary>
		string LicenseType { get; }

		/// <summary>
		/// Gets the licensed diagnostic uses.
		/// </summary>
		LicensedDiagnosticUses DiagnosticUses { get; }

		/// <summary>
		/// Gets the first run date.
		/// </summary>
		DateTime? FirstRun { get; }
	}

	/// <summary>
	/// Flags indicating allowed diagnostic uses of the product license.
	/// </summary>
	[Flags]
	public enum LicensedDiagnosticUses
	{
		/// <summary>
		/// Indicates product no licensed diagnostic uses.
		/// </summary>
		None = 0,

		/// <summary>
		/// Indicates product is licensed for human diagnosis.
		/// </summary>
		Human = 0x1,

		/// <summary>
		/// Indicates product is licensed for veterinary diagnosis.
		/// </summary>
		Veterinary = 0x2,
	}

	/// <summary>
	/// An extension point for <see cref="ILicenseDetailsProvider"/>s.
	/// </summary>
	/// <remarks>
	/// For internal framework use only.
	/// </remarks>
	[ExtensionPoint]
	public sealed class LicenseDetailsProviderExtensionPoint : ExtensionPoint<ILicenseDetailsProvider>
	{
		private LicenseDetailsProviderExtensionPoint() {}

		internal static ILicenseDetailsProvider CreateInstance()
		{
			try
			{
				// check for a provider extension
				return (ILicenseDetailsProvider) new LicenseDetailsProviderExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException)
			{
				return new DefaultLicenseDetailsProvider();
			}
		}

		private sealed class DefaultLicenseDetailsProvider : ILicenseDetailsProvider
		{
			public string LicenseType
			{
				get { return null; }
			}

			public LicensedDiagnosticUses DiagnosticUses
			{
				get { return LicensedDiagnosticUses.None; }
			}

			public DateTime? FirstRun
			{
				get { return null; }
			}
		}
	}
}