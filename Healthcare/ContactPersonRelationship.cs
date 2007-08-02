using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// ContactPersonRelationship enumeration
    /// </summary>
    [EnumValueClass(typeof(ContactPersonRelationshipEnum))]
    public enum ContactPersonRelationship
	{
        /// <summary>
        /// Mother
        /// </summary>
        [EnumValue("Mother", Description = "Mother")]
        rel1
    }
}