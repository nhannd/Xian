using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {

    /// <summary>
    /// ResultCommunicationMode enumeration
    /// </summary>
    [EnumValueClass(typeof(ResultCommunicationModeEnum))]
	public enum ResultCommunicationMode
	{
        /// <summary>
        /// Any, or unknown
        /// </summary>
        [EnumValue("Any", Description = "Any, or unknown")]
        ANY,

        /// <summary>
        /// Mail
        /// </summary>
        [EnumValue("Mail", Description = "Mail")]
        MAIL,

        /// <summary>
        /// Fax
        /// </summary>
        [EnumValue("Fax", Description = "Fax")]
        FAX,

        /// <summary>
        /// Email
        /// </summary>
        [EnumValue("Email", Description = "Email")]
        EMAIL
	}
}