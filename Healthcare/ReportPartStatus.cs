using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    [EnumValueClass(typeof(ReportPartStatusEnum))]
	public enum ReportPartStatus
	{
        /// <summary>
        /// Preliminary
        /// </summary>
        [EnumValue("Preliminary", Description="Prior to report being verified")]
        P,
 
        /// <summary>
        /// Final
        /// </summary>
        [EnumValue("Final", Description = "Report is verified")]
        F,

        /// <summary>
        /// Cancelled
        /// </summary>
        [EnumValue("Cancelled", Description="Report is cancelled")]
        X
	}
}