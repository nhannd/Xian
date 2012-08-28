#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

 // ReSharper disable InconsistentNaming

namespace ClearCanvas.Ris.Application.Common
{
	public static class FeatureTokens
	{
		private const string _risCoreFeatureToken = @"RIS.Core";

		public static class RIS
		{
			/// <summary>
			/// Core RIS features, including login and basic workflow components
			/// </summary>
			public const string Core = _risCoreFeatureToken;
		}
	}
}

// ReSharper restore InconsistentNaming