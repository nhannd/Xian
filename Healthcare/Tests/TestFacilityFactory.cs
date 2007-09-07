using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.Tests
{
    internal static class TestFacilityFactory
    {
        internal static Facility CreateFacility()
        {
            return new Facility("TCH", "Toronto Community Hospital");
        }
    }
}
