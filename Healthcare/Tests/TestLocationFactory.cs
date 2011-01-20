#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.Tests
{
	internal class TestLocationFactory
	{
		internal static Location CreateLocation()
		{
			return new Location(
				"101",
				"Floor 1",
				"Main floor",
				TestFacilityFactory.CreateFacility(),
				"Building 1",
				"Floor 1",
				"POC 1",
				"Rm 1",
				"Bed 1");
		}
	}
}
