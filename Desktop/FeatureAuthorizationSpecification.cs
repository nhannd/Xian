#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// An implementation of <see cref="ISpecification"/> that tests if the identified feature is authorized by application licensing.
	/// </summary>
	public class FeatureAuthorizationSpecification : ISpecification
	{
		private readonly string _featureToken;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="featureToken">The feature identification token to be checked.</param>
		public FeatureAuthorizationSpecification(string featureToken)
		{
			_featureToken = featureToken;
		}

		public TestResult Test(object obj)
		{
			return new TestResult(LicenseInformation.IsFeatureAuthorized(_featureToken));
		}
	}
}