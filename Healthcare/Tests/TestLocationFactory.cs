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
