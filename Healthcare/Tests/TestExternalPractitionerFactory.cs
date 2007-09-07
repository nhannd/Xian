using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare.Tests
{
    internal static class TestExternalPractitionerFactory
    {
        internal static ExternalPractitioner CreatePractitioner()
        {
            return new ExternalPractitioner(
                new PersonName("Who", "Doctor", null, null, null, null),
                new CompositeIdentifier("1234", "CPSO"),
                null,
                null);
        }
    }
}
