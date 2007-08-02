using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// VisitPhysicianRole enumeration
    /// </summary>
    [EnumValueClass(typeof(VisitPractitionerRoleEnum))]
    public enum VisitPractitionerRole
	{
        /// <summary>
        /// Referring
        /// </summary>
        [EnumValue("Referring")]
        RF,

        /// <summary>
        /// Attending
        /// </summary>
        [EnumValue("Attending")]
        AT,

        /// <summary>
        /// Admitting
        /// </summary>
        [EnumValue("Admitting")]
        AD,

        /// <summary>
        /// Consulting
        /// </summary>
        [EnumValue("Consulting")]
        CN,
	}
}